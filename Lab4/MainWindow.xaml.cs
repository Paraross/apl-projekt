using System.Windows;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WpfUI
{
    // delete this and just use float if possible
    public class Root
    {
        public float Value { get; set; }

        public Root(float value)
        {
            Value = value;
        }
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<Root> Roots { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            UIRoots.ItemsSource = Roots = [];
        }

        private void New_Root_Button_Click(object sender, RoutedEventArgs e)
        {
            var root = new Root(0.0f);
            Roots.Add(root);
        }

        private void Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            var rootValues = Roots.Select(root => root.Value);
            var scale = float.Parse(ScaleText.Text);

            var rootsPoly = new PolyRootsScale(rootValues.ToArray(), scale);

            RootsPolyLabel.Content = rootsPoly.ToString();

            var stopwatch = Stopwatch.StartNew();

            var coeffPoly = (PolyCoeffs)rootsPoly;

            stopwatch.Stop();

            TimeTakenLabel.Content = String.Format("{0}", stopwatch.Elapsed);
            CoeffPolyLabel.Content = coeffPoly.ToString();
        }

        public void Calculate_Asm_Button_Click(object sender, RoutedEventArgs e)
        {
            var rootValues = Roots.Select(root => root.Value);
            var scale = float.Parse(ScaleText.Text);

            var rootsPoly = new PolyRootsScale(rootValues.ToArray(), scale);

            RootsPolyLabel.Content = rootsPoly.ToString();

            var len = rootsPoly.roots.Length;

            var stopwatch = Stopwatch.StartNew();

            // allocate space for resultCoeffsPrev and resultCoeffs
            var resultCoeffsPrev = new List<float>(len);
            for (var i = 0; i < resultCoeffsPrev.Capacity; i++)
            {
                resultCoeffsPrev.Add(0.0f);
            }
            var resultCoeffs = new List<float>(len + 1);
            for (var i = 0; i < resultCoeffs.Capacity; i++)
            {
                resultCoeffs.Add(0.0f);
            }

            float[] resultCoeffsPrevArr = [.. resultCoeffsPrev];
            float[] resultCoeffsArr = [.. resultCoeffs];
            unsafe
            {
                fixed (
                    float* rootsPtr = rootsPoly.roots,
                    prevArrPtr = resultCoeffsPrevArr,
                    arrPtr = resultCoeffsArr
                )
                {

                    AsmProxy.ExecuteConvertRaw(rootsPtr, scale, (long)len, prevArrPtr, arrPtr);
                }
            }

            stopwatch.Stop();

            var coeffPoly = new PolyCoeffs(resultCoeffsArr.ToList());

            TimeTakenLabel.Content = String.Format("{0}", stopwatch.Elapsed);
            CoeffPolyLabel.Content = coeffPoly.ToString();
        }

        private void Pop_Root_Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
        }
    }

    public unsafe class AsmProxy
    {
        [DllImport("Asm.dll")]
        private static extern double convertRaw(
            float* roots,
            float scale,
            long len,
            float* resultCoeffsPrev,
            float* resultCoeffs
        );

        public static double ExecuteConvertRaw(
            float* roots,
            float scale,
            long len,
            float* resultCoeffsPrev,
            float* resultCoeffs
        )
        {
            return convertRaw(
                roots,
                scale,
                len,
                resultCoeffsPrev,
                resultCoeffs
            );
        }
    }

    public class PolyRootsScale
    {
        public float[] roots;
        private float scale;

        public PolyRootsScale(float[] roots, float scale)
        {
            this.roots = roots;
            this.scale = scale;
        }

        public override string ToString()
        {
            var s = scale.ToString();
            foreach (var root in roots)
            {
                if (root < 0.0f)
                {
                    s += String.Format("(x + {0})", (-root).ToString());
                }
                else if (root == 0.0f)
                {
                    s += "(x)";
                }
                else /* if (root > 0.0f) */
                {
                    s += String.Format("(x - {0})", root.ToString());
                }
            }
            return s;
        }

        public static explicit operator PolyCoeffs(PolyRootsScale poly)
        {
            if (poly.roots.Length == 0)
            {
                return new PolyCoeffs([poly.scale]);
            }

            var resultCoeffs = new PolyCoeffs([]);

            resultCoeffs.coeffs.Add(poly.roots[0]);
            resultCoeffs.coeffs.Add(1.0f);

            foreach (var root in poly.roots[1..])
            {
                // deep copy?
                var resultCoeffsPrev = new PolyCoeffs(new List<float>(resultCoeffs.coeffs));
                for (var i = 0; i < resultCoeffsPrev.coeffs.Count; i++)
                {
                    resultCoeffsPrev.coeffs[i] *= root;
                }

                resultCoeffs.increasePower();

                for (var i = 0; i < resultCoeffsPrev.coeffs.Count; i++)
                {
                    resultCoeffs.coeffs[i] += resultCoeffsPrev.coeffs[i];
                }
            }

            for (var i = 0; i < resultCoeffs.coeffs.Count; i++)
            {
                resultCoeffs.coeffs[i] *= poly.scale;
                if (i % 2 == 0)
                {
                    resultCoeffs.coeffs[i] *= -1.0f;
                }
            }

            if (resultCoeffs.degree() % 2 == 0)
            {
                for (var i = 0; i < resultCoeffs.coeffs.Count; i++)
                {
                    resultCoeffs.coeffs[i] *= -1.0f;
                }
            }

            return resultCoeffs;
        }
    }

    public class PolyCoeffs
    {
        public List<float> coeffs; // Coeffs? check naming convention

        public PolyCoeffs(List<float> coeffs)
        {
            this.coeffs = coeffs;
        }

        public int degree()
        {
            return coeffs.Count - 1;
        }

        public void increasePower()
        {
            coeffs.Add(0.0f);
            for (var i = coeffs.Count - 1; i >= 1; i--)
            {
                coeffs[i] = coeffs[i - 1];
            }
            coeffs[0] = 0.0f;
        }

        public override string ToString()
        {
            var s = "";

            // iterates over index, value pairs in reverse order
            foreach (var it in coeffs.Select((x, i) => new { Value = x, Index = i }).Reverse())
            {
                var degree = it.Index;
                var coeff = it.Value;

                var degreeStr = degree switch
                {
                    1 => "x",
                    var x when (x > 1) => String.Format("x^{0}", degree),
                    _ => "", // var x when (x < 1)
                };

                if (degree == this.degree())
                {
                    s += coeff.ToString() + degreeStr;
                }
                else
                {
                    var sign = coeff > 0.0f ? '+' : '-';

                    var coeffValue = float.Abs(coeff);
                    s += String.Format(" {0} {1}{2}", sign, coeffValue, degreeStr);
                }
            }

            return s;
        }
    }
}
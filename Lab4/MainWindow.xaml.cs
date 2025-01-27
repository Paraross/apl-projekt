using System.Windows;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WpfUI
{
    public class Root(float value)
    {
        public float Value { get; set; } = value;
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<Root> Roots { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Roots = [];
            UIRoots.ItemsSource = Roots;
        }

        private void New_Root_Button_Click(object sender, RoutedEventArgs e)
        {
            var root = new Root(0.0f);
            Roots.Add(root);
        }

        private void Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            var rootsPoly = GetPolyFromInputAndSetLabel();

            var stopwatch = Stopwatch.StartNew();

            var coeffPoly = (PolyCoeffs)rootsPoly;

            stopwatch.Stop();

            SetLabelContents(stopwatch, coeffPoly);
        }

        public void Calculate_Asm_Button_Click(object sender, RoutedEventArgs e)
        {
            var rootsPoly = GetPolyFromInputAndSetLabel();

            var len = rootsPoly.Roots.Length;

            var stopwatch = Stopwatch.StartNew();

            // allocate space for resultCoeffsPrev and resultCoeffs
            var resultCoeffsPrevArr = new float[len];
            var resultCoeffsArr = new float[len + 1];

            unsafe
            {
                fixed (
                    float* rootsPtr = rootsPoly.Roots,
                    prevArrPtr = resultCoeffsPrevArr,
                    arrPtr = resultCoeffsArr
                )
                {
                    AsmProxy.ExecuteConvertRaw(rootsPtr, rootsPoly.Scale, (long)len, prevArrPtr, arrPtr);
                }
            }

            stopwatch.Stop();

            var coeffPoly = new PolyCoeffs([.. resultCoeffsArr]);

            SetLabelContents(stopwatch, coeffPoly);
        }

        private void Pop_Root_Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
        }

        private PolyRootsScale GetPolyFromInputAndSetLabel()
        {
            var rootValues = Roots.Select(root => root.Value);
            var scale = float.Parse(ScaleText.Text);
            var rootsPoly = new PolyRootsScale(rootValues.ToArray(), scale);

            RootsPolyLabel.Content = rootsPoly.ToString();

            return rootsPoly;
        }

        private void SetLabelContents(Stopwatch stopwatch, PolyCoeffs coeffPoly)
        {
            TimeTakenLabel.Content = stopwatch.Elapsed;
            CoeffPolyLabel.Content = coeffPoly.ToString();
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

    public class PolyRootsScale(float[] roots, float scale)
    {
        public float[] Roots = roots;
        public readonly float Scale = scale;

        public override string ToString()
        {
            var s = Scale.ToString();
            foreach (var root in Roots)
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
            if (poly.Roots.Length == 0)
            {
                return new PolyCoeffs([poly.Scale]);
            }

            var resultCoeffs = new PolyCoeffs([]);

            resultCoeffs.Coeffs.Add(poly.Roots[0]);
            resultCoeffs.Coeffs.Add(1.0f);

            foreach (var root in poly.Roots[1..])
            {
                // deep copy?
                var resultCoeffsPrev = new PolyCoeffs(new List<float>(resultCoeffs.Coeffs));
                for (var i = 0; i < resultCoeffsPrev.Coeffs.Count; i++)
                {
                    resultCoeffsPrev.Coeffs[i] *= root;
                }

                resultCoeffs.IncreasePower();

                for (var i = 0; i < resultCoeffsPrev.Coeffs.Count; i++)
                {
                    resultCoeffs.Coeffs[i] += resultCoeffsPrev.Coeffs[i];
                }
            }

            for (var i = 0; i < resultCoeffs.Coeffs.Count; i++)
            {
                resultCoeffs.Coeffs[i] *= poly.Scale;
                if (i % 2 == 0)
                {
                    resultCoeffs.Coeffs[i] *= -1.0f;
                }
            }

            if (resultCoeffs.Degree() % 2 == 0)
            {
                for (var i = 0; i < resultCoeffs.Coeffs.Count; i++)
                {
                    resultCoeffs.Coeffs[i] *= -1.0f;
                }
            }

            return resultCoeffs;
        }
    }

    public class PolyCoeffs(List<float> coeffs)
    {
        public List<float> Coeffs = coeffs;

        public int Degree()
        {
            return Coeffs.Count - 1;
        }

        public void IncreasePower()
        {
            Coeffs.Add(0.0f);
            for (var i = Coeffs.Count - 1; i >= 1; i--)
            {
                Coeffs[i] = Coeffs[i - 1];
            }
            Coeffs[0] = 0.0f;
        }

        public override string ToString()
        {
            var s = "";

            // iterates over index, value pairs in reverse order
            foreach (var it in Coeffs.Select((x, i) => new { Value = x, Index = i }).Reverse())
            {
                var degree = it.Index;
                var coeff = it.Value;

                var degreeStr = degree switch
                {
                    1 => "x",
                    var x when (x > 1) => String.Format("x^{0}", degree),
                    _ => "", // var x when (x < 1)
                };

                if (degree == this.Degree())
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
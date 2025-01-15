using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.ObjectModel;
using System.Collections;

namespace WpfUI
{
    // NOTE: delete this and just use float
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

            var coeffPoly = new PolyCoeffs([1.0f, 2.5f, -3.1f]);
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
        private static extern double AsmAddTwoDoubles(double a, double b);

        public static double ExecuteAsmAddTwoDoubles(double a, double b)
        {
            return AsmAddTwoDoubles(a, b);
        }
    }

    public class PolyRootsScale
    {
        private float[] roots;
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
    }

    class PolyCoeffs
    {
        private List<float> coeffs;

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
            // not sure if this works
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
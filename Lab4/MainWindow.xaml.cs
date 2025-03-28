﻿using System.CodeDom;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace WpfUI
{
    /// <summary>
    /// References a root in the UI.
    /// </summary>
    /// <param name="value">The value of the root.</param>
    public class Root(float value)
    {
        public float Value { get; set; } = value;
    }

    public partial class MainWindow : Window
    {
        /// <summary>
        /// References the roots in the UI.
        /// </summary>
        public ObservableCollection<Root> Roots { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Roots = [];
            UIRoots.ItemsSource = Roots;
        }

        /// <summary>
        /// Adds a new root when button is pressed.
        /// </summary>
        private void New_Root_Button_Click(object sender, RoutedEventArgs e)
        {
            var root = new Root(0.0f);
            Roots.Add(root);
        }

        /// <summary>
        /// Performs calculations when button is pressed.
        /// C# version.
        /// </summary>
        private void Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            var rootsPoly = GetPolyFromInputAndSetLabel();

            var stopwatch = Stopwatch.StartNew();

            var coeffPoly = (PolyCoeffs)rootsPoly;

            stopwatch.Stop();

            SetLabelContents(stopwatch, coeffPoly);
        }

        /// <summary>
        /// Performs calculations when button is pressed.
        /// Assembly version.
        /// </summary>
        private void Calculate_Asm_Button_Click(object sender, RoutedEventArgs e)
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
                    AsmProxy.ExecuteConvert(rootsPtr, rootsPoly.Scale, (long)len, prevArrPtr, arrPtr);
                }
            }

            stopwatch.Stop();

            var coeffPoly = new PolyCoeffs([.. resultCoeffsArr]);

            SetLabelContents(stopwatch, coeffPoly);
        }

        /// <summary>
        /// Removes the last root when button is pressed.
        /// </summary>
        private void Pop_Root_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Roots.Count != 0)
            {
                Roots.RemoveAt(Roots.Count - 1);
            }
        }

        /// <summary>
        /// Parses the values from the UI and creates a <c>PolyRootsScale</c> from them.
        /// </summary>
        /// <returns>
        /// A <c>PolyRootsScale</c> represented by the values from the UI.
        /// </returns>
        private PolyRootsScale GetPolyFromInputAndSetLabel()
        {
            PolyRootsScale rootsPoly;

            var useFile = UseFile.IsChecked ?? false;
            if (useFile)
            {
                rootsPoly = GetPolyFromFile();
            }
            else
            {
                var rootValues = Roots.Select(root => root.Value);

                var scale = ScaleUpDown.Value ?? 0.0f;

                rootsPoly = new PolyRootsScale(rootValues.ToArray(), scale);
            }

            RootsPolyLabel.Content = rootsPoly.ToString();

            return rootsPoly;
        }

        /// <summary>
        /// Parses a file and creates a <c>PolyRootsScale</c> from them.
        /// </summary>
        /// <returns>
        /// A <c>PolyRootsScale</c> represented by the values from the file.
        /// </returns>
        private static PolyRootsScale GetPolyFromFile()
        {
            // TODO: exception handling
            try
            {
                using StreamReader reader = new("../../../values.txt");

                var firstLine = reader.ReadLine();
                var scale = float.Parse(firstLine ?? "");

                var lines = new List<string>();

                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    lines.Add(line);
                };
                var l = lines.Select(line => float.Parse(line.Trim())).ToArray();
                var poly = new PolyRootsScale(l, scale);

                return poly;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sets the time taken and coeff poly labels.
        /// </summary>
        /// <param name="stopwatch"><c>Stopwatch</c> for the time taken label.</param>
        /// <param name="coeffPoly"><c>PolyCoeffs </c>for the coeff poly label.</param>
        private void SetLabelContents(Stopwatch stopwatch, PolyCoeffs coeffPoly)
        {
            TimeTakenLabel.Content = stopwatch.Elapsed;
            CoeffPolyLabel.Content = coeffPoly.ToString();
        }
    }

    /// <summary>
    /// Class wrapping the assembly procedure.
    /// </summary>
    public unsafe class AsmProxy
    {
        /// <summary>
        /// The assembly procedure.
        /// </summary>
        /// <param name="roots">Roots of the <c>PolyRootsScale</c>.</param>
        /// <param name="scale">Scale of the <c>PolyRootsScale</c>.</param>
        /// <param name="len">Number of roots of the <c>PolyRootsScale</c>.</param>
        /// <param name="resultCoeffsPrev">Additional allocated space used by the procedure.</param>
        /// <param name="resultCoeffs">Where the coefficients of the resulting <c>PolyCoeffs</c> will be written to.</param>
        [DllImport("Asm.dll")]
        private static extern void Convert(
            float* roots,
            float scale,
            long len,
            float* resultCoeffsPrev,
            float* resultCoeffs
        );

        /// <summary>
        /// Function wrapping the assembly procedure.
        /// </summary>
        /// <param name="roots">Roots of the <c>PolyRootsScale</c>.</param>
        /// <param name="scale">Scale of the <c>PolyRootsScale</c>.</param>
        /// <param name="len">Number of roots of the <c>PolyRootsScale</c>.</param>
        /// <param name="resultCoeffsPrev">Additional allocated space used by the procedure.</param>
        /// <param name="resultCoeffs">Where the coefficients of the resulting <c>PolyCoeffs</c> will be written to.</param>
        public static void ExecuteConvert(
            float* roots,
            float scale,
            long len,
            float* resultCoeffsPrev,
            float* resultCoeffs
        )
        {
            Convert(
                roots,
                scale,
                len,
                resultCoeffsPrev,
                resultCoeffs
            );
        }
    }

    /// <summary>
    /// Polynomial in the roots form.
    /// </summary>
    /// <param name="roots">Roots of the polynomial.</param>
    /// <param name="scale">Scale of the polynomial.</param>
    public class PolyRootsScale(float[] roots, float scale)
    {
        /// <summary>
        /// Roots of the polynomial.
        /// </summary>
        public float[] Roots = roots;
        /// <summary>
        /// Scale of the polynomial.
        /// </summary>
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

            var resultPolyCoeffs = new PolyCoeffs(new List<float>(poly.Roots.Length + 1));

            resultPolyCoeffs.Coeffs.Add(poly.Roots[0]);
            resultPolyCoeffs.Coeffs.Add(1.0f);

            var resultPolyCoeffsPrev = new float[poly.Roots.Length];
            //var resultCoeffsPrev = new PolyCoeffs(new List<float>(poly.Roots.Length));

            foreach (var root in poly.Roots[1..])
            {
                var len = resultPolyCoeffs.Coeffs.Count;

                for (var i = 0; i < len; i++)
                {
                    resultPolyCoeffsPrev[i] = resultPolyCoeffs.Coeffs[i] * root;
                }

                resultPolyCoeffs.IncreasePower();

                for (var i = 0; i < len; i++)
                {
                    resultPolyCoeffs.Coeffs[i] += resultPolyCoeffsPrev[i];
                }
            }

            for (var i = 0; i < resultPolyCoeffs.Coeffs.Count; i++)
            {
                resultPolyCoeffs.Coeffs[i] *= poly.Scale;
                if (i % 2 == 0)
                {
                    resultPolyCoeffs.Coeffs[i] *= -1.0f;
                }
            }

            if (resultPolyCoeffs.Degree % 2 == 0)
            {
                for (var i = 0; i < resultPolyCoeffs.Coeffs.Count; i++)
                {
                    resultPolyCoeffs.Coeffs[i] *= -1.0f;
                }
            }

            return resultPolyCoeffs;
        }
    }

    /// <summary>
    /// Polynomial in the standard form.
    /// </summary>
    /// <param name="coeffs">Coefficients of the polynomial.</param>
    public class PolyCoeffs(List<float> coeffs)
    {
        /// <summary>
        /// Coefficients of the polynomial.
        /// </summary>
        public List<float> Coeffs = coeffs;

        /// <summary>
        /// Degree of the polynomial.
        /// </summary>
        public int Degree => Coeffs.Count - 1;

        /// <summary>
        /// Increases the power of each term by 1.
        /// Equivalent to multiplying the whole polynomial by x.
        /// </summary>
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

                if (degree == this.Degree)
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
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
            var sum = 0.0f;

            foreach (Root root1 in Roots)
            {
                sum += root1.Value;
            }

            SumText.Text = sum.ToString();
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
}
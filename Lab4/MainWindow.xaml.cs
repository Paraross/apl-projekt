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
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<Root> Roots { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //Roots = new ObservableCollection<Root>();
            Roots = [];
        }

        private void New_Text_Field_Button_Click(object sender, RoutedEventArgs e)
        {
            
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
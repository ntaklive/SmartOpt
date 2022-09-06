using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double _defaultWidth = 6000.0;
        private readonly double _defaultWaste = 4;
        private readonly int _defaultCount = 5;

        private readonly string _currentExcelPath;

        public MainWindow()
        {
            InitializeComponent();

            //     _currentExcelPath = @"C:\Users\Artsiom_Kharkevich\Desktop\demo\original.xlsm";//args.FirstOrDefault();
            //     
            //     if (_currentExcelPath == null)
            //     {
            //         throw new ArgumentException("Args are required!!!");
            //     }
            //     
            //     WidthTextBox.Text = _defaultWidth.ToString();
            //     CountTextBox.Text = _defaultCount.ToString();
            //     WasteTextBox.Text = _defaultWaste.ToString();
            //     Title = _currentExcelPath;
            //     
            //     try
            //     {
            //         var settings = new LaunchSettings()
            //         {
            //             MaxWidth = double.Parse(this.WidthTextBox.Text.Replace('.', ',')),
            //             OrderCount = int.Parse(this.CountTextBox.Text),
            //             MaxWaste = double.Parse(this.WasteTextBox.Text.Replace('.', ',')),
            //             ExcelPath = _currentExcelPath,
            //             IsRectanglesDraw = IsRectanglesDraw_CheckBox.IsChecked.GetValueOrDefault(),
            //         };
            //     
            //         _launcher = new AppLauncher(settings);
            //     }
            //     catch (Exception ex)
            //     {
            //         MessageBox.Show(ex.Message, "Ошибка.");
            //     }
            // }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IncreaseCount_Button_Click(object sender, RoutedEventArgs e)
        {
            CountTextBox.Text = (int.Parse(this.CountTextBox.Text) + 1).ToString();
        }

        private void DecreaseCount_Button_Click(object sender, RoutedEventArgs e)
        {
            CountTextBox.Text = (int.Parse(this.CountTextBox.Text) - 1).ToString();
        }

        private void WidthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNumeric(e.Text);
        }

        private void CountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNumeric(e.Text);
        }

        private void WasteTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNumeric(e.Text);
        }

        private static bool IsNumeric(string str)
        {
            return new Regex("[^0-9]").IsMatch(str);
        }
    }
}
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VoukoderManager.Language;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Lang _lang;
        private ComponentPage _installedPage;
        private ComponentPage _availiblePage;

        public MainWindow()
        {
            _lang = new Lang();
            _lang.Initialize();
            InitializeComponent();
            Lang.LanguageChanged += LanguageChanged;
            _installedPage = new ComponentPage(true);
            _availiblePage = new ComponentPage(false);
            framePages.Navigate(_installedPage);
        }

        private void LanguageChanged(object sender, LanguageChangeEventArgs e)
        {
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void gridMove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void buttonInstalled_Click(object sender, RoutedEventArgs e)
        {
            buttonAvailible.Foreground = new SolidColorBrush(Colors.Black);
            buttonInstalled.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3399FF"));
            framePages.Navigate(_installedPage);
        }

        private void buttonAvailible_Click(object sender, RoutedEventArgs e)
        {
            buttonAvailible.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3399FF"));
            buttonInstalled.Foreground = new SolidColorBrush(Colors.Black);
            framePages.Navigate(_availiblePage);
        }
    }
}
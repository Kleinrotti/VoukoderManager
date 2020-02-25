using System.Reflection;
using System.Windows;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public string Licence
        {
            get
            {
                return Core.Properties.Resources.Licence;
            }
        }

        public AboutWindow()
        {
            InitializeComponent();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labelName.Content = "VoukoderManager " + version;
            textBlock_licence.Text = Licence;
        }
    }
}
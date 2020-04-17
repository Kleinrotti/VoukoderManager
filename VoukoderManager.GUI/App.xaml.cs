using System.Windows;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            if (e.Args.Length > 0)
            {
                if (e.Args[0] == "--minimized")
                {
                    mainWindow.MinimizeToTray();
                    return;
                }
            }
            mainWindow.Show();
        }
    }
}
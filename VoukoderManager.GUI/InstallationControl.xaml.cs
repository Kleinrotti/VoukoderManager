using System;
using System.Windows.Controls;
using VoukoderManager.Core.Models;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for InstallationControl.xaml
    /// </summary>
    public partial class InstallationControl : UserControl
    {
        public InstallationControl()
        {
            InitializeComponent();
            VKEntry.OperationStatus += Operation_InstallProgressChanged;
            VKProgramEntry.UninstallationFinished += ProgramEntry_UninstallationFinished;
            VKGithubEntry.DownloadProgressChanged += VoukoderEntry_DownloadProgressChanged;
        }

        public event EventHandler<EventArgs> CloseControl;

        private void VoukoderEntry_DownloadProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            barProgress.Visibility = System.Windows.Visibility.Visible;
            barProgress.Value = e.ProgressPercentage;
        }

        private void ProgramEntry_UninstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            listBoxStatus.Items.Add(e.ToString());
        }

        private void Operation_InstallProgressChanged(object sender, ProcessStatusEventArgs e)
        {
            listBoxStatus.Dispatcher.Invoke(() =>
            {
                listBoxStatus.Items.Add(e.StatusMessage);
            });
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            VKEntry.OperationStatus -= Operation_InstallProgressChanged;
            VKProgramEntry.UninstallationFinished -= ProgramEntry_UninstallationFinished;
            VKGithubEntry.DownloadProgressChanged -= VoukoderEntry_DownloadProgressChanged;
            CloseControl?.Invoke(this, e);
        }
    }
}
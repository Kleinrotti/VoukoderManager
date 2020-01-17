using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Controls
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow : Window
    {
        private IEntry _programEntry;
        private List<IVoukoderEntry> _entryList;
        private IVoukoderEntry _selectedEntry;
        private Button _buttonInstall;

        public event EventHandler<InstallEventArgs> InstallEvent;

        public PropertyWindow()
        {
            InitializeComponent();
        }

        public PropertyWindow(IEntry entry)
        {
            InitializeComponent();
            labelTitle.Content = "Properties";
            _programEntry = entry;
            ShowProperties();
        }

        public PropertyWindow(List<IVoukoderEntry> entryList)
        {
            InitializeComponent();
            labelTitle.Content = "Select version to install";
            labelTitle.FontSize = 18.0;
            _entryList = entryList;
            CreateInstallButton();
            CreateFrameworkElement();
        }

        private void CreateInstallButton()
        {
            _buttonInstall = new Button
            {
                Content = "Install",
                Height = 50,
                Width = 100,
                FontSize = 17.0,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Visibility = Visibility.Visible,
                IsEnabled = true
            };
            _buttonInstall.Click += _buttonInstall_Click;
            mainGrid.Children.Add(_buttonInstall);
        }

        private void CreateFrameworkElement()
        {
            var test = new FrameworkElementFactory(typeof(RadioButton));
            test.SetValue(RadioButton.ContentProperty, new Binding("Name"));
            test.SetValue(RadioButton.IsCheckedProperty, false);
            test.SetValue(RadioButton.GroupNameProperty, "version");
            test.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            test.AddHandler(RadioButton.ClickEvent, new RoutedEventHandler(radioButtonClickEvent), true);
            test.SetValue(RadioButton.FontSizeProperty, 17.0);
            test.SetValue(RadioButton.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            var template = new DataTemplate();
            template.VisualTree = test;
            icItems.ItemTemplate = template;
            icItems.ItemsSource = _entryList;
        }

        private void radioButtonClickEvent(object sender, RoutedEventArgs e)
        {
            var t = (RadioButton)e.Source;
            _selectedEntry = (IVoukoderEntry)t.DataContext;
        }

        private void _buttonInstall_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == null)
                return;
            InstallEventArgs args = new InstallEventArgs
            {
                PackageToInstall = _selectedEntry
            };
            OnInstallRequest(args);
            Close();
        }

        private void OnInstallRequest(InstallEventArgs e)
        {
            InstallEvent?.Invoke(this, e);
        }

        private void ShowProperties()
        {
            List<IEntry> lst = new List<IEntry>();
            lst.Add(_programEntry);
            icItems.ItemsSource = lst;
        }
    }
}
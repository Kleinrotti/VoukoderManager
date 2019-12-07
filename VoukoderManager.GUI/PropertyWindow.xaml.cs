using System.Collections.Generic;
using System.Windows;
using VoukoderManager.GUI.Models;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow : Window
    {
        private IProgramEntry _programEntry;

        public PropertyWindow()
        {
            InitializeComponent();
        }

        public PropertyWindow(IProgramEntry programEntry)
        {
            InitializeComponent();
            _programEntry = programEntry;
            LoadEntry();
        }

        private void LoadEntry()
        {
            List<IProgramEntry> lst = new List<IProgramEntry>();
            lst.Add(_programEntry);
            icTodoList.ItemsSource = lst;
        }
    }
}
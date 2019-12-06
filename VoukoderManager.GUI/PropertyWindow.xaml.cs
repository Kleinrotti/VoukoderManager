﻿using System.Collections.Generic;
using System.Windows;
using VoukoderManager.GUI.Models;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow : Window
    {
        private ProgramEntry _programEntry;

        public PropertyWindow()
        {
            InitializeComponent();
        }

        public PropertyWindow(ProgramEntry programEntry)
        {
            InitializeComponent();
            _programEntry = programEntry;
            LoadEntry();
        }

        private void LoadEntry()
        {
            List<ProgramEntry> lst = new List<ProgramEntry>();
            lst.Add(_programEntry);
            icTodoList.ItemsSource = lst;
        }
    }
}
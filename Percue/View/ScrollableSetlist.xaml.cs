﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Percue.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Percue.View
{
    /// <summary>
    /// Interaktionslogik für ScrollableSetlist.xaml
    /// </summary>
    public partial class ScrollableSetlist : UserControl
    {
        public ScrollableSetlist()
        {
            InitializeComponent();
        }

        

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;
            if (!(btn.DataContext is Channel ch)) return;
            var metroWindow = (Application.Current.MainWindow as MetroWindow);

            var newName = await metroWindow.ShowInputAsync("Channel Name", "New Name for Channel", settings: new MetroDialogSettings { DefaultText = ch.Name });
            if (newName == "") return;
            ch.Name = newName;

        }
    }
}

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

        

        private async void ChangeName_Click(object sender, RoutedEventArgs e)
        {
            
            if (!(sender is FrameworkElement fe)) return;
            if (!(fe.DataContext is Channel ch)) return;
                
            
            var metroWindow = (Application.Current.MainWindow as MetroWindow);

            var newName = await metroWindow.ShowInputAsync("Channel Name", "New Name for Channel", settings: new MetroDialogSettings { DefaultText = ch.Name });
            if (newName == "") return;
            ch.Name = newName;

        }

        private async void EditHotKey_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement fe)) return;
            if (!(fe.DataContext is Channel ch)) return;

            var metroWindow = (Application.Current.MainWindow as MetroWindow);

            var newHotKey = await metroWindow.ShowInputAsync("New HotKey", "New Hotkey for channel", settings: new MetroDialogSettings { DefaultText = ch.ChannelHotKey.ToString() });
            if (newHotKey == "")
            {
                
                ch.UnsetHotkey();
            }
            else
            {
                ch.UnsetHotkey();
            }
            
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset - e.Delta/10);
            e.Handled = true;
        }

        private void OpenSettingsWindow_Click(object sender, RoutedEventArgs e)
        {
            if(!(sender is FrameworkElement fe)) return;
            if (!(fe.DataContext is Channel ch)) return;

            var settingsWindow = new ChannelControlWindow();
            settingsWindow.DataContext = ch;

            settingsWindow.Show();
        }

        
        private void DeleteChannel_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem mi)) return;
            if (!(mi.DataContext is Channel ch)) return;            
            if (!(DataContext is Setlist sl)) return;
            
            sl.Remove(ch);

        }

        private void MoveChannelRight_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem mi)) return;
            if (!(mi.DataContext is Channel ch)) return;
            if (!(DataContext is Setlist sl)) return;

            var oldPosition = sl.IndexOf(ch);
            if (oldPosition < sl.Count)
                sl.Move(oldPosition, oldPosition + 1);
        }

        private void MoveChannelLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem mi)) return;
            if (!(mi.DataContext is Channel ch)) return;
            if (!(DataContext is Setlist sl)) return;

            var oldPosition = sl.IndexOf(ch);
            if (oldPosition > 0)
                sl.Move(oldPosition, oldPosition - 1);

        }
    }
}

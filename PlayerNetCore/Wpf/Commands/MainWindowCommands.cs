using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NekoPlayer.Wpf.Commands
{
    public static class MainWindowCommands
    {
        public static readonly RoutedUICommand LeftDrawerButtonPress = new RoutedUICommand("", nameof(LeftDrawerButtonPress), typeof(MainWindowCommands));
    }
}

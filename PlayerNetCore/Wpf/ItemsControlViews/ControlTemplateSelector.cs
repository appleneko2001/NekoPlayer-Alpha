using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    public class ControlTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate ControlContainer { get; private set; }
        public static DataTemplate SeparatorHeader { get; private set; }
        public static DataTemplate Switchable { get; private set; }
        public static DataTemplate ComboBoxList { get; private set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //FindResource
            if (ControlContainer is null)
                ControlContainer = Application.Current.FindResource("ControlContainer") as DataTemplate;
            if (SeparatorHeader is null)
                SeparatorHeader = Application.Current.FindResource("SeparatorHeader") as DataTemplate;
            if (Switchable is null)
                Switchable = Application.Current.FindResource("Switchable") as DataTemplate;
            if (ComboBoxList is null)
                ComboBoxList = Application.Current.FindResource("ComboBoxList") as DataTemplate;

            if (item is GroupControl)
                return ControlContainer;
            else if (item is SeparatorHeaderControl)
                return SeparatorHeader;
            else if (item is BooleanControl)
                return Switchable;
            else if (item is ComboBoxListControl || item is ButtonControl)
                return ComboBoxList;
            return base.SelectTemplate(item, container);
        }
    }
}

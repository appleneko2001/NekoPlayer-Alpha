using NekoPlayer.Wpf.ItemsControlViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NekoPlayer.Wpf.Widget
{
    /// <summary>
    /// Логика взаимодействия для SegmentedBar.xaml
    /// </summary>
    public partial class SegmentedBar : UserControl
    {
        public SegmentedBar()
        {
            InitializeComponent();
        }
        public SegmentedBar(ProcessedSegment data)
        {
            InitializeComponent();
            SetSegmentData(data);
        }
        public void SetSegmentData(ProcessedSegment data)
        {
            DataContext = data;
            foreach (var item in data?.SegmentParts)
            {
                Grid BarGrid = ItemsControlParent.ItemsPanel.FindName("BarGrid", );
                BarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(item.Percent, GridUnitType.Star) });
            }
        }
        public void SetVisibility (bool param)
        {
            Dispatcher.Invoke(() => Visibility = param ? Visibility.Visible : Visibility.Collapsed);
        }
    }
}

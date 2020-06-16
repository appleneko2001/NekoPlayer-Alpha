using NekoPlayer.Wpf.Interfaces;
using NekoPlayer.Wpf.ModelViews;
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
    /// NormalHeaderWidget.xaml 的互動邏輯
    /// </summary>
    public partial class NormalHeaderWidget : UserControl, IGetWidget
    {
        public event EventHandler OnBackButtonPress;
        public NormalHeaderWidget(NormalHeaderModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
            SetBindingText(null);
        }

        public object GetWidget()
        {
            return FindName("Root");
        }
        public void SetBindingText(Binding data)
        {
            if(data is null)
            {
                BindingOperations.ClearBinding(TextLine, TextBlock.TextProperty);
                TextLine.Dispatcher.Invoke(() => TextLine.SetBinding(TextBlock.TextProperty, new Binding("Text")));
            }
            else
            {
                BindingOperations.ClearBinding(TextLine, TextBlock.TextProperty);
                TextLine.Dispatcher.Invoke(() => TextLine.SetBinding(TextBlock.TextProperty, data));
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnBackButtonPress?.Invoke(this, null);
        }
    }
}

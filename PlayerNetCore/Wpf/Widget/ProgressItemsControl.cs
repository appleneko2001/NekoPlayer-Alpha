using LinqToDB.Tools;
using NekoPlayer.Wpf.ItemsControlViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ColouredProgressBar
{
    public static class VisualTreeHelperExtension
    {
        struct StackElement
        {
            public FrameworkElement Element { get; set; }
            public int Position { get; set; }
        }
        public static IEnumerable<FrameworkElement> FindAllVisualDescendants(this FrameworkElement parent)
        {
            if (parent == null)
                yield break;
            Stack<StackElement> stack = new Stack<StackElement>();
            int i = 0;
            while (true)
            {
                if (i < VisualTreeHelper.GetChildrenCount(parent))
                {
                    FrameworkElement child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                    if (child != null)
                    {
                        if (child != null)
                            yield return child;
                        stack.Push(new StackElement { Element = parent, Position = i });
                        parent = child;
                        i = 0;
                        continue;
                    }
                    ++i;
                }
                else
                {
                    // back at the root of the search
                    if (stack.Count == 0)
                        yield break;
                    StackElement element = stack.Pop();
                    parent = element.Element;
                    i = element.Position;
                    ++i;
                }
            }
        }
    }
}



namespace ColouredProgressBar
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ColouredProgressBar"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ColouredProgressBar;assembly=ColouredProgressBar"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ProgressItemsControl/>
    ///
    /// </summary>
    public class ProgressItemsControl : ItemsControl
    {
        private Grid layoutGrid;
        private Grid stepsGrid;
        private Grid overlayGrid;
        private long total = 0;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            layoutGrid = base.GetTemplateChild("layoutGrid") as Grid;
            overlayGrid = base.GetTemplateChild("overlayGrid") as Grid;

            DependencyPropertyDescriptor dpdValue = DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(ProgressItemsControl));
            dpdValue.AddValueChanged(this, (o, e) =>
            {
                double margin;
                if (total != 0)
                    margin = layoutGrid.ActualWidth * Value / total;
                else
                    margin = layoutGrid.ActualWidth;
                overlayGrid.Margin = new Thickness(margin, 0, 0, 0);
                margin += 1;
            });
            DependencyPropertyDescriptor dpdStepItems = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(ProgressItemsControl));
            dpdStepItems.AddValueChanged(this, (o, e) =>
            {
                InitItemsSource();
            });
            overlayGrid.SizeChanged += (o, e) =>
            {
                overlayGrid.Width = layoutGrid.ActualWidth;
            };
        }
        private void InitItemsSource()
        {
            stepsGrid = this.FindAllVisualDescendants()
                            .OfType<Grid>()
                            .FirstOrDefault(elt => elt.Name == "stepsGrid");
            if (stepsGrid == null)
                return;
            stepsGrid.ColumnDefinitions.Clear();
            total = 0;
            if (ItemsSource != null)
            {
                SetColumnWidths();
                //SetGradientsStops();
            }
        }
        /*private void SetGradientsStops()
        {
            int count = ((IEnumerable<SegmentPart>)ItemsSource).Count();
            //GradientStopCollection gradientStops = new GradientStopCollection();
            long currentTotal = 0;
            foreach (SegmentPart stepItem in ItemsSource)
            {
                //gradientStops.Add(new GradientStop() { Color = stepItem.Color, Offset = (double)(currentTotal + stepItem.Data / 2) / total });
                currentTotal += stepItem.Data;
            }
            //LinearGradientBrush backBrush = new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5) };
            //backBrush.GradientStops = gradientStops;
            //layoutGrid.Background = backBrush;
        }*/
        private void SetColumnWidths()
        {
            int i = 0;
            foreach (SegmentPart stepItem in ItemsSource)
            {
                total += stepItem.Data;
                var data = stepItem.Percent is double.NaN ? 0 : stepItem.Percent;
                var columnDefinition = new ColumnDefinition() { Width = new GridLength(data, GridUnitType.Star) };
                stepsGrid.ColumnDefinitions.Add(columnDefinition);
                var uiElement = stepsGrid.Children[i] as UIElement;
                if (uiElement != null)
                {
                    Grid.SetColumn(uiElement, stepItem.Index);
                }
                i++;
            }
        }

        internal void SetSegmentData(ProcessedSegment data)
        {
            Dispatcher.Invoke(() => {
                ItemsSource = data.SegmentParts;
                long maximum = 0;
                foreach(var item in data.SegmentParts)
                {
                    if (item.Data >= maximum)
                        maximum = item.Data;
                }
            });
        }
        internal void SetVisibility(bool param) => Dispatcher.Invoke(() => Visibility = param ? Visibility.Visible : Visibility.Collapsed);
        internal void SetMaximum(long param) => Dispatcher.Invoke(() => Maximum = param);
        static ProgressItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressItemsControl), new FrameworkPropertyMetadata(typeof(ProgressItemsControl)));
        }
        public long Value
        {
            get { return (long)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(long), typeof(ProgressItemsControl), new PropertyMetadata(0L));
        public long Maximum
        {
            get { return (long)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(long), typeof(ProgressItemsControl), new PropertyMetadata(100L));
    }
}

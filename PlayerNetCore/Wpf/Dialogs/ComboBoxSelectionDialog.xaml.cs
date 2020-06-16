using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Globalization;

namespace NekoPlayer.Wpf.Dialogs
{
    public class ComboBoxListItem : ViewModelBase
    {
        private bool m_IsChecked;
        public ComboBoxListItem(object value, string displayName = "")
        {
            Value = value;
            DisplayName = displayName;
        }
        public object Value { get; }
        public string InternalName => Value?.ToString();
        public string DisplayName { get; }
        public string Name { get { return DisplayName; } }
        public bool IsChecked { get => m_IsChecked; set { m_IsChecked = value; OnPropertyChanged(); } }
    }
    public partial class ComboBoxSelectionDialog : UserControl, INotifyPropertyChanged
    {
        public string SelectedItemTag;
        public string Header { get; private set; }
        private DialogSession dialogSession;
        private ObservableCollection<ComboBoxListItem> collection = new ObservableCollection<ComboBoxListItem>();
        public ObservableCollection<ComboBoxListItem> Menus => collection;
        public MessageBoxResult Result = MessageBoxResult.Cancel;

        public ComboBoxSelectionDialog() 
        {
            InitializeComponent();
            Header = "Error";
        }
        public void SetSession(DialogSession session)
        {
            dialogSession = session;
        }
        public void SetMenus(List<ComboBoxListItem> menus)
        {
            if (menus is null)
                return;
            Menus.Clear();
            foreach(var item in menus)
            {
                Menus.Add(item);
                item.IsChecked = false;
            }
        }
        public void SetSelected(string internName)
        {
            var result = Menus.Where((o) => o.InternalName == internName);
            if(result.Any())
            {
                var item = result.First();
                item.IsChecked = true;
                SelectedItemTag = internName;
            }
        }
        public void SetHeader(string text)
        {
            Header = text;
            OnPropertyChanged(nameof(Header));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if(button != null)
            {
                var tag = button.Tag as object;
                SelectedItemTag = tag?.ToString();
                Result = MessageBoxResult.OK;
            }
            dialogSession.Close();
        }
        public object GetItem(string tag)
        {
            var i = Menus.Where(v => v.InternalName == tag);
            if (i is null || !i.Any())
                return null;
            return i.First().Value;
        }
    }
}

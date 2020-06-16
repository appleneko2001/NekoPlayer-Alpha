using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NekoPlayer.Wpf.Dialogs
{
    /// <summary>
    /// A dialog with textbox field, Working in Progress (WIP) still.
    /// and DialogHost is Required for use this widget.
    /// </summary>
    public partial class TextFieldDialog : UserControl, INotifyPropertyChanged
    {
        #region Properties for binding
        public string Header { get; private set; } = "Header";
        public string Hint { get; private set; } = "Hint";
        private string m_Text;
        public string Text { get { return m_Text; } set { m_Text = value; OnPropertyChanged(nameof(Text)); } } 
        // this property can be changed directly and binding TwoWay mode.
        #endregion
        /// <summary>
        /// This property will not work, if you use it on DialogHost. Still in Working in Progress (WIP) but at least it can be work with DialogHost perfectly now.
        /// </summary>
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;
        public TextFieldDialog()
        {
            InitializeComponent();
        }
        public void SetHeader(string text)
        {
            Header = text;
            OnPropertyChanged(nameof(Header));
        }
        public void SetHint(string text)
        {
            Hint = text;
            OnPropertyChanged(nameof(Hint));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

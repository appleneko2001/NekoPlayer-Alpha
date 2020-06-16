using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Wpf.ModelViews
{
    public class NormalHeaderModel : ViewModelBase
    {
        public NormalHeaderModel(string str = null, string defText = null, Action onClick = null, Predicate<object> onCanExecute = null)
        {
            m_DefaultText = defText;
            Text = string.IsNullOrWhiteSpace(str) ? m_DefaultText : str;
            if (onClick is null) 
                onClick = () => { }; // Pass a empty method
            
            BackCommand = new RelayCommand((obj => onClick()), onCanExecute ?? (e => false));
        }
        public void SetText(string str)
        {
            Text = string.IsNullOrWhiteSpace(str) ? m_DefaultText : str;
            OnPropertyChanged(nameof(Text));
        }
        public void SetDefaultText(string str)
        {
            m_DefaultText = str;
            OnPropertyChanged(nameof(Text));
        }

        public RelayCommand BackCommand { get; private set; }
        private string m_DefaultText = "Widget";
        private bool m_BackButton = false;
        public bool BackButton { get { return m_BackButton; } set { m_BackButton = value; OnPropertyChanged(); } }
        public string Text { get; private set; }
    }
}

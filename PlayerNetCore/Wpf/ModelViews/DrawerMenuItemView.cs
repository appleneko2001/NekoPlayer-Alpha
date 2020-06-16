using MaterialDesignThemes.Wpf;
using NekoPlayer.Wpf.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Wpf.ModelViews
{
    public class DrawerMenuItemView : ViewModelBase
    {
        public DrawerMenuItemView(string text, object content, Nullable<PackIconKind> icon = null, string toolTip = null, string tag = null)
        {
            Text = text;
            HeaderContent = content;
            Icon = icon;
            Hint = toolTip;
            Tag = tag;
        }
        public PackIconKind? Icon { get => m_Icon; private set { m_Icon = value; OnPropertyChanged(); } }
        public string Text { get => m_Text; private set { m_Text = value; OnPropertyChanged(); } }
        public string Hint { get => m_Hint; private set { m_Hint = value; OnPropertyChanged(); } }
        public object HeaderContent { get => m_HeaderContent; private set { m_HeaderContent = value; OnPropertyChanged(); } }
        public object WindowContent { get => ((IGetPageContent)m_HeaderContent).PageContent; }
        public string Tag { get => m_Tag; private set { m_Tag = value; OnPropertyChanged(); } }

        private PackIconKind? m_Icon;
        private string m_Text;
        private string m_Hint;
        private object m_HeaderContent;
        private string m_Tag;
    }
}

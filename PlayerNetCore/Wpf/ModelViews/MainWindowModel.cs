using MaterialDesignThemes.Wpf;
using NekoPlayer.Pages;
using NekoPlayer.Wpf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using NekoPlayer.Wpf.Interfaces;
using NekoPlayer.Globalization;

namespace NekoPlayer.Wpf.ModelViews
{
    public class MainWindowModel : ViewModelBase
    {        
        private int m_SelectedIndex;
        private object m_HeaderContext;
        public MainWindowModel()
        {
            JumpPage("Home");
        }

        public void SetHeaderContext(object content)
        {
            HeaderContext = content;
        }

        public ObservableCollection<DrawerMenuItemView> DrawerMenus { get; private set; }
            = new ObservableCollection<DrawerMenuItemView>() { 
                new DrawerMenuItemView(LanguageManager.RequestNode("home.header"), new WindowMainPage(), PackIconKind.Home, null, "Home"),
                new DrawerMenuItemView(LanguageManager.RequestNode("settings.header"), new SettingsPage(), PackIconKind.Wrench, null, "Settings")
            };
        public void JumpPage (int index = 0)
        {
            JumpPage(DrawerMenus[index]);
        }
        public void JumpPage(string tag = "")
        {
            JumpPage(DrawerMenus.Where(f => f.Tag == tag).FirstOrDefault());
        }
        public void JumpPage(DrawerMenuItemView item)
        {
            SelectedItem = item;
            SelectedIndex = DrawerMenus.IndexOf(item);
            var header = item?.HeaderContent as IGetHeader;
            if (header?.ContainHeaderObject() ?? false)
            {
                SetHeaderContext(header.GetHeaderObject());
            }
        }
        DrawerMenuItemView SelectedItem;

        public int SelectedIndex { get { return m_SelectedIndex; } set { m_SelectedIndex = value; OnPropertyChanged(); } }
        public string GetTag() => SelectedItem.Tag;
        public object GetUserControlObject() => SelectedItem.WindowContent;
        public object HeaderContext { get { return m_HeaderContext; } private set { m_HeaderContext = value; OnPropertyChanged(); } } 
    }
}

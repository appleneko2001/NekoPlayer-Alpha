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
using NekoPlayer.Globalization;
using NekoPlayer.Wpf.Interfaces;
using NekoPlayer.Wpf.ModelViews;
using NekoPlayer.Wpf.Widget;

namespace NekoPlayer.Pages
{
    public partial class SettingsPage : UserControl, IGetHeader, IGetPageContent
    {
        private NormalHeaderWidget header;
        private NormalHeaderModel headerContext;
        private SettingsPageModel datas;
        public SettingsPage()
        {
            headerContext = new NormalHeaderModel(null, LanguageManager.RequestNode("settings.header"));
            header = new NormalHeaderWidget(headerContext);
            datas = new SettingsPageModel();
            InitializeComponent();
            Root.DataContext = datas;
        }

        public ViewModelBase GetHeaderContext() => headerContext;
        public object GetHeaderObject() => header;
        public object PageContent => Content;
    }
}

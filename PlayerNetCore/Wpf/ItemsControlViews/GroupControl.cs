using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    public class GroupControl : ViewModelBase
    {
        /// <summary>
        /// Create a grouped widget, can be contains other widget except GroupControl (use SeparatorHeader instead).
        /// </summary>
        /// <param name="header">Text below separator. Required field.</param>
        public GroupControl(string header, object[] widgets)
        {
            Header = header;
            Widgets = new List<object>(widgets);
        }
        public string Header { get; private set; }
        public List<object> Widgets { get; private set; }
    }
}

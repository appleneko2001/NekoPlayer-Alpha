using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    public class SeparatorHeaderControl : ViewModelBase
    {
        /// <summary>
        /// Create a separator with header widget.
        /// </summary>
        /// <param name="header">Text below separator. Required field.</param>
        public SeparatorHeaderControl(string header)
        {
            Header = header;
        }
        public string Header { get; private set; }
    }
}

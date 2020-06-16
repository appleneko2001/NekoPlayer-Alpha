using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace NekoPlayer.Wpf.MarkupExtensions
{
    public class GetAppVersion : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return PlayerNetCore.App.CurrentVersion;
        }
    }
}

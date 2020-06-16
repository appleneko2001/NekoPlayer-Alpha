
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace NekoPlayer.Wpf.Commands
{
    public class RelayCommandParams : MarkupExtension
    {
        [ConstructorArgument("parameter1")]
        public object Parameter1 { get; set; }
        [ConstructorArgument("parameter2")]
        public object Parameter2 { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

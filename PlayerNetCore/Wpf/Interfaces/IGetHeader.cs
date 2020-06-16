using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Wpf.Interfaces
{
    public interface IGetHeader
    {
        public bool ContainHeaderObject() => true;
        public object GetHeaderObject() ;
        public ViewModelBase GetHeaderContext();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.ModelViews
{
    public class ProcedureDialogModel : ViewModelBase
    {
        public ProcedureDialogModel(string header, CompositeCollection menus)
        {
            Header = header;
            Menus = menus;
        }

        public string Header { get; private set; }
        public CompositeCollection Menus { get; private set; }
    }
}

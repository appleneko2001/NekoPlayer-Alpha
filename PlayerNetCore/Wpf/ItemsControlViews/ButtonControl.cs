using MaterialDesignThemes.Wpf;
using System;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    public class ButtonControl : BaseButtonControl
    {
        /// <summary>
        /// Create a button widget
        /// </summary>
        /// <param name="text">The text will shown on inside.</param>
        /// <param name="description">Caution for show description. Can be null for show text only.</param>
        /// <param name="onClick">Event after clicks. Can be null for just fun, nothing will happen.</param>
        /// <param name="iconKind">Icon kind for show icon. Can be null for invisibility.</param>
        public ButtonControl(string text, string description = null, PackIconKind? iconKind = null, Action onClick = null) : base (text, description, iconKind)
        {
            onClickMethodBind = onClick;
        }
        
        public RelayCommand OnClick => new RelayCommand((obj) => onClickMethodBind?.Invoke());

        private Action onClickMethodBind;
    }
}

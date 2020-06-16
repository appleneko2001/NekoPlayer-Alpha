using MaterialDesignThemes.Wpf;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    /// <summary>
    /// An basic object for implements any other types based on it (an button, switch, selection list or etc. types).
    /// </summary>
    public class BaseButtonControl : ViewModelBase
    {
        /// <summary>
        /// Initialize button.
        /// </summary>
        /// <param name="text">The text will shown on inside.</param>
        /// <param name="description">Caution for show description. Can be null for show text only.</param>
        /// <param name="iconKind">Icon kind for show icon. Can be null for invisibility.</param>
        public BaseButtonControl(string text, string description = null, PackIconKind? iconKind = null)
        {
            Text = text;
            Description = description;
            Icon = iconKind;
        }
        public PackIconKind? Icon { get; private set; }
        public string Text { get; }
        public string Description { get; }
    }
}

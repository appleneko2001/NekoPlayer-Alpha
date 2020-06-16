using System;
using MaterialDesignThemes.Wpf;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    public class BooleanControl : BaseButtonControl
    {
        private string internalName;
        private bool value;
        private Predicate<object> isSwitchableCheck;
        private Action<bool> onValueChanged;
        /*
        /// <summary>
        /// Create a simple switchable widget.
        /// </summary>
        /// <param name="internalName">Internal name, for storing state from settings.</param>
        /// <param name="text">Display name, or text.</param>
        /// <param name="description">Caution</param>
        /// <param name="value">Default value</param>
        public BooleanControl(string internalName, string text, string description = null, bool value = false) : base(text, description, null)
        {
            this.internalName = internalName;
            this.value = SettingsManager.GetValue<bool>(internalName, value);
        }*/
        /// <summary>
        /// Create a switchable widget
        /// </summary>
        /// <param name="internalName">Internal name, for storing state from settings. Required field, recommended use nameof keywords if supported.</param>
        /// <param name="text">Display name, or text. Required field.</param>
        /// <param name="description">Caution for show description. can be null for show text only.</param>
        /// <param name="value">Default value</param>
        /// <param name="iconKind">Icon kind for show icon, can be null to make it invisible</param>
        /// <param name="isSwitchableCheck">Event for validate that are allowed to toggle or not, object parameter and return bool are required. Or just do not provide to make it always can be toggled.</param>
        /// <param name="onValueChanged">Event for notify after toggle changes value. Parameter are not very important, nullable.</param>
        public BooleanControl(string internalName, string text, string description = null, bool value = false, PackIconKind? iconKind = null, Predicate<object> isSwitchableCheck = null, Action<bool> onValueChanged = null)
            : base (text, description, iconKind)
        {
            this.internalName = internalName;
            this.value = SettingsManager.GetValue<bool>(internalName, value);
            this.isSwitchableCheck = isSwitchableCheck;
            this.onValueChanged = onValueChanged;
        }
        public object Value { get => value; set { this.value = (bool)value; OnPropertyChanged(); SettingsManager.SetValue(internalName, Value); } }
        public RelayCommand OnClick => new RelayCommand((obj) => { Value = !(bool)Value; onValueChanged?.Invoke((bool)Value); }, (obj) => { if (isSwitchableCheck != null) return isSwitchableCheck.Invoke(obj); else return true; });
    }
}

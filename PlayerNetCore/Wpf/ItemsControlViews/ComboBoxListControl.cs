using NekoPlayer.Wpf.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using NekoPlayer.Core.Engine;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    public class ComboBoxListControl : BaseButtonControl
    {
        public static List<ComboBoxListItem> GenerateListFromGetLanguagePacks(Dictionary<string, string> dicts)
        {
            var list = new List<ComboBoxListItem>();
            if (dicts is null)
                return list;

            foreach(var item in dicts)
            {
                list.Add(new ComboBoxListItem(item.Key, item.Value));
            }

            return list;
        }
        public static List<ComboBoxListItem> GenerateListFromGetBassDevices(IEnumerable<BassDevice> devices)
        {
            var list = new List<ComboBoxListItem>();
            if (devices is null)
                return list;
            foreach (var item in devices)
            {
                list.Add(new ComboBoxListItem(item.DeviceId, item.DeviceName));
            }

            return list;
        }

        public ComboBoxListControl(string internalName, string text, List<ComboBoxListItem> items, string description = null, string defValue = "", PackIconKind? iconKind = null, Predicate<object> isClickableCheck = null, Action<object, EventArgs> onDialogClosedEvent = null)
            : base(text, description, iconKind)
        {
            this.internalName = internalName;
            this.value = SettingsManager.GetValue<string>(internalName, value) ?? defValue;
            ListItems = items;
            this.isClickableCheck = isClickableCheck;
            this.onDialogClosedEvent = onDialogClosedEvent;
        }
        public ComboBoxListControl(string internalName, string text, Func<List<ComboBoxListItem>> dynamicGenerateItems, string description = null, string defValue = "", PackIconKind? iconKind = null, Predicate<object> isClickableCheck = null, Action<object, EventArgs> onDialogClosedEvent = null)
            : base(text, description, iconKind)
        {
            this.internalName = internalName;
            this.value = SettingsManager.GetValue<string>(internalName, value) ?? defValue;
            //ListItems = items;
            dynamicGenerate = true;
            this.dynamicGenerateItems = dynamicGenerateItems;
            this.isClickableCheck = isClickableCheck;
            this.onDialogClosedEvent = onDialogClosedEvent;
        }
        private bool dynamicGenerate = false;
        private string value;
        private string internalName;
        private Predicate<object> isClickableCheck;
        private Action<object, EventArgs> onDialogClosedEvent;
        private Func<List<ComboBoxListItem>> dynamicGenerateItems;
        public List<ComboBoxListItem> ListItems { get; private set; }
        public object Value { get => value; set { this.value = (string)value; OnPropertyChanged(); SettingsManager.SetValue(internalName, Value); } }
        public RelayCommand OnClick => new RelayCommand((obj) => ShowDialog(), (obj) => { if (isClickableCheck != null) return isClickableCheck.Invoke(obj); else return true; });
        private ComboBoxSelectionDialog DialogInstance;
        async void ShowDialog()
        {
            if(DialogInstance is null)
            {
                DialogInstance = new ComboBoxSelectionDialog();
            }
            if (dynamicGenerate)
            {
                if (ListItems != null)
                {
                    ListItems.Clear();
                    ListItems = null;
                }
                ListItems = dynamicGenerateItems();
            }
            DialogInstance.SetMenus(ListItems);
            if (string.IsNullOrEmpty(value))
                value = SettingsManager.GetValue<string>(internalName);
            DialogInstance.SetSelected(value);
            DialogInstance.SetHeader(Text);
            await DialogHost.Show(DialogInstance, (sender, eventArgs) => DialogInstance.SetSession(eventArgs.Session), OnDialogClosed).ConfigureAwait(false);
        }
        void OnDialogClosed(object sender, DialogClosingEventArgs args)
        {
            if (DialogInstance.Result == System.Windows.MessageBoxResult.OK)
            {
                Value = DialogInstance.SelectedItemTag;
                onDialogClosedEvent?.Invoke(this, null);
            }
            DialogInstance = null;
        }
    }
}

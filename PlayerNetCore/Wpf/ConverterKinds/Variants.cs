using MaterialDesignThemes.Wpf;
using NekoPlayer.Globalization;
using NekoPlayer.Wpf.Converters;

namespace NekoPlayer.Wpf.ConverterKinds
{
    public static class Variants
    {
        /// <summary>
        /// Load or replace all icons variants instance
        /// </summary>
        public static void LoadPackIconVariants()
        {
            PlayPauseVariants = new BTPVariants(PackIconKind.Pause, PackIconKind.Play);
            ShuffleVariants = new BTPVariants(PackIconKind.ShuffleVariant, PackIconKind.ShuffleDisabled);
            MuteVariants = new BTPVariants(PackIconKind.VolumeMute, PackIconKind.VolumeHigh);

            RepeatVariants = new ITPVariants(new PackIconKind[3] { PackIconKind.RepeatOff, PackIconKind.Repeat, PackIconKind.RepeatOne });
        }
        /// <summary>
        /// Load or replace all string variants instance
        /// </summary>
        public static void LoadStringVariants()
        {
            SwitchTextVariants = new ITSVariant(new string[] { LanguageManager.RequestNode("common.off"), LanguageManager.RequestNode("common.on") });
            RepeatSwitchTextVariants = new ITSVariant(new string[] { LanguageManager.RequestNode("common.off"), LanguageManager.RequestNode("player.repeatplaylist"), LanguageManager.RequestNode("player.repeatone") });
        }
        public static BTPVariants PlayPauseVariants;
        public static BTPVariants ShuffleVariants;
        public static BTPVariants MuteVariants;

        public static ITPVariants RepeatVariants;

        public static ITSVariant SwitchTextVariants;
        public static ITSVariant RepeatSwitchTextVariants;
    }
}

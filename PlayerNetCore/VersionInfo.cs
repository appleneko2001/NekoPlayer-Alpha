using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.VersionInfos
{
    /// <summary>
    /// Interface implements for encapsulating.
    /// </summary>
    public interface IUpdateChangesInfo
    {
        /// <summary>
        /// Changelog content
        /// </summary>
        public string ChangesText { get; }
        /// <summary>
        /// Prefix
        /// </summary>
        public string ChangesTitle { get; }
        /// <summary>
        /// Icon
        /// </summary>
        public PackIconKind? IconKind { get; }
    }
    /// <summary>
    /// A node that contains detail of added new features.
    /// Changelog node type.
    /// </summary>
    public class AddedFeature : IUpdateChangesInfo
    {
        public AddedFeature(string text)
        {
            ChangesText = text;
        }
        public string ChangesText { get; private set; }

        public string ChangesTitle => "Added";

        public PackIconKind? IconKind =>  PackIconKind.Add;
    }
    /// <summary>
    /// A node that contains detail of improvement and changes.
    /// Changelog node type.
    /// </summary>
    public class ImproveFeature : IUpdateChangesInfo
    {
        public ImproveFeature(string text)
        {
            ChangesText = text;
        }
        public string ChangesText { get; private set; }

        public string ChangesTitle => "Improved";

        public PackIconKind? IconKind => PackIconKind.Wrench;
    }
    /// <summary>
    /// A node that contains detail of problem and bugs fixes.
    /// Changelog node type.
    /// </summary>
    public class BugFixes : IUpdateChangesInfo
    {
        public BugFixes(string text)
        {
            ChangesText = text;
        }
        public string ChangesText { get; private set; }

        public string ChangesTitle => "Fixed";

        public PackIconKind? IconKind => PackIconKind.Wrench;
    }
    /// <summary>
    /// A node that contains detail of feature are no longer will be use and remove.
    /// Changelog node type.
    /// </summary>
    public class RemovedFeature : IUpdateChangesInfo
    {
        public RemovedFeature(string text)
        {
            ChangesText = text;
        }
        public string ChangesText { get; private set; }

        public string ChangesTitle => "Removed";

        public PackIconKind? IconKind => PackIconKind.Remove;
    }
    public class HintInfoFeature : IUpdateChangesInfo
    {
        public HintInfoFeature(string text)
        {
            ChangesText = text;
        }
        public string ChangesText { get; private set; }

        public string ChangesTitle => "Hint";

        public PackIconKind? IconKind => null;
    }
    /// <summary>
    /// A ChangelogInfo object that contains version string and changes. With GC Ready (Dynamic construct and destructring)
    /// actually idk it will work or not about GC Cleaning I wish it will work plz  ;_;
    /// </summary>
    public sealed class ChangelogInfo : IDisposable
    {
        private bool Disposed;
        /// <summary>
        /// Create object.
        /// </summary>
        /// <param name="verName">Method for get version string.</param>
        /// <param name="changeinfos">Method for get change infos. Can be collected by method GarbageCollect(true)</param>
        public ChangelogInfo(Func<string> verName, Func<List<IUpdateChangesInfo>> changeinfos)
        {
            this.verName = verName;
            this.changeinfos = changeinfos;
        }
        private Func<string> verName;
        private Func<List<IUpdateChangesInfo>> changeinfos;
        public object[] instances = new object[2];
        public string DisplayVersion { get { if(instances[0] == null) instances[0] = verName?.Invoke(); return (string)instances[0]; } }
        public List<IUpdateChangesInfo> ChangeInfos { get { if (instances[1] == null) instances[1] = changeinfos?.Invoke(); return instances[1] as List<IUpdateChangesInfo>; } }
        public void GarbageCollect(bool keepTitle = false)
        {
            if(!keepTitle)
            {
                instances[0] = null;
            }
            List<IUpdateChangesInfo> lst = instances[1] as List<IUpdateChangesInfo>;
            lst?.Clear();
            instances[1] = null;
        }
        public override string ToString()
        {
            return DisplayVersion;
        }
        /// <summary>
        /// Call this to completely destruct this object.
        /// </summary>
        public void Dispose()
        {
            if (Disposed)
                return;

            GarbageCollect();
            Disposed = true;
            verName = null;
            changeinfos = null;
            instances = null;
        }
    }
    /// <summary>
    /// Version information provider. Contains version string, and changelog details.
    /// </summary>
    public static class VersionInfo
    {
        public static ChangelogInfo CurrentVersion { get; } = new ChangelogInfo(() => "2020.06.16_Alpha", () => new List<IUpdateChangesInfo>()
        {
            new AddedFeature("Cache management (Show usage and clear)."),
            new AddedFeature("Cover image processing for minimize RAM usage and scaling size."),
            new AddedFeature("Media hotkeys trigger (Play/Pause, next track and previous track button on keyboard)."),
            new AddedFeature("Now playing list."),
            new ImproveFeature("Only one instance of player are allowed to prevent any major/minor errors."),
            new ImproveFeature("Animations transitioners."),
            new ImproveFeature("Changed BASS Audio library API provider as ManagedBass for some reasons."),
            new ImproveFeature("Cover image will be load when required to reduce RAM usage."),
            new ImproveFeature("Context program structure, and BASS host controller slightly changed for fixes bugs."),
            new ImproveFeature("Encapsulated playlist class for less repeatment of code."),
            new ImproveFeature("Hash calculation mechanism changed for reduce RAM usage."),
            new ImproveFeature("Merged repeated codes on some source code files."),
            new ImproveFeature("Settings page mechanism slightly changed."),
            new ImproveFeature("Slightly changed styles and templates of flat button."),
            new ImproveFeature("Slightly changed data context model to support some functionality."),
            new ImproveFeature("Support show full exceptions by startup argument /verbose. Console support are so ugly but it just works now."),
            new ImproveFeature("UX Improvement: Back button shortcut on home page."),

            new BugFixes("Main-Window cloning, program closes when device fails, program suddenly closes when reload with enabled feature \"Close program when window closed\", and etc."),
            new BugFixes("Old cache linking cannot be overwrited (Fix restored wrong status)."),
            new HintInfoFeature("You have to run once link correction to correct all tags caches (Player window will reload)."),
            new HintInfoFeature("After image processing can increase RAM usage. Now we can't fix it on recently updates by cannot dispose object reason."),
            new HintInfoFeature("Program barely working without smooth movement because transitioner are too laggy, could be fixed on QT version (Future updates)."),
            new HintInfoFeature("The stupid BUG Master are creating bugs again, now we fixed some minor bugs (Features working abnormally). We still have much work and challenges to do for improving experiences.")
        });
        /// <summary>
        /// Incomplete method.
        /// </summary>
        public static void LoadOldChangelogs()
        {

        }
        public static void DoOptimize()
        {
            CurrentVersion.GarbageCollect(true);
        }
        /*
         * Old changelogs.
         * 
        public static ChangelogInfo 2020.05.07_Alpha { get; } = new ChangelogInfo(() => "2020.05.07_Alpha", () => new List<IUpdateChangesInfo>()
        {
            new AddedFeature("Maximize album picture by place cursor to the image widget."),
            new AddedFeature("Settings: Size preview album picture."),
            new AddedFeature("Search real tags on Internet (for now only one API can be used)."),
            new AddedFeature("Internet tags caching (Track info and picture album)."),
            new ImproveFeature("UI Improvement: Settings page, dialogs changed."),
            new ImproveFeature("Mechanism widget slightly changed."),
            new HintInfoFeature("Memory requirement increased, we are trying to find problem about memory leaks and apply fixes.")
        });
        public static ChangelogInfo 2020.04.07_Alpha { get; } = new ChangelogInfo(() => "2020.04.07_Alpha", () => new List<IUpdateChangesInfo>()
        {
            new AddedFeature("Output device can be chosen by settings."),
            new ImproveFeature("UX Improvement: Tab foreach path optimization."),
            new ImproveFeature("After changes language will restart window automatically and keep host working."),
            new ImproveFeature("Icon and tray icon updated."),
            new ImproveFeature("Minor changes on playback host controller."),
            new ImproveFeature("Window can be closed for reload, not hide."),
        });
        public static List<List<IUpdateChangesInfo>> OldChangelogs { get; } = new List<List<IUpdateChangesInfo>>()
        {
            new List<IUpdateChangesInfo>()
            {
                new AddedFeature("Changelog"),
                new ImproveFeature("UI Improvement: Corner of dialog, lists."),
                new ImproveFeature("UI Improvement: Ripple effect more quickly."),
                new ImproveFeature("UI Improvement: Dialog effect using fade-in and fade-out instead now."),
                new HintInfoFeature("We modified the material design ui toolbox to improvements."),
                new ImproveFeature("UI Improvement: Borders on inner the playlist button has removed."),
                new ImproveFeature("UX Improvement: Rotate the mouse wheel to adjust volume."),
                new ImproveFeature("UX Improvement: Space button to control play or pause."),
                new BugFixes("Cannot unload the old playable sometimes, and it will be uncontrollable until it playing to end."),
                new BugFixes("Ripple effect sometimes will slow down when it almost filled."),
                new BugFixes("The recent playlist will sorted correctly on load."),
                new RemovedFeature("Unused resources for minimize size of application."),
                new RemovedFeature("Now playing list has removed temporarily due to it doesn't implemented.")
            }
        };*/
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace NekoPlayer.Core.Utilities
{
    /// <summary>
    /// A component that used for register the hotkey, it will works even without any window.
    /// For start to use should call the <see cref="InitializeComponent(int)"/> first.
    /// </summary>
    public class HotKeyBinding
    {
        private readonly static WeakReference<HotKeyBinding> instance = new WeakReference<HotKeyBinding>(null);

        public static HotKeyBinding GetInstance()
        {
            if (instance.TryGetTarget(out var result))
                return result;
            return null;
        }
        private int StartId;
        private Dictionary<int, ICommand> CommandBindings;
        public static void InitializeComponent(int startId = 0xC460)
        {
            var obj = new HotKeyBinding();
            obj.StartId = startId;
            obj.CommandBindings = new Dictionary<int, ICommand>();
            ComponentDispatcher.ThreadPreprocessMessage += obj.HwndHook;
            instance.SetTarget(obj);
        }
        /// <summary>
        /// Call this if you want to stop binding service.
        /// And unbind all hotkey automatically.
        /// </summary>
        public static void StopComponent()
        {
            var handler = GetInstance();
            foreach (var item in handler.CommandBindings)
            {
                UnregisterHotKey(IntPtr.Zero, item.Key);
            }
            ComponentDispatcher.ThreadPreprocessMessage -= handler.HwndHook;
            handler.CommandBindings.Clear();
            handler.CommandBindings = null;
            instance.SetTarget(null);
        }
        /// <summary>
        /// Register a hot key.
        /// </summary>
        /// <param name="windowId">Target a window. Can be IntPtr.Zero if you need background binding support.</param>
        /// <param name="hotKey">A key that should be handle.</param>
        /// <param name="modifierKeys"></param>
        /// <param name="command">Command will be used when pressed handled key.</param>
        /// <returns>A binding id that could be unregister when no need anymore.</returns>
        public int Register(IntPtr windowId,Keys hotKey, ModifierKeys modifierKeys, ICommand command)
        {
            int id = StartId;
            while (CommandBindings.ContainsKey(id))
            {
                id++;
            }
            if (!RegisterHotKey(windowId, (int)id, (uint)modifierKeys, (int)hotKey))
            {
                ExceptMessage.PopupExcept($"An exception occurred when binding hotkeys. \r\nPlease ensure no any program are using {hotKey}{(modifierKeys == ModifierKeys.None? "" : "with " + modifierKeys)} button.");
                return -1;
            }
            else
            {
                CommandBindings.Add(id, command);
                return id;
            }
        }
        /// <summary>
        /// Unregister a hot key
        /// </summary>
        /// <param name="windowId">Target a window. Can be IntPtr.Zero if you bind key to background.</param>
        /// <param name="bindingId">Identicator from return of method <see cref="Register(IntPtr, Keys, ModifierKeys, ICommand)"/></param>
        public void Unregister(IntPtr windowId, int bindingId)
        {
            if (CommandBindings.ContainsKey(bindingId))
            { 
                if(!UnregisterHotKey(windowId, bindingId))
                    ExceptMessage.PopupExcept("An exception occurred when unbinding hotkeys. ");
                CommandBindings.Remove(bindingId);
            }
        }

        private void HwndHook(ref MSG msg, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312; // Pressed
            switch (msg.message)
            {
                case WM_HOTKEY:
                    {
                        var id = msg.wParam.ToInt32();
                        if (CommandBindings.ContainsKey(id))
                        {
                            var result = CommandBindings[id];
                            if (result.CanExecute(true)) result.Execute(true);
                            handled = true;
                            break;
                        }
                    }
                    break;
            } 
        }


        #region PInvokes
        [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
        [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion
    }
}

using NekoPlayer.Globalization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NekoPlayer.Core.Utilities
{
    public static class ExceptMessage
    {
        private static bool showFullErrorOnCMD = false;
        public static void SetDebugMode(bool v) => showFullErrorOnCMD = v;
        public static void PopupExcept(object obj, bool critical = false, string additionalMessage = null)
        {
            StringBuilder builder = new StringBuilder();

            if (obj is Exception)
            {
                builder.AppendLine(critical ? LanguageManager.RequestNode("error.critical.header") : LanguageManager.RequestNode("error.common.header"));
                Exception e = obj as Exception;
                var type = obj.GetType().Name;
                builder.AppendLine($"Unhandled {type}: ");
                builder.AppendLine(e.Message);
                if (showFullErrorOnCMD)
                {
                    Console.WriteLine($"Error: {e.Message}\n\nStacktrace: {e.StackTrace}");
                    if(critical)
                        Console.WriteLine($"Program will aborted on this case.");
                }
            }
            else if (obj is ManagedBass.Errors)
            {
                builder.AppendLine(critical ? LanguageManager.RequestNode("error.critical.header") : LanguageManager.RequestNode("error.common.header"));
                ManagedBass.Errors e = (ManagedBass.Errors)obj;
                builder.AppendLine(LanguageManager.RequestNode("error.bass.header"));
                builder.AppendLine(e.ToString());
                if (showFullErrorOnCMD)
                {
                    Console.WriteLine($"Un4seen.Bass Host Error: {e}\n");
                    if (critical)
                        Console.WriteLine($"Program will aborted on this case.");
                }
            }
            else
            {
                builder.AppendLine(obj?.ToString() ?? "");
            }
            if (additionalMessage != null)
                builder.AppendLine(additionalMessage ?? "");
            if (critical)
                builder.AppendLine(LanguageManager.RequestNode("error.critical.suffix"));
            try
            {

                if (MessageBox.Show(builder.ToString(), null, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    if (critical)
                        Application.Current.Shutdown();
                }
            }
            catch
            {

            }
        }
        public static void PrintConsole(object param) => PrintConsole(0,param);
        /// <summary>
        /// Print some texts to console if you have activated verbose mode on startup.
        /// </summary>
        /// <param name="messageType">0 --> Automatically detect  1 --> Info   2 --> Warning  3 --> Error  4 --> Severe (Work in progress...)</param>
        /// <param name="parameters"></param>
        public static void PrintConsole(int messageType = 0, params object[] parameters)
        {
            if (!showFullErrorOnCMD)
                return;
            StringBuilder builder = new StringBuilder();
            if (messageType is 0)
            {
                foreach (var param in parameters)
                {
                    if (param is Exception)
                    {
                        Exception e = param as Exception;
                        builder.AppendLine($"Error: {e.Message}\n\nStacktrace: {e.StackTrace}");
                    }
                    else if (param is ManagedBass.Errors)
                    {
                        ManagedBass.Errors e = (ManagedBass.Errors)param;
                        builder.AppendLine($"Un4seen.Bass Host Error: {e}\n");
                    }
                    else
                    {
                        builder.AppendLine($"Info: {param}");
                    }
                }
            }
            else if(messageType is 1)
            {
                builder.Append($"Info: ");
                foreach (var param in parameters)
                {
                    builder.AppendLine(param.ToString());
                }
            }
            else if (messageType is 2)
            {
                builder.Append($"Warning: ");
                foreach (var param in parameters)
                {
                    builder.AppendLine(param.ToString());
                }
            }
            Console.WriteLine(builder);
        }
    }
}

using NekoPlayer.Globalization;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class LanguagePackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter is string)
            {
                string result = "";
                string v = value?.ToString() ?? "";
                bool anotherMethod = (parameter as string).StartsWith("#", StringComparison.InvariantCulture);
                if (!anotherMethod)
                {
                    result = LanguageManager.RequestNode((parameter as string));
                }
                else
                {
                    string str = (parameter as string).Remove(0, 1);
                    int startPos = 0;
                    bool start = false;
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (!start)
                        {
                            if (str[i] == '[')
                            {
                                startPos = i + 1;
                                start = true;
                                continue;
                            }
                            else if (str[i] == '%')
                                result += v;
                            else
                                result += str[i];
                        }
                        else
                        {
                            if (str[i] == ']')
                            {
                                int length = i - startPos;
                                start = false;
                                string subStr = str.Substring(startPos, length);
                                result += LanguageManager.RequestNode(subStr);
                            }
                        }
                    }
                }
                if (value is string)
                    result += " " + value;
                return result;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

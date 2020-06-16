using NekoPlayer.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace NekoPlayer.Wpf.ValidationRules
{
    public class ValidationNameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = null;
            if (value is null)
                result = new ValidationResult(false, "Value is null.");
            else if(value is string)
            {
                string v = (string)value;
                if (string.IsNullOrWhiteSpace(v))
                    result = new ValidationResult(false, LanguageManager.RequestNode("validaterule.emptyfield"));
                else
                    result = new ValidationResult(true, LanguageManager.RequestNode("validaterule.ok"));
            }
            return result ?? new ValidationResult(false, "Result is null.");
        }
    }
}

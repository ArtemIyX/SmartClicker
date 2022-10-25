using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartClicker_WPF.Validations
{
    public class NotNulTextValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is null)
                return new ValidationResult(false, "Not empty");

            if (string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, "Not empty");

            return ValidationResult.ValidResult;
        }
    }
}

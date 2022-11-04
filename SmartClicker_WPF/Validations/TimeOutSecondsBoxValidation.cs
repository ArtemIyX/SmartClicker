using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartClicker_WPF.Validations
{
    public class TimeOutSecondsBoxValidation : ValidationRule
    {
        private static readonly int RangeMax = 500;
        public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
        {
            if (value is null)
                return new ValidationResult(false, "Not empty");

            if (string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, "Not empty");

            int i;
            if (!int.TryParse(value.ToString(), out i))
                return new ValidationResult(false, "Only numeric");

            if (i > RangeMax)
                return new ValidationResult(false, $"{RangeMax} Max");

            if (i <= 0)
                return new ValidationResult(false, "1 Min");

            return ValidationResult.ValidResult;
        }

    }
}

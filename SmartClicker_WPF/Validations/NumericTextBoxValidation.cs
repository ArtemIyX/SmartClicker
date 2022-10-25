using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartClicker_WPF.Validations
{
    public class NumericTextBoxValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            Regex regex = new Regex("[^0-9]+"); 
            if (regex.IsMatch(value.ToString() ?? ""))
                return new ValidationResult(true, null);

            return new ValidationResult(false, "Please enter a valid integer value.");
        }

    }
}

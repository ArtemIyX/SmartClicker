using System.Globalization;
using System.Windows.Controls;

namespace SmartClicker_WPF.Validations
{
    public class NumericTextBoxValidation : ValidationRule
    {
        public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
        {
            if (value is null)
                return new ValidationResult(false, "Not empty");

            if(string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, "Not empty");

            int i;
            if (int.TryParse(value.ToString(), out i))
                return ValidationResult.ValidResult;


            return new ValidationResult(false, "Only numeric");
        }

    }
}

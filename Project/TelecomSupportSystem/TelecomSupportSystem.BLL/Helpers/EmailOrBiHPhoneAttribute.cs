using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TelecomSupportSystem.BLL.Helpers
{
    public sealed class EmailOrBiHPhoneAttribute : ValidationAttribute
    {
        private static readonly Regex PhoneRegex = new(@"^\+387[0-9]{8,10}$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string input || string.IsNullOrWhiteSpace(input))
                return new ValidationResult("Email ili broj telefona je obavezan.");

            var trimmed = input.Trim();

            if (trimmed.Contains('@'))
            {
                var emailAttr = new EmailAddressAttribute();
                if (emailAttr.IsValid(trimmed))
                    return ValidationResult.Success;

                return new ValidationResult("Unesite ispravnu email adresu.");
            }

            if (PhoneRegex.IsMatch(trimmed))
                return ValidationResult.Success;

            return new ValidationResult("Unesite ispravnu email adresu ili broj telefona u formatu +387XXXXXXXX.");
        }
    }
}

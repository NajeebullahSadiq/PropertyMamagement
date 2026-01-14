using WebAPIBackend.Application.Common.Models;

namespace WebAPIBackend.Application.Common.Validators
{
    /// <summary>
    /// Validator for property type selection
    /// </summary>
    public static class PropertyTypeValidator
    {
        private static readonly HashSet<string> AllowedPropertyTypeNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "House", "Apartment", "Shop", "Block", "Land", "Garden", "Hill", "Other"
        };

        public static ValidationResult Validate(string? propertyTypeName, string? customPropertyType)
        {
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
                return ValidationResult.Invalid("انتخاب نوعیت ملکیت الزامی است");
            }

            if (!AllowedPropertyTypeNames.Contains(propertyTypeName))
            {
                return ValidationResult.Invalid("نوعیت ملکیت انتخاب‌شده درست نیست");
            }

            if (propertyTypeName.Equals("Other", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(customPropertyType))
                {
                    return ValidationResult.Invalid("نوشتن نوع ملکیت (سایر) الزامی است");
                }
            }

            return ValidationResult.Valid();
        }

        public static string? NormalizeCustomPropertyType(string? propertyTypeName, string? customPropertyType)
        {
            if (propertyTypeName?.Equals("Other", StringComparison.OrdinalIgnoreCase) == true)
            {
                return customPropertyType?.Trim();
            }
            return null;
        }
    }
}

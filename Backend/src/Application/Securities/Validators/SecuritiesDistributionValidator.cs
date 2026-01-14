using WebAPIBackend.Application.Common.Models;
using WebAPIBackend.Application.Securities.DTOs;
using WebAPIBackend.Shared.Constants;

namespace WebAPIBackend.Application.Securities.Validators
{
    /// <summary>
    /// Validator for securities distribution data
    /// </summary>
    public static class SecuritiesDistributionValidator
    {
        public static ValidationResult Validate(CreateSecuritiesDistributionRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.RegistrationNumber))
            {
                return ValidationResult.Invalid("نمبر ثبت الزامی است");
            }

            if (string.IsNullOrWhiteSpace(data.LicenseNumber))
            {
                return ValidationResult.Invalid("نمبر جواز الزامی است");
            }

            // Validate document type specific fields
            if (data.DocumentType == DocumentTypes.Property)
            {
                if (!data.PropertySubType.HasValue)
                {
                    return ValidationResult.Invalid("لطفاً نوع سته جایداد را انتخاب کنید");
                }
            }
            else if (data.DocumentType == DocumentTypes.Vehicle)
            {
                if (!data.VehicleSubType.HasValue)
                {
                    return ValidationResult.Invalid("لطفاً نوع سته وسایط نقلیه را انتخاب کنید");
                }
            }

            // Validate registration book type
            if (data.RegistrationBookType == RegistrationBookTypes.Original && !data.RegistrationBookCount.HasValue)
            {
                return ValidationResult.Invalid("لطفاً تعداد کتاب ثبت را وارد کنید");
            }

            if (data.RegistrationBookType == RegistrationBookTypes.Duplicate && !data.DuplicateBookCount.HasValue)
            {
                return ValidationResult.Invalid("لطفاً تعداد کتاب ثبت مثنی را وارد کنید");
            }

            return ValidationResult.Valid();
        }

        /// <summary>
        /// Clear fields that don't apply based on document type
        /// </summary>
        public static void NormalizeFields(CreateSecuritiesDistributionRequest data)
        {
            if (data.DocumentType == DocumentTypes.Property)
            {
                // Clear vehicle fields
                data.VehicleSubType = null;
                data.VehicleSaleCount = null;
                data.VehicleSaleSerialStart = null;
                data.VehicleSaleSerialEnd = null;
                data.VehicleExchangeCount = null;
                data.VehicleExchangeSerialStart = null;
                data.VehicleExchangeSerialEnd = null;
            }
            else if (data.DocumentType == DocumentTypes.Vehicle)
            {
                // Clear property fields
                data.PropertySubType = null;
                data.PropertySaleCount = null;
                data.PropertySaleSerialStart = null;
                data.PropertySaleSerialEnd = null;
                data.BayWafaCount = null;
                data.BayWafaSerialStart = null;
                data.BayWafaSerialEnd = null;
                data.RentCount = null;
                data.RentSerialStart = null;
                data.RentSerialEnd = null;
            }
        }
    }
}

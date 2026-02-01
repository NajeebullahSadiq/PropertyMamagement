namespace WebAPIBackend.Models.Common
{
    /// <summary>
    /// Interface for entities that have province-based access control
    /// Entities implementing this interface will be filtered by province for COMPANY_REGISTRAR users
    /// </summary>
    public interface IProvinceEntity
    {
        /// <summary>
        /// Province ID for province-based access control
        /// </summary>
        int? ProvinceId { get; set; }
    }
}

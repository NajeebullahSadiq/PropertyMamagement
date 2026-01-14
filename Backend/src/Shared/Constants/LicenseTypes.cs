namespace WebAPIBackend.Shared.Constants
{
    /// <summary>
    /// License type constants for company operations
    /// </summary>
    public static class LicenseTypes
    {
        public const string RealEstate = "realEstate";
        public const string CarSale = "carSale";
        public const string Both = "both";
    }

    /// <summary>
    /// Module names for authorization
    /// </summary>
    public static class ModuleNames
    {
        public const string Company = "company";
        public const string Property = "property";
        public const string Vehicle = "vehicle";
        public const string Reports = "reports";
        public const string Dashboard = "dashboard";
        public const string Users = "users";
        public const string Securities = "securities";
    }

    /// <summary>
    /// Document types for securities
    /// </summary>
    public static class DocumentTypes
    {
        public const int Property = 1;
        public const int Vehicle = 2;
    }

    /// <summary>
    /// Registration book types
    /// </summary>
    public static class RegistrationBookTypes
    {
        public const int Original = 1;
        public const int Duplicate = 2;
    }
}

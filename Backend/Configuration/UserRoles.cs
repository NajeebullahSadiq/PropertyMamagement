namespace WebAPIBackend.Configuration
{
    /// <summary>
    /// Defines all user roles in the system
    /// </summary>
    public static class UserRoles
    {
        /// <summary>
        /// مدیر سیستم - Full system access
        /// </summary>
        public const string Admin = "ADMIN";

        /// <summary>
        /// مقام / رهبری - View-only access to all modules
        /// </summary>
        public const string Authority = "AUTHORITY";

        /// <summary>
        /// کاربر ثبت جواز رهنما - Company registration user
        /// </summary>
        public const string CompanyRegistrar = "COMPANY_REGISTRAR";

        /// <summary>
        /// ریاست بررسی و ثبت جواز - License review directorate (view-only company list)
        /// </summary>
        public const string LicenseReviewer = "LICENSE_REVIEWER";

        /// <summary>
        /// کاربر عملیاتی رهنما - املاک - Real estate company operational user
        /// </summary>
        public const string PropertyOperator = "PROPERTY_OPERATOR";

        /// <summary>
        /// کاربر عملیاتی رهنما - موتر فروشی - Vehicle company operational user
        /// </summary>
        public const string VehicleOperator = "VEHICLE_OPERATOR";

        /// <summary>
        /// کاربر مدیریت درخواست جواز - License application manager with full access to license applications and read-only to other modules
        /// </summary>
        public const string LicenseApplicationManager = "LICENSE_APPLICATION_MANAGER";

        /// <summary>
        /// Get all roles as array
        /// </summary>
        public static string[] AllRoles => new[]
        {
            Admin,
            Authority,
            CompanyRegistrar,
            LicenseReviewer,
            PropertyOperator,
            VehicleOperator,
            LicenseApplicationManager
        };

        /// <summary>
        /// Get Dari translation for role
        /// </summary>
        public static string GetDariName(string role)
        {
            return role switch
            {
                Admin => "مدیر سیستم",
                Authority => "مقام / رهبری",
                CompanyRegistrar => "کاربر ثبت جواز رهنما",
                LicenseReviewer => "ریاست بررسی و ثبت جواز",
                PropertyOperator => "کاربر عملیاتی املاک",
                VehicleOperator => "کاربر عملیاتی موتر فروشی",
                LicenseApplicationManager => "کاربر مدیریت درخواست جواز",
                _ => role
            };
        }
    }

    /// <summary>
    /// Defines all permissions in the system
    /// </summary>
    public static class Permissions
    {
        // User Management
        public const string UsersView = "users.view";
        public const string UsersCreate = "users.create";
        public const string UsersEdit = "users.edit";
        public const string UsersDelete = "users.delete";
        public const string UsersLock = "users.lock";

        // License Application Management
        public const string LicenseApplicationView = "licenseapplication.view";
        public const string LicenseApplicationCreate = "licenseapplication.create";
        public const string LicenseApplicationEdit = "licenseapplication.edit";
        public const string LicenseApplicationDelete = "licenseapplication.delete";

        // Company Management
        public const string CompanyView = "company.view";
        public const string CompanyCreate = "company.create";
        public const string CompanyEdit = "company.edit";
        public const string CompanyDelete = "company.delete";
        public const string CompanyApprove = "company.approve";

        // Property Management
        public const string PropertyView = "property.view";
        public const string PropertyCreate = "property.create";
        public const string PropertyEdit = "property.edit";
        public const string PropertyEditOwn = "property.edit.own";
        public const string PropertyDelete = "property.delete";

        // Vehicle Management
        public const string VehicleView = "vehicle.view";
        public const string VehicleCreate = "vehicle.create";
        public const string VehicleEdit = "vehicle.edit";
        public const string VehicleEditOwn = "vehicle.edit.own";
        public const string VehicleDelete = "vehicle.delete";

        // License Management
        public const string LicenseView = "license.view";
        public const string LicenseCreate = "license.create";
        public const string LicenseEdit = "license.edit";
        public const string LicenseApprove = "license.approve";

        // Reports
        public const string ReportsView = "reports.view";
        public const string ReportsExport = "reports.export";

        // Dashboard
        public const string DashboardView = "dashboard.view";

        // System
        public const string SystemConfigure = "system.configure";
    }

    /// <summary>
    /// Maps roles to their permissions
    /// </summary>
    public static class RolePermissions
    {
        public static string[] GetPermissionsForRole(string role)
        {
            return role switch
            {
                UserRoles.Admin => new[]
                {
                    // All permissions
                    Permissions.UsersView, Permissions.UsersCreate, Permissions.UsersEdit, 
                    Permissions.UsersDelete, Permissions.UsersLock,
                    Permissions.CompanyView, Permissions.CompanyCreate, Permissions.CompanyEdit, 
                    Permissions.CompanyDelete, Permissions.CompanyApprove,
                    Permissions.PropertyView, Permissions.PropertyCreate, Permissions.PropertyEdit, 
                    Permissions.PropertyDelete,
                    Permissions.VehicleView, Permissions.VehicleCreate, Permissions.VehicleEdit, 
                    Permissions.VehicleDelete,
                    Permissions.LicenseView, Permissions.LicenseCreate, Permissions.LicenseEdit, 
                    Permissions.LicenseApprove,
                    Permissions.LicenseApplicationView, Permissions.LicenseApplicationCreate, 
                    Permissions.LicenseApplicationEdit, Permissions.LicenseApplicationDelete,
                    Permissions.ReportsView, Permissions.ReportsExport,
                    Permissions.DashboardView,
                    Permissions.SystemConfigure
                },

                UserRoles.Authority => new[]
                {
                    // View-only access to everything
                    Permissions.UsersView,
                    Permissions.CompanyView,
                    Permissions.PropertyView,
                    Permissions.VehicleView,
                    Permissions.LicenseView,
                    Permissions.LicenseApplicationView,
                    Permissions.ReportsView,
                    Permissions.DashboardView
                },

                UserRoles.CompanyRegistrar => new[]
                {
                    // Full access to company module + view all property and vehicle records
                    Permissions.CompanyView, Permissions.CompanyCreate, Permissions.CompanyEdit, Permissions.CompanyDelete,
                    Permissions.LicenseView, Permissions.LicenseCreate, Permissions.LicenseEdit,
                    Permissions.PropertyView,  // Can view all property records
                    Permissions.VehicleView,   // Can view all vehicle records
                    Permissions.ReportsView,
                    Permissions.LicenseApplicationView  // Can only view license applications, no edit/delete
                },

                UserRoles.LicenseReviewer => new[]
                {
                    // View-only company list
                    Permissions.CompanyView,
                    Permissions.LicenseView,
                    Permissions.LicenseApplicationView
                },

                UserRoles.PropertyOperator => new[]
                {
                    // Property module - can create, edit own records
                    Permissions.PropertyView, Permissions.PropertyCreate, Permissions.PropertyEditOwn,
                    Permissions.DashboardView,
                    Permissions.ReportsView
                },

                UserRoles.VehicleOperator => new[]
                {
                    // Vehicle module - can create, edit own records
                    Permissions.VehicleView, Permissions.VehicleCreate, Permissions.VehicleEditOwn,
                    Permissions.DashboardView,
                    Permissions.ReportsView
                },

                UserRoles.LicenseApplicationManager => new[]
                {
                    // Full access to license applications
                    Permissions.LicenseApplicationView, Permissions.LicenseApplicationCreate, 
                    Permissions.LicenseApplicationEdit, Permissions.LicenseApplicationDelete,
                    // Read-only access to other modules
                    Permissions.CompanyView,
                    Permissions.PropertyView,
                    Permissions.VehicleView,
                    Permissions.ReportsView,
                    Permissions.DashboardView
                },

                _ => Array.Empty<string>()
            };
        }
    }
}

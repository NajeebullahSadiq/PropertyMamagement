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
        /// کاربر مدیریت نظارت بر فعالیت‌ها - Activity monitoring manager with full access to activity monitoring and read-only to other modules
        /// </summary>
        public const string ActivityMonitoringManager = "ACTIVITY_MONITORING_MANAGER";

        /// <summary>
        /// کاربر مدیریت اسناد بهادار - Securities manager with full access to securities and petition writer securities modules
        /// </summary>
        public const string SecuritiesManager = "SECURITIES_MANAGER";

        /// <summary>
        /// کاربر ثبت اسناد بهادار - Securities entry manager with create-only access to securities module
        /// </summary>
        public const string SecuritiesEntryManager = "SECURITIES_ENTRY_MANAGER";

        /// <summary>
        /// کاربر ثبت سند بهادار عریضه‌نویسان - Petition writer securities entry manager with create-only access
        /// </summary>
        public const string PetitionWriterSecuritiesEntryManager = "PETITION_WRITER_SECURITIES_ENTRY_MANAGER";

        /// <summary>
        /// کاربر مدیریت جواز عریضه‌نویسان - Petition writer license manager with full access to petition writer license module
        /// </summary>
        public const string PetitionWriterLicenseManager = "PETITION_WRITER_LICENSE_MANAGER";

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
            LicenseApplicationManager,
            ActivityMonitoringManager,
            SecuritiesManager,
            SecuritiesEntryManager,
            PetitionWriterSecuritiesEntryManager,
            PetitionWriterLicenseManager
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
                ActivityMonitoringManager => "کاربر مدیریت نظارت بر فعالیت‌ها",
                SecuritiesManager => "کاربر مدیریت اسناد بهادار",
                SecuritiesEntryManager => "کاربر ثبت اسناد بهادار",
                PetitionWriterSecuritiesEntryManager => "کاربر ثبت سند بهادار عریضه‌نویسان",
                PetitionWriterLicenseManager => "کاربر مدیریت جواز عریضه‌نویسان",
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

        // Securities (اسناد بهادار رهنمای معاملات)
        public const string SecuritiesView = "securities.view";
        public const string SecuritiesCreate = "securities.create";
        public const string SecuritiesEdit = "securities.edit";
        public const string SecuritiesDelete = "securities.delete";

        // Petition Writer Securities (سند بهادار عریضه‌نویسان)
        public const string PetitionWriterSecuritiesView = "petitionwritersecurities.view";
        public const string PetitionWriterSecuritiesCreate = "petitionwritersecurities.create";
        public const string PetitionWriterSecuritiesEdit = "petitionwritersecurities.edit";
        public const string PetitionWriterSecuritiesDelete = "petitionwritersecurities.delete";

        // Petition Writer License (جواز عریضه‌نویسان)
        public const string PetitionWriterLicenseView = "petitionwriterlicense.view";
        public const string PetitionWriterLicenseCreate = "petitionwriterlicense.create";
        public const string PetitionWriterLicenseEdit = "petitionwriterlicense.edit";
        public const string PetitionWriterLicenseDelete = "petitionwriterlicense.delete";

        // Activity Monitoring (نظارت بر فعالیت‌ها)
        public const string ActivityMonitoringView = "activitymonitoring.view";
        public const string ActivityMonitoringCreate = "activitymonitoring.create";
        public const string ActivityMonitoringEdit = "activitymonitoring.edit";
        public const string ActivityMonitoringDelete = "activitymonitoring.delete";

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
                    Permissions.SecuritiesView, Permissions.SecuritiesCreate, Permissions.SecuritiesEdit, Permissions.SecuritiesDelete,
                    Permissions.PetitionWriterSecuritiesView, Permissions.PetitionWriterSecuritiesCreate, Permissions.PetitionWriterSecuritiesEdit, Permissions.PetitionWriterSecuritiesDelete,
                    Permissions.PetitionWriterLicenseView, Permissions.PetitionWriterLicenseCreate, Permissions.PetitionWriterLicenseEdit, Permissions.PetitionWriterLicenseDelete,
                    Permissions.ActivityMonitoringView, Permissions.ActivityMonitoringCreate, Permissions.ActivityMonitoringEdit, Permissions.ActivityMonitoringDelete,
                    Permissions.ReportsView, Permissions.ReportsExport,
                    Permissions.DashboardView,
                    Permissions.SystemConfigure
                },

                UserRoles.Authority => new[]
                {
                    Permissions.UsersView,
                    Permissions.CompanyView, Permissions.PropertyView, Permissions.VehicleView,
                    Permissions.LicenseView, Permissions.LicenseApplicationView,
                    Permissions.SecuritiesView, Permissions.PetitionWriterSecuritiesView,
                    Permissions.PetitionWriterLicenseView, Permissions.ActivityMonitoringView,
                    Permissions.ReportsView, Permissions.DashboardView
                },

                UserRoles.CompanyRegistrar => new[]
                {
                    Permissions.CompanyView, Permissions.CompanyCreate, Permissions.CompanyEdit, Permissions.CompanyDelete,
                    Permissions.LicenseView, Permissions.LicenseCreate, Permissions.LicenseEdit,
                    Permissions.PropertyView, Permissions.VehicleView,
                    Permissions.LicenseApplicationView,
                    Permissions.SecuritiesView, Permissions.SecuritiesCreate, Permissions.SecuritiesEdit,
                    Permissions.ReportsView
                },

                UserRoles.LicenseReviewer => new[]
                {
                    Permissions.CompanyView, Permissions.LicenseView, Permissions.LicenseApplicationView
                },

                UserRoles.PropertyOperator => new[]
                {
                    Permissions.PropertyView, Permissions.PropertyCreate, Permissions.PropertyEditOwn,
                    Permissions.DashboardView, Permissions.ReportsView
                },

                UserRoles.VehicleOperator => new[]
                {
                    Permissions.VehicleView, Permissions.VehicleCreate, Permissions.VehicleEditOwn,
                    Permissions.DashboardView, Permissions.ReportsView
                },

                UserRoles.LicenseApplicationManager => new[]
                {
                    Permissions.LicenseApplicationView, Permissions.LicenseApplicationCreate,
                    Permissions.LicenseApplicationEdit, Permissions.LicenseApplicationDelete,
                    Permissions.CompanyView, Permissions.PropertyView, Permissions.VehicleView,
                    Permissions.ReportsView, Permissions.DashboardView
                },

                UserRoles.ActivityMonitoringManager => new[]
                {
                    Permissions.ActivityMonitoringView, Permissions.ActivityMonitoringCreate,
                    Permissions.ActivityMonitoringEdit, Permissions.ActivityMonitoringDelete,
                    Permissions.CompanyView, Permissions.PropertyView, Permissions.VehicleView,
                    Permissions.ReportsView, Permissions.DashboardView
                },

                UserRoles.SecuritiesManager => new[]
                {
                    Permissions.SecuritiesView, Permissions.SecuritiesCreate, Permissions.SecuritiesEdit, Permissions.SecuritiesDelete,
                    Permissions.PetitionWriterSecuritiesView, Permissions.PetitionWriterSecuritiesCreate, Permissions.PetitionWriterSecuritiesEdit,
                    Permissions.CompanyView, Permissions.PropertyView, Permissions.VehicleView,
                    Permissions.ReportsView, Permissions.DashboardView
                },

                UserRoles.SecuritiesEntryManager => new[]
                {
                    Permissions.SecuritiesView, Permissions.SecuritiesCreate,
                    Permissions.DashboardView
                },

                UserRoles.PetitionWriterSecuritiesEntryManager => new[]
                {
                    Permissions.PetitionWriterSecuritiesView, Permissions.PetitionWriterSecuritiesCreate,
                    Permissions.DashboardView
                },

                UserRoles.PetitionWriterLicenseManager => new[]
                {
                    Permissions.PetitionWriterLicenseView, Permissions.PetitionWriterLicenseCreate,
                    Permissions.PetitionWriterLicenseEdit, Permissions.PetitionWriterLicenseDelete,
                    Permissions.PetitionWriterSecuritiesView, Permissions.PetitionWriterSecuritiesCreate, Permissions.PetitionWriterSecuritiesEdit,
                    Permissions.CompanyView, Permissions.PropertyView, Permissions.VehicleView,
                    Permissions.ReportsView, Permissions.DashboardView
                },

                _ => Array.Empty<string>()
            };
        }
    }
}

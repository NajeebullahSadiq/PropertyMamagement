namespace WebAPIBackend.Configuration
{
    public class CustomClaimTypes
    {
        public const string Permission = "Permission";
        public const string ProvinceId = "province_id";
    }

    public static class UserPermissions
    {
        public const string View = "Can_view_UserDetails";
        public const string ViewUserTest = "Can_View_Users";
        public const string Add = "users.add";
        public const string Edit = "users.edit";
        
    }
}

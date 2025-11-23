namespace WebAPIBackend.Configuration
{
    public static class PolicyTypes
    {
        public static class Users
        {
            public const string View = "users.view.details";
            public const string Manage = "users.manage.policy";
            public const string EditRole = "users.edit.role.policy";
        }

        public static class Teams
        {
            public const string Manage = "teams.manage.policy";

            public const string AddRemove = "teams.addremove.policy";
        }
    }
    }

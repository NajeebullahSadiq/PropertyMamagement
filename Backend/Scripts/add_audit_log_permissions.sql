-- =====================================================
-- Add Audit Log Permissions to Admin Role
-- Run this in production to enable audit log access for Admin
-- =====================================================

DO $$
DECLARE
    admin_role_id TEXT;
BEGIN
    -- Get Admin role ID
    SELECT "Id" INTO admin_role_id FROM public."AspNetRoles" WHERE "Name" = 'ADMIN';
    
    IF admin_role_id IS NULL THEN
        RAISE EXCEPTION 'ADMIN role not found';
    END IF;
    
    RAISE NOTICE 'Found ADMIN role with ID: %', admin_role_id;
    
    -- Add auditlog.view permission
    INSERT INTO public."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue")
    SELECT nextval('public."AspNetRoleClaims_Id_seq"'), admin_role_id, 'Permission', 'auditlog.view'
    WHERE NOT EXISTS (
        SELECT 1 FROM public."AspNetRoleClaims" 
        WHERE "RoleId" = admin_role_id AND "ClaimValue" = 'auditlog.view'
    );
    
    -- Add auditlog.export permission
    INSERT INTO public."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue")
    SELECT nextval('public."AspNetRoleClaims_Id_seq"'), admin_role_id, 'Permission', 'auditlog.export'
    WHERE NOT EXISTS (
        SELECT 1 FROM public."AspNetRoleClaims" 
        WHERE "RoleId" = admin_role_id AND "ClaimValue" = 'auditlog.export'
    );
    
    RAISE NOTICE '✓ Audit log permissions added to ADMIN role';
    RAISE NOTICE '  - auditlog.view';
    RAISE NOTICE '  - auditlog.export';
    RAISE NOTICE '';
    RAISE NOTICE 'Important: Users need to log out and log back in to get the new permissions.';
END $$;

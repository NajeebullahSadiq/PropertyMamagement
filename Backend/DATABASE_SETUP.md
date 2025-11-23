# Database Setup Guide

## Prerequisites
- PostgreSQL installed and running
- Database connection configured in `appsettings.json`

## Current Configuration
```json
Server: localhost
Database: PRMIS
Username: postgres
Password: Khan@223344
```

## Automatic Setup (Recommended)

The application is now configured to **automatically**:
1. Apply all pending migrations
2. Create the database if it doesn't exist
3. Create the Admin role with all permissions
4. Create the default admin user

### Default Admin Credentials
```
Email: admin@prmis.gov.af
Username: admin
Password: Admin@123
```

### How to Run
Simply start the application:
```bash
dotnet run
```

The database will be created and seeded automatically on startup!

---

## Manual Migration Commands (Optional)

If you need to manage migrations manually:

### Create a New Migration
```bash
dotnet ef migrations add InitialCreate
```

### Apply Migrations
```bash
dotnet ef database update
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

### Drop Database (Caution!)
```bash
dotnet ef database drop
```

---

## Troubleshooting

### If you get "Database does not exist" error:
1. Make sure PostgreSQL is running
2. Create the database manually:
   ```sql
   CREATE DATABASE "PRMIS";
   ```
3. Run the application again

### If migrations fail:
1. Check your connection string in `appsettings.json`
2. Ensure PostgreSQL user has CREATE DATABASE permissions
3. Try running: `dotnet ef database update --verbose`

### To reset everything:
```bash
dotnet ef database drop --force
dotnet run
```

---

## Security Notes

⚠️ **IMPORTANT**: Change the default admin password immediately after first login!

⚠️ **PRODUCTION**: Update the connection string and admin credentials before deploying to production.

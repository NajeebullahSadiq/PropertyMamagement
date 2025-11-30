# Environment Configuration Guide

## Configuration Files Overview

### Backend Configuration Files

#### 1. `appsettings.json` (Development)
Used for local development on Windows/Mac/Linux
```json
{
  "connection": {
    "connectionString": "Server=localhost; Database=PRMIS; Username=postgres; Password=Khan@223344"
  },
  "ApplicationSettings": {
    "JWT_Secret": "aj@jahanbeen@mcit@!*",
    "Client_URL": "http://localhost:4200"
  }
}
```

#### 2. `appsettings.Development.json`
Used when running `dotnet run` with Development environment
```json
{
  // Minimal overrides for development
}
```

#### 3. `appsettings.Production.json` (NEW - For Linux Server)
Used when running on production Linux server
```json
{
  "connection": {
    "connectionString": "Server=localhost; Database=PRMIS; Username=prmis_user; Password=SecurePassword@2024; Port=5432; Pooling=true; MaxPoolSize=20;"
  },
  "ApplicationSettings": {
    "JWT_Secret": "aj@jahanbeen@mcit@!*",
    "Client_URL": "http://your-server-ip-or-domain"
  }
}
```

---

## Environment Variables

### Setting Environment on Linux Server

```bash
# Option 1: Set in systemd service (Recommended)
# Edit: /etc/systemd/system/prmis-backend.service
Environment="ASPNETCORE_ENVIRONMENT=Production"

# Option 2: Set in shell before running
export ASPNETCORE_ENVIRONMENT=Production
dotnet run

# Option 3: Set in .bashrc for persistent setting
echo 'export ASPNETCORE_ENVIRONMENT=Production' >> ~/.bashrc
source ~/.bashrc
```

### How .NET Selects Configuration File

.NET automatically loads configuration files in this order:
1. `appsettings.json` (always loaded)
2. `appsettings.{ASPNETCORE_ENVIRONMENT}.json` (if environment is set)

**Example:**
- If `ASPNETCORE_ENVIRONMENT=Production`, it loads:
  - `appsettings.json` first
  - `appsettings.Production.json` second (overrides values from first)

---

## Configuration Parameters Explained

### Connection String
```
Server=localhost              # PostgreSQL server address
Database=PRMIS               # Database name
Username=prmis_user          # Database user
Password=SecurePassword@2024 # Database password
Port=5432                    # PostgreSQL port (default)
Pooling=true                 # Enable connection pooling
MaxPoolSize=20               # Maximum connections in pool
```

### JWT Secret
```
JWT_Secret: "aj@jahanbeen@mcit@!*"
```
- Used to sign and verify JWT tokens
- Keep this secret and secure
- Change for production if needed
- Must be the same across all instances

### Client URL
```
Client_URL: "http://your-server-ip-or-domain"
```
- Used for CORS (Cross-Origin Resource Sharing)
- Allows frontend to communicate with backend
- Set to your frontend URL
- In production: `http://your-domain.com` or `https://your-domain.com`

---

## Step-by-Step Configuration for Production

### 1. Create Production Configuration File

```bash
# SSH into server
ssh user@your-server-ip

# Create the file
sudo nano /var/www/prmis/backend/appsettings.Production.json
```

### 2. Add Production Settings

Copy this content and update values:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "connection": {
    "connectionString": "Server=localhost; Database=PRMIS; Username=prmis_user; Password=SecurePassword@2024; Port=5432; Pooling=true; MaxPoolSize=20;"
  },
  "ApplicationSettings": {
    "JWT_Secret": "aj@jahanbeen@mcit@!*",
    "Client_URL": "http://your-actual-server-ip-or-domain"
  },
  "AllowedHosts": "*"
}
```

### 3. Update Values

Replace these with your actual values:
- `Client_URL`: Your server IP or domain name
- `Password`: Your PostgreSQL password (if changed)
- `JWT_Secret`: Keep as is or change to a new secure value

### 4. Set Permissions

```bash
sudo chown www-data:www-data /var/www/prmis/backend/appsettings.Production.json
sudo chmod 600 /var/www/prmis/backend/appsettings.Production.json
```

### 5. Verify Configuration

```bash
# Check the file
sudo cat /var/www/prmis/backend/appsettings.Production.json

# Test database connection
psql -h localhost -U prmis_user -d PRMIS -c "SELECT 1;"
```

---

## Frontend Configuration

### Angular Environment Files

Angular uses environment files for configuration:

#### `src/environments/environment.ts` (Development)
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

#### `src/environments/environment.prod.ts` (Production)
```typescript
export const environment = {
  production: true,
  apiUrl: 'http://your-server-ip/api'
};
```

### Building for Production

```bash
cd /var/www/prmis/frontend

# Build with production environment
npm run build -- --configuration production

# Or use the default build (uses production by default)
npm run build
```

### Update API URL

If your API URL is different, update in Angular:

```bash
# Edit environment.prod.ts
sudo nano /var/www/prmis/frontend/src/environments/environment.prod.ts
```

Change `apiUrl` to match your backend URL:
```typescript
apiUrl: 'http://your-server-ip/api'
// or
apiUrl: 'https://your-domain.com/api'
```

Then rebuild:
```bash
npm run build
```

---

## Database Configuration

### PostgreSQL Connection

```bash
# Test connection
psql -h localhost -U prmis_user -d PRMIS

# View database info
psql -h localhost -U prmis_user -d PRMIS -c "\l"

# View database size
psql -h localhost -U prmis_user -d PRMIS -c "SELECT pg_size_pretty(pg_database_size('PRMIS'));"
```

### Connection Pooling

The production configuration uses connection pooling:
- `Pooling=true`: Enables connection pooling
- `MaxPoolSize=20`: Maximum 20 connections in pool

This is important for performance on a production server.

---

## Nginx Configuration

### Backend API Routing

Nginx routes `/api/` requests to the backend:

```nginx
location /api/ {
    proxy_pass http://backend;
    # ... other settings
}
```

This means:
- Request to `http://server/api/users` goes to `http://localhost:5000/api/users`

### Frontend Routing

Nginx serves Angular frontend:

```nginx
location / {
    root /var/www/prmis/frontend/dist/property-registeration-mis;
    try_files $uri $uri/ /index.html;
}
```

This means:
- All requests go to Angular's `index.html`
- Angular router handles the routing

---

## Security Considerations

### 1. Change Default Passwords
```bash
# PostgreSQL password
sudo -u postgres psql
ALTER USER prmis_user WITH PASSWORD 'NewSecurePassword@2024';

# Update in appsettings.Production.json
sudo nano /var/www/prmis/backend/appsettings.Production.json

# Restart backend
sudo systemctl restart prmis-backend.service
```

### 2. Change JWT Secret (Optional)
```bash
# Generate new secret (at least 32 characters)
openssl rand -base64 32

# Update in appsettings.Production.json
sudo nano /var/www/prmis/backend/appsettings.Production.json

# Restart backend
sudo systemctl restart prmis-backend.service
```

### 3. Use HTTPS
```bash
# Install SSL certificate
sudo certbot --nginx -d your-domain.com

# Nginx will automatically redirect HTTP to HTTPS
```

### 4. Secure Configuration Files
```bash
# Restrict access to configuration files
sudo chmod 600 /var/www/prmis/backend/appsettings.Production.json
sudo chown www-data:www-data /var/www/prmis/backend/appsettings.Production.json
```

---

## Troubleshooting Configuration Issues

### Issue: "Connection refused" error

**Cause**: Backend can't connect to database

**Solution**:
```bash
# Check PostgreSQL is running
sudo systemctl status postgresql

# Test connection manually
psql -h localhost -U prmis_user -d PRMIS

# Check connection string in appsettings.Production.json
cat /var/www/prmis/backend/appsettings.Production.json
```

### Issue: "CORS error" in frontend

**Cause**: Frontend URL not in allowed origins

**Solution**:
```bash
# Check Client_URL in appsettings.Production.json
cat /var/www/prmis/backend/appsettings.Production.json

# Update if needed
sudo nano /var/www/prmis/backend/appsettings.Production.json

# Restart backend
sudo systemctl restart prmis-backend.service
```

### Issue: Frontend shows blank page

**Cause**: Wrong API URL or frontend not built

**Solution**:
```bash
# Check environment.prod.ts
cat /var/www/prmis/frontend/src/environments/environment.prod.ts

# Rebuild frontend
cd /var/www/prmis/frontend
npm run build

# Check Nginx logs
sudo tail -f /var/log/nginx/error.log
```

---

## Configuration Checklist

- [ ] `appsettings.Production.json` created
- [ ] Database connection string updated
- [ ] Client_URL set to server IP/domain
- [ ] JWT_Secret is secure
- [ ] PostgreSQL user and password configured
- [ ] Frontend environment files updated
- [ ] Nginx configuration updated
- [ ] SSL certificate installed (if using HTTPS)
- [ ] Configuration files have correct permissions
- [ ] Backend service restarted after changes
- [ ] Frontend rebuilt after changes
- [ ] Tested API connectivity
- [ ] Tested frontend loading
- [ ] Tested login functionality

---

## Quick Reference

| Component | Config File | Key Settings |
|-----------|------------|--------------|
| Backend | `appsettings.Production.json` | Connection string, JWT, Client URL |
| Frontend | `environment.prod.ts` | API URL |
| Database | PostgreSQL | User, password, database name |
| Web Server | `/etc/nginx/sites-available/prmis` | Proxy settings, SSL |
| Service | `/etc/systemd/system/prmis-backend.service` | Environment variables |

---

**Last Updated**: 2024
**Version**: 1.0

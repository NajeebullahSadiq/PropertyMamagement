# PRMIS Complete Deployment Guide

This guide will help you deploy the Property Management System from scratch on a clean server.

## Prerequisites

- Ubuntu/Debian Linux server
- Root or sudo access
- Domain or IP address (currently: 103.132.98.92)

---

## Step 1: Install Required Software

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 9 SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 9.0 --install-dir /usr/share/dotnet
sudo ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet

# Install Node.js 18.x (LTS)
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs

# Install PostgreSQL
sudo apt install -y postgresql postgresql-contrib

# Install Nginx
sudo apt install -y nginx

# Install Git
sudo apt install -y git
```

---

## Step 2: Setup PostgreSQL Database

```bash
# Switch to postgres user
sudo -u postgres psql

# Inside PostgreSQL prompt, run:
CREATE DATABASE "PRMIS";
CREATE USER prmis_user WITH ENCRYPTED PASSWORD 'your_secure_password_here';
GRANT ALL PRIVILEGES ON DATABASE "PRMIS" TO prmis_user;
\q

# Test connection
psql -U prmis_user -d PRMIS -h localhost
```

---

## Step 3: Clone Repository

```bash
# Create directory and clone
cd ~
git clone <your-repository-url> PropertyMamagement
cd PropertyMamagement
```

---

## Step 4: Configure Backend

```bash
cd ~/PropertyMamagement/Backend

# Edit appsettings.Production.json with your database credentials
nano appsettings.Production.json
```

Update the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PRMIS;Username=prmis_user;Password=your_secure_password_here"
  }
}
```

---

## Step 5: Run Database Migrations

```bash
cd ~/PropertyMamagement/Backend

# Run migrations in order
sudo -u postgres psql -d PRMIS -f Scripts/Modules/01_Shared_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/02_UserManagement_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/03_Company_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/04_Property_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/05_Vehicle_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/06_Securities_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/07_Audit_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/08_LicenseApplication_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/09_PetitionWriterLicense_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/10_Verification_Initial.sql
sudo -u postgres psql -d PRMIS -f Scripts/Modules/11_ActivityMonitoring_Initial.sql
```

---

## Step 6: Create Directory Structure

```bash
# Create main directories
sudo mkdir -p /var/www/prmis/{frontend,backend,storage}

# Create storage subdirectories
sudo mkdir -p /var/www/prmis/storage/Resources/Documents/{Profile,Identity,Property,Vehicle,Company,License}
sudo mkdir -p /var/www/prmis/storage/Resources/Images

# Set ownership
sudo chown -R www-data:www-data /var/www/prmis
sudo chmod -R 755 /var/www/prmis

# Create .dotnet directory for www-data user
sudo mkdir -p /var/www/.dotnet
sudo chown -R www-data:www-data /var/www/.dotnet
sudo chmod -R 755 /var/www/.dotnet
```

---

## Step 7: Build and Deploy Backend

```bash
cd ~/PropertyMamagement/Backend

# Build and publish
dotnet build WebAPIBackend.csproj --configuration Release
dotnet publish WebAPIBackend.csproj --configuration Release --output ./publish

# Copy to deployment directory
sudo cp -r ./publish/* /var/www/prmis/backend/
sudo chown -R www-data:www-data /var/www/prmis/backend
sudo chmod -R 755 /var/www/prmis/backend
```

---

## Step 8: Setup Backend Service

```bash
# Copy service file
sudo cp ~/PropertyMamagement/prmis-backend.service /etc/systemd/system/

# Reload systemd and enable service
sudo systemctl daemon-reload
sudo systemctl enable prmis-backend
sudo systemctl start prmis-backend

# Check status
sudo systemctl status prmis-backend

# View logs if needed
sudo journalctl -u prmis-backend -n 50 --no-pager
```

---

## Step 9: Build and Deploy Frontend

```bash
cd ~/PropertyMamagement/Frontend

# Install dependencies
npm install --legacy-peer-deps

# Build for production
npx ng build --configuration production

# Deploy
sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/
sudo chown -R www-data:www-data /var/www/prmis/frontend
sudo chmod -R 755 /var/www/prmis/frontend
```

---

## Step 10: Configure Nginx

```bash
# Copy nginx configuration
sudo cp ~/PropertyMamagement/nginx-prmis.conf /etc/nginx/sites-available/prmis

# Enable site
sudo ln -s /etc/nginx/sites-available/prmis /etc/nginx/sites-enabled/prmis

# Remove default site (optional)
sudo rm /etc/nginx/sites-enabled/default

# Test configuration
sudo nginx -t

# Restart nginx
sudo systemctl restart nginx
sudo systemctl enable nginx
```

---

## Step 11: Configure Firewall

```bash
# Allow HTTP and HTTPS
sudo ufw allow 'Nginx Full'
sudo ufw allow OpenSSH
sudo ufw enable
sudo ufw status
```

---

## Step 12: Verify Deployment

```bash
# Check backend service
sudo systemctl status prmis-backend

# Check nginx
sudo systemctl status nginx

# Test backend API
curl http://localhost:5000/api/health

# Test frontend
curl http://localhost/

# Check logs
sudo journalctl -u prmis-backend -n 20
sudo tail -f /var/log/nginx/prmis_error.log
```

---

## Optional: Setup SSL with Let's Encrypt

```bash
# Install certbot
sudo apt install -y certbot python3-certbot-nginx

# Get certificate (replace with your domain)
sudo certbot --nginx -d your-domain.com

# Auto-renewal is configured automatically
sudo certbot renew --dry-run
```

After SSL setup, uncomment the HTTPS sections in `/etc/nginx/sites-available/prmis` and restart nginx.

---

## Troubleshooting

### Backend not starting
```bash
# Check logs
sudo journalctl -u prmis-backend -n 100 --no-pager

# Check if port 5000 is in use
sudo netstat -tulpn | grep 5000

# Restart service
sudo systemctl restart prmis-backend
```

### Frontend not loading
```bash
# Check nginx logs
sudo tail -f /var/log/nginx/prmis_error.log

# Verify files exist
ls -la /var/www/prmis/frontend/

# Test nginx config
sudo nginx -t
```

### Database connection issues
```bash
# Test database connection
psql -U prmis_user -d PRMIS -h localhost

# Check PostgreSQL is running
sudo systemctl status postgresql
```

### Permission issues
```bash
# Fix ownership
sudo chown -R www-data:www-data /var/www/prmis
sudo chmod -R 755 /var/www/prmis

# Fix storage permissions
sudo chown -R www-data:www-data /var/www/prmis/storage
sudo chmod -R 755 /var/www/prmis/storage
```

---

## Maintenance Commands

### Update deployment (after code changes)
```bash
# See Backend/propertyManagement.txt for quick deployment commands
cd ~/PropertyMamagement
git pull origin main

# Then follow either:
# - Deploy Frontend Only
# - Deploy Backend Only  
# - Deploy Both
```

### Backup database
```bash
sudo -u postgres pg_dump PRMIS > prmis_backup_$(date +%Y%m%d).sql
```

### Restore database
```bash
sudo -u postgres psql PRMIS < prmis_backup_20260130.sql
```

---

## Important Notes

1. **Storage Directory**: `/var/www/prmis/storage/Resources/` persists across deployments
2. **Database Credentials**: Update in `appsettings.Production.json`
3. **Server IP**: Currently configured for `103.132.98.92`
4. **Default Admin**: Created by DatabaseSeeder (check Backend/Configuration/DatabaseSeeder.cs)
5. **Logs Location**: 
   - Backend: `sudo journalctl -u prmis-backend`
   - Nginx: `/var/log/nginx/prmis_*.log`

---

## Quick Reference

| Component | Location | Command |
|-----------|----------|---------|
| Frontend | `/var/www/prmis/frontend` | - |
| Backend | `/var/www/prmis/backend` | `sudo systemctl restart prmis-backend` |
| Storage | `/var/www/prmis/storage` | - |
| Nginx Config | `/etc/nginx/sites-available/prmis` | `sudo systemctl restart nginx` |
| Service File | `/etc/systemd/system/prmis-backend.service` | `sudo systemctl daemon-reload` |
| Backend Logs | - | `sudo journalctl -u prmis-backend -f` |
| Nginx Logs | `/var/log/nginx/` | `sudo tail -f /var/log/nginx/prmis_error.log` |

---

## Support

For issues, check:
1. Backend logs: `sudo journalctl -u prmis-backend -n 50`
2. Nginx logs: `sudo tail -f /var/log/nginx/prmis_error.log`
3. Database connection: Test with psql
4. File permissions: Ensure www-data owns all files

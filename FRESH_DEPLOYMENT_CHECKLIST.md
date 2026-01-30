# Fresh Server Deployment Checklist

Use this checklist when deploying to a completely clean server.

## ☑️ Pre-Deployment Checklist

- [ ] Server has Ubuntu/Debian Linux installed
- [ ] You have sudo/root access
- [ ] Server IP: `103.132.98.92` (or update in nginx-prmis.conf)
- [ ] PostgreSQL database password ready

---

## Step 1: Install Prerequisites

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 9 SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 9.0 --install-dir /usr/share/dotnet
sudo ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet
dotnet --version  # Verify installation

# Install Node.js 18.x
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs
node --version  # Verify installation
npm --version   # Verify installation

# Install PostgreSQL
sudo apt install -y postgresql postgresql-contrib
sudo systemctl status postgresql  # Verify running

# Install Nginx
sudo apt install -y nginx
sudo systemctl status nginx  # Verify running

# Install Git
sudo apt install -y git
```

**✓ Checkpoint:** All software installed and running

---

## Step 2: Setup PostgreSQL Database

```bash
# Switch to postgres user and create database
sudo -u postgres psql << EOF
CREATE DATABASE "PRMIS";
CREATE USER prmis_user WITH ENCRYPTED PASSWORD 'YourSecurePassword123!';
GRANT ALL PRIVILEGES ON DATABASE "PRMIS" TO prmis_user;
\q
EOF

# Test connection (enter password when prompted)
psql -U prmis_user -d PRMIS -h localhost -c "SELECT version();"
```

**✓ Checkpoint:** Database created and accessible

---

## Step 3: Clone Repository

```bash
# Clone to home directory
cd ~
git clone <your-repository-url> PropertyMamagement
cd PropertyMamagement
ls -la  # Verify files are there
```

**✓ Checkpoint:** Repository cloned successfully

---

## Step 4: Configure Backend

```bash
cd ~/PropertyMamagement/Backend

# Edit production settings
nano appsettings.Production.json
```

Update the connection string with your password:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PRMIS;Username=prmis_user;Password=YourSecurePassword123!"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Save and exit (Ctrl+X, Y, Enter)

**✓ Checkpoint:** Backend configured with database credentials

---

## Step 5: Create Directory Structure

```bash
# Create all required directories
sudo mkdir -p /var/www/prmis/{frontend,backend,storage}
sudo mkdir -p /var/www/prmis/storage/Resources/Documents/{Profile,Identity,Property,Vehicle,Company,License}
sudo mkdir -p /var/www/prmis/storage/Resources/Images
sudo mkdir -p /var/www/.dotnet

# Set ownership and permissions
sudo chown -R www-data:www-data /var/www/prmis
sudo chown -R www-data:www-data /var/www/.dotnet
sudo chmod -R 755 /var/www/prmis
sudo chmod -R 755 /var/www/.dotnet

# Verify
ls -la /var/www/prmis/
```

**✓ Checkpoint:** Directory structure created

---

## Step 6: Run Database Migrations

```bash
cd ~/PropertyMamagement/Backend

# Run migrations in order (this takes a few minutes)
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

# Verify tables were created
sudo -u postgres psql -d PRMIS -c "\dt"
```

**✓ Checkpoint:** Database schema created

---

## Step 7: Build and Deploy Backend

```bash
cd ~/PropertyMamagement/Backend

# Build
dotnet build WebAPIBackend.csproj --configuration Release

# Publish
dotnet publish WebAPIBackend.csproj --configuration Release --output ./publish

# Deploy
sudo cp -r ./publish/* /var/www/prmis/backend/
sudo chown -R www-data:www-data /var/www/prmis/backend
sudo chmod -R 755 /var/www/prmis/backend

# Verify files copied
ls -la /var/www/prmis/backend/
```

**✓ Checkpoint:** Backend built and deployed

---

## Step 8: Setup Backend Service

```bash
# Copy service file
sudo cp ~/PropertyMamagement/prmis-backend.service /etc/systemd/system/

# Reload systemd
sudo systemctl daemon-reload

# Enable and start service
sudo systemctl enable prmis-backend
sudo systemctl start prmis-backend

# Check status (should show "active (running)")
sudo systemctl status prmis-backend

# If not running, check logs
sudo journalctl -u prmis-backend -n 50 --no-pager
```

**✓ Checkpoint:** Backend service running

---

## Step 9: Build and Deploy Frontend

```bash
cd ~/PropertyMamagement/Frontend

# Install dependencies (this takes several minutes)
npm install --legacy-peer-deps

# Build for production (this takes a few minutes)
npx ng build --configuration production

# Deploy
sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/
sudo chown -R www-data:www-data /var/www/prmis/frontend
sudo chmod -R 755 /var/www/prmis/frontend

# Verify files copied
ls -la /var/www/prmis/frontend/
```

**✓ Checkpoint:** Frontend built and deployed

---

## Step 10: Setup Nginx

```bash
# Copy nginx config
sudo cp ~/PropertyMamagement/nginx-prmis.conf /etc/nginx/sites-available/prmis

# Enable site
sudo ln -s /etc/nginx/sites-available/prmis /etc/nginx/sites-enabled/prmis

# Remove default site (optional)
sudo rm -f /etc/nginx/sites-enabled/default

# Test configuration
sudo nginx -t

# Restart nginx
sudo systemctl restart nginx
sudo systemctl enable nginx

# Check status
sudo systemctl status nginx
```

**✓ Checkpoint:** Nginx configured and running

---

## Step 11: Configure Firewall

```bash
# Allow HTTP/HTTPS
sudo ufw allow 'Nginx Full'

# Allow SSH (IMPORTANT - don't lock yourself out!)
sudo ufw allow OpenSSH

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status
```

**✓ Checkpoint:** Firewall configured

---

## Step 12: Verify Deployment

```bash
# Check all services
sudo systemctl status prmis-backend nginx postgresql

# Test backend API
curl http://localhost:5000/api/health

# Test frontend
curl http://localhost/

# Check from browser
# Open: http://103.132.98.92
```

**✓ Checkpoint:** Application accessible

---

## Step 13: Check Logs

```bash
# Backend logs
sudo journalctl -u prmis-backend -n 50

# Nginx error logs
sudo tail -n 50 /var/log/nginx/prmis_error.log

# Nginx access logs
sudo tail -n 50 /var/log/nginx/prmis_access.log
```

**✓ Checkpoint:** No errors in logs

---

## 🎉 Deployment Complete!

Your application should now be accessible at: **http://103.132.98.92**

### Default Admin Credentials
Check `Backend/Configuration/DatabaseSeeder.cs` for default admin user credentials.

### Next Steps

1. **Test the application** - Try logging in and creating test data
2. **Setup SSL** (optional but recommended) - See DEPLOYMENT_GUIDE.md
3. **Setup backups** - Create a backup schedule for the database
4. **Monitor logs** - Check logs regularly for issues

### Quick Commands

```bash
# Restart backend
sudo systemctl restart prmis-backend

# Restart nginx
sudo systemctl restart nginx

# View backend logs
sudo journalctl -u prmis-backend -f

# View nginx logs
sudo tail -f /var/log/nginx/prmis_error.log
```

---

## 🔧 Troubleshooting

### Backend won't start
```bash
sudo journalctl -u prmis-backend -n 100 --no-pager
# Check for database connection errors or missing files
```

### Frontend shows blank page
```bash
# Check nginx logs
sudo tail -f /var/log/nginx/prmis_error.log

# Verify files exist
ls -la /var/www/prmis/frontend/
```

### Can't connect to database
```bash
# Test connection
psql -U prmis_user -d PRMIS -h localhost

# Check PostgreSQL is running
sudo systemctl status postgresql
```

### 502 Bad Gateway
```bash
# Backend is not running
sudo systemctl status prmis-backend
sudo systemctl start prmis-backend
```

---

## 📞 Support

For detailed troubleshooting, see:
- **DEPLOYMENT_GUIDE.md** - Complete deployment guide
- **QUICK_REFERENCE.md** - Command reference
- **Backend/propertyManagement.txt** - Quick deployment commands

Check logs first:
```bash
sudo journalctl -u prmis-backend -n 50
sudo tail -f /var/log/nginx/prmis_error.log
```

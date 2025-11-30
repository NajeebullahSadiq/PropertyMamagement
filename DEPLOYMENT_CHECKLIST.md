# Property Management System - Deployment Checklist

## Pre-Deployment Checklist

### Server Preparation
- [ ] Server is running Ubuntu 24.04 LTS
- [ ] Server has 32 GB RAM, 8 cores, 2.4 TB storage
- [ ] SSH access is configured
- [ ] Server has internet connectivity
- [ ] Firewall is configured (or will be configured by script)

### Local Preparation
- [ ] Project code is ready and tested
- [ ] All source files are available
- [ ] Database backup is available (if migrating)
- [ ] Configuration files are prepared

---

## Phase 1: Automated Setup (Recommended)

### Option A: Run Automated Script
```bash
# On the Linux server, run:
sudo bash deployment-setup.sh

# This will:
# ✓ Update system packages
# ✓ Install PostgreSQL 15
# ✓ Install .NET 9 SDK/Runtime
# ✓ Install Node.js 18
# ✓ Install Nginx
# ✓ Create application directories
# ✓ Setup PostgreSQL database
# ✓ Configure firewall
# ✓ Create systemd service
# ✓ Configure Nginx
# ✓ Setup backup script
```

**Time Required**: ~15-20 minutes

### Option B: Manual Setup
Follow the steps in `LINUX_DEPLOYMENT_GUIDE.md` Phases 1-5

---

## Phase 2: Application Deployment

### Step 1: Copy Project Files
```bash
# From your local machine or existing server location
# Copy Backend
scp -r Backend/* user@server:/var/www/prmis/backend/

# Copy Frontend
scp -r Frontend/* user@server:/var/www/prmis/frontend/

# Verify
ssh user@server "ls -la /var/www/prmis/backend/"
ssh user@server "ls -la /var/www/prmis/frontend/"
```

- [ ] Backend files copied
- [ ] Frontend files copied
- [ ] Files verified on server

### Step 2: Configure Backend
```bash
# SSH into server
ssh user@server

# Edit production configuration
sudo nano /var/www/prmis/backend/appsettings.Production.json
```

Update these values:
- [ ] `connection.connectionString` - Verify PostgreSQL credentials
- [ ] `ApplicationSettings.Client_URL` - Set to your server IP or domain
- [ ] `ApplicationSettings.JWT_Secret` - Keep secure (or change if needed)

### Step 3: Build Backend
```bash
cd /var/www/prmis/backend

# Restore NuGet packages
dotnet restore

# Build for production
dotnet build -c Release

# Publish
dotnet publish -c Release -o /var/www/prmis/backend/publish

# Verify publish
ls -la /var/www/prmis/backend/publish/
```

- [ ] Backend restored
- [ ] Backend built successfully
- [ ] Backend published to /var/www/prmis/backend/publish/

### Step 4: Build Frontend
```bash
cd /var/www/prmis/frontend

# Install npm dependencies
npm install

# Build for production
npm run build

# Verify build
ls -la /var/www/prmis/frontend/dist/
```

- [ ] Frontend dependencies installed
- [ ] Frontend built successfully
- [ ] Build output in dist/ folder

### Step 5: Set Permissions
```bash
# Set correct ownership and permissions
sudo chown -R www-data:www-data /var/www/prmis
sudo chmod -R 755 /var/www/prmis
sudo chmod -R 755 /var/log/prmis
```

- [ ] Permissions set correctly

---

## Phase 3: Service Startup and Verification

### Step 1: Start Backend Service
```bash
# Enable and start the backend service
sudo systemctl enable prmis-backend.service
sudo systemctl start prmis-backend.service

# Check status
sudo systemctl status prmis-backend.service

# View logs
sudo journalctl -u prmis-backend.service -f
```

- [ ] Backend service started
- [ ] No errors in logs
- [ ] Service is running

### Step 2: Verify Nginx
```bash
# Check Nginx configuration
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx

# Check status
sudo systemctl status nginx
```

- [ ] Nginx configuration is valid
- [ ] Nginx reloaded successfully

### Step 3: Test API Endpoint
```bash
# Test backend API
curl http://localhost:5000/api/health

# Or from another machine
curl http://your-server-ip/api/health
```

- [ ] API responds with 200 status
- [ ] Response is valid

### Step 4: Test Frontend
```bash
# Open browser and navigate to:
# http://your-server-ip
# or
# http://your-domain
```

- [ ] Frontend loads successfully
- [ ] No console errors
- [ ] Can navigate to login page

### Step 5: Test Login
```bash
# Use default admin credentials:
# Email: admin@prmis.gov.af
# Password: Admin@123
```

- [ ] Login page loads
- [ ] Can enter credentials
- [ ] Login succeeds
- [ ] Dashboard displays

---

## Phase 4: Post-Deployment Configuration

### Step 1: Change Default Admin Password
- [ ] Login with default admin credentials
- [ ] Navigate to user profile/settings
- [ ] Change password to a strong, unique password
- [ ] Test login with new password

### Step 2: Update Database Password (Optional but Recommended)
```bash
# SSH to server
sudo -u postgres psql

# Change password
ALTER USER prmis_user WITH PASSWORD 'NewSecurePassword@2024';

# Update appsettings.Production.json
sudo nano /var/www/prmis/backend/appsettings.Production.json

# Restart backend service
sudo systemctl restart prmis-backend.service
```

- [ ] Database password changed
- [ ] Configuration updated
- [ ] Backend restarted successfully

### Step 3: Setup SSL/TLS Certificate (Recommended)
```bash
# Install Certbot
sudo apt install -y certbot python3-certbot-nginx

# Generate certificate
sudo certbot --nginx -d your-domain.com

# Auto-renewal is configured automatically
sudo systemctl enable certbot.timer
```

- [ ] SSL certificate installed
- [ ] HTTPS working
- [ ] Auto-renewal configured

### Step 4: Configure Backup Schedule
```bash
# Verify backup script is scheduled
crontab -u www-data -l

# Manual backup test
sudo /var/www/prmis/backup-db.sh

# Verify backup file
ls -la /var/www/prmis/backups/
```

- [ ] Backup script scheduled
- [ ] Manual backup successful
- [ ] Backup files exist

---

## Phase 5: Monitoring and Maintenance

### Daily Tasks
- [ ] Check backend service status: `sudo systemctl status prmis-backend.service`
- [ ] Monitor disk usage: `df -h`
- [ ] Check for errors in logs: `sudo journalctl -u prmis-backend.service -n 50`

### Weekly Tasks
- [ ] Review application logs for errors
- [ ] Verify database backups are being created
- [ ] Monitor system performance: `htop`
- [ ] Check for available system updates: `sudo apt update`

### Monthly Tasks
- [ ] Update system packages: `sudo apt upgrade -y`
- [ ] Review and optimize database performance
- [ ] Test database restore from backup
- [ ] Review security logs and firewall rules

### Quarterly Tasks
- [ ] Full security audit
- [ ] Performance optimization review
- [ ] Backup retention policy review
- [ ] Update SSL certificates if needed

---

## Troubleshooting

### Backend Service Won't Start
```bash
# Check service status
sudo systemctl status prmis-backend.service

# View detailed logs
sudo journalctl -u prmis-backend.service -n 100

# Check if port 5000 is in use
sudo lsof -i :5000

# Check database connection
psql -h localhost -U prmis_user -d PRMIS -c "SELECT 1;"
```

### Frontend Not Loading
```bash
# Check Nginx configuration
sudo nginx -t

# Check Nginx logs
sudo tail -f /var/log/nginx/error.log

# Verify frontend files
ls -la /var/www/prmis/frontend/dist/property-registeration-mis/
```

### Database Connection Issues
```bash
# Check PostgreSQL is running
sudo systemctl status postgresql

# Test connection
psql -h localhost -U prmis_user -d PRMIS

# Check connection string
cat /var/www/prmis/backend/appsettings.Production.json
```

### High CPU/Memory Usage
```bash
# Check running processes
top
# or
htop

# Check disk usage
df -h
du -sh /var/www/prmis/*

# Check database size
sudo -u postgres psql -d PRMIS -c "SELECT pg_size_pretty(pg_database_size('PRMIS'));"
```

---

## Rollback Plan

If deployment fails:

### Step 1: Stop Services
```bash
sudo systemctl stop prmis-backend.service
sudo systemctl stop nginx
```

### Step 2: Restore Previous Version
```bash
# If you have a backup of previous publish folder
sudo cp -r /var/www/prmis/backend/publish.backup /var/www/prmis/backend/publish
```

### Step 3: Restore Database (if needed)
```bash
# List available backups
ls -la /var/www/prmis/backups/

# Restore from backup
psql -h localhost -U prmis_user -d PRMIS < /var/www/prmis/backups/PRMIS_YYYYMMDD_HHMMSS.sql
```

### Step 4: Restart Services
```bash
sudo systemctl start prmis-backend.service
sudo systemctl start nginx
```

---

## Important Notes

### Security
- ⚠️ Change default admin password immediately
- ⚠️ Use strong, unique passwords for database
- ⚠️ Keep JWT_Secret secure
- ⚠️ Enable HTTPS in production
- ⚠️ Regularly update system packages
- ⚠️ Monitor logs for suspicious activity

### Performance
- ✓ Server has sufficient resources (32GB RAM, 8 cores)
- ✓ PostgreSQL is tuned for production
- ✓ Nginx is configured with compression and caching
- ✓ Backup script runs daily at 2 AM

### Maintenance
- ✓ Database backups are automated
- ✓ Logs are centralized with systemd journal
- ✓ Service auto-restarts on failure
- ✓ SSL auto-renewal is configured

---

## Support Resources

- **Deployment Guide**: `LINUX_DEPLOYMENT_GUIDE.md`
- **Nginx Logs**: `/var/log/nginx/prmis_access.log` and `/var/log/nginx/prmis_error.log`
- **Backend Logs**: `sudo journalctl -u prmis-backend.service -f`
- **Database Logs**: `/var/log/postgresql/postgresql-15-main.log`

---

**Deployment Date**: _______________
**Deployed By**: _______________
**Version**: _______________
**Notes**: _______________________________________________

---

**Last Updated**: 2024
**Version**: 1.0

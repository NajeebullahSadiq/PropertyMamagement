# Property Management System - Linux Deployment Guide

## Server Specifications
- **OS**: Ubuntu 24.04 LTS
- **RAM**: 32 GiB
- **CPU**: 8 cores
- **Storage**: 2×1.2 TiB
- **Model**: Dell R730

## Project Stack
- **Frontend**: Angular 15
- **Backend**: .NET 9.0
- **Database**: PostgreSQL
- **Web Server**: Nginx (reverse proxy)
- **Process Manager**: Systemd

---

## Phase 1: Server Preparation

### 1.1 Update System
```bash
sudo apt update
sudo apt upgrade -y
sudo apt install -y curl wget git nano htop
```

### 1.2 Install PostgreSQL 15
```bash
sudo apt install -y postgresql postgresql-contrib postgresql-15-contrib

# Start and enable PostgreSQL
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Verify installation
sudo -u postgres psql --version
```

### 1.3 Install .NET 9 Runtime and SDK
```bash
# Add Microsoft repository
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK and Runtime
sudo apt update
sudo apt install -y dotnet-sdk-9.0 dotnet-runtime-9.0

# Verify installation
dotnet --version
```

### 1.4 Install Node.js and npm
```bash
# Install Node.js 18 LTS
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs

# Verify installation
node --version
npm --version
```

### 1.5 Install Nginx
```bash
sudo apt install -y nginx

# Start and enable Nginx
sudo systemctl start nginx
sudo systemctl enable nginx

# Verify installation
sudo systemctl status nginx
```

---

## Phase 2: Database Setup

### 2.1 Create PostgreSQL Database and User
```bash
# Connect to PostgreSQL
sudo -u postgres psql

# Inside PostgreSQL prompt, run these commands:
CREATE USER prmis_user WITH PASSWORD 'SecurePassword@2024';
CREATE DATABASE "PRMIS" OWNER prmis_user;
ALTER ROLE prmis_user WITH CREATEDB;
GRANT ALL PRIVILEGES ON DATABASE "PRMIS" TO prmis_user;
\q
```

### 2.2 Verify Database Connection
```bash
psql -h localhost -U prmis_user -d PRMIS -c "SELECT version();"
```

---

## Phase 3: Application Deployment

### 3.1 Create Application Directory Structure
```bash
# Create directories
sudo mkdir -p /var/www/prmis
sudo mkdir -p /var/www/prmis/backend
sudo mkdir -p /var/www/prmis/frontend
sudo mkdir -p /var/www/prmis/backend/Resources
sudo mkdir -p /var/log/prmis

# Set permissions
sudo chown -R $USER:$USER /var/www/prmis
sudo chown -R $USER:$USER /var/log/prmis
sudo chmod -R 755 /var/www/prmis
```

### 3.2 Copy Project Files
```bash
# From your local machine or copy from the server where code exists
# Copy Backend
cp -r /path/to/Backend/* /var/www/prmis/backend/

# Copy Frontend
cp -r /path/to/Frontend/* /var/www/prmis/frontend/

# Verify
ls -la /var/www/prmis/backend/
ls -la /var/www/prmis/frontend/
```

### 3.3 Configure Backend (appsettings.Production.json)
Create `/var/www/prmis/backend/appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "connection": {
    "connectionString": "Server=localhost; Database=PRMIS; Username=prmis_user; Password=SecurePassword@2024"
  },
  "ApplicationSettings": {
    "JWT_Secret": "aj@jahanbeen@mcit@!*",
    "Client_URL": "http://your-server-ip-or-domain"
  },
  "AllowedHosts": "*"
}
```

### 3.4 Build and Publish Backend
```bash
cd /var/www/prmis/backend

# Restore dependencies
dotnet restore

# Build for production
dotnet build -c Release

# Publish
dotnet publish -c Release -o /var/www/prmis/backend/publish

# Verify publish
ls -la /var/www/prmis/backend/publish/
```

### 3.5 Build Frontend
```bash
cd /var/www/prmis/frontend

# Install dependencies
npm install

# Build for production
npm run build

# Verify build
ls -la /var/www/prmis/frontend/dist/
```

---

## Phase 4: Systemd Service Configuration

### 4.1 Create Backend Service File
Create `/etc/systemd/system/prmis-backend.service`:
```ini
[Unit]
Description=Property Management System - Backend
After=network.target postgresql.service

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/prmis/backend/publish
ExecStart=/usr/bin/dotnet /var/www/prmis/backend/publish/WebAPIBackend.dll
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://localhost:5000"

[Install]
WantedBy=multi-user.target
```

### 4.2 Set Permissions and Enable Service
```bash
# Set permissions
sudo chown root:root /etc/systemd/system/prmis-backend.service
sudo chmod 644 /etc/systemd/system/prmis-backend.service

# Reload systemd
sudo systemctl daemon-reload

# Enable and start service
sudo systemctl enable prmis-backend.service
sudo systemctl start prmis-backend.service

# Verify service
sudo systemctl status prmis-backend.service
```

---

## Phase 5: Nginx Configuration

### 5.1 Create Nginx Configuration
Create `/etc/nginx/sites-available/prmis`:
```nginx
# Backend API Server
upstream backend {
    server localhost:5000;
}

# Frontend Server
server {
    listen 80;
    server_name your-server-ip-or-domain;

    # Frontend
    location / {
        root /var/www/prmis/frontend/dist/property-registeration-mis;
        try_files $uri $uri/ /index.html;
        
        # Cache busting for index.html
        location = /index.html {
            add_header Cache-Control "no-cache, no-store, must-revalidate";
        }
    }

    # Backend API
    location /api/ {
        proxy_pass http://backend;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # Static resources
    location /Resources/ {
        alias /var/www/prmis/backend/publish/Resources/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css text/xml text/javascript 
               application/x-javascript application/xml+rss 
               application/javascript application/json;
    gzip_min_length 1000;
    gzip_proxied any;
    gzip_vary on;
}
```

### 5.2 Enable Nginx Configuration
```bash
# Create symbolic link
sudo ln -s /etc/nginx/sites-available/prmis /etc/nginx/sites-enabled/prmis

# Remove default site if needed
sudo rm /etc/nginx/sites-enabled/default

# Test Nginx configuration
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx
```

---

## Phase 6: SSL/TLS Certificate (Optional but Recommended)

### 6.1 Install Certbot
```bash
sudo apt install -y certbot python3-certbot-nginx
```

### 6.2 Generate SSL Certificate
```bash
sudo certbot --nginx -d your-domain.com

# Auto-renewal is configured automatically
sudo systemctl enable certbot.timer
```

### 6.3 Update Nginx Configuration for HTTPS
The certbot will automatically update your Nginx configuration.

---

## Phase 7: Verification and Testing

### 7.1 Check Service Status
```bash
# Backend service
sudo systemctl status prmis-backend.service

# Nginx
sudo systemctl status nginx

# PostgreSQL
sudo systemctl status postgresql

# View backend logs
sudo journalctl -u prmis-backend.service -f

# View Nginx logs
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

### 7.2 Test API Endpoint
```bash
# Test backend API
curl http://localhost:5000/api/health

# Or from another machine
curl http://your-server-ip/api/health
```

### 7.3 Test Frontend
Open browser and navigate to:
- `http://your-server-ip` (or your domain)

---

## Phase 8: Backup and Maintenance

### 8.1 Database Backup Script
Create `/var/www/prmis/backup-db.sh`:
```bash
#!/bin/bash
BACKUP_DIR="/var/www/prmis/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/PRMIS_$TIMESTAMP.sql"

mkdir -p $BACKUP_DIR

pg_dump -h localhost -U prmis_user -d PRMIS > $BACKUP_FILE

# Keep only last 30 days of backups
find $BACKUP_DIR -name "PRMIS_*.sql" -mtime +30 -delete

echo "Backup completed: $BACKUP_FILE"
```

### 8.2 Schedule Daily Backup
```bash
# Make script executable
chmod +x /var/www/prmis/backup-db.sh

# Add to crontab
sudo crontab -e

# Add this line (runs daily at 2 AM)
0 2 * * * /var/www/prmis/backup-db.sh
```

---

## Phase 9: Monitoring and Logging

### 9.1 View Application Logs
```bash
# Backend logs
sudo journalctl -u prmis-backend.service -n 100 -f

# Nginx access logs
sudo tail -f /var/log/nginx/access.log

# Nginx error logs
sudo tail -f /var/log/nginx/error.log

# PostgreSQL logs
sudo tail -f /var/log/postgresql/postgresql-15-main.log
```

### 9.2 System Resource Monitoring
```bash
# Real-time monitoring
htop

# Disk usage
df -h

# Memory usage
free -h

# Network connections
netstat -tuln | grep LISTEN
```

---

## Phase 10: Troubleshooting

### Issue: Backend service won't start
```bash
# Check service status
sudo systemctl status prmis-backend.service

# View detailed logs
sudo journalctl -u prmis-backend.service -n 50

# Check if port 5000 is in use
sudo lsof -i :5000

# Check database connection
psql -h localhost -U prmis_user -d PRMIS -c "SELECT 1;"
```

### Issue: Frontend not loading
```bash
# Check Nginx configuration
sudo nginx -t

# Check Nginx logs
sudo tail -f /var/log/nginx/error.log

# Verify frontend files exist
ls -la /var/www/prmis/frontend/dist/
```

### Issue: Database connection fails
```bash
# Check PostgreSQL is running
sudo systemctl status postgresql

# Test connection
psql -h localhost -U prmis_user -d PRMIS

# Check connection string in appsettings.Production.json
cat /var/www/prmis/backend/appsettings.Production.json
```

---

## Phase 11: Security Hardening

### 11.1 Firewall Configuration
```bash
# Enable UFW
sudo ufw enable

# Allow SSH
sudo ufw allow 22/tcp

# Allow HTTP
sudo ufw allow 80/tcp

# Allow HTTPS
sudo ufw allow 443/tcp

# Check firewall status
sudo ufw status
```

### 11.2 Security Best Practices
- Change default admin password immediately
- Use strong PostgreSQL password
- Keep JWT_Secret secure and unique
- Regularly update system packages: `sudo apt update && sudo apt upgrade -y`
- Monitor logs regularly
- Set up automated backups
- Use HTTPS in production

---

## Phase 12: Performance Optimization

### 12.1 PostgreSQL Tuning
Edit `/etc/postgresql/15/main/postgresql.conf`:
```ini
# For 32GB RAM server
shared_buffers = 8GB
effective_cache_size = 24GB
maintenance_work_mem = 2GB
checkpoint_completion_target = 0.9
wal_buffers = 16MB
default_statistics_target = 100
random_page_cost = 1.1
effective_io_concurrency = 200
work_mem = 20MB
min_wal_size = 2GB
max_wal_size = 8GB
```

Then restart PostgreSQL:
```bash
sudo systemctl restart postgresql
```

### 12.2 Nginx Performance
Already configured with:
- Gzip compression
- Connection pooling
- Caching headers
- Proxy timeouts

---

## Quick Reference Commands

```bash
# Start/Stop/Restart services
sudo systemctl start prmis-backend.service
sudo systemctl stop prmis-backend.service
sudo systemctl restart prmis-backend.service

# View logs
sudo journalctl -u prmis-backend.service -f
sudo tail -f /var/log/nginx/access.log

# Database operations
psql -h localhost -U prmis_user -d PRMIS
pg_dump -h localhost -U prmis_user -d PRMIS > backup.sql

# Rebuild and redeploy backend
cd /var/www/prmis/backend
dotnet publish -c Release -o /var/www/prmis/backend/publish
sudo systemctl restart prmis-backend.service

# Rebuild and redeploy frontend
cd /var/www/prmis/frontend
npm run build
sudo systemctl reload nginx
```

---

## Support and Documentation

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Angular Documentation](https://angular.io/docs)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Nginx Documentation](https://nginx.org/en/docs/)

---

**Last Updated**: 2024
**Version**: 1.0

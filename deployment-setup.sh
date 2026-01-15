#!/bin/bash

# Property Management System - Linux Deployment Setup Script
# Run this script on the Ubuntu 24.04 server as root or with sudo
# Usage: sudo bash deployment-setup.sh

set -e

echo "=========================================="
echo "Property Management System - Deployment"
echo "=========================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[✓]${NC} $1"
}

print_error() {
    echo -e "${RED}[✗]${NC} $1"
}

print_info() {
    echo -e "${YELLOW}[i]${NC} $1"
}

# ============================================
# Phase 1: System Update
# ============================================
print_info "Phase 1: Updating system packages..."
apt update
apt upgrade -y
apt install -y curl wget git nano htop net-tools
print_status "System packages updated"

# ============================================
# Phase 2: PostgreSQL Installation
# ============================================
print_info "Phase 2: Installing PostgreSQL..."
apt install -y postgresql postgresql-contrib postgresql-15-contrib
systemctl start postgresql
systemctl enable postgresql
print_status "PostgreSQL installed and enabled"

# ============================================
# Phase 3: .NET Installation
# ============================================
print_info "Phase 3: Installing .NET 9..."
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
apt update
apt install -y dotnet-sdk-9.0 dotnet-runtime-9.0
print_status ".NET 9 installed"
dotnet --version

# ============================================
# Phase 4: Node.js Installation
# ============================================
print_info "Phase 4: Installing Node.js..."
curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
apt install -y nodejs
print_status "Node.js installed"
node --version
npm --version

# ============================================
# Phase 5: Nginx Installation
# ============================================
print_info "Phase 5: Installing Nginx..."
apt install -y nginx
systemctl start nginx
systemctl enable nginx
print_status "Nginx installed and enabled"

# ============================================
# Phase 6: Create Application Directories
# ============================================
print_info "Phase 6: Creating application directories..."
mkdir -p /var/www/prmis/backend
mkdir -p /var/www/prmis/backend/publish
mkdir -p /var/www/prmis/frontend
mkdir -p /var/www/prmis/storage/Resources
mkdir -p /var/www/prmis/storage/Resources/Images
mkdir -p /var/www/prmis/storage/Resources/Documents
mkdir -p /var/www/prmis/storage/Resources/Documents/Identity
mkdir -p /var/www/prmis/storage/Resources/Documents/Profile
mkdir -p /var/www/prmis/storage/Resources/Documents/Property
mkdir -p /var/www/prmis/storage/Resources/Documents/Vehicle
mkdir -p /var/www/prmis/storage/Resources/Documents/Company
mkdir -p /var/www/prmis/storage/Resources/Documents/License
mkdir -p /var/www/prmis/backups
mkdir -p /var/log/prmis

# Set permissions
chown -R www-data:www-data /var/www/prmis
chmod -R 755 /var/www/prmis
chown -R www-data:www-data /var/log/prmis
chmod -R 755 /var/log/prmis

print_status "Application directories created"

# ============================================
# Phase 7: PostgreSQL Database Setup
# ============================================
print_info "Phase 7: Setting up PostgreSQL database..."

# Create database and user
sudo -u postgres psql <<EOF
CREATE USER prmis_user WITH PASSWORD 'SecurePassword@2024';
CREATE DATABASE "PRMIS" OWNER prmis_user;
ALTER ROLE prmis_user WITH CREATEDB;
GRANT ALL PRIVILEGES ON DATABASE "PRMIS" TO prmis_user;
\c PRMIS
GRANT ALL PRIVILEGES ON SCHEMA public TO prmis_user;
EOF

print_status "PostgreSQL database and user created"

# ============================================
# Phase 8: Firewall Configuration
# ============================================
print_info "Phase 8: Configuring firewall..."
ufw --force enable
ufw allow 22/tcp
ufw allow 80/tcp
ufw allow 443/tcp
print_status "Firewall configured"
ufw status

# ============================================
# Phase 9: Create Systemd Service
# ============================================
print_info "Phase 9: Creating systemd service..."

cat > /etc/systemd/system/prmis-backend.service <<'SERVICEFILE'
[Unit]
Description=Property Management System - Backend
After=network.target postgresql.service
Wants=postgresql.service

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/prmis/backend
ExecStart=/usr/bin/dotnet /var/www/prmis/backend/WebAPIBackend.dll
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
SyslogIdentifier=prmis-backend
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://localhost:5000"
Environment="DOTNET_CLI_TELEMETRY_OPTOUT=1"

NoNewPrivileges=true
PrivateTmp=true
ProtectSystem=strict
ProtectHome=true
ReadWritePaths=/var/www/prmis/storage

[Install]
WantedBy=multi-user.target
SERVICEFILE

chmod 644 /etc/systemd/system/prmis-backend.service
systemctl daemon-reload
print_status "Systemd service created"

# ============================================
# Phase 10: Configure Nginx
# ============================================
print_info "Phase 10: Configuring Nginx..."

# Remove default site
rm -f /etc/nginx/sites-enabled/default

# Create nginx configuration
cat > /etc/nginx/sites-available/prmis <<'NGINXFILE'
upstream backend {
    server localhost:5000;
    keepalive 32;
}

server {
    listen 80;
    server_name _;
    
    access_log /var/log/nginx/prmis_access.log;
    error_log /var/log/nginx/prmis_error.log;

    client_max_body_size 100M;

    gzip on;
    gzip_vary on;
    gzip_min_length 1000;
    gzip_proxied any;
    gzip_types text/plain text/css text/xml text/javascript 
               application/x-javascript application/xml+rss 
               application/rss+xml application/atom+xml 
               application/javascript application/json;

    location / {
        root /var/www/prmis/frontend/dist/property-registeration-mis;
        try_files $uri $uri/ /index.html;
        
        location = /index.html {
            add_header Cache-Control "no-cache, no-store, must-revalidate";
            add_header Pragma "no-cache";
            add_header Expires "0";
        }
        
        location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
            expires 30d;
            add_header Cache-Control "public, immutable";
        }
    }

    location /api/ {
        proxy_pass http://backend;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
        proxy_cache_bypass $http_upgrade;
    }

    location /Resources/ {
        alias /var/www/prmis/storage/Resources/;
        expires 30d;
        add_header Cache-Control "public, immutable";
        access_log off;
    }

    location /api/Resources/ {
        alias /var/www/prmis/storage/Resources/;
        expires 30d;
        add_header Cache-Control "public, immutable";
        access_log off;
    }

    location /health {
        access_log off;
        return 200 "healthy\n";
        add_header Content-Type text/plain;
    }

    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    location ~ /\. {
        deny all;
        access_log off;
        log_not_found off;
    }
}
NGINXFILE

ln -sf /etc/nginx/sites-available/prmis /etc/nginx/sites-enabled/prmis
nginx -t
systemctl reload nginx
print_status "Nginx configured"

# ============================================
# Phase 11: Create Backup Script
# ============================================
print_info "Phase 11: Creating backup script..."

cat > /var/www/prmis/backup-db.sh <<'BACKUPFILE'
#!/bin/bash
BACKUP_DIR="/var/www/prmis/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/PRMIS_$TIMESTAMP.sql"

mkdir -p $BACKUP_DIR

pg_dump -h localhost -U prmis_user -d PRMIS > $BACKUP_FILE

# Keep only last 30 days of backups
find $BACKUP_DIR -name "PRMIS_*.sql" -mtime +30 -delete

echo "Backup completed: $BACKUP_FILE"
BACKUPFILE

chmod +x /var/www/prmis/backup-db.sh
chown www-data:www-data /var/www/prmis/backup-db.sh

# Add to crontab (runs daily at 2 AM)
(crontab -u www-data -l 2>/dev/null; echo "0 2 * * * /var/www/prmis/backup-db.sh") | crontab -u www-data -
print_status "Backup script created and scheduled"

# ============================================
# Summary
# ============================================
echo ""
echo "=========================================="
echo -e "${GREEN}Installation Complete!${NC}"
echo "=========================================="
echo ""
echo "Next Steps:"
echo "1. Copy your application files to:"
echo "   - Backend: /var/www/prmis/backend/"
echo "   - Frontend: /var/www/prmis/frontend/"
echo ""
echo "2. Update configuration files:"
echo "   - Edit: /var/www/prmis/backend/appsettings.Production.json"
echo "   - Update Client_URL to your server IP/domain"
echo ""
echo "3. Build and publish backend:"
echo "   cd /var/www/prmis/backend"
echo "   dotnet restore"
echo "   dotnet publish -c Release -o /var/www/prmis/backend/publish"
echo ""
echo "4. Build frontend:"
echo "   cd /var/www/prmis/frontend"
echo "   npm install"
echo "   npm run build"
echo ""
echo "5. Set permissions:"
echo "   sudo chown -R www-data:www-data /var/www/prmis"
echo ""
echo "6. Start backend service:"
echo "   sudo systemctl start prmis-backend.service"
echo "   sudo systemctl status prmis-backend.service"
echo ""
echo "7. Access application:"
echo "   http://your-server-ip"
echo ""
echo "Database Credentials:"
echo "   Host: localhost"
echo "   Database: PRMIS"
echo "   Username: prmis_user"
echo "   Password: SecurePassword@2024"
echo ""
echo "=========================================="

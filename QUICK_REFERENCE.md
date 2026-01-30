# PRMIS Quick Reference Card

## 🚀 Deployment Commands

### First-Time Setup (Clean Server)
```bash
# Make script executable
chmod +x deploy.sh

# Run initial setup
./deploy.sh setup
```

### Regular Deployments
```bash
# Deploy everything
./deploy.sh full

# Deploy only frontend
./deploy.sh frontend

# Deploy only backend
./deploy.sh backend
```

---

## 🔧 Service Management

### Backend Service
```bash
# Start
sudo systemctl start prmis-backend

# Stop
sudo systemctl stop prmis-backend

# Restart
sudo systemctl restart prmis-backend

# Status
sudo systemctl status prmis-backend

# Enable on boot
sudo systemctl enable prmis-backend

# Disable on boot
sudo systemctl disable prmis-backend
```

### Nginx
```bash
# Start
sudo systemctl start nginx

# Stop
sudo systemctl stop nginx

# Restart
sudo systemctl restart nginx

# Reload config (no downtime)
sudo systemctl reload nginx

# Test config
sudo nginx -t

# Status
sudo systemctl status nginx
```

---

## 📋 Logs

### Backend Logs
```bash
# View last 50 lines
sudo journalctl -u prmis-backend -n 50

# Follow logs (real-time)
sudo journalctl -u prmis-backend -f

# View logs since today
sudo journalctl -u prmis-backend --since today

# View logs with timestamps
sudo journalctl -u prmis-backend -n 50 --no-pager
```

### Nginx Logs
```bash
# Error log (real-time)
sudo tail -f /var/log/nginx/prmis_error.log

# Access log (real-time)
sudo tail -f /var/log/nginx/prmis_access.log

# Last 100 lines of error log
sudo tail -n 100 /var/log/nginx/prmis_error.log
```

---

## 🗄️ Database Operations

### Connect to Database
```bash
sudo -u postgres psql -d PRMIS
```

### Run Migration
```bash
cd ~/PropertyMamagement/Backend
sudo -u postgres psql -d PRMIS -f Scripts/Modules/01_Shared_Initial.sql
```

### Backup Database
```bash
sudo -u postgres pg_dump PRMIS > prmis_backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restore Database
```bash
sudo -u postgres psql PRMIS < prmis_backup_20260130_120000.sql
```

### Check Database Size
```bash
sudo -u postgres psql -d PRMIS -c "SELECT pg_size_pretty(pg_database_size('PRMIS'));"
```

---

## 📁 File Locations

| Component | Path |
|-----------|------|
| Frontend | `/var/www/prmis/frontend` |
| Backend | `/var/www/prmis/backend` |
| Storage | `/var/www/prmis/storage` |
| Nginx Config | `/etc/nginx/sites-available/prmis` |
| Service File | `/etc/systemd/system/prmis-backend.service` |
| Repository | `~/PropertyMamagement` |

---

## 🔍 Troubleshooting

### Backend Won't Start
```bash
# Check logs
sudo journalctl -u prmis-backend -n 100 --no-pager

# Check if port is in use
sudo netstat -tulpn | grep 5000

# Check file permissions
ls -la /var/www/prmis/backend/

# Verify .NET is installed
dotnet --version

# Test backend manually
cd /var/www/prmis/backend
sudo -u www-data dotnet WebAPIBackend.dll
```

### Frontend Not Loading
```bash
# Check nginx logs
sudo tail -f /var/log/nginx/prmis_error.log

# Verify files exist
ls -la /var/www/prmis/frontend/

# Check nginx config
sudo nginx -t

# Restart nginx
sudo systemctl restart nginx
```

### Database Connection Failed
```bash
# Test connection
psql -U prmis_user -d PRMIS -h localhost

# Check PostgreSQL status
sudo systemctl status postgresql

# Restart PostgreSQL
sudo systemctl restart postgresql

# Check connection string in appsettings
cat /var/www/prmis/backend/appsettings.Production.json
```

### Permission Denied Errors
```bash
# Fix all permissions
sudo chown -R www-data:www-data /var/www/prmis
sudo chmod -R 755 /var/www/prmis

# Fix storage permissions specifically
sudo chown -R www-data:www-data /var/www/prmis/storage
sudo chmod -R 755 /var/www/prmis/storage

# Fix .dotnet directory
sudo chown -R www-data:www-data /var/www/.dotnet
sudo chmod -R 755 /var/www/.dotnet
```

### 502 Bad Gateway
```bash
# Backend is not running - check status
sudo systemctl status prmis-backend

# Start backend
sudo systemctl start prmis-backend

# Check if backend is listening
curl http://localhost:5000/api/health
```

### 404 Not Found (Frontend Routes)
```bash
# Nginx not configured for Angular routing
# Verify try_files directive in nginx config
sudo nano /etc/nginx/sites-available/prmis

# Should have: try_files $uri $uri/ /index.html;
# Then restart nginx
sudo systemctl restart nginx
```

---

## 🔐 Security

### Update Firewall
```bash
# Allow HTTP/HTTPS
sudo ufw allow 'Nginx Full'

# Allow SSH
sudo ufw allow OpenSSH

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status
```

### SSL Certificate (Let's Encrypt)
```bash
# Install certbot
sudo apt install certbot python3-certbot-nginx

# Get certificate
sudo certbot --nginx -d your-domain.com

# Test renewal
sudo certbot renew --dry-run
```

---

## 📊 Monitoring

### Check System Resources
```bash
# CPU and Memory
htop

# Disk usage
df -h

# Check specific directory size
du -sh /var/www/prmis/*
```

### Check Service Status
```bash
# All services
sudo systemctl status prmis-backend nginx postgresql

# Check if services are enabled
sudo systemctl is-enabled prmis-backend nginx postgresql
```

### Network Connections
```bash
# Check listening ports
sudo netstat -tulpn | grep -E '(5000|80|443|5432)'

# Check active connections
sudo netstat -an | grep ESTABLISHED
```

---

## 🔄 Update Code

### Pull Latest Changes
```bash
cd ~/PropertyMamagement
git pull origin main
```

### Deploy After Update
```bash
# If only frontend changed
./deploy.sh frontend

# If only backend changed
./deploy.sh backend

# If both changed
./deploy.sh full
```

---

## 🧹 Cleanup

### Clear Old Logs
```bash
# Clear journal logs older than 7 days
sudo journalctl --vacuum-time=7d

# Clear nginx logs
sudo truncate -s 0 /var/log/nginx/prmis_access.log
sudo truncate -s 0 /var/log/nginx/prmis_error.log
```

### Remove Old Backups
```bash
# Find backups older than 30 days
find ~/ -name "prmis_backup_*.sql" -mtime +30

# Delete them
find ~/ -name "prmis_backup_*.sql" -mtime +30 -delete
```

---

## 📞 Emergency Commands

### Complete Restart
```bash
sudo systemctl restart prmis-backend
sudo systemctl restart nginx
sudo systemctl restart postgresql
```

### Check Everything
```bash
# Services
sudo systemctl status prmis-backend nginx postgresql

# Logs
sudo journalctl -u prmis-backend -n 20
sudo tail -n 20 /var/log/nginx/prmis_error.log

# Disk space
df -h

# Processes
ps aux | grep -E '(dotnet|nginx|postgres)'
```

### Nuclear Option (Full Redeploy)
```bash
cd ~/PropertyMamagement
git pull origin main
./deploy.sh full
```

---

## 📝 Configuration Files

### Edit Backend Config
```bash
sudo nano /var/www/prmis/backend/appsettings.Production.json
# After editing, restart backend
sudo systemctl restart prmis-backend
```

### Edit Nginx Config
```bash
sudo nano /etc/nginx/sites-available/prmis
# Test config
sudo nginx -t
# Reload nginx
sudo systemctl reload nginx
```

### Edit Service File
```bash
sudo nano /etc/systemd/system/prmis-backend.service
# Reload daemon
sudo systemctl daemon-reload
# Restart service
sudo systemctl restart prmis-backend
```

---

## 🎯 Common Tasks

### Add New Admin User (via Database)
```bash
sudo -u postgres psql -d PRMIS
# Then run SQL to insert user
```

### Clear Application Cache
```bash
# Clear browser cache on client side
# Or restart backend to clear server cache
sudo systemctl restart prmis-backend
```

### Update Environment Variables
```bash
# Edit service file
sudo nano /etc/systemd/system/prmis-backend.service
# Add/modify Environment= lines
sudo systemctl daemon-reload
sudo systemctl restart prmis-backend
```

---

## 📱 Quick Health Check

```bash
# One-liner to check everything
echo "Backend:" && sudo systemctl is-active prmis-backend && \
echo "Nginx:" && sudo systemctl is-active nginx && \
echo "PostgreSQL:" && sudo systemctl is-active postgresql && \
echo "Backend API:" && curl -s http://localhost:5000/api/health && \
echo -e "\nAll systems operational!"
```

---

## 🆘 Get Help

1. Check logs first: `sudo journalctl -u prmis-backend -n 50`
2. Check nginx logs: `sudo tail -f /var/log/nginx/prmis_error.log`
3. Verify services running: `sudo systemctl status prmis-backend nginx`
4. Check file permissions: `ls -la /var/www/prmis/`
5. Test database connection: `psql -U prmis_user -d PRMIS -h localhost`

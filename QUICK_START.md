# Quick Start - Linux Deployment

## 🚀 Fast Track Deployment (30 minutes)

### Prerequisites
- Ubuntu 24.04 server with sudo access
- SSH connection to server
- Project code available

---

## Step 1: Run Automated Setup (10 minutes)

```bash
# SSH into your server
ssh user@your-server-ip

# Download and run setup script
# Option A: If you have the script file
sudo bash deployment-setup.sh

# Option B: If you need to create it first
# Copy the content from deployment-setup.sh file and run it
```

**What this does:**
- ✓ Updates system
- ✓ Installs PostgreSQL, .NET 9, Node.js, Nginx
- ✓ Creates directories and database
- ✓ Configures firewall
- ✓ Sets up systemd service
- ✓ Configures Nginx

---

## Step 2: Copy Application Files (5 minutes)

```bash
# From your local machine or existing server
# Copy Backend
scp -r Backend/* user@server:/var/www/prmis/backend/

# Copy Frontend  
scp -r Frontend/* user@server:/var/www/prmis/frontend/
```

---

## Step 3: Configure and Build (10 minutes)

```bash
# SSH into server
ssh user@server

# Update backend configuration
sudo nano /var/www/prmis/backend/appsettings.Production.json
# Change: Client_URL to your server IP/domain

# Build backend
cd /var/www/prmis/backend
dotnet restore
dotnet publish -c Release -o /var/www/prmis/backend/publish

# Build frontend
cd /var/www/prmis/frontend
npm install
npm run build

# Set permissions
sudo chown -R www-data:www-data /var/www/prmis
```

---

## Step 4: Start Services (5 minutes)

```bash
# Start backend
sudo systemctl start prmis-backend.service
sudo systemctl status prmis-backend.service

# Reload Nginx
sudo systemctl reload nginx

# Test
curl http://localhost/api/health
```

---

## Step 5: Access Application

Open browser:
```
http://your-server-ip
```

**Default Login:**
- Email: `admin@prmis.gov.af`
- Password: `Admin@123`

⚠️ **Change password immediately after login!**

---

## 🔧 Common Commands

```bash
# Check backend status
sudo systemctl status prmis-backend.service

# View backend logs
sudo journalctl -u prmis-backend.service -f

# Restart backend
sudo systemctl restart prmis-backend.service

# Check Nginx
sudo systemctl status nginx
sudo nginx -t

# Database backup
sudo /var/www/prmis/backup-db.sh

# System monitoring
htop
df -h
free -h
```

---

## 📊 Database Credentials

```
Host: localhost
Database: PRMIS
Username: prmis_user
Password: SecurePassword@2024
```

---

## 🐛 Troubleshooting

### Backend won't start?
```bash
sudo journalctl -u prmis-backend.service -n 50
# Check logs for errors
```

### Frontend not loading?
```bash
sudo nginx -t
sudo tail -f /var/log/nginx/error.log
```

### Database connection failed?
```bash
psql -h localhost -U prmis_user -d PRMIS
# Test connection directly
```

---

## 📚 Full Documentation

See `LINUX_DEPLOYMENT_GUIDE.md` for complete details

See `DEPLOYMENT_CHECKLIST.md` for verification steps

---

## ✅ Verification Checklist

- [ ] Backend service running: `sudo systemctl status prmis-backend.service`
- [ ] Nginx running: `sudo systemctl status nginx`
- [ ] API responds: `curl http://localhost/api/health`
- [ ] Frontend loads: Open `http://your-server-ip` in browser
- [ ] Can login with admin credentials
- [ ] Changed admin password
- [ ] Database backups scheduled

---

**Ready to deploy? Start with Step 1!**

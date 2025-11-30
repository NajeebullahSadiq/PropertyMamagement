# Property Management System - Linux Deployment Package

## 📋 Overview

This deployment package contains everything needed to deploy the Property Management System (PRMIS) to a Linux server (Ubuntu 24.04 LTS).

**Stack:**
- Frontend: Angular 15
- Backend: .NET 9.0
- Database: PostgreSQL 15
- Web Server: Nginx
- Process Manager: Systemd

**Server Specs:**
- OS: Ubuntu 24.04 LTS
- RAM: 32 GB
- CPU: 8 cores
- Storage: 2×1.2 TB

---

## 📁 Files in This Package

### Documentation Files

| File | Purpose |
|------|---------|
| `QUICK_START.md` | **START HERE** - Fast track deployment (30 min) |
| `LINUX_DEPLOYMENT_GUIDE.md` | Complete step-by-step deployment guide |
| `DEPLOYMENT_CHECKLIST.md` | Verification checklist and troubleshooting |
| `ENVIRONMENT_SETUP.md` | Configuration and environment variables |
| `MONITORING_AND_LOGS.md` | System monitoring and log management |
| `BACKUP_AND_RECOVERY.md` | Backup strategy and recovery procedures |

### Configuration Files

| File | Purpose | Destination |
|------|---------|-------------|
| `appsettings.Production.json` | Backend production config | `/var/www/prmis/backend/` |
| `prmis-backend.service` | Systemd service file | `/etc/systemd/system/` |
| `nginx-prmis.conf` | Nginx configuration | `/etc/nginx/sites-available/` |

### Automation Scripts

| File | Purpose |
|------|---------|
| `deployment-setup.sh` | Automated server setup script |

---

## 🚀 Quick Start (Recommended)

### For First-Time Deployment

1. **Read the Quick Start Guide**
   ```
   Open: QUICK_START.md
   Time: 5 minutes
   ```

2. **Run Automated Setup**
   ```bash
   sudo bash deployment-setup.sh
   ```
   Time: 15-20 minutes

3. **Deploy Application**
   ```bash
   # Copy files, build, and start services
   # Follow QUICK_START.md steps 2-5
   ```
   Time: 10 minutes

4. **Verify Deployment**
   ```bash
   # Test API and frontend
   # Follow DEPLOYMENT_CHECKLIST.md
   ```
   Time: 5 minutes

**Total Time: ~30-40 minutes**

---

## 📖 Detailed Deployment Path

### For Complete Understanding

1. **Read LINUX_DEPLOYMENT_GUIDE.md**
   - Understand each phase
   - Learn what each component does
   - Understand the architecture

2. **Follow DEPLOYMENT_CHECKLIST.md**
   - Execute each phase
   - Verify each step
   - Troubleshoot issues

3. **Reference ENVIRONMENT_SETUP.md**
   - Configure production settings
   - Update connection strings
   - Set environment variables

4. **Setup MONITORING_AND_LOGS.md**
   - Configure monitoring
   - Setup log rotation
   - Create alerts

5. **Implement BACKUP_AND_RECOVERY.md**
   - Setup automated backups
   - Test recovery procedures
   - Document recovery plan

---

## 🔧 Key Configuration Steps

### 1. Update Backend Configuration

Edit `/var/www/prmis/backend/appsettings.Production.json`:

```json
{
  "connection": {
    "connectionString": "Server=localhost; Database=PRMIS; Username=prmis_user; Password=YOUR_PASSWORD"
  },
  "ApplicationSettings": {
    "Client_URL": "http://YOUR_SERVER_IP_OR_DOMAIN"
  }
}
```

### 2. Update Nginx Configuration

Edit `/etc/nginx/sites-available/prmis`:

```nginx
server {
    server_name your-server-ip-or-domain;
    # ... rest of config
}
```

### 3. Build and Deploy

```bash
# Backend
cd /var/www/prmis/backend
dotnet publish -c Release -o /var/www/prmis/backend/publish

# Frontend
cd /var/www/prmis/frontend
npm install
npm run build
```

### 4. Start Services

```bash
sudo systemctl start prmis-backend.service
sudo systemctl reload nginx
```

---

## 📊 Default Credentials

### Admin User
```
Email: admin@prmis.gov.af
Password: Admin@123
```
⚠️ **Change immediately after first login!**

### Database User
```
Username: prmis_user
Password: SecurePassword@2024
```

---

## 🔐 Security Checklist

- [ ] Change default admin password
- [ ] Change database password (optional but recommended)
- [ ] Update JWT_Secret if needed
- [ ] Enable HTTPS with SSL certificate
- [ ] Configure firewall rules
- [ ] Setup automated backups
- [ ] Enable log monitoring
- [ ] Regular security updates

---

## 📈 Performance Optimization

### Server Resources
- 32 GB RAM: Plenty for production use
- 8 cores: Sufficient for expected load
- 2.4 TB storage: Adequate for database and files

### Optimizations Included
- ✓ PostgreSQL connection pooling
- ✓ Nginx gzip compression
- ✓ Static file caching
- ✓ Database query optimization
- ✓ Systemd service auto-restart

---

## 🛠️ Common Tasks

### View Logs
```bash
# Backend logs
sudo journalctl -u prmis-backend.service -f

# Nginx logs
sudo tail -f /var/log/nginx/prmis_error.log
```

### Restart Services
```bash
# Backend
sudo systemctl restart prmis-backend.service

# Nginx
sudo systemctl reload nginx
```

### Database Backup
```bash
# Manual backup
pg_dump -h localhost -U prmis_user -d PRMIS > backup.sql

# Automated (runs daily at 2 AM)
# Check: /var/www/prmis/backups/
```

### Monitor System
```bash
# Real-time monitoring
htop

# Disk usage
df -h

# Memory usage
free -h
```

---

## 🐛 Troubleshooting

### Backend Won't Start
```bash
# Check logs
sudo journalctl -u prmis-backend.service -n 50

# Common issues:
# - Database connection failed: Check PostgreSQL is running
# - Port 5000 in use: Kill process or change port
# - File not found: Check file permissions
```

### Frontend Not Loading
```bash
# Check Nginx
sudo nginx -t
sudo systemctl status nginx

# Check files
ls -la /var/www/prmis/frontend/dist/
```

### Database Issues
```bash
# Test connection
psql -h localhost -U prmis_user -d PRMIS

# Check PostgreSQL
sudo systemctl status postgresql
```

See **DEPLOYMENT_CHECKLIST.md** for more troubleshooting steps.

---

## 📞 Support Resources

### Documentation
- `LINUX_DEPLOYMENT_GUIDE.md` - Complete deployment guide
- `DEPLOYMENT_CHECKLIST.md` - Verification and troubleshooting
- `ENVIRONMENT_SETUP.md` - Configuration reference
- `MONITORING_AND_LOGS.md` - Monitoring and logging
- `BACKUP_AND_RECOVERY.md` - Backup procedures

### External Resources
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Angular Documentation](https://angular.io/docs)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Nginx Documentation](https://nginx.org/en/docs/)

### Logs and Diagnostics
- Backend logs: `sudo journalctl -u prmis-backend.service -f`
- Nginx logs: `/var/log/nginx/prmis_*.log`
- PostgreSQL logs: `/var/log/postgresql/postgresql-15-main.log`
- System logs: `sudo journalctl -f`

---

## 📋 Deployment Verification

After deployment, verify:

- [ ] Backend service running: `sudo systemctl status prmis-backend.service`
- [ ] Nginx running: `sudo systemctl status nginx`
- [ ] PostgreSQL running: `sudo systemctl status postgresql`
- [ ] API responding: `curl http://localhost/api/health`
- [ ] Frontend loading: Open `http://server-ip` in browser
- [ ] Can login with admin credentials
- [ ] Database backups scheduled
- [ ] Logs are being generated
- [ ] Firewall rules configured
- [ ] SSL certificate installed (if using HTTPS)

---

## 🔄 Maintenance Schedule

### Daily
- Monitor service status
- Check error logs
- Verify backups

### Weekly
- Review performance metrics
- Check disk usage
- Test backup restoration

### Monthly
- Security audit
- Performance optimization
- Update system packages

### Quarterly
- Full system review
- Capacity planning
- Security assessment

---

## 📝 Deployment Log

| Date | Deployed By | Version | Status | Notes |
|------|-------------|---------|--------|-------|
| | | | | |

---

## 🎯 Next Steps

1. **Start with QUICK_START.md** for fastest deployment
2. **Reference LINUX_DEPLOYMENT_GUIDE.md** for detailed steps
3. **Use DEPLOYMENT_CHECKLIST.md** to verify everything works
4. **Setup MONITORING_AND_LOGS.md** for ongoing monitoring
5. **Implement BACKUP_AND_RECOVERY.md** for data protection

---

## ⚠️ Important Notes

### Before Deployment
- Ensure server meets specifications
- Have SSH access configured
- Backup any existing data
- Have domain/IP address ready

### During Deployment
- Follow steps in order
- Don't skip verification steps
- Save configuration files
- Document any changes

### After Deployment
- Change default passwords immediately
- Enable HTTPS if possible
- Setup monitoring and alerts
- Test backup and recovery
- Document your setup

---

## 📞 Support

For issues or questions:
1. Check the relevant documentation file
2. Review DEPLOYMENT_CHECKLIST.md troubleshooting section
3. Check logs using commands in MONITORING_AND_LOGS.md
4. Consult external documentation links

---

**Version**: 1.0  
**Last Updated**: 2024  
**Status**: Ready for Deployment

---

## Quick Links

- 🚀 [Quick Start](QUICK_START.md)
- 📖 [Full Guide](LINUX_DEPLOYMENT_GUIDE.md)
- ✅ [Checklist](DEPLOYMENT_CHECKLIST.md)
- ⚙️ [Configuration](ENVIRONMENT_SETUP.md)
- 📊 [Monitoring](MONITORING_AND_LOGS.md)
- 💾 [Backup](BACKUP_AND_RECOVERY.md)

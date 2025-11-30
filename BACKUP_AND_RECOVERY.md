# Backup and Recovery Guide

## Backup Strategy

### What to Backup

1. **Database** - PostgreSQL PRMIS database
2. **Application Files** - Backend and Frontend code
3. **Configuration Files** - appsettings.Production.json, Nginx config
4. **Uploaded Files** - Resources folder

### Backup Schedule

- **Database**: Daily at 2 AM (automated)
- **Application Code**: Weekly (manual or automated)
- **Full System**: Monthly (manual)

---

## Database Backup

### Automated Daily Backup

The backup script is already configured in the deployment setup:

```bash
# Backup script location
/var/www/prmis/backup-db.sh

# Scheduled to run daily at 2 AM
# Cron job: 0 2 * * * /var/www/prmis/backup-db.sh

# Backups stored in
/var/www/prmis/backups/
```

### Manual Database Backup

#### Full Database Backup
```bash
# Create backup
pg_dump -h localhost -U prmis_user -d PRMIS > /var/www/prmis/backups/PRMIS_manual_$(date +%Y%m%d_%H%M%S).sql

# Compressed backup (recommended for large databases)
pg_dump -h localhost -U prmis_user -d PRMIS | gzip > /var/www/prmis/backups/PRMIS_manual_$(date +%Y%m%d_%H%M%S).sql.gz

# Verify backup
ls -lh /var/www/prmis/backups/
```

#### Backup Specific Table
```bash
# Backup single table
pg_dump -h localhost -U prmis_user -d PRMIS -t "TableName" > table_backup.sql

# Backup multiple tables
pg_dump -h localhost -U prmis_user -d PRMIS -t "Table1" -t "Table2" > tables_backup.sql
```

#### Backup with Custom Options
```bash
# Backup with verbose output
pg_dump -h localhost -U prmis_user -d PRMIS -v > backup.sql

# Backup in custom format (smaller file, faster restore)
pg_dump -h localhost -U prmis_user -d PRMIS -F c -f backup.dump

# Backup with data only (no schema)
pg_dump -h localhost -U prmis_user -d PRMIS -a > data_only.sql

# Backup with schema only (no data)
pg_dump -h localhost -U prmis_user -d PRMIS -s > schema_only.sql
```

### Verify Backup

```bash
# Check backup file size
ls -lh /var/www/prmis/backups/

# Check backup integrity
pg_restore -h localhost -U prmis_user --list /var/www/prmis/backups/PRMIS_*.dump

# Count tables in backup
pg_dump -h localhost -U prmis_user -d PRMIS -s | grep "CREATE TABLE" | wc -l
```

---

## Database Recovery

### Restore Full Database

#### From SQL Backup
```bash
# Method 1: Drop and recreate database
sudo -u postgres psql -c "DROP DATABASE IF EXISTS PRMIS;"
sudo -u postgres psql -c "CREATE DATABASE PRMIS OWNER prmis_user;"

# Restore from backup
psql -h localhost -U prmis_user -d PRMIS < /var/www/prmis/backups/PRMIS_YYYYMMDD_HHMMSS.sql

# Verify restore
psql -h localhost -U prmis_user -d PRMIS -c "SELECT COUNT(*) FROM \"AspNetUsers\";"
```

#### From Compressed Backup
```bash
# Decompress and restore
gunzip -c /var/www/prmis/backups/PRMIS_YYYYMMDD_HHMMSS.sql.gz | psql -h localhost -U prmis_user -d PRMIS
```

#### From Custom Format Backup
```bash
# Restore from custom format
pg_restore -h localhost -U prmis_user -d PRMIS /var/www/prmis/backups/PRMIS_YYYYMMDD_HHMMSS.dump

# Restore with verbose output
pg_restore -h localhost -U prmis_user -d PRMIS -v /var/www/prmis/backups/PRMIS_YYYYMMDD_HHMMSS.dump
```

### Restore Specific Table

```bash
# Restore single table
psql -h localhost -U prmis_user -d PRMIS < table_backup.sql

# Restore from custom format
pg_restore -h localhost -U prmis_user -d PRMIS -t "TableName" backup.dump
```

### Point-in-Time Recovery

```bash
# Enable WAL archiving first (advanced topic)
# Then restore to specific time:
psql -h localhost -U prmis_user -d PRMIS -c "SELECT pg_wal_replay_pause();"
# ... restore from backup ...
psql -h localhost -U prmis_user -d PRMIS -c "SELECT pg_wal_replay_resume();"
```

---

## Application Files Backup

### Backup Backend

```bash
# Create backup of backend code
tar -czf /var/www/prmis/backups/backend_$(date +%Y%m%d_%H%M%S).tar.gz /var/www/prmis/backend/

# Exclude publish folder (can be rebuilt)
tar -czf /var/www/prmis/backups/backend_$(date +%Y%m%d_%H%M%S).tar.gz \
  --exclude='/var/www/prmis/backend/publish' \
  --exclude='/var/www/prmis/backend/bin' \
  --exclude='/var/www/prmis/backend/obj' \
  /var/www/prmis/backend/

# Verify backup
tar -tzf /var/www/prmis/backups/backend_*.tar.gz | head -20
```

### Backup Frontend

```bash
# Create backup of frontend code
tar -czf /var/www/prmis/backups/frontend_$(date +%Y%m%d_%H%M%S).tar.gz /var/www/prmis/frontend/

# Exclude node_modules and dist (can be rebuilt)
tar -czf /var/www/prmis/backups/frontend_$(date +%Y%m%d_%H%M%S).tar.gz \
  --exclude='/var/www/prmis/frontend/node_modules' \
  --exclude='/var/www/prmis/frontend/dist' \
  /var/www/prmis/frontend/

# Verify backup
tar -tzf /var/www/prmis/backups/frontend_*.tar.gz | head -20
```

### Backup Configuration Files

```bash
# Backup configuration
tar -czf /var/www/prmis/backups/config_$(date +%Y%m%d_%H%M%S).tar.gz \
  /var/www/prmis/backend/appsettings.Production.json \
  /etc/nginx/sites-available/prmis \
  /etc/systemd/system/prmis-backend.service

# Verify backup
tar -tzf /var/www/prmis/backups/config_*.tar.gz
```

### Backup Resources (Uploaded Files)

```bash
# Backup uploaded files
tar -czf /var/www/prmis/backups/resources_$(date +%Y%m%d_%H%M%S).tar.gz \
  /var/www/prmis/backend/publish/Resources/

# Verify backup
tar -tzf /var/www/prmis/backups/resources_*.tar.gz | head -20
```

---

## Application Recovery

### Restore Backend

```bash
# Stop backend service
sudo systemctl stop prmis-backend.service

# Restore from backup
tar -xzf /var/www/prmis/backups/backend_YYYYMMDD_HHMMSS.tar.gz -C /

# Rebuild and publish
cd /var/www/prmis/backend
dotnet restore
dotnet publish -c Release -o /var/www/prmis/backend/publish

# Set permissions
sudo chown -R www-data:www-data /var/www/prmis/backend

# Start service
sudo systemctl start prmis-backend.service
```

### Restore Frontend

```bash
# Restore from backup
tar -xzf /var/www/prmis/backups/frontend_YYYYMMDD_HHMMSS.tar.gz -C /

# Rebuild
cd /var/www/prmis/frontend
npm install
npm run build

# Set permissions
sudo chown -R www-data:www-data /var/www/prmis/frontend

# Reload Nginx
sudo systemctl reload nginx
```

### Restore Configuration

```bash
# Restore configuration files
tar -xzf /var/www/prmis/backups/config_YYYYMMDD_HHMMSS.tar.gz -C /

# Verify configuration
sudo nginx -t

# Restart services
sudo systemctl restart prmis-backend.service
sudo systemctl restart nginx
```

---

## Backup to External Storage

### Backup to USB Drive

```bash
# Mount USB drive
sudo mkdir -p /mnt/backup
sudo mount /dev/sdb1 /mnt/backup

# Copy backups
sudo cp -r /var/www/prmis/backups/* /mnt/backup/

# Unmount
sudo umount /mnt/backup
```

### Backup to Network Share (NFS)

```bash
# Install NFS client
sudo apt install -y nfs-common

# Mount network share
sudo mkdir -p /mnt/nfs-backup
sudo mount -t nfs server-ip:/export/backup /mnt/nfs-backup

# Copy backups
sudo cp -r /var/www/prmis/backups/* /mnt/nfs-backup/

# Add to fstab for automatic mounting
echo "server-ip:/export/backup /mnt/nfs-backup nfs defaults 0 0" | sudo tee -a /etc/fstab
```

### Backup to Cloud (AWS S3)

```bash
# Install AWS CLI
sudo apt install -y awscli

# Configure AWS credentials
aws configure

# Upload backups to S3
aws s3 sync /var/www/prmis/backups/ s3://your-bucket-name/prmis-backups/

# Schedule with cron
# Add to crontab:
# 0 3 * * * aws s3 sync /var/www/prmis/backups/ s3://your-bucket-name/prmis-backups/
```

---

## Backup Verification and Testing

### Verify Backup Integrity

```bash
# Check backup file
ls -lh /var/www/prmis/backups/

# Verify SQL backup
head -20 /var/www/prmis/backups/PRMIS_*.sql

# Verify compressed backup
gunzip -t /var/www/prmis/backups/PRMIS_*.sql.gz

# Verify tar archive
tar -tzf /var/www/prmis/backups/*.tar.gz | head -20
```

### Test Restore Process

```bash
# Create test database
sudo -u postgres psql -c "CREATE DATABASE PRMIS_TEST OWNER prmis_user;"

# Restore to test database
psql -h localhost -U prmis_user -d PRMIS_TEST < /var/www/prmis/backups/PRMIS_YYYYMMDD_HHMMSS.sql

# Verify data
psql -h localhost -U prmis_user -d PRMIS_TEST -c "SELECT COUNT(*) FROM \"AspNetUsers\";"

# Drop test database
sudo -u postgres psql -c "DROP DATABASE PRMIS_TEST;"
```

---

## Backup Automation Script

### Create Comprehensive Backup Script

```bash
# Create script
sudo nano /var/www/prmis/full-backup.sh
```

Add this content:
```bash
#!/bin/bash

BACKUP_DIR="/var/www/prmis/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
LOG_FILE="$BACKUP_DIR/backup_$TIMESTAMP.log"

mkdir -p $BACKUP_DIR

echo "Starting full backup at $(date)" > $LOG_FILE

# Database backup
echo "Backing up database..." >> $LOG_FILE
pg_dump -h localhost -U prmis_user -d PRMIS | gzip > $BACKUP_DIR/PRMIS_$TIMESTAMP.sql.gz
echo "Database backup completed" >> $LOG_FILE

# Backend backup
echo "Backing up backend..." >> $LOG_FILE
tar -czf $BACKUP_DIR/backend_$TIMESTAMP.tar.gz \
  --exclude='/var/www/prmis/backend/publish' \
  --exclude='/var/www/prmis/backend/bin' \
  --exclude='/var/www/prmis/backend/obj' \
  /var/www/prmis/backend/ >> $LOG_FILE 2>&1
echo "Backend backup completed" >> $LOG_FILE

# Frontend backup
echo "Backing up frontend..." >> $LOG_FILE
tar -czf $BACKUP_DIR/frontend_$TIMESTAMP.tar.gz \
  --exclude='/var/www/prmis/frontend/node_modules' \
  --exclude='/var/www/prmis/frontend/dist' \
  /var/www/prmis/frontend/ >> $LOG_FILE 2>&1
echo "Frontend backup completed" >> $LOG_FILE

# Configuration backup
echo "Backing up configuration..." >> $LOG_FILE
tar -czf $BACKUP_DIR/config_$TIMESTAMP.tar.gz \
  /var/www/prmis/backend/appsettings.Production.json \
  /etc/nginx/sites-available/prmis \
  /etc/systemd/system/prmis-backend.service >> $LOG_FILE 2>&1
echo "Configuration backup completed" >> $LOG_FILE

# Resources backup
echo "Backing up resources..." >> $LOG_FILE
tar -czf $BACKUP_DIR/resources_$TIMESTAMP.tar.gz \
  /var/www/prmis/backend/publish/Resources/ >> $LOG_FILE 2>&1
echo "Resources backup completed" >> $LOG_FILE

# Cleanup old backups (keep last 30 days)
echo "Cleaning up old backups..." >> $LOG_FILE
find $BACKUP_DIR -name "*.sql.gz" -mtime +30 -delete
find $BACKUP_DIR -name "*.tar.gz" -mtime +30 -delete
echo "Cleanup completed" >> $LOG_FILE

# Summary
echo "Backup completed at $(date)" >> $LOG_FILE
echo "Total backup size: $(du -sh $BACKUP_DIR | awk '{print $1}')" >> $LOG_FILE

# Display summary
cat $LOG_FILE
```

Make it executable:
```bash
sudo chmod +x /var/www/prmis/full-backup.sh
```

### Schedule Full Backup

```bash
# Add to crontab (runs weekly on Sunday at 3 AM)
sudo crontab -e

# Add this line:
0 3 * * 0 /var/www/prmis/full-backup.sh
```

---

## Disaster Recovery Plan

### Recovery Time Objectives (RTO)

- **Database**: < 1 hour
- **Backend**: < 30 minutes
- **Frontend**: < 15 minutes
- **Full System**: < 2 hours

### Recovery Point Objectives (RPO)

- **Database**: 24 hours (daily backups)
- **Application**: 7 days (weekly backups)
- **Configuration**: 7 days (weekly backups)

### Step-by-Step Recovery

1. **Assess Damage**
   - Determine what needs to be recovered
   - Check backup availability

2. **Recover Database**
   ```bash
   sudo systemctl stop prmis-backend.service
   # Restore database from backup
   psql -h localhost -U prmis_user -d PRMIS < backup.sql
   ```

3. **Recover Backend**
   ```bash
   tar -xzf backend_backup.tar.gz -C /
   cd /var/www/prmis/backend
   dotnet publish -c Release -o /var/www/prmis/backend/publish
   sudo systemctl start prmis-backend.service
   ```

4. **Recover Frontend**
   ```bash
   tar -xzf frontend_backup.tar.gz -C /
   cd /var/www/prmis/frontend
   npm install
   npm run build
   sudo systemctl reload nginx
   ```

5. **Verify System**
   - Test API endpoints
   - Test frontend loading
   - Test user login
   - Check data integrity

---

## Backup Checklist

Daily:
- [ ] Database backup runs successfully
- [ ] Backup file size is reasonable
- [ ] No errors in backup logs

Weekly:
- [ ] Full backup completed
- [ ] All backup types present
- [ ] Backup files are readable

Monthly:
- [ ] Test restore process
- [ ] Verify data integrity after restore
- [ ] Update recovery documentation
- [ ] Review backup retention policy

---

**Last Updated**: 2024
**Version**: 1.0

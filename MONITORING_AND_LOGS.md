# Monitoring and Logging Guide

## System Monitoring

### Real-Time Monitoring

#### CPU and Memory Usage
```bash
# Interactive monitoring
htop

# One-time snapshot
top -b -n 1

# Per-process monitoring
ps aux | grep dotnet
```

#### Disk Usage
```bash
# Overall disk usage
df -h

# Detailed directory sizes
du -sh /var/www/prmis/*
du -sh /var/log/*

# Database size
sudo -u postgres psql -d PRMIS -c "SELECT pg_size_pretty(pg_database_size('PRMIS'));"

# Table sizes
sudo -u postgres psql -d PRMIS -c "
SELECT schemaname, tablename, pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) 
FROM pg_tables 
WHERE schemaname NOT IN ('pg_catalog', 'information_schema') 
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;"
```

#### Network Connections
```bash
# Check listening ports
sudo netstat -tuln | grep LISTEN

# Check connections to backend
sudo netstat -tuln | grep 5000

# Monitor network traffic
sudo iftop
```

#### Service Status
```bash
# Backend service
sudo systemctl status prmis-backend.service

# Nginx
sudo systemctl status nginx

# PostgreSQL
sudo systemctl status postgresql

# All services
sudo systemctl list-units --type=service --state=running
```

---

## Log Management

### Backend Logs (Systemd Journal)

#### View Logs
```bash
# Last 50 lines
sudo journalctl -u prmis-backend.service -n 50

# Follow logs in real-time
sudo journalctl -u prmis-backend.service -f

# Last 1 hour
sudo journalctl -u prmis-backend.service --since "1 hour ago"

# Last 24 hours
sudo journalctl -u prmis-backend.service --since "24 hours ago"

# Specific date range
sudo journalctl -u prmis-backend.service --since "2024-01-01" --until "2024-01-02"

# Show all fields
sudo journalctl -u prmis-backend.service -o verbose

# JSON format
sudo journalctl -u prmis-backend.service -o json
```

#### Filter Logs
```bash
# Show only errors
sudo journalctl -u prmis-backend.service -p err

# Show errors and warnings
sudo journalctl -u prmis-backend.service -p warning

# Show info and above
sudo journalctl -u prmis-backend.service -p info

# Search for specific text
sudo journalctl -u prmis-backend.service | grep "error"
sudo journalctl -u prmis-backend.service | grep "exception"
```

#### Export Logs
```bash
# Export to file
sudo journalctl -u prmis-backend.service > backend_logs.txt

# Export JSON
sudo journalctl -u prmis-backend.service -o json > backend_logs.json

# Export last 1000 lines
sudo journalctl -u prmis-backend.service -n 1000 > backend_logs.txt
```

#### Clear Logs
```bash
# Clear logs older than 30 days
sudo journalctl --vacuum-time=30d

# Clear logs keeping only 100MB
sudo journalctl --vacuum-size=100M

# Clear all logs
sudo journalctl --vacuum-time=1s
```

### Nginx Logs

#### Access Logs
```bash
# View access logs
sudo tail -f /var/log/nginx/prmis_access.log

# Last 100 lines
sudo tail -n 100 /var/log/nginx/prmis_access.log

# Search for specific IP
sudo grep "192.168.1.100" /var/log/nginx/prmis_access.log

# Count requests by IP
sudo awk '{print $1}' /var/log/nginx/prmis_access.log | sort | uniq -c | sort -rn

# Count requests by endpoint
sudo awk '{print $7}' /var/log/nginx/prmis_access.log | sort | uniq -c | sort -rn

# Count HTTP status codes
sudo awk '{print $9}' /var/log/nginx/prmis_access.log | sort | uniq -c
```

#### Error Logs
```bash
# View error logs
sudo tail -f /var/log/nginx/prmis_error.log

# Last 50 lines
sudo tail -n 50 /var/log/nginx/prmis_error.log

# Search for errors
sudo grep "error" /var/log/nginx/prmis_error.log
```

#### Log Rotation
```bash
# Check log rotation configuration
cat /etc/logrotate.d/nginx

# Manually rotate logs
sudo logrotate -f /etc/logrotate.d/nginx

# Check rotation schedule
sudo cat /etc/cron.daily/logrotate
```

### PostgreSQL Logs

#### View Logs
```bash
# View PostgreSQL logs
sudo tail -f /var/log/postgresql/postgresql-15-main.log

# Last 100 lines
sudo tail -n 100 /var/log/postgresql/postgresql-15-main.log

# Search for errors
sudo grep "ERROR" /var/log/postgresql/postgresql-15-main.log

# Search for warnings
sudo grep "WARNING" /var/log/postgresql/postgresql-15-main.log
```

#### Enable Query Logging
```bash
# SSH to server
sudo -u postgres psql

# Enable query logging
ALTER SYSTEM SET log_statement = 'all';
ALTER SYSTEM SET log_duration = on;

# Reload configuration
SELECT pg_reload_conf();

# Exit
\q

# Restart PostgreSQL
sudo systemctl restart postgresql
```

---

## Performance Monitoring

### Backend Performance

#### Response Times
```bash
# Check response times in logs
sudo journalctl -u prmis-backend.service | grep "duration"

# Monitor in real-time
sudo journalctl -u prmis-backend.service -f | grep "duration"
```

#### Request Count
```bash
# Count requests per minute
sudo tail -f /var/log/nginx/prmis_access.log | awk '{print $4}' | cut -d: -f1-3 | uniq -c
```

#### Error Rate
```bash
# Calculate error rate
sudo awk '$9 >= 400 {errors++} END {print "Error rate: " errors/NR*100 "%"}' /var/log/nginx/prmis_access.log
```

### Database Performance

#### Query Performance
```bash
# Connect to database
sudo -u postgres psql -d PRMIS

# Enable query timing
\timing

# Run query and see execution time
SELECT * FROM "AspNetUsers" LIMIT 10;

# View query plans
EXPLAIN ANALYZE SELECT * FROM "AspNetUsers" LIMIT 10;

# Exit
\q
```

#### Connection Pool Status
```bash
# Check active connections
sudo -u postgres psql -d PRMIS -c "SELECT datname, count(*) FROM pg_stat_activity GROUP BY datname;"

# Check connection details
sudo -u postgres psql -d PRMIS -c "SELECT pid, usename, application_name, state FROM pg_stat_activity;"

# Kill idle connections (if needed)
sudo -u postgres psql -d PRMIS -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE state = 'idle' AND query_start < now() - interval '30 minutes';"
```

#### Index Usage
```bash
# Check unused indexes
sudo -u postgres psql -d PRMIS -c "
SELECT schemaname, tablename, indexname 
FROM pg_indexes 
WHERE schemaname NOT IN ('pg_catalog', 'information_schema');"

# Check index sizes
sudo -u postgres psql -d PRMIS -c "
SELECT indexname, pg_size_pretty(pg_relation_size(indexrelid)) 
FROM pg_indexes 
JOIN pg_class ON pg_class.relname = indexname 
WHERE schemaname NOT IN ('pg_catalog', 'information_schema');"
```

### System Performance

#### Load Average
```bash
# Check load average
uptime

# Check over time
watch -n 1 uptime
```

#### Memory Usage
```bash
# Detailed memory info
free -h

# Memory usage by process
ps aux --sort=-%mem | head -20

# Check swap usage
swapon --show
```

#### Disk I/O
```bash
# Monitor disk I/O
iostat -x 1

# Check disk usage
df -h

# Find large files
find /var/www/prmis -type f -size +100M
```

---

## Alerting and Notifications

### Setup Email Alerts

#### Install Mail Utility
```bash
sudo apt install -y mailutils
```

#### Create Alert Script
```bash
# Create script
sudo nano /var/www/prmis/check-health.sh
```

Add this content:
```bash
#!/bin/bash

EMAIL="admin@example.com"
HOSTNAME=$(hostname)

# Check backend service
if ! sudo systemctl is-active --quiet prmis-backend.service; then
    echo "Backend service is down on $HOSTNAME" | mail -s "Alert: Backend Down" $EMAIL
fi

# Check Nginx
if ! sudo systemctl is-active --quiet nginx; then
    echo "Nginx is down on $HOSTNAME" | mail -s "Alert: Nginx Down" $EMAIL
fi

# Check PostgreSQL
if ! sudo systemctl is-active --quiet postgresql; then
    echo "PostgreSQL is down on $HOSTNAME" | mail -s "Alert: PostgreSQL Down" $EMAIL
fi

# Check disk usage
DISK_USAGE=$(df /var/www/prmis | awk 'NR==2 {print $5}' | cut -d'%' -f1)
if [ $DISK_USAGE -gt 80 ]; then
    echo "Disk usage is at ${DISK_USAGE}% on $HOSTNAME" | mail -s "Alert: High Disk Usage" $EMAIL
fi

# Check memory usage
MEM_USAGE=$(free | awk 'NR==2 {print int($3/$2 * 100)}')
if [ $MEM_USAGE -gt 80 ]; then
    echo "Memory usage is at ${MEM_USAGE}% on $HOSTNAME" | mail -s "Alert: High Memory Usage" $EMAIL
fi
```

Make it executable:
```bash
sudo chmod +x /var/www/prmis/check-health.sh
```

#### Schedule Health Checks
```bash
# Add to crontab (runs every 5 minutes)
sudo crontab -e

# Add this line:
*/5 * * * * /var/www/prmis/check-health.sh
```

---

## Log Analysis

### Common Issues and Log Patterns

#### Backend Won't Start
```bash
# Check logs
sudo journalctl -u prmis-backend.service -n 100

# Look for:
# - "Connection refused" - Database not running
# - "Address already in use" - Port 5000 in use
# - "File not found" - Missing dependencies
```

#### High CPU Usage
```bash
# Find process using CPU
top -b -n 1 | head -20

# Check backend logs for infinite loops
sudo journalctl -u prmis-backend.service -f

# Check database queries
sudo -u postgres psql -d PRMIS -c "SELECT * FROM pg_stat_statements ORDER BY mean_time DESC LIMIT 10;"
```

#### High Memory Usage
```bash
# Check memory by process
ps aux --sort=-%mem | head -10

# Check for memory leaks in backend
sudo journalctl -u prmis-backend.service | grep -i "memory"

# Check database cache
sudo -u postgres psql -d PRMIS -c "SELECT name, setting FROM pg_settings WHERE name LIKE '%cache%';"
```

#### Slow Queries
```bash
# Enable slow query logging in PostgreSQL
sudo -u postgres psql -d PRMIS -c "ALTER SYSTEM SET log_min_duration_statement = 1000;"
sudo -u postgres psql -d PRMIS -c "SELECT pg_reload_conf();"

# View slow queries
sudo tail -f /var/log/postgresql/postgresql-15-main.log | grep "duration:"
```

#### Connection Pool Exhaustion
```bash
# Check active connections
sudo -u postgres psql -d PRMIS -c "SELECT count(*) FROM pg_stat_activity WHERE datname = 'PRMIS';"

# Increase pool size in appsettings.Production.json
# Change: MaxPoolSize=20 to MaxPoolSize=30

# Restart backend
sudo systemctl restart prmis-backend.service
```

---

## Monitoring Dashboard Commands

### Create Monitoring Script
```bash
# Create script
sudo nano /var/www/prmis/monitor-dashboard.sh
```

Add this content:
```bash
#!/bin/bash

while true; do
    clear
    echo "=========================================="
    echo "Property Management System - Monitoring"
    echo "=========================================="
    echo ""
    
    echo "=== Services Status ==="
    echo -n "Backend: "
    sudo systemctl is-active prmis-backend.service
    echo -n "Nginx: "
    sudo systemctl is-active nginx
    echo -n "PostgreSQL: "
    sudo systemctl is-active postgresql
    echo ""
    
    echo "=== System Resources ==="
    echo "CPU & Memory:"
    free -h | head -2
    echo ""
    echo "Disk Usage:"
    df -h /var/www/prmis | tail -1
    echo ""
    
    echo "=== Network Connections ==="
    echo "Backend connections:"
    sudo netstat -tuln | grep 5000
    echo ""
    
    echo "=== Recent Errors ==="
    sudo journalctl -u prmis-backend.service -n 5 -p err
    echo ""
    
    echo "Press Ctrl+C to exit. Refreshing in 10 seconds..."
    sleep 10
done
```

Make it executable:
```bash
sudo chmod +x /var/www/prmis/monitor-dashboard.sh
```

Run it:
```bash
sudo /var/www/prmis/monitor-dashboard.sh
```

---

## Log Retention Policy

### Configure Log Rotation

#### Systemd Journal
```bash
# Edit configuration
sudo nano /etc/systemd/journald.conf

# Set these values:
SystemMaxUse=1G
SystemMaxFileSize=100M
MaxRetentionSec=30day

# Restart service
sudo systemctl restart systemd-journald
```

#### Nginx Logs
```bash
# Create rotation config
sudo nano /etc/logrotate.d/prmis

# Add this content:
/var/log/nginx/prmis_*.log {
    daily
    rotate 30
    compress
    delaycompress
    notifempty
    create 0640 www-data adm
    sharedscripts
    postrotate
        if [ -f /var/run/nginx.pid ]; then
            kill -USR1 `cat /var/run/nginx.pid`
        fi
    endscript
}
```

#### PostgreSQL Logs
```bash
# Edit PostgreSQL configuration
sudo nano /etc/postgresql/15/main/postgresql.conf

# Set these values:
log_rotation_age = 1d
log_rotation_size = 100MB
```

---

## Monitoring Checklist

Daily:
- [ ] Check backend service status
- [ ] Review error logs
- [ ] Monitor disk usage
- [ ] Check database backups

Weekly:
- [ ] Analyze performance metrics
- [ ] Review slow queries
- [ ] Check connection pool usage
- [ ] Verify backup integrity

Monthly:
- [ ] Full system audit
- [ ] Performance optimization review
- [ ] Security log review
- [ ] Capacity planning

---

**Last Updated**: 2024
**Version**: 1.0

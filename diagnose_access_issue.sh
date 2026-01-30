#!/bin/bash

echo "========================================="
echo "PRMIS Access Diagnostic Tool"
echo "========================================="
echo ""

echo "1. Checking Nginx Status..."
systemctl status nginx | grep "Active:"
echo ""

echo "2. Checking if Nginx is listening on port 80..."
netstat -tlnp | grep ':80' | head -3
echo ""

echo "3. Checking UFW Firewall Status..."
sudo ufw status | grep -E "Status:|80"
echo ""

echo "4. Testing Local Access (localhost)..."
curl -I http://localhost 2>&1 | head -5
echo ""

echo "5. Testing Public IP Access (from server)..."
curl -I http://103.132.98.92 2>&1 | head -5
echo ""

echo "6. Checking Frontend Files..."
ls -lh /var/www/prmis/frontend/main*.js 2>&1 | tail -1
echo ""

echo "7. Checking Resources Folder..."
echo "Resources folder structure:"
ls -la /var/www/prmis/backend/Resources/Documents/Company/ 2>&1 | head -10
echo ""

echo "8. Checking for uploaded photos..."
PHOTO_COUNT=$(find /var/www/prmis/backend/Resources -name "*.jpg" 2>/dev/null | wc -l)
echo "Total photos found: $PHOTO_COUNT"
if [ $PHOTO_COUNT -gt 0 ]; then
    echo "Sample photos:"
    find /var/www/prmis/backend/Resources -name "*.jpg" 2>/dev/null | head -5
fi
echo ""

echo "9. Checking Backend Service..."
systemctl status prmis-backend | grep "Active:"
echo ""

echo "10. Checking Nginx Configuration..."
echo "Nginx sites-enabled/default (first 30 lines):"
head -30 /etc/nginx/sites-enabled/default
echo ""

echo "========================================="
echo "DIAGNOSIS SUMMARY"
echo "========================================="
echo ""

# Check if nginx is running
if systemctl is-active --quiet nginx; then
    echo "✓ Nginx is running"
else
    echo "✗ Nginx is NOT running"
fi

# Check if port 80 is listening
if netstat -tlnp | grep -q ':80'; then
    echo "✓ Port 80 is listening"
else
    echo "✗ Port 80 is NOT listening"
fi

# Check if UFW allows port 80
if sudo ufw status | grep -q "80/tcp.*ALLOW"; then
    echo "✓ UFW firewall allows port 80"
else
    echo "✗ UFW firewall does NOT allow port 80"
fi

# Check if local access works
if curl -s -o /dev/null -w "%{http_code}" http://localhost | grep -q "200"; then
    echo "✓ Local access works (http://localhost)"
else
    echo "✗ Local access FAILED"
fi

# Check if frontend is deployed
if [ -f "/var/www/prmis/frontend/index.html" ]; then
    echo "✓ Frontend is deployed"
else
    echo "✗ Frontend is NOT deployed"
fi

# Check if photos exist
if [ $PHOTO_COUNT -gt 0 ]; then
    echo "✓ Photos exist ($PHOTO_COUNT files)"
else
    echo "✗ NO photos found (Resources folder is empty)"
fi

# Check if backend is running
if systemctl is-active --quiet prmis-backend; then
    echo "✓ Backend service is running"
else
    echo "✗ Backend service is NOT running"
fi

echo ""
echo "========================================="
echo "RECOMMENDATIONS"
echo "========================================="
echo ""

# Check if external access might be blocked
echo "Testing external access from server..."
if curl -s -o /dev/null -w "%{http_code}" http://103.132.98.92 | grep -q "200"; then
    echo "✓ Server can access itself via public IP"
    echo ""
    echo "LIKELY ISSUE: Cloud provider firewall (Security Group)"
    echo ""
    echo "The server is configured correctly, but external access is blocked."
    echo "This is typically caused by cloud provider firewall rules."
    echo ""
    echo "ACTION REQUIRED:"
    echo "1. Check your cloud provider's security group/firewall settings"
    echo "2. Allow inbound traffic on port 80 (HTTP) and 443 (HTTPS)"
    echo "3. Source should be 0.0.0.0/0 (allow from anywhere)"
    echo ""
    echo "For AWS: EC2 → Security Groups → Add Inbound Rule"
    echo "For Azure: VM → Networking → Add inbound port rule"
    echo "For GCP: VPC Network → Firewall → Create firewall rule"
else
    echo "✗ Server CANNOT access itself via public IP"
    echo ""
    echo "This could indicate:"
    echo "1. Network configuration issue"
    echo "2. Nginx not binding to public interface"
    echo "3. Routing problem"
fi

if [ $PHOTO_COUNT -eq 0 ]; then
    echo ""
    echo "PHOTO ISSUE: Resources folder is empty"
    echo ""
    echo "ACTION REQUIRED:"
    echo "1. Upload photos through the application, OR"
    echo "2. Copy existing photos to /var/www/prmis/backend/Resources/Documents/Company/"
    echo "3. Set permissions: sudo chown -R www-data:www-data /var/www/prmis/backend/Resources"
fi

echo ""
echo "========================================="
echo "For detailed solutions, see:"
echo "FRONTEND_ACCESS_AND_PHOTO_FIX.md"
echo "========================================="

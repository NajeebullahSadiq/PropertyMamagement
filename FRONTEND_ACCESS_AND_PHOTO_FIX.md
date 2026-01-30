# Frontend Access and Photo Fix - Complete Solution

## Current Status

### ✅ What's Working
1. **Frontend deployed** - Latest build at `/var/www/prmis/frontend/` (main.36459fc53d94a846.js from Jan 26 09:25)
2. **Nginx configured** - Correctly serving frontend and proxying API
3. **UFW firewall open** - Port 80/443 allowed
4. **Nginx listening** - On 0.0.0.0:80 (all interfaces)
5. **Local access works** - `curl http://localhost` returns HTTP 200 OK
6. **Code fix applied** - HTML template fixed to pass `imageName` instead of `imagePath`

### ❌ What's NOT Working
1. **External access blocked** - Browser from external network gets "ERR_CONNECTION_REFUSED"
2. **Photos missing** - `/var/www/prmis/backend/Resources/Documents/Company/` is EMPTY

## Root Causes

### Issue 1: External Access Blocked
The server responds locally but not externally. This indicates a **cloud provider firewall** (security group) is blocking port 80.

**Possible causes:**
- AWS Security Group not allowing inbound port 80
- Azure Network Security Group blocking HTTP
- Google Cloud Firewall rules blocking port 80
- ISP/Network firewall blocking traffic

### Issue 2: Photos Don't Exist
The database has paths like `Resources/Documents/Company/profile_20260126_074257_233.jpg` but these files don't exist on the server. The photos were uploaded during testing but:
- Either uploaded to a different server/environment
- Or uploaded before the current deployment
- Or the Resources folder was cleared during deployment

## Solutions

### Solution 1: Fix External Access (Cloud Firewall)

You need to configure your cloud provider's security group/firewall to allow inbound HTTP/HTTPS traffic.

#### For AWS EC2:
```bash
# Go to AWS Console → EC2 → Security Groups
# Find your instance's security group
# Add Inbound Rules:
# - Type: HTTP, Protocol: TCP, Port: 80, Source: 0.0.0.0/0
# - Type: HTTPS, Protocol: TCP, Port: 443, Source: 0.0.0.0/0
```

#### For Azure VM:
```bash
# Go to Azure Portal → Virtual Machines → Your VM → Networking
# Add inbound port rule:
# - Service: HTTP, Port: 80, Source: Any
# - Service: HTTPS, Port: 443, Source: Any
```

#### For Google Cloud:
```bash
# Go to GCP Console → VPC Network → Firewall
# Create firewall rule:
# - Targets: All instances
# - Source IP ranges: 0.0.0.0/0
# - Protocols and ports: tcp:80,tcp:443
```

#### Check if it's a cloud firewall:
```bash
# From your local machine, try to telnet to port 80:
telnet 103.132.98.92 80

# If it says "Connection refused" or times out, it's a firewall issue
# If it connects, the issue is elsewhere
```

### Solution 2: Upload Test Photos

Since the Resources folder is empty, you need to upload photos. Here are your options:

#### Option A: Re-upload photos through the application
1. Once external access is fixed, login to the application
2. Go to Company Owner form
3. Upload photos again for each company owner
4. The backend will save them to `/var/www/prmis/backend/Resources/Documents/Company/`

#### Option B: Copy sample photos for testing
```bash
# Create sample test photos on the server
ssh moj@103.132.98.92

# Create a test photo
sudo apt-get install imagemagick -y
convert -size 200x200 xc:blue /tmp/test_photo.jpg

# Copy to Resources folder with the exact filename from database
sudo cp /tmp/test_photo.jpg /var/www/prmis/backend/Resources/Documents/Company/profile_20260126_074257_233.jpg
sudo cp /tmp/test_photo.jpg /var/www/prmis/backend/Resources/Documents/Company/profile_20260126_074353_133.jpg

# Set permissions
sudo chown -R www-data:www-data /var/www/prmis/backend/Resources
sudo chmod -R 755 /var/www/prmis/backend/Resources

# Verify
ls -la /var/www/prmis/backend/Resources/Documents/Company/
```

#### Option C: Check if photos exist elsewhere
```bash
# Search for photos in other locations
find /home -name "profile_*.jpg" 2>/dev/null
find /var -name "profile_*.jpg" 2>/dev/null
find /tmp -name "profile_*.jpg" 2>/dev/null

# If found, copy them to the correct location
```

## Testing Steps

### Step 1: Test External Access
```bash
# From your local machine (not the server):
curl -I http://103.132.98.92

# Expected: HTTP/1.1 200 OK
# If you get connection refused/timeout: Fix cloud firewall first
```

### Step 2: Test Photo Access
```bash
# From your local machine:
curl -I http://103.132.98.92/api/Resources/Documents/Company/profile_20260126_074257_233.jpg

# Expected: HTTP/1.1 200 OK (if photo exists)
# Expected: HTTP/1.1 404 Not Found (if photo doesn't exist - normal until you upload)
```

### Step 3: Test in Browser
1. Open browser (use Incognito/Private mode to avoid cache)
2. Navigate to: `http://103.132.98.92`
3. Login to the application
4. Go to Company Owner form
5. Check if photo loads (will be 404 until you upload photos)

### Step 4: Clear Browser Cache (After External Access Works)
Once external access is working, users must clear browser cache:

**Method 1 - Hard Refresh:**
- Press `Ctrl+Shift+R` (Windows/Linux)
- Press `Cmd+Shift+R` (Mac)

**Method 2 - Clear Cache:**
1. Press `Ctrl+Shift+Delete`
2. Select "Cached images and files"
3. Select "All time"
4. Click "Clear data"

**Method 3 - Test in Incognito:**
- Open Incognito/Private window
- Navigate to site
- If it works, the fix is good - just need to clear normal browser cache

## Verification Checklist

- [ ] External access works (can access http://103.132.98.92 from browser)
- [ ] Frontend loads (see login page)
- [ ] Can login successfully
- [ ] Can navigate to Company Owner form
- [ ] Photos load correctly (after uploading)
- [ ] Photo URLs are correct format: `http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg`
- [ ] No double `/api/` in URLs
- [ ] Browser cache cleared

## Quick Diagnostic Commands

Run these on the server to diagnose issues:

```bash
# Check if nginx is running
systemctl status nginx

# Check if nginx is listening on port 80
netstat -tlnp | grep ':80'

# Check UFW firewall
sudo ufw status

# Check if site responds locally
curl -I http://localhost

# Check if site responds via public IP (from server)
curl -I http://103.132.98.92

# Check Resources folder
ls -la /var/www/prmis/backend/Resources/Documents/Company/

# Check nginx logs for errors
sudo tail -f /var/log/nginx/error.log

# Check backend logs
sudo journalctl -u prmis-backend -n 50 --no-pager
```

## Expected Behavior After Fix

### Before Fix (WRONG):
```
URL: http://103.132.98.92/api/Upload/view//api/Resources/Documents/Company/profile_xxx.jpg
                                        ^^
                                        Double /api/ - WRONG!
Result: 404 Not Found
```

### After Fix (CORRECT):
```
URL: http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg
                             ^
                             Single path - CORRECT!
Result: 200 OK (if photo exists) or 404 (if photo not uploaded yet)
```

## Next Steps

1. **FIRST**: Fix cloud provider firewall to allow port 80/443
2. **SECOND**: Test external access from browser
3. **THIRD**: Clear browser cache (Ctrl+Shift+R)
4. **FOURTH**: Upload photos through the application OR copy test photos
5. **FIFTH**: Verify photos load correctly

## Contact Your Cloud Provider

If you're unsure how to configure the firewall, contact your hosting provider:
- **AWS**: Check EC2 Security Groups
- **Azure**: Check Network Security Groups
- **Google Cloud**: Check VPC Firewall Rules
- **DigitalOcean**: Check Droplet Firewall
- **Linode**: Check Cloud Firewall
- **Other**: Contact your hosting provider's support

The server configuration is correct. The issue is network-level access control.

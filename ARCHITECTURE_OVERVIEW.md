# System Architecture Overview

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT BROWSER                            │
│                    (User's Web Browser)                          │
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTP/HTTPS
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      NGINX REVERSE PROXY                         │
│                    (Port 80/443)                                 │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ • Static file serving (Angular frontend)                │   │
│  │ • SSL/TLS termination                                   │   │
│  │ • Request routing                                       │   │
│  │ • Gzip compression                                      │   │
│  │ • Caching headers                                       │   │
│  └──────────────────────────────────────────────────────────┘   │
└────────┬──────────────────────────────────────────────────┬─────┘
         │ /api/*                                           │ /*
         ▼                                                  ▼
    ┌─────────────────┐                          ┌──────────────────┐
    │  .NET BACKEND   │                          │  ANGULAR FRONTEND│
    │  (Port 5000)    │                          │  (Static Files)  │
    │                 │                          │                  │
    │ • REST API      │                          │ • SPA            │
    │ • Business Logic│                          │ • UI Components  │
    │ • Authentication│                          │ • Routing        │
    │ • JWT Tokens    │                          │ • State Mgmt     │
    └────────┬────────┘                          └──────────────────┘
             │
             │ TCP Connection (localhost:5432)
             ▼
    ┌─────────────────────────┐
    │   POSTGRESQL DATABASE   │
    │   (Port 5432)           │
    │                         │
    │ • PRMIS Database        │
    │ • User Data             │
    │ • Property Data         │
    │ • Transaction Records   │
    │ • Connection Pooling    │
    └─────────────────────────┘
```

---

## Deployment Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│                    UBUNTU 24.04 SERVER                           │
│                    (Dell R730)                                   │
│                    32GB RAM, 8 cores                             │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    SYSTEMD (Init System)                   │ │
│  │  ┌──────────────────────────────────────────────────────┐ │ │
│  │  │  prmis-backend.service                              │ │ │
│  │  │  ┌────────────────────────────────────────────────┐ │ │ │
│  │  │  │  .NET 9 Runtime                               │ │ │ │
│  │  │  │  ┌──────────────────────────────────────────┐ │ │ │ │
│  │  │  │  │  WebAPIBackend.dll                       │ │ │ │ │
│  │  │  │  │  • Controllers                           │ │ │ │ │
│  │  │  │  │  • Services                              │ │ │ │ │
│  │  │  │  │  • Database Context (EF Core)           │ │ │ │ │
│  │  │  │  │  • Authentication (JWT)                 │ │ │ │ │
│  │  │  │  └──────────────────────────────────────────┘ │ │ │ │
│  │  │  │  Listens on: localhost:5000                   │ │ │ │
│  │  │  │  Auto-restart on failure                      │ │ │ │
│  │  │  └────────────────────────────────────────────────┘ │ │ │
│  │  │                                                      │ │ │
│  │  │  nginx.service                                      │ │ │
│  │  │  ┌────────────────────────────────────────────────┐ │ │ │
│  │  │  │  Nginx Web Server                             │ │ │ │
│  │  │  │  • Reverse Proxy to Backend                  │ │ │ │
│  │  │  │  • Static File Serving (Frontend)            │ │ │ │
│  │  │  │  • SSL/TLS Termination                       │ │ │ │
│  │  │  │  • Gzip Compression                          │ │ │ │
│  │  │  │  • Load Balancing (if needed)                │ │ │ │
│  │  │  └────────────────────────────────────────────────┘ │ │ │
│  │  │  Listens on: 0.0.0.0:80 (and 443 for HTTPS)        │ │ │
│  │  │                                                      │ │ │
│  │  │  postgresql.service                                 │ │ │
│  │  │  ┌────────────────────────────────────────────────┐ │ │ │
│  │  │  │  PostgreSQL Database Server                   │ │ │ │
│  │  │  │  • PRMIS Database                             │ │ │ │
│  │  │  │  • Connection Pooling                         │ │ │ │
│  │  │  │  • Query Optimization                         │ │ │ │
│  │  │  │  • Backup Management                          │ │ │ │
│  │  │  └────────────────────────────────────────────────┘ │ │ │
│  │  │  Listens on: localhost:5432                         │ │ │
│  │  │                                                      │ │ │
│  │  └──────────────────────────────────────────────────────┘ │ │
│  │                                                            │ │
│  │  Cron Jobs                                                │ │
│  │  ┌──────────────────────────────────────────────────────┐ │ │
│  │  │  • Database Backup (Daily 2 AM)                     │ │ │
│  │  │  • Log Rotation (Daily)                             │ │ │
│  │  │  • Health Checks (Every 5 minutes)                  │ │ │
│  │  └──────────────────────────────────────────────────────┘ │ │
│  │                                                            │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    FILE SYSTEM                             │ │
│  │  /var/www/prmis/                                           │ │
│  │  ├── backend/                                             │ │
│  │  │   ├── publish/                (Published .NET app)    │ │
│  │  │   ├── Resources/              (Uploaded files)        │ │
│  │  │   └── appsettings.*.json      (Configuration)         │ │
│  │  ├── frontend/                                           │ │
│  │  │   └── dist/                   (Built Angular app)     │ │
│  │  └── backups/                    (Database backups)      │ │
│  │                                                            │ │
│  │  /etc/nginx/                                              │ │
│  │  └── sites-available/prmis       (Nginx config)          │ │
│  │                                                            │ │
│  │  /etc/systemd/system/                                     │ │
│  │  └── prmis-backend.service       (Service config)        │ │
│  │                                                            │ │
│  │  /var/log/                                                │ │
│  │  ├── nginx/                      (Nginx logs)            │ │
│  │  └── postgresql/                 (PostgreSQL logs)       │ │
│  │                                                            │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

---

## Request Flow

### Frontend Request (Static Content)

```
1. Browser Request
   GET http://server/index.html
   
2. Nginx Receives Request
   ├─ Check if file exists
   ├─ Apply caching headers
   └─ Return static file
   
3. Browser Receives Response
   ├─ Parse HTML
   ├─ Load CSS/JS
   └─ Render UI
```

### API Request (Dynamic Content)

```
1. Angular Frontend
   POST http://server/api/users/login
   {
     "email": "admin@prmis.gov.af",
     "password": "Admin@123"
   }
   
2. Nginx Reverse Proxy
   ├─ Receives request on port 80/443
   ├─ Adds headers (X-Real-IP, X-Forwarded-For, etc.)
   └─ Forwards to backend on localhost:5000
   
3. .NET Backend
   ├─ Receives request
   ├─ Routes to LoginController
   ├─ Validates credentials
   ├─ Queries database
   ├─ Generates JWT token
   └─ Returns response
   
4. Nginx Receives Response
   ├─ Applies compression (gzip)
   ├─ Adds caching headers
   └─ Forwards to browser
   
5. Angular Frontend
   ├─ Receives response
   ├─ Stores JWT token
   ├─ Updates UI
   └─ Redirects to dashboard
```

---

## Data Flow

### User Registration

```
Frontend (Angular)
    │
    ├─ User enters data
    ├─ Validates input
    └─ Sends POST /api/auth/register
        │
        Nginx (Reverse Proxy)
        │
        Backend (.NET)
        │
        ├─ Receives request
        ├─ Validates data
        ├─ Hashes password
        ├─ Creates user object
        └─ Calls DbContext.SaveChangesAsync()
            │
            Entity Framework Core
            │
            ├─ Generates SQL INSERT
            └─ Sends to PostgreSQL
                │
                PostgreSQL
                │
                ├─ Validates constraints
                ├─ Inserts record
                ├─ Returns success
                └─ Commits transaction
                    │
                    Returns to Backend
                    │
                    Returns to Frontend
                    │
                    Shows success message
```

### Property Search

```
Frontend (Angular)
    │
    ├─ User enters search criteria
    └─ Sends GET /api/properties?type=house&city=kabul
        │
        Nginx (Reverse Proxy)
        │
        Backend (.NET)
        │
        ├─ Receives request
        ├─ Parses query parameters
        ├─ Builds LINQ query
        └─ Calls DbContext.Properties.Where(...)
            │
            Entity Framework Core
            │
            ├─ Generates SQL SELECT
            └─ Sends to PostgreSQL
                │
                PostgreSQL
                │
                ├─ Executes query
                ├─ Applies WHERE clause
                ├─ Applies ORDER BY
                ├─ Applies LIMIT
                └─ Returns results
                    │
                    Returns to Backend
                    │
                    Maps to DTOs
                    │
                    Returns JSON
                    │
                    Nginx applies gzip
                    │
                    Frontend receives
                    │
                    Displays results
```

---

## Database Schema (Simplified)

```
PostgreSQL Database: PRMIS

Tables:
├── AspNetUsers
│   ├── Id (PK)
│   ├── UserName
│   ├── Email
│   ├── PasswordHash
│   └── ...
│
├── AspNetRoles
│   ├── Id (PK)
│   ├── Name
│   └── ...
│
├── AspNetUserRoles
│   ├── UserId (FK)
│   ├── RoleId (FK)
│   └── ...
│
├── Properties
│   ├── Id (PK)
│   ├── Title
│   ├── Description
│   ├── Type
│   ├── Price
│   ├── Location
│   ├── OwnerId (FK)
│   └── ...
│
├── Transactions
│   ├── Id (PK)
│   ├── PropertyId (FK)
│   ├── BuyerId (FK)
│   ├── SellerId (FK)
│   ├── Amount
│   ├── Date
│   └── ...
│
└── ... (other tables)
```

---

## Security Architecture

```
┌─────────────────────────────────────────────────────────┐
│                   SECURITY LAYERS                       │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Layer 1: Network Security                             │
│  ├─ Firewall (UFW)                                     │
│  │  ├─ Allow: SSH (22)                                 │
│  │  ├─ Allow: HTTP (80)                                │
│  │  ├─ Allow: HTTPS (443)                              │
│  │  └─ Deny: All others                                │
│  │                                                      │
│  └─ SSL/TLS (HTTPS)                                    │
│     ├─ Encrypt data in transit                         │
│     ├─ Certificate validation                          │
│     └─ Auto-renewal with Certbot                       │
│                                                         │
│  Layer 2: Application Security                         │
│  ├─ Authentication (JWT Tokens)                        │
│  │  ├─ User login → JWT token issued                   │
│  │  ├─ Token stored in browser                         │
│  │  └─ Token sent with each request                    │
│  │                                                      │
│  ├─ Authorization (Role-Based)                         │
│  │  ├─ Admin role                                      │
│  │  ├─ User role                                       │
│  │  └─ Custom permissions                              │
│  │                                                      │
│  ├─ CORS (Cross-Origin Resource Sharing)               │
│  │  ├─ Whitelist allowed origins                       │
│  │  ├─ Prevent unauthorized requests                   │
│  │  └─ Configured in Backend                           │
│  │                                                      │
│  └─ Input Validation                                   │
│     ├─ Server-side validation                          │
│     ├─ SQL injection prevention (EF Core)              │
│     └─ XSS prevention                                  │
│                                                         │
│  Layer 3: Database Security                            │
│  ├─ Database User Permissions                          │
│  │  ├─ Limited privileges                              │
│  │  ├─ No superuser access                             │
│  │  └─ Connection pooling                              │
│  │                                                      │
│  ├─ Password Hashing                                   │
│  │  ├─ Bcrypt/PBKDF2 for user passwords                │
│  │  └─ Never stored in plain text                      │
│  │                                                      │
│  └─ Data Encryption                                    │
│     ├─ Sensitive data encrypted at rest                │
│     └─ Encrypted in transit                            │
│                                                         │
│  Layer 4: Infrastructure Security                      │
│  ├─ File Permissions                                   │
│  │  ├─ Configuration files: 600                        │
│  │  ├─ Application files: 755                          │
│  │  └─ Owned by www-data user                          │
│  │                                                      │
│  ├─ Service Isolation                                  │
│  │  ├─ Backend runs as www-data                        │
│  │  ├─ Database runs as postgres                       │
│  │  └─ No shared credentials                           │
│  │                                                      │
│  └─ Logging & Monitoring                               │
│     ├─ All requests logged                             │
│     ├─ Error tracking                                  │
│     └─ Security audit trail                            │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## Backup Architecture

```
┌──────────────────────────────────────────────────────────┐
│                   BACKUP STRATEGY                        │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  Automated Daily Backups (2 AM)                          │
│  ├─ Database Backup                                     │
│  │  ├─ Full dump: PRMIS_YYYYMMDD_HHMMSS.sql.gz         │
│  │  ├─ Compressed: ~100MB (depends on data)            │
│  │  ├─ Stored: /var/www/prmis/backups/                 │
│  │  └─ Retention: 30 days                              │
│  │                                                      │
│  └─ Cron Job                                            │
│     └─ /var/www/prmis/backup-db.sh                     │
│                                                          │
│  Manual Backups (On Demand)                             │
│  ├─ Database Backup                                     │
│  ├─ Backend Code Backup                                │
│  ├─ Frontend Code Backup                               │
│  ├─ Configuration Backup                               │
│  └─ Resources Backup                                   │
│                                                          │
│  External Backups (Optional)                            │
│  ├─ USB Drive Backup                                   │
│  ├─ NFS Network Backup                                 │
│  └─ Cloud Storage (AWS S3, etc.)                       │
│                                                          │
│  Recovery Process                                       │
│  ├─ Restore database from backup                       │
│  ├─ Restore application files                          │
│  ├─ Verify data integrity                              │
│  └─ Restart services                                   │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

---

## Monitoring Architecture

```
┌──────────────────────────────────────────────────────────┐
│                   MONITORING STACK                       │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  System Metrics                                          │
│  ├─ CPU Usage (htop, top)                               │
│  ├─ Memory Usage (free, ps)                             │
│  ├─ Disk Usage (df, du)                                 │
│  ├─ Network I/O (netstat, iftop)                        │
│  └─ Process Status (systemctl, ps)                      │
│                                                          │
│  Application Logs                                        │
│  ├─ Backend Logs (systemd journal)                      │
│  │  └─ journalctl -u prmis-backend.service -f           │
│  │                                                      │
│  ├─ Nginx Logs                                          │
│  │  ├─ Access logs: /var/log/nginx/prmis_access.log    │
│  │  └─ Error logs: /var/log/nginx/prmis_error.log      │
│  │                                                      │
│  └─ PostgreSQL Logs                                     │
│     └─ /var/log/postgresql/postgresql-15-main.log      │
│                                                          │
│  Health Checks                                           │
│  ├─ Backend Health: /api/health                         │
│  ├─ Database Connection: psql test                      │
│  ├─ Nginx Status: systemctl status nginx                │
│  └─ Service Status: systemctl status prmis-backend      │
│                                                          │
│  Alerting (Optional)                                     │
│  ├─ Email Alerts                                        │
│  ├─ Service Down Alert                                  │
│  ├─ High CPU Alert                                      │
│  ├─ High Memory Alert                                   │
│  ├─ Disk Space Alert                                    │
│  └─ Backup Failure Alert                                │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

---

## Performance Optimization

```
┌──────────────────────────────────────────────────────────┐
│              PERFORMANCE OPTIMIZATION                    │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  Frontend Optimization                                   │
│  ├─ Angular Production Build                            │
│  │  ├─ Tree shaking                                     │
│  │  ├─ Minification                                     │
│  │  └─ Code splitting                                  │
│  │                                                      │
│  ├─ Nginx Caching                                       │
│  │  ├─ Static files: 30 days                            │
│  │  ├─ index.html: no-cache                             │
│  │  └─ Cache-Control headers                            │
│  │                                                      │
│  └─ Gzip Compression                                    │
│     ├─ Enabled for text/css/js                          │
│     ├─ Reduces bandwidth by ~70%                        │
│     └─ Transparent to browser                           │
│                                                          │
│  Backend Optimization                                    │
│  ├─ Connection Pooling                                  │
│  │  ├─ MaxPoolSize: 20                                  │
│  │  └─ Reuses connections                               │
│  │                                                      │
│  ├─ Async/Await                                         │
│  │  ├─ Non-blocking I/O                                 │
│  │  └─ Better resource utilization                      │
│  │                                                      │
│  └─ Caching                                             │
│     ├─ In-memory caching                                │
│     └─ Reduces database queries                         │
│                                                          │
│  Database Optimization                                   │
│  ├─ Indexes                                             │
│  │  ├─ Primary keys                                     │
│  │  ├─ Foreign keys                                     │
│  │  └─ Query optimization                               │
│  │                                                      │
│  ├─ Query Optimization                                  │
│  │  ├─ EXPLAIN ANALYZE                                  │
│  │  ├─ Avoid N+1 queries                                │
│  │  └─ Use joins efficiently                            │
│  │                                                      │
│  └─ Connection Management                               │
│     ├─ Connection pooling                               │
│     ├─ Idle connection cleanup                          │
│     └─ Max connections limit                            │
│                                                          │
│  Infrastructure Optimization                            │
│  ├─ Server Resources                                    │
│  │  ├─ 32GB RAM: Sufficient                             │
│  │  ├─ 8 cores: Adequate                                │
│  │  └─ 2.4TB storage: Plenty                            │
│  │                                                      │
│  └─ Load Distribution                                   │
│     ├─ Nginx load balancing (if multiple backends)      │
│     ├─ Database replication (if needed)                 │
│     └─ Horizontal scaling (future)                      │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

---

## Deployment Workflow

```
┌─────────────────────────────────────────────────────────┐
│              DEPLOYMENT WORKFLOW                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  1. Preparation                                         │
│     ├─ Review code changes                              │
│     ├─ Run tests                                        │
│     ├─ Create backups                                   │
│     └─ Prepare deployment files                         │
│                                                         │
│  2. Build                                               │
│     ├─ Backend: dotnet publish -c Release               │
│     ├─ Frontend: npm run build                          │
│     └─ Verify builds                                    │
│                                                         │
│  3. Deploy                                              │
│     ├─ Copy files to server                             │
│     ├─ Update configuration                             │
│     ├─ Set permissions                                  │
│     └─ Restart services                                 │
│                                                         │
│  4. Verification                                        │
│     ├─ Check service status                             │
│     ├─ Test API endpoints                               │
│     ├─ Test frontend loading                            │
│     ├─ Test user login                                  │
│     └─ Verify data integrity                            │
│                                                         │
│  5. Monitoring                                          │
│     ├─ Watch logs for errors                            │
│     ├─ Monitor system resources                         │
│     ├─ Check performance metrics                        │
│     └─ Verify backups                                   │
│                                                         │
│  6. Rollback (if needed)                                │
│     ├─ Stop services                                    │
│     ├─ Restore previous version                         │
│     ├─ Restore database (if needed)                     │
│     └─ Restart services                                 │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

This architecture provides:
- **Scalability**: Can handle expected load
- **Reliability**: Auto-restart services, backups
- **Security**: Multiple security layers
- **Performance**: Optimized caching and compression
- **Maintainability**: Clear separation of concerns
- **Monitoring**: Comprehensive logging and alerting

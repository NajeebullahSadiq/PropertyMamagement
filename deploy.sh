#!/bin/bash

# ============================================
# PRMIS Automated Deployment Script
# ============================================
# This script automates the deployment process
# Run with: bash deploy.sh [frontend|backend|full]
# ============================================

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
REPO_DIR=~/PropertyMamagement
FRONTEND_DIR=/var/www/prmis/frontend
BACKEND_DIR=/var/www/prmis/backend
STORAGE_DIR=/var/www/prmis/storage

# Functions
print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}→ $1${NC}"
}

check_prerequisites() {
    print_info "Checking prerequisites..."
    
    if [ ! -d "$REPO_DIR" ]; then
        print_error "Repository not found at $REPO_DIR"
        exit 1
    fi
    
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK not found. Please install .NET 9"
        exit 1
    fi
    
    if ! command -v npm &> /dev/null; then
        print_error "Node.js/npm not found. Please install Node.js"
        exit 1
    fi
    
    if ! command -v nginx &> /dev/null; then
        print_error "Nginx not found. Please install nginx"
        exit 1
    fi
    
    print_success "All prerequisites met"
}

setup_directories() {
    print_info "Setting up directory structure..."
    
    sudo mkdir -p $FRONTEND_DIR
    sudo mkdir -p $BACKEND_DIR
    sudo mkdir -p $STORAGE_DIR/Resources/Documents/{Profile,Identity,Property,Vehicle,Company,License}
    sudo mkdir -p $STORAGE_DIR/Resources/Images
    sudo mkdir -p /var/www/.dotnet
    
    sudo chown -R www-data:www-data /var/www/prmis
    sudo chown -R www-data:www-data /var/www/.dotnet
    sudo chmod -R 755 /var/www/prmis
    sudo chmod -R 755 /var/www/.dotnet
    
    print_success "Directories created"
}

deploy_frontend() {
    print_info "Deploying Frontend..."
    
    cd $REPO_DIR
    git pull origin main
    
    cd Frontend
    print_info "Installing npm dependencies..."
    npm install --legacy-peer-deps
    
    print_info "Building Angular application..."
    npx ng build --configuration production
    
    print_info "Copying files to deployment directory..."
    sudo rm -rf $FRONTEND_DIR/*
    sudo cp -r dist/property-registeration-mis/* $FRONTEND_DIR/
    sudo chown -R www-data:www-data $FRONTEND_DIR
    sudo chmod -R 755 $FRONTEND_DIR
    
    print_success "Frontend deployed successfully!"
}

deploy_backend() {
    print_info "Deploying Backend..."
    
    cd $REPO_DIR
    git pull origin main
    
    cd Backend
    print_info "Building .NET application..."
    dotnet build WebAPIBackend.csproj --configuration Release
    
    print_info "Publishing .NET application..."
    dotnet publish WebAPIBackend.csproj --configuration Release --output ./publish
    
    print_info "Copying files to deployment directory..."
    sudo find $BACKEND_DIR -mindepth 1 -maxdepth 1 ! -name 'Resources' -exec rm -rf {} +
    sudo cp -r ./publish/* $BACKEND_DIR/
    sudo chown -R www-data:www-data $BACKEND_DIR
    sudo chmod -R 755 $BACKEND_DIR
    
    print_info "Restarting backend service..."
    sudo systemctl restart prmis-backend
    
    sleep 2
    if sudo systemctl is-active --quiet prmis-backend; then
        print_success "Backend deployed and running!"
    else
        print_error "Backend service failed to start. Check logs with: sudo journalctl -u prmis-backend -n 50"
        exit 1
    fi
}

setup_nginx() {
    print_info "Setting up Nginx..."
    
    if [ ! -f /etc/nginx/sites-available/prmis ]; then
        sudo cp $REPO_DIR/nginx-prmis.conf /etc/nginx/sites-available/prmis
        print_success "Nginx config copied"
    fi
    
    if [ ! -L /etc/nginx/sites-enabled/prmis ]; then
        sudo ln -s /etc/nginx/sites-available/prmis /etc/nginx/sites-enabled/prmis
        print_success "Nginx site enabled"
    fi
    
    print_info "Testing Nginx configuration..."
    if sudo nginx -t; then
        print_success "Nginx configuration valid"
        sudo systemctl restart nginx
        print_success "Nginx restarted"
    else
        print_error "Nginx configuration invalid"
        exit 1
    fi
}

setup_backend_service() {
    print_info "Setting up backend service..."
    
    sudo cp $REPO_DIR/prmis-backend.service /etc/systemd/system/
    sudo systemctl daemon-reload
    sudo systemctl enable prmis-backend
    sudo systemctl start prmis-backend
    
    sleep 2
    if sudo systemctl is-active --quiet prmis-backend; then
        print_success "Backend service running"
    else
        print_error "Backend service failed to start"
        sudo journalctl -u prmis-backend -n 20 --no-pager
        exit 1
    fi
}

show_status() {
    echo ""
    echo "=========================================="
    echo "Deployment Status"
    echo "=========================================="
    
    echo -n "Backend Service: "
    if sudo systemctl is-active --quiet prmis-backend; then
        print_success "Running"
    else
        print_error "Stopped"
    fi
    
    echo -n "Nginx Service: "
    if sudo systemctl is-active --quiet nginx; then
        print_success "Running"
    else
        print_error "Stopped"
    fi
    
    echo ""
    echo "Frontend: $FRONTEND_DIR"
    echo "Backend: $BACKEND_DIR"
    echo "Storage: $STORAGE_DIR"
    echo ""
    echo "View backend logs: sudo journalctl -u prmis-backend -f"
    echo "View nginx logs: sudo tail -f /var/log/nginx/prmis_error.log"
    echo "=========================================="
}

# Main script
case "$1" in
    frontend)
        print_info "Frontend-only deployment"
        check_prerequisites
        deploy_frontend
        show_status
        ;;
    backend)
        print_info "Backend-only deployment"
        check_prerequisites
        deploy_backend
        show_status
        ;;
    full)
        print_info "Full deployment (Frontend + Backend)"
        check_prerequisites
        deploy_frontend
        deploy_backend
        show_status
        ;;
    setup)
        print_info "Initial setup (first-time deployment)"
        check_prerequisites
        setup_directories
        setup_backend_service
        setup_nginx
        deploy_frontend
        deploy_backend
        show_status
        ;;
    *)
        echo "Usage: $0 {frontend|backend|full|setup}"
        echo ""
        echo "Commands:"
        echo "  frontend  - Deploy only frontend"
        echo "  backend   - Deploy only backend"
        echo "  full      - Deploy both frontend and backend"
        echo "  setup     - First-time setup (directories, services, nginx)"
        echo ""
        exit 1
        ;;
esac

print_success "Deployment completed!"

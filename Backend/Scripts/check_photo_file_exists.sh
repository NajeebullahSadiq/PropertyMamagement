#!/bin/bash

# Check if the photo file exists on the server

echo "Checking for photo file..."
echo ""

# Get the backend directory
BACKEND_DIR=~/PropertyMamagement/Backend

# Check the Resources directory structure
echo "=== Resources Directory Structure ==="
ls -la "$BACKEND_DIR/Resources/" 2>/dev/null || echo "Resources directory not found at $BACKEND_DIR/Resources/"
echo ""

echo "=== Documents Directory ==="
ls -la "$BACKEND_DIR/Resources/Documents/" 2>/dev/null || echo "Documents directory not found"
echo ""

echo "=== Company Directory ==="
ls -la "$BACKEND_DIR/Resources/Documents/Company/" 2>/dev/null || echo "Company directory not found"
echo ""

echo "=== Profile Directory ==="
ls -la "$BACKEND_DIR/Resources/Documents/Profile/" 2>/dev/null || echo "Profile directory not found"
echo ""

# Search for the specific file
echo "=== Searching for profile_20260126_070151_288.jpg ==="
find "$BACKEND_DIR/Resources/" -name "profile_20260126_070151_288.jpg" 2>/dev/null || echo "File not found"
echo ""

# Check all profile images
echo "=== All profile images in Resources ==="
find "$BACKEND_DIR/Resources/" -name "profile_*.jpg" 2>/dev/null | head -20
echo ""

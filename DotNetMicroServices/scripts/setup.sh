#!/bin/bash

# Setup script for DotNetMicroServices project
# This script sets up the development environment

echo "Setting up DotNetMicroServices development environment..."

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed. Please install it from https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✓ .NET SDK found"

# Restore NuGet packages
echo "Restoring NuGet packages..."
dotnet restore

# Build the solution
echo "Building solution..."
dotnet build

# Run tests
echo "Running tests..."
dotnet test

echo "Setup completed successfully!"



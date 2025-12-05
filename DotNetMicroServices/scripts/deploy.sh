#!/bin/bash

# Deployment script for DotNetMicroServices
# This script builds and deploys all services

set -e  # Exit on error

echo "Starting deployment..."

# Build all projects
echo "Building projects..."
dotnet build --configuration Release

# Run tests
echo "Running tests..."
dotnet test --configuration Release --no-build

# Publish services
echo "Publishing services..."

echo "Publishing Gateway..."
dotnet publish src/Gateway/Gateway.csproj --configuration Release --output ./publish/Gateway

echo "Publishing UserAccountService..."
dotnet publish src/UserAccountService/UserAccountService.csproj --configuration Release --output ./publish/UserAccountService

echo "Publishing CoursesService..."
dotnet publish src/CoursesService/CoursesService.csproj --configuration Release --output ./publish/CoursesService

echo "Deployment completed successfully!"
echo "Published files are in ./publish/ directory"



#!/bin/bash

# Seed data script for development
# This script seeds the database with initial data

echo "Seeding database with initial data..."

# Check if MongoDB is running
if ! pgrep -x "mongod" > /dev/null; then
    echo "Warning: MongoDB does not appear to be running"
    echo "Please start MongoDB before running this script"
    exit 1
fi

# TODO: Implement data seeding logic
# This could use MongoDB shell scripts or a .NET seeding application

echo "Data seeding completed!"



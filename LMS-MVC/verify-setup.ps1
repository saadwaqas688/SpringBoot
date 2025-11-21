# LMS Project Setup Verification Script
Write-Host "=== LMS Project Setup Verification ===" -ForegroundColor Cyan
Write-Host ""

# Check .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK is installed: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK is NOT installed" -ForegroundColor Red
    Write-Host "  Download from: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Yellow
}
Write-Host ""

# Check SQL Server LocalDB
Write-Host "Checking SQL Server LocalDB..." -ForegroundColor Yellow
try {
    $localdbInfo = sqllocaldb info 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ SQL Server LocalDB is installed" -ForegroundColor Green
        Write-Host "  Available instances:" -ForegroundColor Gray
        $localdbInfo | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
    } else {
        Write-Host "✗ SQL Server LocalDB is NOT installed" -ForegroundColor Red
        Write-Host "  Download SQL Server Express from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ SQL Server LocalDB is NOT installed" -ForegroundColor Red
    Write-Host "  Download SQL Server Express from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads" -ForegroundColor Yellow
}
Write-Host ""

# Check if we're in the project directory
Write-Host "Checking project files..." -ForegroundColor Yellow
if (Test-Path "LMS-MVC.csproj") {
    Write-Host "✓ Project file found" -ForegroundColor Green
} else {
    Write-Host "✗ Project file not found. Make sure you're in the project directory." -ForegroundColor Red
}
Write-Host ""

# Check connection string
Write-Host "Checking connection string..." -ForegroundColor Yellow
if (Test-Path "appsettings.json") {
    $appsettings = Get-Content "appsettings.json" | ConvertFrom-Json
    if ($appsettings.ConnectionStrings.DefaultConnection) {
        Write-Host "✓ Connection string configured" -ForegroundColor Green
        Write-Host "  Connection: $($appsettings.ConnectionStrings.DefaultConnection)" -ForegroundColor Gray
    } else {
        Write-Host "✗ Connection string not found in appsettings.json" -ForegroundColor Red
    }
} else {
    Write-Host "✗ appsettings.json not found" -ForegroundColor Red
}
Write-Host ""

Write-Host "=== Verification Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Install missing prerequisites (if any)" -ForegroundColor White
Write-Host "2. Run: dotnet restore" -ForegroundColor White
Write-Host "3. Run: dotnet run" -ForegroundColor White


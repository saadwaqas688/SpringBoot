# PowerShell script to run both backend and frontend with auto-restart
Write-Host "Starting development environment with auto-restart..." -ForegroundColor Green
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Start backend in watch mode (in new window)
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$scriptPath\backend\DotNetMessaging.API'; dotnet watch run"

# Wait a bit for backend to start
Start-Sleep -Seconds 2

# Start frontend (in new window)
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$scriptPath\frontend'; npm start"

Write-Host "Backend starting on http://localhost:5000" -ForegroundColor Cyan
Write-Host "Frontend starting on http://localhost:3000" -ForegroundColor Cyan
Write-Host ""
Write-Host "Both servers will auto-restart on code changes." -ForegroundColor Yellow
Write-Host "Close the PowerShell windows to stop the servers." -ForegroundColor Yellow
Write-Host ""
Write-Host "Press any key to exit this script (servers will continue running)..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")


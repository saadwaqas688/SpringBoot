@echo off
echo Starting development environment with auto-restart...
echo.

REM Start backend in watch mode (in new window)
start "Backend Server" cmd /k "cd backend\DotNetMessaging.API && dotnet watch run"

REM Wait a bit for backend to start
timeout /t 3 /nobreak >nul

REM Start frontend (in new window)
start "Frontend Server" cmd /k "cd frontend && npm start"

echo.
echo Backend starting on http://localhost:5000
echo Frontend starting on http://localhost:3000
echo.
echo Both servers will auto-restart on code changes.
echo Close the windows to stop the servers.
echo.
pause




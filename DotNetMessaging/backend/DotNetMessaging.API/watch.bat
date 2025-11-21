@echo off
echo Starting backend with auto-restart (watch mode)...
echo Similar to nodemon in Node.js - automatically restarts on file changes
echo The server will automatically restart when you make code changes.
echo Watching: .cs, .csproj, .json files
echo Ignoring: bin/, obj/, uploads/, .db files
echo Press Ctrl+C to stop.
echo.

dotnet watch run


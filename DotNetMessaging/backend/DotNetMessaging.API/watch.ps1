# PowerShell script to run backend with auto-restart (dotnet watch)
# Similar to nodemon in Node.js - automatically restarts on file changes
Write-Host "🚀 Starting backend with auto-restart (watch mode)..." -ForegroundColor Green
Write-Host "📝 The server will automatically restart when you make code changes." -ForegroundColor Yellow
Write-Host "👀 Watching: .cs, .csproj, .json files" -ForegroundColor Cyan
Write-Host "🚫 Ignoring: bin/, obj/, uploads/, .db files" -ForegroundColor Gray
Write-Host "⏹️  Press Ctrl+C to stop." -ForegroundColor Yellow
Write-Host ""
Write-Host "💡 TIP: Make sure you SAVE the file (Ctrl+S) for changes to be detected!" -ForegroundColor Magenta
Write-Host ""

# Use --verbose to see what files are being watched
dotnet watch run --verbose


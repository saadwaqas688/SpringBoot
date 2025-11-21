# Development Guide - Auto Restart

## Quick Start with Auto-Restart

### Backend Only (with auto-restart)

Navigate to `backend/DotNetMessaging.API` and run:

```powershell
# PowerShell
.\watch.ps1

# Or CMD
watch.bat

# Or directly
dotnet watch run
```

The server will automatically restart when you make changes to:
- `.cs` files (Controllers, Services, Models, etc.)
- `.csproj` files
- `appsettings.json` files
- `Program.cs`

### Frontend Only (already has hot reload)

Navigate to `frontend` and run:

```bash
npm start
```

The frontend already has hot reloading built-in. Changes to React components will update automatically in the browser without a full page refresh.

### Both Backend and Frontend Together

From the root directory (`DotNetMessaging`):

```powershell
# PowerShell
.\start-dev.ps1

# Or CMD
start-dev.bat
```

This will start both servers in separate windows, both with auto-restart enabled.

## How It Works

### Backend (`dotnet watch`)
- Monitors file changes in the project
- Automatically rebuilds and restarts when changes are detected
- Shows build output and restart messages in the console
- Much faster than manually stopping and starting the server

### Frontend (`npm start` / `react-scripts`)
- Uses Webpack's Hot Module Replacement (HMR)
- Updates components in the browser without full page refresh
- Preserves component state when possible
- Shows compilation status in the terminal

## Tips

1. **Backend changes**: The server will restart automatically. You'll see a message like:
   ```
   File changed: Services/AuthService.cs
   Restarting...
   ```

2. **Frontend changes**: The browser will update automatically. You'll see:
   ```
   Compiled successfully!
   ```

3. **If auto-restart doesn't work**:
   - Make sure you're using `dotnet watch run` (not just `dotnet run`)
   - Check that the file you're editing is in the project directory
   - Try saving the file again (some editors require explicit save)

4. **Performance**: 
   - Backend restart takes 2-5 seconds
   - Frontend hot reload is nearly instant (< 1 second)

## Troubleshooting

**Backend not restarting:**
- Ensure you're using `dotnet watch run`
- Check that your `.cs` files are saved
- Verify the project builds successfully (`dotnet build`)

**Frontend not updating:**
- Check the browser console for errors
- Try hard refresh (Ctrl+F5)
- Restart the dev server if needed




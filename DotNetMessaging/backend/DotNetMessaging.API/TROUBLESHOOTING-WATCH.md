# Troubleshooting: Auto-Restart Not Working

If your changes to `.cs` files (like `ChatHub.cs`) are not triggering auto-restart, try these solutions:

## ✅ Solution 1: Make Sure You're Using `dotnet watch run`

**WRONG:**
```bash
dotnet run  # ❌ This does NOT auto-restart
```

**CORRECT:**
```bash
dotnet watch run  # ✅ This auto-restarts on changes
```

Or use the scripts:
```bash
.\watch.ps1    # PowerShell
watch.bat      # CMD
```

## ✅ Solution 2: Save Your File

Make sure you **SAVE** the file after making changes:
- Press `Ctrl+S` in your editor
- Or enable auto-save in your IDE

`dotnet watch` only detects **saved** file changes, not unsaved changes.

## ✅ Solution 3: Check the Console Output

When you save a `.cs` file, you should see:
```
File changed: Hubs/ChatHub.cs
Restarting...
```

If you don't see this message, `dotnet watch` might not be running.

## ✅ Solution 4: Restart the Watch Process

1. Stop the current process (`Ctrl+C`)
2. Run `dotnet watch run` again
3. Make a change and save

## ✅ Solution 5: Clean and Rebuild

Sometimes cached files can cause issues:

```bash
dotnet clean
dotnet build
dotnet watch run
```

## ✅ Solution 6: Check for Build Errors

If there are compilation errors, the watch won't restart. Check the console for error messages and fix them first.

## ✅ Solution 7: Verify File Location

Make sure the file you're editing is:
- Inside the project directory (`DotNetMessaging.API`)
- Not in `bin/` or `obj/` folders (these are ignored)
- Has a `.cs` extension

## Quick Test

1. Open `ChatHub.cs`
2. Add a comment: `// Test change`
3. **Save the file** (`Ctrl+S`)
4. You should see in console: `File changed: Hubs/ChatHub.cs` → `Restarting...`

If this doesn't work, you're likely running `dotnet run` instead of `dotnet watch run`.




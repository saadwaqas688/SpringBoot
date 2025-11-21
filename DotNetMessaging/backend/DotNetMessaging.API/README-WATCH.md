# Auto-Restart Configuration (Nodemon-like for .NET)

This project uses `dotnet watch` which works similarly to `nodemon` in Node.js.

## Quick Start

Just run:
```bash
dotnet watch run
```

Or use the convenience scripts:
```bash
# PowerShell
.\watch.ps1

# CMD
watch.bat
```

## Configuration

The watch behavior is configured in `watch.json` (similar to `nodemon.json`):

- **Watches**: `.cs`, `.csproj`, `.json` files
- **Ignores**: `bin/`, `obj/`, `wwwroot/uploads/`, `.db` files

## How It Works

When you save any `.cs`, `.csproj`, or `.json` file:
1. `dotnet watch` detects the change
2. Automatically rebuilds the project
3. Restarts the server
4. Shows restart message in console

## Customization

Edit `watch.json` to customize what files are watched or ignored.

Example - to also watch `.cshtml` files:
```json
{
  "watch": [
    {
      "include": "**/*.cshtml"
    }
  ]
}
```

## Comparison with Nodemon

| Nodemon (Node.js) | dotnet watch (.NET) |
|-------------------|---------------------|
| `nodemon.json` | `watch.json` |
| `nodemon app.js` | `dotnet watch run` |
| Watches `.js`, `.json` | Watches `.cs`, `.csproj`, `.json` |
| Auto-restart on changes | Auto-restart on changes |

## Tips

- Changes to `.cs` files trigger a full restart (2-5 seconds)
- The server will show: `File changed: Controllers/AuthController.cs` then `Restarting...`
- No need to manually stop/start the server anymore!




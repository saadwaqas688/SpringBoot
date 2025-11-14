# Hot Reload / Auto-Restart Guide (Like nodemon)

Spring Boot DevTools provides automatic application restart when code changes, similar to `nodemon` in Express.js.

## How It Works

When you save a file, DevTools automatically:

1. **Detects the change** in your Java files or configuration
2. **Restarts the application** (only the Spring context, not the full JVM - very fast!)
3. **Your changes are live** - no manual restart needed!

## Setup

✅ **Already Configured!** DevTools is included and configured in this project.

## Usage

### Option 1: Run with Maven (Recommended)

```cmd
.\mvnw.cmd spring-boot:run
```

**What happens:**

- Application starts
- DevTools monitors file changes
- When you save a `.java` file → automatic restart
- When you save `application.properties` → automatic restart
- Restart is fast (1-2 seconds) - only Spring context restarts, not the JVM

### Option 2: Run from IDE

**IntelliJ IDEA:**

1. Run the application normally
2. Enable "Build project automatically":
   - File → Settings → Build, Execution, Deployment → Compiler
   - Check "Build project automatically"
3. Enable auto-make:
   - Press `Ctrl+Shift+A` → Search "Registry"
   - Enable `compiler.automake.allow.when.app.running`
4. Save any file → automatic restart!

**Eclipse:**

- DevTools works automatically
- Just save files and the app restarts

**VS Code:**

- Install "Spring Boot Extension Pack"
- Run the application
- Auto-restart works automatically

## What Triggers Restart

✅ **These changes trigger restart:**

- Java files in `src/main/java/**/*.java`
- Configuration files: `application.properties`, `application.yml`
- XML configuration files
- Any file in `src/main/resources`

❌ **These changes DON'T trigger restart:**

- Static files (HTML, CSS, JS in `static/` folder)
- Test files (`src/test/**`)
- Files in `target/` directory

## LiveReload (Browser Auto-Refresh)

DevTools also includes **LiveReload** server on port `35729`:

1. **Install LiveReload browser extension:**

   - Chrome: [LiveReload](https://chrome.google.com/webstore/detail/livereload/jnihajbhpnppcggbcgedagnkighmdajd)
   - Firefox: [LiveReload](https://addons.mozilla.org/en-US/firefox/addon/livereload-web-extension/)

2. **Enable the extension** in your browser

3. **Open your app:** `http://localhost:8080`

4. **When you change code:**
   - DevTools restarts the app
   - LiveReload automatically refreshes your browser
   - No manual refresh needed!

## Monitoring Restart Activity

Watch the console for restart messages:

```
2025-11-06 12:00:00.123  INFO --- [restartedMain] c.e.t.TodoMicroserviceApplication : Starting TodoMicroserviceApplication
...
2025-11-06 12:00:01.456  INFO --- [restartedMain] c.e.t.TodoMicroserviceApplication : Started TodoMicroserviceApplication in 1.333 seconds

# After you save a file:
2025-11-06 12:00:30.789  INFO --- [File Watcher] o.s.b.d.a.OptionalLiveReloadServer : LiveReload server is running on port 35729
2025-11-06 12:00:30.890  INFO --- [restartedMain] c.e.t.TodoMicroserviceApplication : Starting TodoMicroserviceApplication
...
2025-11-06 12:00:32.123  INFO --- [restartedMain] c.e.t.TodoMicroserviceApplication : Started TodoMicroserviceApplication in 1.233 seconds
```

## Configuration

Current settings in `application.properties`:

```properties
# Enable auto-restart
spring.devtools.restart.enabled=true

# Watch these paths
spring.devtools.restart.additional-paths=src/main/java,src/main/resources

# Exclude these from triggering restart
spring.devtools.restart.exclude=static/**,public/**,templates/**

# Enable LiveReload
spring.devtools.livereload.enabled=true
spring.devtools.livereload.port=35729

# Fast polling (check for changes every 1 second)
spring.devtools.restart.poll-interval=1s
spring.devtools.restart.quiet-period=400ms
```

## Troubleshooting

### Issue: Changes not triggering restart

**Solution 1: Check if DevTools is active**

- Look for `[restartedMain]` in logs (not just `[main]`)
- If you see `[main]`, DevTools might not be active

**Solution 2: Rebuild the project**

```cmd
.\mvnw.cmd clean compile
```

**Solution 3: Verify IDE settings**

- IntelliJ: Enable "Build project automatically"
- Make sure files are being compiled on save

**Solution 4: Check file paths**

- Make sure you're editing files in `src/main/java` or `src/main/resources`
- Files in `target/` won't trigger restart

### Issue: Restart is too slow

**Solution:**

- DevTools uses "fast restart" (only Spring context, not JVM)
- If still slow, check for:
  - Large number of classes
  - Heavy initialization code
  - Database connection issues

### Issue: Too many restarts

**Solution:**

- Exclude specific paths in `application.properties`:

```properties
spring.devtools.restart.exclude=static/**,logs/**
```

### Issue: LiveReload not working

**Solution:**

1. Check if LiveReload server is running (port 35729)
2. Install browser extension
3. Enable extension in browser
4. Check browser console for LiveReload connection

## Disable Auto-Restart (if needed)

If you want to disable auto-restart temporarily:

```properties
spring.devtools.restart.enabled=false
```

Or remove DevTools dependency from `pom.xml` (not recommended for development).

## Comparison with nodemon

| Feature             | nodemon (Express.js) | Spring Boot DevTools          |
| ------------------- | -------------------- | ----------------------------- |
| **Auto-restart**    | ✅ Yes               | ✅ Yes                        |
| **Fast restart**    | Full process restart | Only Spring context (faster!) |
| **File watching**   | ✅ Yes               | ✅ Yes                        |
| **Configurable**    | ✅ Yes               | ✅ Yes                        |
| **Browser refresh** | Manual or with tools | ✅ LiveReload built-in        |
| **Trigger on save** | ✅ Yes               | ✅ Yes                        |

## Best Practices

1. **Keep DevTools enabled** during development
2. **Use LiveReload** for frontend development
3. **Watch console** for restart confirmations
4. **Don't commit** DevTools to production (it's excluded automatically)
5. **Use fast restart** - it's much faster than full JVM restart

## Production Note

⚠️ **DevTools is automatically excluded from production builds!**

- It's marked as `optional=true` in `pom.xml`
- Spring Boot excludes it when packaging for production
- No need to remove it manually

## Summary

✅ **DevTools is configured and ready!**

Just run:

```cmd
.\mvnw.cmd spring-boot:run
```

Then:

- Edit any Java file → Save → Auto-restart! 🚀
- Edit `application.properties` → Save → Auto-restart! 🚀
- Changes are live in 1-2 seconds!

No more manual restarts needed - just like nodemon! 🎉









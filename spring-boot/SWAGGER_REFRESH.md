# Swagger UI Not Showing Changes - Troubleshooting Guide

If your Swagger UI is not reflecting the latest API changes, follow these steps:

## Step 1: Restart the Application

**Most Important:** Swagger scans the code at application startup. You MUST restart the application for changes to appear.

1. **Stop the application:**

   - Press `Ctrl+C` in the terminal where the app is running
   - Or stop it from your IDE

2. **Rebuild and restart:**

   ```cmd
   .\mvnw.cmd clean spring-boot:run
   ```

   Or if using IDE:

   - Clean and rebuild the project
   - Run the application again

## Step 2: Clear Browser Cache

Swagger UI caches the API documentation. Clear your browser cache:

### Chrome/Edge:

1. Open Swagger UI: `http://localhost:8080/swagger-ui.html`
2. Press `Ctrl+Shift+R` (Windows) or `Cmd+Shift+R` (Mac) for hard refresh
3. Or press `F12` → Right-click refresh button → "Empty Cache and Hard Reload"

### Firefox:

1. Press `Ctrl+Shift+R` or `Cmd+Shift+R`
2. Or `F12` → Network tab → Check "Disable cache" → Refresh

### Alternative: Use Incognito/Private Mode

- Open Swagger UI in an incognito/private window to bypass cache

## Step 3: Verify API Documentation Endpoint

Check if the OpenAPI JSON is updated:

1. **Open in browser:**

   ```
   http://localhost:8080/api-docs
   ```

2. **Look for your changes:**

   - Search for "Get all todos with optional filters"
   - Check if `completed` and `title` parameters are present
   - Verify the endpoint description

3. **If the JSON is correct but Swagger UI isn't:**
   - The issue is browser cache (go back to Step 2)
   - Try accessing: `http://localhost:8080/swagger-ui/index.html` instead

## Step 4: Check Application Logs

When the application starts, look for SpringDoc initialization logs:

```
... SpringDoc OpenAPI documentation initialized ...
```

If you see errors related to SpringDoc, there might be a configuration issue.

## Step 5: Verify Code Changes

Make sure your controller has the correct annotations:

```java
@GetMapping
public ResponseEntity<List<Todo>> getAllTodos(
    @Parameter(description = "Filter by completion status (true/false). Optional.", required = false)
    @RequestParam(required = false) Boolean completed,
    @Parameter(description = "Search term to filter todos by title (case-insensitive). Optional.", required = false)
    @RequestParam(required = false) String title) {
    // ...
}
```

## Step 6: Force Swagger Refresh

1. **Close all browser tabs** with Swagger UI
2. **Clear browser cache completely:**
   - Chrome: Settings → Privacy → Clear browsing data → Cached images and files
3. **Restart application** (Step 1)
4. **Open Swagger UI in a new tab**

## Step 7: Check SpringDoc Configuration

Verify `application.properties` has cache disabled:

```properties
springdoc.cache.disabled=true
springdoc.swagger-ui.disable-swagger-default-url=true
```

## Quick Test

After restarting, test directly:

```bash
# Should show updated OpenAPI spec
curl http://localhost:8080/api-docs

# Should work with new parameters
curl "http://localhost:8080/api/todos?completed=true&title=test"
```

## Common Issues

### Issue: "Swagger shows old endpoints"

**Solution:** Restart application + Hard refresh browser (Ctrl+Shift+R)

### Issue: "Parameters not showing in Swagger"

**Solution:**

- Verify `@RequestParam(required = false)` is present
- Check `@Parameter` annotations are correct
- Restart application

### Issue: "Swagger UI is blank"

**Solution:**

- Check `http://localhost:8080/api-docs` works
- Verify SpringDoc dependency in `pom.xml`
- Check application logs for errors

### Issue: "Changes appear in /api-docs but not in Swagger UI"

**Solution:** This is definitely browser cache - use hard refresh or incognito mode

## Still Not Working?

1. **Check Maven dependencies:**

   ```cmd
   .\mvnw.cmd dependency:tree | findstr springdoc
   ```

2. **Verify SpringDoc version compatibility:**

   - Spring Boot 3.2.0 should use SpringDoc 2.3.0 (already configured)

3. **Check for compilation errors:**

   ```cmd
   .\mvnw.cmd clean compile
   ```

4. **Try accessing different Swagger UI path:**
   - `http://localhost:8080/swagger-ui/index.html`
   - `http://localhost:8080/swagger-ui.html`

## Expected Result

After following these steps, you should see in Swagger UI:

- **Endpoint:** `GET /api/todos`
- **Description:** "Get all todos with optional filters"
- **Parameters:**
  - `completed` (query, optional, Boolean)
  - `title` (query, optional, String)
- **Try it out** button should work with these parameters









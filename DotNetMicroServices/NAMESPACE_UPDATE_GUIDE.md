# Namespace Update Guide

## Overview

After the folder structure refactoring, all namespaces need to be updated to match the new folder locations. This guide lists all the namespace changes required.

---

## UserAccountService Namespace Changes

### Old → New

| Old Namespace                                  | New Namespace                                |
| ---------------------------------------------- | -------------------------------------------- |
| `UserAccountService.Data`                      | `UserAccountService.Infrastructure.Data`     |
| `UserAccountService.Services` (business logic) | `UserAccountService.Application.Services`    |
| `UserAccountService.Services` (infrastructure) | `UserAccountService.Infrastructure.Services` |
| `UserAccountService.DTOs`                      | `UserAccountService.Application.DTOs`        |

### Files to Update

1. **Controllers/AuthController.cs**

   - `using UserAccountService.DTOs;` → `using UserAccountService.Application.DTOs;`
   - `using UserAccountService.Services;` → `using UserAccountService.Application.Services;` and `using UserAccountService.Infrastructure.Services;`

2. **Program.cs**

   - `using UserAccountService.Data;` → `using UserAccountService.Infrastructure.Data;`
   - `using UserAccountService.Services;` → Update to new namespaces

3. **Extensions/ServiceCollectionExtensions.cs**

   - Update all using statements

4. **Infrastructure/Data/MongoDbContext.cs**

   - `namespace UserAccountService.Data;` → `namespace UserAccountService.Infrastructure.Data;`

5. **Application/Services/IUserAccountService.cs**

   - `namespace UserAccountService.Services;` → `namespace UserAccountService.Application.Services;`
   - `using UserAccountService.DTOs;` → `using UserAccountService.Application.DTOs;`

6. **Application/Services/UserAccountService.cs**

   - `namespace UserAccountService.Services;` → `namespace UserAccountService.Application.Services;`
   - Update all using statements

7. **Infrastructure/Services/IAuthService.cs**

   - `namespace UserAccountService.Services;` → `namespace UserAccountService.Infrastructure.Services;`

8. **Infrastructure/Services/AuthService.cs**

   - `namespace UserAccountService.Services;` → `namespace UserAccountService.Infrastructure.Services;`

9. **Infrastructure/Services/UserAccountMessageHandler.cs**
   - `namespace UserAccountService.Services;` → `namespace UserAccountService.Infrastructure.Services;`
   - Update all using statements

---

## CoursesService Namespace Changes

### Old → New

| Old Namespace                              | New Namespace                                |
| ------------------------------------------ | -------------------------------------------- |
| `CoursesService.Data`                      | `CoursesService.Infrastructure.Data`         |
| `CoursesService.Repositories`              | `CoursesService.Infrastructure.Repositories` |
| `CoursesService.Services` (business logic) | `CoursesService.Application.Services`        |
| `CoursesService.Services` (infrastructure) | `CoursesService.Infrastructure.Services`     |
| `CoursesService.DTOs`                      | `CoursesService.Application.DTOs`            |

### Files to Update

1. **All Controllers** - Update using statements
2. **Program.cs** - Update using statements
3. **Extensions/ServiceCollectionExtensions.cs** - Update using statements
4. **Infrastructure/Data/CoursesDbContext.cs** - Update namespace
5. **All Infrastructure/Repositories/** - Update namespaces
6. **Application/Services/** - Update namespaces
7. **Infrastructure/Services/** - Update namespaces

---

## Gateway Namespace Changes

### Old → New

| Old Namespace                       | New Namespace                     |
| ----------------------------------- | --------------------------------- |
| `Gateway.Services` (business logic) | `Gateway.Application.Services`    |
| `Gateway.Services` (infrastructure) | `Gateway.Infrastructure.Services` |
| `Gateway.DTOs`                      | `Gateway.Application.DTOs`        |

### Files to Update

1. **All Controllers** - Update using statements
2. **Program.cs** - Update using statements
3. **Infrastructure/Services/** - Update namespaces
4. **Application/Services/** - Update namespaces (if any)

---

## Shared Library Namespace Changes

### Old → New

| Old Namespace         | New Namespace                        |
| --------------------- | ------------------------------------ |
| `Shared.Common`       | `Shared.Core.Common`                 |
| `Shared.Models`       | `Shared.Core.Models`                 |
| `Shared.Services`     | `Shared.Infrastructure.Services`     |
| `Shared.Repositories` | `Shared.Infrastructure.Repositories` |
| `Shared.Extensions`   | `Shared.Application.Extensions`      |
| `Shared.Filters`      | `Shared.Application.Filters`         |
| `Shared.Middleware`   | `Shared.Application.Middleware`      |
| `Shared.Options`      | `Shared.Application.Options`         |
| `Shared.Validators`   | `Shared.Application.Validators`      |
| `Shared.Attributes`   | `Shared.Application.Attributes`      |
| `Shared.ModelBinders` | `Shared.Application.ModelBinders`    |
| `Shared.Transformers` | `Shared.Application.Transformers`    |

### Files to Update

**All files in Shared library** need namespace updates:

- `Core/Common/*.cs` - Update namespace
- `Core/Models/*.cs` - Update namespace
- `Infrastructure/Services/*.cs` - Update namespace
- `Infrastructure/Repositories/*.cs` - Update namespace
- `Application/Extensions/*.cs` - Update namespace
- `Application/Filters/*.cs` - Update namespace
- `Application/Middleware/*.cs` - Update namespace
- `Application/Options/*.cs` - Update namespace
- `Application/Validators/*.cs` - Update namespace
- `Application/Attributes/*.cs` - Update namespace
- `Application/ModelBinders/*.cs` - Update namespace
- `Application/Transformers/*.cs` - Update namespace

**All files that use Shared library** need using statement updates:

- All services
- All controllers
- All Program.cs files

---

## Quick Fix Commands

### Find and Replace Pattern

You can use find-and-replace in your IDE:

1. **UserAccountService:**

   - Find: `using UserAccountService.Data;`
   - Replace: `using UserAccountService.Infrastructure.Data;`

2. **Shared:**

   - Find: `using Shared.Common;`
   - Replace: `using Shared.Core.Common;`

3. **Namespace declarations:**
   - Find: `namespace UserAccountService.Data;`
   - Replace: `namespace UserAccountService.Infrastructure.Data;`

---

## Verification

After updating namespaces:

1. Build the solution: `dotnet build`
2. Check for compilation errors
3. Fix any remaining namespace issues
4. Run tests: `dotnet test`

---

## Important Notes

- **Breaking Changes:** This is a breaking change - all projects need to be updated
- **Build Order:** Update Shared library first, then services
- **Test After Each Service:** Update one service at a time and test
- **IDE Help:** Use IDE refactoring tools to update namespaces automatically


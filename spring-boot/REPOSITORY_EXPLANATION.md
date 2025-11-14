# TodoRepository Explanation

Complete explanation of the `TodoRepository` interface and how Spring Data MongoDB works.

## 📋 Overview

The `TodoRepository` is a **Spring Data MongoDB Repository** interface that provides database operations for the `Todo` entity. The key feature is that **you only define the interface** - Spring automatically creates the implementation at runtime!

## 🔍 Code Breakdown

```java
@Repository
public interface TodoRepository extends MongoRepository<Todo, String> {

    List<Todo> findByCompleted(Boolean completed);

    List<Todo> findByTitleContainingIgnoreCase(String title);

    List<Todo> findByCompletedAndTitleContainingIgnoreCase(Boolean completed, String title);
}
```

### Line-by-Line Explanation

#### 1. `@Repository` Annotation

```java
@Repository
```

- **Purpose:** Marks this interface as a Spring repository component
- **Effect:** Spring automatically detects and registers it as a bean
- **Benefit:** Can be injected into other classes (like `TodoService`)

#### 2. Interface Declaration

```java
public interface TodoRepository extends MongoRepository<Todo, String>
```

**What this means:**

- `TodoRepository` is an **interface** (not a class!)
- It **extends** `MongoRepository<Todo, String>`
  - `Todo` = The entity type (what we're storing)
  - `String` = The ID type (MongoDB uses String/ObjectId)

**What you get for FREE from `MongoRepository`:**

- `save(Todo)` - Save or update a todo
- `findById(String)` - Find by ID
- `findAll()` - Get all todos
- `deleteById(String)` - Delete by ID
- `deleteAll()` - Delete all todos
- `existsById(String)` - Check if exists
- `count()` - Count all todos
- And many more!

#### 3. Custom Query Methods

These are methods **you define**, and Spring automatically implements them based on the method name!

##### Method 1: `findByCompleted`

```java
List<Todo> findByCompleted(Boolean completed);
```

**How it works:**

- Spring reads the method name: `findByCompleted`
- Breaks it down:
  - `findBy` = Query operation
  - `Completed` = Field name in `Todo` entity
- **Automatically generates MongoDB query:**
  ```javascript
  db.todos.find({ completed: true }); // or false
  ```

**Usage Example:**

```java
List<Todo> completedTodos = todoRepository.findByCompleted(true);
List<Todo> incompleteTodos = todoRepository.findByCompleted(false);
```

##### Method 2: `findByTitleContainingIgnoreCase`

```java
List<Todo> findByTitleContainingIgnoreCase(String title);
```

**How it works:**

- Method name breakdown:
  - `findBy` = Query operation
  - `Title` = Field name
  - `Containing` = MongoDB `$regex` with pattern matching
  - `IgnoreCase` = Case-insensitive search
- **Automatically generates MongoDB query:**
  ```javascript
  db.todos.find({
    title: {
      $regex: "searchTerm",
      $options: "i", // 'i' = case-insensitive
    },
  });
  ```

**Usage Example:**

```java
List<Todo> results = todoRepository.findByTitleContainingIgnoreCase("spring");
// Finds todos with titles containing "spring", "Spring", "SPRING", etc.
```

##### Method 3: `findByCompletedAndTitleContainingIgnoreCase`

```java
List<Todo> findByCompletedAndTitleContainingIgnoreCase(Boolean completed, String title);
```

**How it works:**

- Method name breakdown:
  - `findBy` = Query operation
  - `Completed` = First field
  - `And` = Logical AND operator
  - `TitleContainingIgnoreCase` = Second field with pattern matching
- **Automatically generates MongoDB query:**
  ```javascript
  db.todos.find({
    completed: true,
    title: {
      $regex: "searchTerm",
      $options: "i",
    },
  });
  ```

**Usage Example:**

```java
// Find all completed todos with "project" in the title
List<Todo> results = todoRepository.findByCompletedAndTitleContainingIgnoreCase(
    true,
    "project"
);
```

## 🎯 Spring Data Query Derivation

Spring Data MongoDB uses **Query Derivation** - it creates queries from method names!

### Query Keywords

| Keyword        | MongoDB Operation | Example                      |
| -------------- | ----------------- | ---------------------------- |
| `findBy`       | Query             | `findByTitle`                |
| `And`          | `$and`            | `findByCompletedAndTitle`    |
| `Or`           | `$or`             | `findByCompletedOrTitle`     |
| `Containing`   | `$regex`          | `findByTitleContaining`      |
| `IgnoreCase`   | Case-insensitive  | `findByTitleIgnoreCase`      |
| `Like`         | Pattern match     | `findByTitleLike`            |
| `StartingWith` | Starts with       | `findByTitleStartingWith`    |
| `EndingWith`   | Ends with         | `findByTitleEndingWith`      |
| `Between`      | Range             | `findByCreatedAtBetween`     |
| `GreaterThan`  | `$gt`             | `findByCreatedAtGreaterThan` |
| `LessThan`     | `$lt`             | `findByCreatedAtLessThan`    |
| `IsNull`       | `null` check      | `findByDescriptionIsNull`    |
| `IsNotNull`    | Not null          | `findByDescriptionIsNotNull` |

### More Examples

```java
// Find todos created after a date
List<Todo> findByCreatedAtAfter(LocalDateTime date);

// Find todos with description not null
List<Todo> findByDescriptionIsNotNull();

// Find todos by title starting with
List<Todo> findByTitleStartingWith(String prefix);

// Find todos created between two dates
List<Todo> findByCreatedAtBetween(LocalDateTime start, LocalDateTime end);

// Find todos ordered by creation date
List<Todo> findByCompletedOrderByCreatedAtDesc(Boolean completed);
```

## 🔄 How It Works Behind the Scenes

### 1. **At Application Startup:**

```
Spring Boot scans for repositories
    ↓
Finds TodoRepository interface
    ↓
Creates a Proxy implementation
    ↓
Registers it as a Spring Bean
```

### 2. **When You Call a Method:**

```
todoRepository.findByCompleted(true)
    ↓
Spring intercepts the call
    ↓
Analyzes method name
    ↓
Generates MongoDB query
    ↓
Executes query against MongoDB
    ↓
Maps results to Todo objects
    ↓
Returns List<Todo>
```

### 3. **The Magic Happens Here:**

- Spring uses **Java Reflection** to analyze method names
- Uses **MongoDB Java Driver** to execute queries
- Uses **Jackson** to serialize/deserialize JSON ↔ Java objects

## 💡 Why This is Powerful

### ✅ **No Implementation Code Needed**

```java
// You write this:
List<Todo> findByCompleted(Boolean completed);

// Spring automatically creates this implementation:
@Override
public List<Todo> findByCompleted(Boolean completed) {
    Query query = new Query();
    query.addCriteria(Criteria.where("completed").is(completed));
    return mongoTemplate.find(query, Todo.class);
}
```

### ✅ **Type-Safe**

- Compile-time checking
- IDE autocomplete
- Refactoring support

### ✅ **Automatic Query Optimization**

- Spring optimizes queries
- Uses indexes when available
- Efficient MongoDB operations

## 🔗 How It's Used in TodoService

```java
@Service
public class TodoService {
    private final TodoRepository todoRepository;

    // Injected automatically by Spring
    public TodoService(TodoRepository todoRepository) {
        this.todoRepository = todoRepository;
    }

    public List<Todo> getAllTodos(Boolean completed, String title) {
        // Uses repository methods
        if (completed != null && title != null) {
            return todoRepository.findByCompletedAndTitleContainingIgnoreCase(
                completed, title
            );
        }
        // ... more logic
    }
}
```

## 📊 MongoDB Collection Mapping

The `Todo` entity is mapped to MongoDB collection:

```java
@Document(collection = "todos")
public class Todo {
    @Id
    private String id;  // Maps to MongoDB _id field
    // ... other fields
}
```

**MongoDB Document Structure:**

```json
{
  "_id": "507f1f77bcf86cd799439011",
  "title": "Complete Spring Boot project",
  "description": "Finish the todo microservice",
  "completed": false,
  "createdAt": "2025-11-06T12:00:00",
  "updatedAt": "2025-11-06T12:00:00"
}
```

## 🎓 Key Concepts

### 1. **Repository Pattern**

- Abstraction layer between business logic and data access
- Makes code testable (can mock repositories)
- Separates concerns

### 2. **Dependency Injection**

- Spring automatically creates and injects repository
- No manual instantiation needed
- Lifecycle managed by Spring

### 3. **Convention over Configuration**

- Method names follow conventions
- Spring understands the conventions
- Less code, more functionality

## 🚀 Advanced Usage

### Custom Queries with `@Query`

If you need complex queries, you can use `@Query` annotation:

```java
@Query("{ 'title': { $regex: ?0, $options: 'i' }, 'completed': ?1 }")
List<Todo> findCustomQuery(String title, Boolean completed);
```

### Sorting and Pagination

```java
// With sorting
List<Todo> findByCompletedOrderByCreatedAtDesc(Boolean completed);

// With pagination
Page<Todo> findByCompleted(Boolean completed, Pageable pageable);
```

## 📝 Summary

1. **`TodoRepository`** is an interface, not a class
2. **Spring automatically implements** all methods at runtime
3. **Method names define queries** - Spring derives MongoDB queries from names
4. **No SQL/MongoDB code needed** - Spring handles it
5. **Type-safe and IDE-friendly** - Full autocomplete support
6. **Extends `MongoRepository`** - Gets free CRUD operations
7. **Custom methods** - Automatically implemented based on naming conventions

## 🎯 Real-World Example

When you call:

```java
todoRepository.findByCompletedAndTitleContainingIgnoreCase(true, "project");
```

Spring internally executes:

```javascript
// MongoDB query
db.todos.find({
  completed: true,
  title: { $regex: "project", $options: "i" },
});
```

And returns a `List<Todo>` with matching results!

---

**The beauty of Spring Data MongoDB:** Write less code, get more functionality! 🎉









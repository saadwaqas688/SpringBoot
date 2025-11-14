# MongoTemplate Aggregation - Detailed Explanation with Examples

Complete explanation of how `MongoTemplate` works for complex MongoDB aggregation pipelines.

## 📋 Overview

`MongoTemplate` is Spring Data MongoDB's core class for executing MongoDB operations. For complex aggregations, it provides full control over the aggregation pipeline.

## 🔍 Code Breakdown

```java
@Autowired
private MongoTemplate mongoTemplate;

public List<Result> getComplexAnalytics(Criteria criteria) {
    List<AggregationOperation> operations = new ArrayList<>();
    // Build pipeline dynamically
    operations.add(Aggregation.match(criteria));
    operations.add(Aggregation.group(...));
    // ...
    Aggregation aggregation = Aggregation.newAggregation(operations);
    return mongoTemplate.aggregate(aggregation, "todos", Result.class)
        .getMappedResults();
}
```

### Line-by-Line Explanation

#### 1. `@Autowired private MongoTemplate mongoTemplate;`

**What it is:**

- `MongoTemplate` is Spring's main class for MongoDB operations
- It's automatically created by Spring Boot when MongoDB is configured
- `@Autowired` tells Spring to inject it automatically

**What it does:**

- Provides methods to execute MongoDB queries
- Handles connection to MongoDB
- Maps MongoDB documents to Java objects
- Executes aggregation pipelines

**Example:**

```java
@Service
public class TodoAnalyticsService {

    @Autowired
    private MongoTemplate mongoTemplate;  // Spring injects this automatically

    // Now you can use mongoTemplate to execute queries
}
```

#### 2. `List<AggregationOperation> operations = new ArrayList<>();`

**What it is:**

- A list that holds aggregation pipeline stages
- Each stage is an `AggregationOperation`
- Stages execute in order (like a factory assembly line)

**What it does:**

- Builds the aggregation pipeline step by step
- Each `operations.add()` adds a new stage to the pipeline

**MongoDB Pipeline Concept:**

```
Input Documents
    ↓
Stage 1: Match (filter)
    ↓
Stage 2: Group (aggregate)
    ↓
Stage 3: Project (reshape)
    ↓
Stage 4: Sort (order)
    ↓
Output Documents
```

**Example:**

```java
List<AggregationOperation> operations = new ArrayList<>();
// Empty list - no stages yet

operations.add(Aggregation.match(...));  // Stage 1: Filter
operations.add(Aggregation.group(...));  // Stage 2: Group
operations.add(Aggregation.sort(...));   // Stage 3: Sort
// Now we have 3 stages in the pipeline
```

#### 3. `operations.add(Aggregation.match(criteria));`

**What it is:**

- Adds a **$match** stage to the pipeline
- Filters documents (like SQL WHERE clause)
- Should be early in pipeline for performance

**What it does:**

- Only documents matching the criteria pass to next stage
- Reduces data processed in later stages

**Example 1: Simple Match**

```java
// Match todos that are completed
Criteria criteria = Criteria.where("completed").is(true);
operations.add(Aggregation.match(criteria));

// MongoDB equivalent:
// { $match: { completed: true } }
```

**Example 2: Complex Match**

```java
// Match todos created in date range AND completed
Criteria criteria = new Criteria()
    .and("completed").is(true)
    .and("createdAt")
        .gte(LocalDateTime.of(2025, 1, 1, 0, 0))
        .lte(LocalDateTime.of(2025, 12, 31, 23, 59));

operations.add(Aggregation.match(criteria));

// MongoDB equivalent:
// { $match: {
//     completed: true,
//     createdAt: { $gte: ISODate("2025-01-01"), $lte: ISODate("2025-12-31") }
// } }
```

**Example 3: Dynamic Match (from your code)**

```java
Criteria matchCriteria = new Criteria();

// Build criteria dynamically based on input
if (startDate != null && endDate != null) {
    matchCriteria.and("createdAt")
        .gte(startDate)
        .lte(endDate);
}

if (completed != null) {
    matchCriteria.and("completed").is(completed);
}

// Only add match if criteria exists
if (matchCriteria.getCriteriaObject().size() > 0) {
    operations.add(Aggregation.match(matchCriteria));
}
```

#### 4. `operations.add(Aggregation.group(...));`

**What it is:**

- Adds a **$group** stage to the pipeline
- Groups documents by specified fields
- Performs aggregations (count, sum, avg, etc.)

**What it does:**

- Groups documents with same values together
- Calculates statistics for each group
- Like SQL GROUP BY

**Example 1: Simple Group**

```java
// Group by completion status and count
operations.add(Aggregation.group("completed")
    .count().as("count"));

// MongoDB equivalent:
// { $group: {
//     _id: "$completed",
//     count: { $sum: 1 }
// } }
```

**Example 2: Group with Multiple Aggregations**

```java
// Group by completion status with multiple stats
operations.add(Aggregation.group("completed")
    .count().as("total")
    .min("createdAt").as("oldest")
    .max("createdAt").as("newest")
    .avg("titleLength").as("avgTitleLength"));

// MongoDB equivalent:
// { $group: {
//     _id: "$completed",
//     total: { $sum: 1 },
//     oldest: { $min: "$createdAt" },
//     newest: { $max: "$createdAt" },
//     avgTitleLength: { $avg: "$titleLength" }
// } }
```

**Example 3: Group by Multiple Fields**

```java
// Group by year and month
operations.add(Aggregation.group(
    Aggregation.fields()
        .and("year", DateOperators.year("createdAt"))
        .and("month", DateOperators.month("createdAt")))
    .count().as("count"));

// MongoDB equivalent:
// { $group: {
//     _id: {
//         year: { $year: "$createdAt" },
//         month: { $month: "$createdAt" }
//     },
//     count: { $sum: 1 }
// } }
```

#### 5. `Aggregation aggregation = Aggregation.newAggregation(operations);`

**What it is:**

- Creates an `Aggregation` object from the list of operations
- Combines all stages into a single pipeline
- Ready to execute

**What it does:**

- Takes all stages and creates executable pipeline
- Validates the pipeline structure
- Prepares for execution

**Example:**

```java
List<AggregationOperation> operations = new ArrayList<>();
operations.add(Aggregation.match(criteria));      // Stage 1
operations.add(Aggregation.group("completed"));    // Stage 2
operations.add(Aggregation.sort(...));             // Stage 3

// Combine all stages into one aggregation
Aggregation aggregation = Aggregation.newAggregation(operations);

// Now 'aggregation' contains the complete pipeline:
// [
//   { $match: { ... } },
//   { $group: { ... } },
//   { $sort: { ... } }
// ]
```

#### 6. `mongoTemplate.aggregate(aggregation, "todos", Result.class)`

**What it is:**

- Executes the aggregation pipeline
- `aggregation` - the pipeline to execute
- `"todos"` - collection name in MongoDB
- `Result.class` - Java class to map results to

**What it does:**

1. Sends pipeline to MongoDB
2. MongoDB executes the pipeline
3. Returns results
4. Maps MongoDB documents to Java objects

**Example:**

```java
// Execute aggregation on "todos" collection
// Map results to CompletionStatistics objects
AggregationResults<CompletionStatistics> results =
    mongoTemplate.aggregate(
        aggregation,
        "todos",                    // Collection name
        CompletionStatistics.class   // Result type
    );

// Results contain:
// - Mapped Java objects
// - Raw MongoDB documents
// - Execution statistics
```

#### 7. `.getMappedResults()`

**What it is:**

- Extracts the mapped Java objects from results
- Converts MongoDB documents to Java objects

**What it does:**

- Returns `List<Result>` with Java objects
- Each MongoDB document becomes a Java object
- Fields are automatically mapped

**Example:**

```java
// Get list of Java objects
List<CompletionStatistics> stats =
    mongoTemplate.aggregate(aggregation, "todos", CompletionStatistics.class)
        .getMappedResults();

// stats now contains:
// [
//   CompletionStatistics{completed=true, count=5, ...},
//   CompletionStatistics{completed=false, count=3, ...}
// ]
```

## 🎯 Complete Real-World Example

### Example: Get Todo Statistics by Completion Status

```java
@Service
public class TodoStatisticsService {

    @Autowired
    private MongoTemplate mongoTemplate;

    public List<CompletionStatistics> getCompletionStatistics() {
        // Step 1: Create empty pipeline
        List<AggregationOperation> operations = new ArrayList<>();

        // Step 2: Add match stage (optional - filter if needed)
        Criteria matchCriteria = Criteria.where("createdAt")
            .gte(LocalDateTime.now().minusMonths(6));  // Last 6 months
        operations.add(Aggregation.match(matchCriteria));

        // Step 3: Add group stage
        operations.add(Aggregation.group("completed")
            .count().as("count")
            .min("createdAt").as("oldest")
            .max("createdAt").as("newest"));

        // Step 4: Add project stage (reshape output)
        operations.add(Aggregation.project()
            .and("_id").as("completed")      // Rename _id to completed
            .and("count").as("count")
            .and("oldest").as("oldest")
            .and("newest").as("newest")
            .andExclude("_id"));             // Remove _id field

        // Step 5: Add sort stage
        operations.add(Aggregation.sort(Sort.Direction.ASC, "completed"));

        // Step 6: Create aggregation
        Aggregation aggregation = Aggregation.newAggregation(operations);

        // Step 7: Execute and get results
        return mongoTemplate.aggregate(
            aggregation,
            "todos",
            CompletionStatistics.class
        ).getMappedResults();
    }
}

// Result interface
interface CompletionStatistics {
    Boolean getCompleted();
    Long getCount();
    LocalDateTime getOldest();
    LocalDateTime getNewest();
}
```

### What Happens Step by Step

**Input Data (MongoDB):**

```json
[
  {
    "_id": "1",
    "title": "Task 1",
    "completed": true,
    "createdAt": "2025-01-01"
  },
  {
    "_id": "2",
    "title": "Task 2",
    "completed": true,
    "createdAt": "2025-01-15"
  },
  {
    "_id": "3",
    "title": "Task 3",
    "completed": false,
    "createdAt": "2025-02-01"
  },
  {
    "_id": "4",
    "title": "Task 4",
    "completed": false,
    "createdAt": "2025-02-10"
  }
]
```

**After Match Stage:**

```json
// All documents pass (no filtering in this example)
[
  {
    "_id": "1",
    "title": "Task 1",
    "completed": true,
    "createdAt": "2025-01-01"
  },
  {
    "_id": "2",
    "title": "Task 2",
    "completed": true,
    "createdAt": "2025-01-15"
  },
  {
    "_id": "3",
    "title": "Task 3",
    "completed": false,
    "createdAt": "2025-02-01"
  },
  {
    "_id": "4",
    "title": "Task 4",
    "completed": false,
    "createdAt": "2025-02-10"
  }
]
```

**After Group Stage:**

```json
[
  {
    "_id": true,
    "count": 2,
    "oldest": "2025-01-01",
    "newest": "2025-01-15"
  },
  {
    "_id": false,
    "count": 2,
    "oldest": "2025-02-01",
    "newest": "2025-02-10"
  }
]
```

**After Project Stage:**

```json
[
  {
    "completed": true,
    "count": 2,
    "oldest": "2025-01-01",
    "newest": "2025-01-15"
  },
  {
    "completed": false,
    "count": 2,
    "oldest": "2025-02-01",
    "newest": "2025-02-10"
  }
]
```

**After Sort Stage:**

```json
// Sorted by completed (false comes before true)
[
  {
    "completed": false,
    "count": 2,
    "oldest": "2025-02-01",
    "newest": "2025-02-10"
  },
  {
    "completed": true,
    "count": 2,
    "oldest": "2025-01-01",
    "newest": "2025-01-15"
  }
]
```

**Final Java Objects:**

```java
List<CompletionStatistics> results = [
    CompletionStatistics{
        completed = false,
        count = 2,
        oldest = 2025-02-01T00:00:00,
        newest = 2025-02-10T00:00:00
    },
    CompletionStatistics{
        completed = true,
        count = 2,
        oldest = 2025-01-01T00:00:00,
        newest = 2025-01-15T00:00:00
    }
]
```

## 🔄 Common Aggregation Stages

### 1. Match (Filter)

```java
// Filter documents
Criteria criteria = Criteria.where("completed").is(true)
    .and("createdAt").gte(startDate);
operations.add(Aggregation.match(criteria));
```

### 2. Group (Aggregate)

```java
// Group and calculate
operations.add(Aggregation.group("completed")
    .count().as("count")
    .sum("value").as("total")
    .avg("value").as("average"));
```

### 3. Project (Reshape)

```java
// Select and rename fields
operations.add(Aggregation.project()
    .and("_id").as("id")
    .and("title").as("name")
    .andExclude("description"));
```

### 4. Sort (Order)

```java
// Sort results
operations.add(Aggregation.sort(Sort.Direction.DESC, "count"));
```

### 5. Limit (Restrict)

```java
// Limit number of results
operations.add(Aggregation.limit(10));
```

### 6. Skip (Offset)

```java
// Skip first N results
operations.add(Aggregation.skip(5));
```

### 7. Unwind (Array Expansion)

```java
// Expand array fields
operations.add(Aggregation.unwind("tags"));
```

### 8. AddFields (Computed Fields)

```java
// Add calculated fields
operations.add(Aggregation.addFields()
    .addField("titleLength")
    .withValue(StringOperators.valueOf("title").length())
    .build());
```

## 💡 Why Use MongoTemplate?

### ✅ **Full Control**

- Build pipelines dynamically
- Add/remove stages based on conditions
- Complex business logic

### ✅ **Type Safety**

- Compile-time checking
- IDE autocomplete
- Refactoring support

### ✅ **Flexibility**

- Can't express in query derivation? Use MongoTemplate
- Need dynamic queries? Use MongoTemplate
- Complex aggregations? Use MongoTemplate

### ✅ **Performance**

- Optimize pipeline stages
- Control execution order
- Add indexes strategically

## 🎓 Key Concepts

### 1. **Pipeline Stages Execute in Order**

```java
operations.add(Aggregation.match(...));   // Stage 1: Filter
operations.add(Aggregation.group(...));   // Stage 2: Group (works on filtered data)
operations.add(Aggregation.sort(...));    // Stage 3: Sort (works on grouped data)
```

### 2. **Each Stage Transforms Data**

- Input: Documents from previous stage
- Process: Apply stage operation
- Output: Transformed documents to next stage

### 3. **Result Mapping**

- MongoDB returns JSON documents
- Spring maps to Java objects automatically
- Field names must match (or use `@Field` annotation)

## 📝 Summary

1. **`MongoTemplate`** - Spring's MongoDB operations class
2. **`AggregationOperation`** - Represents one pipeline stage
3. **`List<AggregationOperation>`** - Build pipeline step by step
4. **`Aggregation.newAggregation()`** - Combine stages into pipeline
5. **`mongoTemplate.aggregate()`** - Execute pipeline
6. **`.getMappedResults()`** - Get Java objects

**The Flow:**

```
Create operations list
    ↓
Add stages (match, group, project, sort, etc.)
    ↓
Create Aggregation object
    ↓
Execute with MongoTemplate
    ↓
Get mapped Java objects
```

---

**MongoTemplate gives you the power to build any aggregation pipeline you need!** 🚀









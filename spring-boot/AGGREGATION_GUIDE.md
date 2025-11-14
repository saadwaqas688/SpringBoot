# Complex Aggregation Pipelines in Spring Data MongoDB

Guide for handling complex MongoDB aggregation pipelines in Spring Boot applications.

## 📋 Overview

While Spring Data MongoDB's query derivation is great for simple queries, **complex aggregation pipelines** require a different approach. This guide covers multiple methods to handle complex aggregations.

## 🎯 When to Use Aggregation Pipelines

Use aggregation pipelines when you need:

- **Grouping** data (GROUP BY equivalent)
- **Joining** collections (lookup operations)
- **Calculations** (sum, average, count)
- **Transformations** (reshape documents)
- **Filtering** at multiple stages
- **Sorting** and **pagination** on aggregated results
- **Complex business logic** that can't be expressed in simple queries

## 🔧 Method 1: Using `@Aggregation` Annotation

The simplest way to define aggregation pipelines in repositories.

### Example 1: Group Todos by Completion Status

```java
@Repository
public interface TodoRepository extends MongoRepository<Todo, String> {

    // Simple aggregation: Count todos by completion status
    @Aggregation(pipeline = {
        "{ $group: { _id: '$completed', count: { $sum: 1 } } }",
        "{ $sort: { _id: 1 } }"
    })
    List<CompletionStats> countTodosByCompletionStatus();

    // Result interface
    interface CompletionStats {
        Boolean get_id();  // completion status
        Integer getCount();  // count
    }
}
```

### Example 2: Get Todos with Statistics

```java
@Aggregation(pipeline = {
    "{ $match: { completed: ?0 } }",
    "{ $group: { " +
        "_id: null, " +
        "total: { $sum: 1 }, " +
        "avgTitleLength: { $avg: { $strLenCP: '$title' } }, " +
        "oldest: { $min: '$createdAt' }, " +
        "newest: { $max: '$createdAt' } " +
    "} }"
})
TodoStatistics getTodoStatistics(Boolean completed);

interface TodoStatistics {
    Integer getTotal();
    Double getAvgTitleLength();
    LocalDateTime getOldest();
    LocalDateTime getNewest();
}
```

### Example 3: Complex Aggregation with Multiple Stages

```java
@Aggregation(pipeline = {
    // Stage 1: Match completed todos
    "{ $match: { completed: true } }",

    // Stage 2: Add computed field (days since creation)
    "{ $addFields: { " +
        "daysSinceCreation: { " +
            "$divide: [ " +
                "{ $subtract: [ new Date(), '$createdAt' ] }, " +
                "86400000 " +  // milliseconds in a day
            "] " +
        "} " +
    "} }",

    // Stage 3: Group by month of creation
    "{ $group: { " +
        "_id: { " +
            "year: { $year: '$createdAt' }, " +
            "month: { $month: '$createdAt' } " +
        "}, " +
        "count: { $sum: 1 }, " +
        "avgDays: { $avg: '$daysSinceCreation' }, " +
        "todos: { $push: { " +
            "id: '$_id', " +
            "title: '$title', " +
            "daysSinceCreation: '$daysSinceCreation' " +
        "} } " +
    "} }",

    // Stage 4: Sort by year and month
    "{ $sort: { '_id.year': 1, '_id.month': 1 } }",

    // Stage 5: Project final shape
    "{ $project: { " +
        "_id: 0, " +
        "year: '$_id.year', " +
        "month: '$_id.month', " +
        "count: 1, " +
        "avgDays: { $round: ['$avgDays', 2] }, " +
        "todos: 1 " +
    "} }"
})
List<MonthlyTodoStats> getMonthlyCompletedTodoStatistics();

interface MonthlyTodoStats {
    Integer getYear();
    Integer getMonth();
    Integer getCount();
    Double getAvgDays();
    List<TodoSummary> getTodos();

    interface TodoSummary {
        String getId();
        String getTitle();
        Double getDaysSinceCreation();
    }
}
```

## 🔧 Method 2: Using `MongoTemplate` (Most Flexible)

For very complex aggregations or dynamic pipelines, use `MongoTemplate` directly.

### Example: Complex Aggregation Service

```java
@Service
public class TodoAnalyticsService {

    @Autowired
    private MongoTemplate mongoTemplate;

    /**
     * Get todos with complex filtering and grouping
     */
    public List<TodoAnalytics> getTodoAnalytics(TodoAnalyticsRequest request) {
        // Build aggregation pipeline dynamically
        List<AggregationOperation> operations = new ArrayList<>();

        // Stage 1: Match stage (filtering)
        Criteria matchCriteria = new Criteria();

        if (request.getCompleted() != null) {
            matchCriteria.and("completed").is(request.getCompleted());
        }

        if (request.getStartDate() != null && request.getEndDate() != null) {
            matchCriteria.and("createdAt")
                .gte(request.getStartDate())
                .lte(request.getEndDate());
        }

        if (request.getTitleKeyword() != null) {
            matchCriteria.and("title")
                .regex(request.getTitleKeyword(), "i");
        }

        operations.add(Aggregation.match(matchCriteria));

        // Stage 2: Add computed fields
        operations.add(Aggregation.addFields()
            .addField("titleLength")
            .withValue(StringOperators.valueOf("title").length())
            .build());

        operations.add(Aggregation.addFields()
            .addField("daysSinceCreation")
            .withValue(ArithmeticOperators.Subtract
                .valueOf(ArithmeticOperators.Subtract
                    .valueOf(new Date())
                    .subtract("$createdAt"))
                .divideBy(86400000))  // milliseconds to days
            .build());

        // Stage 3: Group by completion status
        operations.add(Aggregation.group("completed")
            .count().as("total")
            .avg("titleLength").as("avgTitleLength")
            .min("createdAt").as("oldest")
            .max("createdAt").as("newest")
            .avg("daysSinceCreation").as("avgDaysSinceCreation")
            .push("$$ROOT").as("todos"));

        // Stage 4: Sort
        operations.add(Aggregation.sort(Sort.Direction.ASC, "_id"));

        // Stage 5: Project final shape
        operations.add(Aggregation.project()
            .and("_id").as("completed")
            .and("total").as("total")
            .and("avgTitleLength").as("avgTitleLength")
            .and("oldest").as("oldest")
            .and("newest").as("newest")
            .and("avgDaysSinceCreation").as("avgDaysSinceCreation")
            .and("todos").slice(10).as("sampleTodos")  // Limit to 10 todos
            .andExclude("_id"));

        // Execute aggregation
        Aggregation aggregation = Aggregation.newAggregation(operations);

        return mongoTemplate.aggregate(
            aggregation,
            "todos",  // collection name
            TodoAnalytics.class
        ).getMappedResults();
    }
}

// Result class
public class TodoAnalytics {
    private Boolean completed;
    private Integer total;
    private Double avgTitleLength;
    private LocalDateTime oldest;
    private LocalDateTime newest;
    private Double avgDaysSinceCreation;
    private List<Todo> sampleTodos;

    // Getters and setters
}
```

## 🔧 Method 3: Custom Repository Implementation

For reusable complex aggregations, create a custom repository implementation.

### Step 1: Define Custom Interface

```java
public interface TodoRepositoryCustom {
    List<TodoAnalytics> getComplexAnalytics(TodoAnalyticsRequest request);
    List<MonthlyStats> getMonthlyStatistics(int year);
    List<Todo> findTodosWithComplexCriteria(ComplexSearchCriteria criteria);
}
```

### Step 2: Extend Main Repository

```java
@Repository
public interface TodoRepository extends MongoRepository<Todo, String>, TodoRepositoryCustom {
    // ... existing methods
}
```

### Step 3: Implement Custom Repository

```java
@Repository
public class TodoRepositoryImpl implements TodoRepositoryCustom {

    @Autowired
    private MongoTemplate mongoTemplate;

    @Override
    public List<TodoAnalytics> getComplexAnalytics(TodoAnalyticsRequest request) {
        // Complex aggregation logic here
        List<AggregationOperation> pipeline = buildAggregationPipeline(request);
        Aggregation aggregation = Aggregation.newAggregation(pipeline);

        return mongoTemplate.aggregate(
            aggregation,
            "todos",
            TodoAnalytics.class
        ).getMappedResults();
    }

    private List<AggregationOperation> buildAggregationPipeline(TodoAnalyticsRequest request) {
        List<AggregationOperation> operations = new ArrayList<>();

        // Build pipeline dynamically based on request
        // ... complex logic

        return operations;
    }
}
```

## 📊 Real-World Complex Aggregation Examples

### Example 1: Todos with Completion Trends

```java
@Aggregation(pipeline = {
    // Group by completion status and date
    "{ $group: { " +
        "_id: { " +
            "completed: '$completed', " +
            "date: { $dateToString: { format: '%Y-%m-%d', date: '$createdAt' } } " +
        "}, " +
        "count: { $sum: 1 }, " +
        "todos: { $push: { id: '$_id', title: '$title' } } " +
    "} }",

    // Sort by date
    "{ $sort: { '_id.date': 1 } }",

    // Group by completion status
    "{ $group: { " +
        "_id: '$_id.completed', " +
        "dailyStats: { " +
            "$push: { " +
                "date: '$_id.date', " +
                "count: '$count', " +
                "todos: '$todos' " +
            "} " +
        "}, " +
        "totalCount: { $sum: '$count' } " +
    "} }",

    // Final projection
    "{ $project: { " +
        "_id: 0, " +
        "completed: '$_id', " +
        "totalCount: 1, " +
        "dailyStats: 1 " +
    "} }"
})
List<CompletionTrend> getCompletionTrends();
```

### Example 2: Search with Relevance Scoring

```java
public List<TodoSearchResult> searchTodosWithRelevance(String searchTerm) {
    List<AggregationOperation> operations = new ArrayList<>();

    // Stage 1: Match todos containing search term
    operations.add(Aggregation.match(
        Criteria.where("title").regex(searchTerm, "i")
            .orOperator(
                Criteria.where("description").regex(searchTerm, "i")
            )
    ));

    // Stage 2: Add relevance score
    operations.add(Aggregation.addFields()
        .addField("relevanceScore")
        .withValue(
            ConditionalOperators.ifNull(
                ConditionalOperators.when(
                    ComparisonOperators.valueOf("title")
                        .regex(Pattern.compile(searchTerm, Pattern.CASE_INSENSITIVE))
                ).then(10)
                .otherwise(5)
            ).ifNull(0)
        )
        .build());

    // Stage 3: Sort by relevance
    operations.add(Aggregation.sort(Sort.Direction.DESC, "relevanceScore"));

    // Stage 4: Limit results
    operations.add(Aggregation.limit(20));

    Aggregation aggregation = Aggregation.newAggregation(operations);

    return mongoTemplate.aggregate(
        aggregation,
        "todos",
        TodoSearchResult.class
    ).getMappedResults();
}
```

### Example 3: Time-Based Analytics

```java
@Aggregation(pipeline = {
    // Match todos in date range
    "{ $match: { " +
        "createdAt: { " +
            "$gte: ?0, " +
            "$lte: ?1 " +
        "} " +
    "} }",

    // Add time-based fields
    "{ $addFields: { " +
        "year: { $year: '$createdAt' }, " +
        "month: { $month: '$createdAt' }, " +
        "dayOfWeek: { $dayOfWeek: '$createdAt' }, " +
        "hour: { $hour: '$createdAt' } " +
    "} }",

    // Group by multiple dimensions
    "{ $group: { " +
        "_id: { " +
            "year: '$year', " +
            "month: '$month', " +
            "dayOfWeek: '$dayOfWeek', " +
            "completed: '$completed' " +
        "}, " +
        "count: { $sum: 1 }, " +
        "avgCompletionTime: { " +
            "$avg: { " +
                "$subtract: [ '$updatedAt', '$createdAt' ] " +
            "} " +
        "} " +
    "} }",

    // Sort
    "{ $sort: { " +
        "'_id.year': 1, " +
        "'_id.month': 1, " +
        "'_id.dayOfWeek': 1 " +
    "} }"
})
List<TimeBasedAnalytics> getTimeBasedAnalytics(
    LocalDateTime startDate,
    LocalDateTime endDate
);
```

## 🎯 Best Practices

### 1. **Use @Aggregation for Static Pipelines**

```java
// Good for fixed, reusable aggregations
@Aggregation(pipeline = { "..." })
List<Result> getFixedAnalytics();
```

### 2. **Use MongoTemplate for Dynamic Pipelines**

```java
// Good for user-driven, dynamic queries
public List<Result> getDynamicAnalytics(SearchCriteria criteria) {
    // Build pipeline based on criteria
}
```

### 3. **Use Custom Repository for Complex Reusable Logic**

```java
// Good for complex business logic that's reused
public interface TodoRepositoryCustom {
    List<Analytics> getComplexAnalytics();
}
```

### 4. **Optimize with Indexes**

```java
// Create indexes for frequently queried fields
@Document(collection = "todos")
@CompoundIndex(name = "completed_created_idx", def = "{'completed': 1, 'createdAt': 1}")
public class Todo {
    // ...
}
```

### 5. **Use Projection to Limit Data**

```java
// Only return needed fields
operations.add(Aggregation.project()
    .and("title").as("title")
    .and("completed").as("completed")
    .andExclude("_id", "description", "updatedAt"));
```

## 🔍 Performance Considerations

### 1. **Limit Results Early**

```java
// Add limit stage early in pipeline
operations.add(Aggregation.limit(100));
```

### 2. **Use Match Early**

```java
// Filter data as early as possible
operations.add(Aggregation.match(criteria));
```

### 3. **Create Appropriate Indexes**

```java
// Index fields used in match and sort stages
@CompoundIndex(name = "search_idx", def = "{'title': 'text', 'description': 'text'}")
```

### 4. **Use Facet for Multiple Aggregations**

```java
@Aggregation(pipeline = {
    "{ $facet: { " +
        "byStatus: [ " +
            "{ $group: { _id: '$completed', count: { $sum: 1 } } } " +
        "], " +
        "byMonth: [ " +
            "{ $group: { _id: { $month: '$createdAt' }, count: { $sum: 1 } } } " +
        "], " +
        "recent: [ " +
            "{ $sort: { createdAt: -1 } }, " +
            "{ $limit: 10 } " +
        "] " +
    "} }"
})
MultiFacetResult getMultiFacetAnalytics();
```

## 📝 Complete Example: Advanced Todo Analytics

```java
@Service
public class AdvancedTodoAnalyticsService {

    @Autowired
    private MongoTemplate mongoTemplate;

    public TodoDashboardAnalytics getDashboardAnalytics() {
        List<AggregationOperation> operations = new ArrayList<>();

        // Stage 1: Add computed fields
        operations.add(Aggregation.addFields()
            .addField("completionTime")
            .withValue(
                ConditionalOperators.ifNull(
                    ArithmeticOperators.Subtract
                        .valueOf("$updatedAt")
                        .subtract("$createdAt")
                ).ifNull(0)
            )
            .build());

        // Stage 2: Group by completion status
        operations.add(Aggregation.group("completed")
            .count().as("count")
            .avg("completionTime").as("avgCompletionTime")
            .sum("completionTime").as("totalCompletionTime")
            .push("$$ROOT").as("todos"));

        // Stage 3: Unwind and group for additional stats
        operations.add(Aggregation.unwind("todos"));
        operations.add(Aggregation.group("_id")
            .first("count").as("count")
            .first("avgCompletionTime").as("avgCompletionTime")
            .first("totalCompletionTime").as("totalCompletionTime")
            .avg(ArithmeticOperators.valueOf("todos.title").length()).as("avgTitleLength")
            .max("todos.createdAt").as("newest")
            .min("todos.createdAt").as("oldest"));

        // Stage 4: Project final shape
        operations.add(Aggregation.project()
            .and("_id").as("completed")
            .and("count").as("count")
            .and("avgCompletionTime").as("avgCompletionTime")
            .and("avgTitleLength").as("avgTitleLength")
            .and("newest").as("newest")
            .and("oldest").as("oldest")
            .andExclude("_id"));

        Aggregation aggregation = Aggregation.newAggregation(operations);

        return mongoTemplate.aggregate(
            aggregation,
            "todos",
            TodoDashboardAnalytics.class
        ).getUniqueMappedResult();
    }
}
```

## 🎓 Key Takeaways

1. **@Aggregation** - Best for static, reusable pipelines
2. **MongoTemplate** - Best for dynamic, complex pipelines
3. **Custom Repository** - Best for reusable complex business logic
4. **Always optimize** - Use indexes, limit early, match early
5. **Test thoroughly** - Complex aggregations can be tricky to debug

## 📚 Additional Resources

- [Spring Data MongoDB Aggregation](https://docs.spring.io/spring-data/mongodb/docs/current/reference/html/#mongodb-template-aggregation)
- [MongoDB Aggregation Pipeline](https://docs.mongodb.com/manual/core/aggregation-pipeline/)
- [Spring Data MongoDB Reference](https://docs.spring.io/spring-data/mongodb/docs/current/reference/html/)

---

**Remember:** For complex aggregations, `MongoTemplate` gives you the most flexibility and control! 🚀

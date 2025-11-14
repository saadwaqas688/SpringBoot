# Course Model - Java vs Node.js/NestJS Equivalents

This document shows how the Java `Course.java` model translates to Node.js and NestJS.

## 1. Plain Node.js with Express + Mongoose

### File: `src/models/course.js`

```javascript
const mongoose = require("mongoose");

// Define the schema (equivalent to @Document + fields)
const courseSchema = new mongoose.Schema(
  {
    // _id is automatically created by Mongoose (equivalent to @Id)

    name: {
      type: String,
      required: [true, "Course name is required"],
      minlength: [1, "Course name must be at least 1 character"],
      maxlength: [200, "Course name must not exceed 200 characters"],
      trim: true, // Removes whitespace (like @NotBlank)
    },

    description: {
      type: String,
      maxlength: [1000, "Description must not exceed 1000 characters"],
      default: null, // Optional field
    },

    createdAt: {
      type: Date,
      default: Date.now,
    },

    updatedAt: {
      type: Date,
      default: Date.now,
    },
  },
  {
    collection: "courses", // Equivalent to @Document(collection = "courses")
    timestamps: false, // We're managing timestamps manually
  }
);

// Middleware to update updatedAt before saving
courseSchema.pre("save", function (next) {
  this.updatedAt = new Date();
  next();
});

// Create and export the model
const Course = mongoose.model("Course", courseSchema);

module.exports = Course;
```

### Usage Example:

```javascript
// Create a course
const course = new Course({
  name: "Spring Boot Fundamentals",
  description: "Learn Spring Boot from scratch",
});

// Save to database
await course.save();

// Access properties directly (no getters/setters needed)
console.log(course.name);
course.name = "New Name";
```

---

## 2. NestJS with TypeScript + Mongoose

### File: `src/course/entities/course.entity.ts`

```typescript
import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import { ApiProperty } from "@nestjs/swagger";
import { Document } from "mongoose";
import { IsNotEmpty, IsOptional, Length, MaxLength } from "class-validator";

// Equivalent to @Document(collection = "courses")
@Schema({ collection: "courses", timestamps: false })
export class Course extends Document {
  // @Id equivalent - Mongoose automatically creates _id
  // You don't need to declare it, but if you want to reference it:
  // _id?: mongoose.Types.ObjectId;

  @ApiProperty({
    description: "Name of the course",
    example: "Spring Boot Fundamentals",
    required: true,
  })
  @IsNotEmpty({ message: "Course name is required" })
  @Length(1, 200, {
    message: "Course name must be between 1 and 200 characters",
  })
  @Prop({
    required: true,
    trim: true,
    minlength: 1,
    maxlength: 200,
  })
  name: string; // Equivalent to private String name with getters/setters

  @ApiProperty({
    description: "Course description",
    example: "Learn Spring Boot from scratch",
    required: false,
  })
  @IsOptional()
  @MaxLength(1000, {
    message: "Description must not exceed 1000 characters",
  })
  @Prop({
    maxlength: 1000,
  })
  description?: string; // Optional field (equivalent to nullable)

  @ApiProperty({
    description: "Timestamp when the course was created",
    example: "2025-11-06T12:00:00",
    readOnly: true,
  })
  @Prop({ default: Date.now })
  createdAt?: Date; // Equivalent to LocalDateTime

  @ApiProperty({
    description: "Timestamp when the course was last updated",
    example: "2025-11-06T12:30:00",
    readOnly: true,
  })
  @Prop({ default: Date.now })
  updatedAt?: Date; // Equivalent to LocalDateTime
}

// Create the schema factory
export const CourseSchema = SchemaFactory.createForClass(Course);

// Middleware to update updatedAt before saving
CourseSchema.pre("save", function (next) {
  this.updatedAt = new Date();
  next();
});
```

### Usage in NestJS Service:

```typescript
import { Injectable } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model } from "mongoose";
import { Course } from "./entities/course.entity";

@Injectable()
export class CourseService {
  constructor(
    @InjectModel(Course.name) // Dependency injection (like Spring's @Autowired)
    private courseModel: Model<Course>
  ) {}

  async create(name: string, description?: string): Promise<Course> {
    const course = new this.courseModel({
      name,
      description,
    });
    return await course.save();
  }

  async findAll(): Promise<Course[]> {
    return await this.courseModel.find().exec();
  }
}
```

---

## 3. Key Differences Summary

### Annotations vs Decorators

| Java (Spring Boot)                  | Node.js (Mongoose)                                  | NestJS (TypeScript)                                           |
| ----------------------------------- | --------------------------------------------------- | ------------------------------------------------------------- |
| `@Document(collection = "courses")` | `mongoose.Schema({...}, { collection: 'courses' })` | `@Schema({ collection: 'courses' })`                          |
| `@Id`                               | Automatically handled by Mongoose (`_id`)           | Automatically handled                                         |
| `@NotBlank`                         | `required: true` + `trim: true`                     | `@IsNotEmpty()` + `@Prop({ required: true, trim: true })`     |
| `@Size(min=1, max=200)`             | `minlength: 1, maxlength: 200`                      | `@Length(1, 200)` + `@Prop({ minlength: 1, maxlength: 200 })` |
| `@Schema` (Swagger)                 | JSDoc comments                                      | `@ApiProperty()`                                              |
| `private String name`               | `name: String` in schema                            | `name: string` (public by default)                            |

### Getters and Setters

**Java:**

```java
private String name;

public String getName() {
    return name;
}

public void setName(String name) {
    this.name = name;
}
```

**Node.js/NestJS:**

```typescript
// Direct property access (most common)
name: string;

// Usage:
course.name = 'Spring Boot';
console.log(course.name);

// Or with getters/setters if you need validation:
private _name: string;

get name(): string {
    return this._name;
}

set name(value: string) {
    if (!value || value.trim() === '') {
        throw new Error('Name required');
    }
    this._name = value;
}
```

### Date Handling

**Java:**

```java
private LocalDateTime createdAt;
```

**Node.js:**

```javascript
createdAt: {
  type: Date,
  default: Date.now
}
```

**NestJS:**

```typescript
@Prop({ default: Date.now })
createdAt?: Date;
```

### Constructor

**Java:**

```java
public Course(String name, String description) {
    this.name = name;
    this.description = description;
}
```

**Node.js:**

```javascript
// Option 1: Plain object
const course = { name: "Spring Boot", description: "Learn" };

// Option 2: Class constructor
class Course {
  constructor(name, description) {
    this.name = name;
    this.description = description;
  }
}
```

**NestJS:**

```typescript
constructor(name?: string, description?: string) {
  this.name = name;
  this.description = description;
}
```

---

## 4. Complete NestJS Module Example

### File: `src/course/course.module.ts`

```typescript
import { Module } from "@nestjs/common";
import { MongooseModule } from "@nestjs/mongoose";
import { CourseController } from "./course.controller";
import { CourseService } from "./course.service";
import { Course, CourseSchema } from "./entities/course.entity";

@Module({
  imports: [
    MongooseModule.forFeature([{ name: Course.name, schema: CourseSchema }]),
  ],
  controllers: [CourseController],
  providers: [CourseService],
  exports: [CourseService],
})
export class CourseModule {}
```

### File: `src/course/course.controller.ts`

```typescript
import { Controller, Get, Post, Body } from "@nestjs/common";
import { ApiTags, ApiOperation } from "@nestjs/swagger";
import { CourseService } from "./course.service";
import { CreateCourseDto } from "./dto/create-course.dto";

@ApiTags("Courses")
@Controller("courses")
export class CourseController {
  constructor(private readonly courseService: CourseService) {}

  @Post()
  @ApiOperation({ summary: "Create a new course" })
  async create(@Body() createCourseDto: CreateCourseDto) {
    return await this.courseService.create(
      createCourseDto.name,
      createCourseDto.description
    );
  }

  @Get()
  @ApiOperation({ summary: "Get all courses" })
  async findAll() {
    return await this.courseService.findAll();
  }
}
```

### File: `src/course/dto/create-course.dto.ts`

```typescript
import { ApiProperty } from "@nestjs/swagger";
import { IsNotEmpty, IsOptional, Length, MaxLength } from "class-validator";

export class CreateCourseDto {
  @ApiProperty({
    description: "Name of the course",
    example: "Spring Boot Fundamentals",
  })
  @IsNotEmpty({ message: "Course name is required" })
  @Length(1, 200, {
    message: "Course name must be between 1 and 200 characters",
  })
  name: string;

  @ApiProperty({
    description: "Course description",
    example: "Learn Spring Boot from scratch",
    required: false,
  })
  @IsOptional()
  @MaxLength(1000, {
    message: "Description must not exceed 1000 characters",
  })
  description?: string;
}
```

---

## Summary

- **Java Spring Boot**: Uses annotations (`@Document`, `@Id`, `@NotBlank`) and getters/setters
- **Plain Node.js**: Uses Mongoose schemas with validation options
- **NestJS**: Uses decorators (similar to annotations) and TypeScript types, most similar to Spring Boot structure

The main difference is that Java requires explicit getters/setters, while JavaScript/TypeScript allows direct property access, making the code more concise.






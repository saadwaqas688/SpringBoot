# Course Management Frontend

React frontend application for the Course Management System.

## Features

- ✅ Authentication (Login, Signup, Admin Signup)
- ✅ Courses Management (CRUD operations)
- ✅ Discussions Management
- ✅ Posts with nested Comments and Replies
- ✅ Enrollments Management (Admin only)
- ✅ JWT Token Authentication
- ✅ Protected Routes
- ✅ Modern UI with Tailwind CSS

## Tech Stack

- **React 18** - UI library
- **React Router** - Routing
- **Axios** - HTTP client
- **Tailwind CSS** - Styling
- **React Toastify** - Notifications
- **Vite** - Build tool

## Installation

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
```

## Project Structure

```
frontend/
├── src/
│   ├── components/       # Reusable components
│   │   ├── Layout.jsx
│   │   └── ProtectedRoute.jsx
│   ├── context/          # React Context
│   │   └── AuthContext.jsx
│   ├── pages/            # Page components
│   │   ├── Home.jsx
│   │   ├── Login.jsx
│   │   ├── Signup.jsx
│   │   ├── SignupAdmin.jsx
│   │   ├── Courses.jsx
│   │   ├── Discussions.jsx
│   │   ├── Posts.jsx
│   │   └── Enrollments.jsx
│   ├── services/         # API services
│   │   └── api.js
│   ├── config/           # Configuration
│   │   └── api.js
│   ├── App.jsx           # Main app component
│   ├── main.jsx          # Entry point
│   └── index.css         # Global styles
├── package.json
├── vite.config.js
└── tailwind.config.js
```

## API Endpoints Used

### Authentication

- `POST /api/auth/signup` - User signup
- `POST /api/auth/signin` - User signin
- `POST /api/auth/signup-admin` - Admin signup

### Courses

- `GET /api/courses` - Get all courses
- `GET /api/courses/:id` - Get course by ID
- `POST /api/courses` - Create course
- `PUT /api/courses/:id` - Update course
- `DELETE /api/courses/:id` - Delete course

### Discussions

- `GET /api/discussions` - Get all discussions
- `GET /api/discussions/:id` - Get discussion by ID
- `GET /api/discussions/course/:courseId` - Get discussions by course
- `POST /api/discussions` - Create discussion
- `PUT /api/discussions/:id` - Update discussion
- `DELETE /api/discussions/:id` - Delete discussion

### Posts

- `GET /api/posts/discussion/:discussionId` - Get all posts by discussion
- `GET /api/posts/discussion/:discussionId/main` - Get main posts
- `GET /api/posts/:id` - Get post by ID
- `POST /api/posts` - Create post (requires auth)
- `PUT /api/posts/:id` - Update post
- `DELETE /api/posts/:id` - Delete post

### Enrollments

- `POST /api/enrollments` - Enroll users (Admin only)
- `GET /api/enrollments/course/:courseId` - Get enrolled users (Admin only)

## Environment Setup

Make sure your Spring Boot backend is running on `http://localhost:8080`

The frontend runs on `http://localhost:3000` by default.

## Usage

1. Start the backend Spring Boot application
2. Run `npm install` to install dependencies
3. Run `npm run dev` to start the frontend
4. Open `http://localhost:3000` in your browser
5. Sign up or login to start using the application

## Features by Page

### Home

- Landing page with navigation to all features

### Login/Signup

- User authentication
- Admin signup option

### Courses

- View all courses
- Create new courses
- Edit existing courses
- Delete courses

### Discussions

- View all discussions
- Filter by course
- Create/edit/delete discussions

### Posts

- View posts for a discussion
- Create posts, comments, and replies
- Nested comment/reply structure
- Delete posts

### Enrollments (Admin Only)

- View enrolled users for a course
- Enroll multiple users to a course






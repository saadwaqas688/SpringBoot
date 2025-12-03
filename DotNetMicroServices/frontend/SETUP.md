# Frontend Setup Guide

## Quick Start

1. **Navigate to the frontend directory:**

   ```bash
   cd frontend
   ```

2. **Install dependencies:**

   ```bash
   npm install
   ```

3. **Create environment file:**
   Create a `.env.local` file in the `frontend` directory:

   ```env
   NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api
   ```

4. **Start the development server:**

   ```bash
   npm run dev
   ```

5. **Open your browser:**
   Navigate to [http://localhost:3000](http://localhost:3000)

## Backend Requirements

Make sure your backend services are running:

- Gateway service on port 5000
- CoursesService on port 5004
- UserAccountService on port 5003
- RabbitMQ on port 5672

## Features Implemented

### ✅ Authentication

- Sign In page (matches screenshot design)
- Sign Up (ready for implementation)
- Forgot Password (ready for implementation)
- JWT token management with Redux Persist

### ✅ Dashboard

- Sidebar navigation with expandable menu
- Header with search and notifications
- Responsive layout

### ✅ Courses

- Courses listing page with grid view
- Course cards with status badges
- Search and filter functionality
- Course detail page with lessons and slides
- Create/Edit course functionality (ready)

### ✅ Discussions

- Discussion thread view
- Post creation and replies
- User avatars and timestamps
- Discussion prompt sidebar

### ✅ API Integration

All endpoints are integrated via RTK Query:

- Auth API (sign in, sign up, refresh token, etc.)
- Courses API (CRUD operations)
- Lessons API
- Slides API
- Discussions API
- Quizzes API
- Progress API
- Admin API

## Project Structure

```
frontend/
├── src/
│   ├── app/                    # Next.js pages
│   │   ├── (auth)/            # Authentication pages
│   │   │   └── sign-in/      # Sign in page
│   │   ├── (dashboard)/      # Dashboard pages
│   │   │   ├── dashboard/    # Dashboard home
│   │   │   ├── courses/      # Courses listing
│   │   │   │   └── [id]/     # Course detail
│   │   │   └── discussions/   # Discussions page
│   │   └── layout.tsx         # Root layout
│   ├── components/            # Reusable components
│   │   └── providers/         # Redux & Theme providers
│   ├── constants/            # Constants
│   │   └── api-endpoints.ts  # API endpoint definitions
│   ├── redux-store/          # Redux configuration
│   │   ├── slices/           # Redux slices
│   │   ├── hooks.ts          # Typed hooks
│   │   └── store.ts          # Store setup
│   └── services/             # RTK Query APIs
│       ├── auth-api.ts
│       ├── courses-api.ts
│       ├── lessons-api.ts
│       ├── discussions-api.ts
│       ├── quizzes-api.ts
│       ├── slides-api.ts
│       └── progress-api.ts
```

## Styling

The app uses:

- **Material-UI (MUI)**: Component library
- **Styled Components**: Custom styling
- **Theme**: Custom purple/indigo theme matching the Compliance Sheet brand

## Authentication Flow

1. User signs in via `/sign-in`
2. JWT token is stored in Redux state (persisted to localStorage)
3. Token is automatically included in all API requests
4. Protected routes can check `isAuthenticated` from Redux state

## Next Steps

To extend the application:

1. **Add more pages:**

   - Quiz creation/attempt pages
   - User management
   - Settings
   - Analytics dashboard

2. **Enhance features:**

   - Course creation modal/form
   - Lesson/slide editor
   - Rich text editor for discussions
   - File uploads for course media

3. **Add guards:**

   - Auth guard for protected routes
   - Role-based access control

4. **Improve UX:**
   - Loading states
   - Error handling
   - Toast notifications
   - Form validation

## Troubleshooting

### API Connection Issues

- Verify backend services are running
- Check `NEXT_PUBLIC_API_BASE_URL` in `.env.local`
- Check browser console for CORS errors

### Build Errors

- Clear `.next` folder: `rm -rf .next`
- Reinstall dependencies: `rm -rf node_modules && npm install`

### TypeScript Errors

- Run `npm run build` to see all type errors
- Check that all API response types match backend DTOs











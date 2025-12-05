# Compliance Sheet L&D Frontend

A Next.js application with Redux Toolkit and RTK Query for the Compliance Sheet Learning & Development platform.

## Features

- **Authentication**: Sign in, Sign up, Forgot password, Reset password
- **Courses Management**: Create, view, update, delete courses
- **Lessons & Slides**: Manage course content
- **Discussions**: Interactive discussion forums
- **Quizzes**: Create and take quizzes
- **Progress Tracking**: Track user progress through courses
- **Admin Analytics**: View analytics and insights

## Tech Stack

- **Next.js 15.1.3**: React framework with App Router
- **Redux Toolkit**: State management
- **RTK Query**: Data fetching and caching
- **Material-UI (MUI)**: UI component library
- **Styled Components**: Styling
- **TypeScript**: Type safety
- **React Hook Form**: Form management

## Getting Started

### Prerequisites

- Node.js 18+
- npm or yarn

### Installation

1. Install dependencies:

```bash
npm install
```

2. Create a `.env.local` file in the root directory:

```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api
```

3. Run the development server:

```bash
npm run dev
```

4. Open [http://localhost:3000](http://localhost:3000) in your browser.

## Project Structure

```
frontend/
├── src/
│   ├── app/                    # Next.js App Router pages
│   │   ├── (auth)/            # Auth routes (sign-in, sign-up)
│   │   ├── (dashboard)/       # Dashboard routes
│   │   └── layout.tsx         # Root layout
│   ├── components/            # Reusable components
│   │   └── providers/         # Redux and theme providers
│   ├── constants/            # Constants and configurations
│   │   └── api-endpoints.ts  # API endpoint definitions
│   ├── redux-store/          # Redux store configuration
│   │   ├── slices/           # Redux slices
│   │   └── store.ts          # Store setup
│   ├── services/             # RTK Query API services
│   │   ├── auth-api.ts       # Authentication endpoints
│   │   ├── courses-api.ts    # Courses endpoints
│   │   ├── lessons-api.ts    # Lessons endpoints
│   │   ├── discussions-api.ts # Discussion endpoints
│   │   └── ...               # Other API services
│   └── types/                # TypeScript type definitions
├── public/                   # Static assets
├── package.json
├── tsconfig.json
└── next.config.ts
```

## API Integration

All API endpoints are defined in `src/constants/api-endpoints.ts` and integrated using RTK Query in the `src/services/` directory.

### Available API Services

- **auth-api**: Authentication (sign in, sign up, refresh token, etc.)
- **courses-api**: Course management
- **lessons-api**: Lesson management
- **slides-api**: Slide management
- **discussions-api**: Discussion posts and comments
- **quizzes-api**: Quiz management and attempts
- **progress-api**: Progress tracking
- **admin-api**: Admin analytics

## Authentication

The app uses JWT tokens stored in Redux state (persisted to localStorage). The token is automatically included in API requests via the base API configuration.

## Styling

The app uses Material-UI for components and styled-components for custom styling. The theme is configured in `src/components/providers/Providers.tsx`.

## Building for Production

```bash
npm run build
npm start
```

## Environment Variables

- `NEXT_PUBLIC_API_BASE_URL`: Base URL for the backend API (default: http://localhost:5000/api)

## License

ISC














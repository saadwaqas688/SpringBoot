export const API_ENDPOINTS = {
  // Auth endpoints
  AUTH: {
    SIGN_UP: "/Auth/signup",
    SIGN_IN: "/Auth/signin",
    REFRESH_TOKEN: "/Auth/refresh-token",
    FORGOT_PASSWORD: "/Auth/forgot-password",
    RESET_PASSWORD: "/Auth/reset-password",
    PROFILE: "/Auth/profile",
    ME: "/Auth/me",
  },
  // Courses endpoints
  COURSES: {
    GET_ALL: "/Courses",
    GET_BY_ID: (id: string) => `/Courses/${id}`,
    CREATE: "/Courses",
    UPDATE: (id: string) => `/Courses/${id}`,
    DELETE: (id: string) => `/Courses/${id}`,
    SEARCH: "/Courses/search",
    GET_BY_STATUS: "/Courses/status",
    ASSIGN_USER: "/Courses/assign-user",
    REMOVE_USER: "/Courses/remove-user",
    ASSIGNED_USERS: (id: string) => `/Courses/${id}/assigned-users`,
    USER_PROGRESS: (id: string) => `/Courses/${id}/user-progress`,
    HEALTH: "/Courses/health",
  },
  // Lessons endpoints
  LESSONS: {
    GET_ALL: "/Lessons",
    GET_BY_ID: (id: string) => `/Lessons/${id}`,
    CREATE: "/Lessons",
    UPDATE: (id: string) => `/Lessons/${id}`,
    DELETE: (id: string) => `/Lessons/${id}`,
    GET_BY_COURSE: (courseId: string) => `/Courses/${courseId}/lessons`,
  },
  // Slides endpoints
  SLIDES: {
    GET_BY_ID: (id: string) => `/slides/${id}`,
    CREATE: "/slides",
    UPDATE: (id: string) => `/slides/${id}`,
    DELETE: (id: string) => `/slides/${id}`,
    GET_BY_LESSON: (lessonId: string) => `/lessons/${lessonId}/slides`,
  },
  // Discussion Posts endpoints
  POSTS: {
    GET_ALL: "/DiscussionPosts",
    GET_BY_ID: (id: string) => `/DiscussionPosts/${id}`,
    CREATE: "/DiscussionPosts",
    UPDATE: (id: string) => `/DiscussionPosts/${id}`,
    DELETE: (id: string) => `/DiscussionPosts/${id}`,
    GET_BY_LESSON: (lessonId: string) => `/DiscussionPosts/lesson/${lessonId}`,
    GET_COMMENTS: (postId: string) => `/DiscussionPosts/${postId}/comments`,
  },
  // Quizzes endpoints
  QUIZZES: {
    GET_ALL: "/Quizzes",
    GET_BY_ID: (id: string) => `/Quizzes/${id}`,
    CREATE: "/Quizzes",
    UPDATE: (id: string) => `/Quizzes/${id}`,
    DELETE: (id: string) => `/Quizzes/${id}`,
    GET_BY_LESSON: (lessonId: string) => `/Quizzes/lesson/${lessonId}`,
  },
  // Quiz Questions endpoints
  QUIZ_QUESTIONS: {
    GET_ALL: "/QuizQuestions",
    GET_BY_ID: (id: string) => `/QuizQuestions/${id}`,
    CREATE: "/QuizQuestions",
    UPDATE: (id: string) => `/QuizQuestions/${id}`,
    DELETE: (id: string) => `/QuizQuestions/${id}`,
    GET_BY_QUIZ: (quizId: string) => `/QuizQuestions/quiz/${quizId}`,
  },
  // Quiz Attempts endpoints
  QUIZ_ATTEMPTS: {
    CREATE: "/QuizAttempts",
    GET_BY_ID: (id: string) => `/QuizAttempts/${id}`,
    ADD_ANSWER: (id: string) => `/QuizAttempts/${id}/answers`,
    SUBMIT: (id: string) => `/QuizAttempts/${id}/submit`,
    GET_RESULTS: (id: string) => `/QuizAttempts/${id}/results`,
  },
  // Progress endpoints
  PROGRESS: {
    GET_COURSE_PROGRESS: (courseId: string, userId: string) =>
      `/Progress/course/${courseId}/user/${userId}`,
    GET_LESSON_PROGRESS: (lessonId: string, userId: string) =>
      `/Progress/lesson/${lessonId}/user/${userId}`,
    GET_SLIDE_PROGRESS: (slideId: string, userId: string) =>
      `/Progress/slide/${slideId}/user/${userId}`,
    UPDATE_LESSON_PROGRESS: "/Progress/lesson",
    UPDATE_SLIDE_PROGRESS: "/Progress/slide",
    LOG_ACTIVITY: "/Progress/activity",
  },
  // Users endpoints (Courses-related)
  USERS: {
    GET_ALL: "/Users",
    GET_BY_ID: (id: string) => `/Users/${id}`,
    SEARCH: "/Users/search",
    GET_COURSES: (userId: string) => `/Users/${userId}/courses`,
    GET_PROGRESS_SUMMARY: (userId: string) =>
      `/Users/${userId}/progress-summary`,
  },
  // Admin endpoints
  ADMIN: {
    COURSE_ANALYTICS: "/Admin/analytics/courses",
    USER_ANALYTICS: "/Admin/analytics/users",
    ENGAGEMENT_ANALYTICS: "/Admin/analytics/engagement",
    PERFORMANCE_ANALYTICS: "/Admin/analytics/performance",
  },
  // Upload endpoints (handles both images and videos)
  UPLOAD: {
    IMAGE: "/Upload/image",
    DELETE_IMAGE: (fileName: string) => `/Upload/image/${fileName}`,
  },
};

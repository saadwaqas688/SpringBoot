import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface Course {
  id: string;
  title: string;
  description: string;
  thumbnailUrl?: string;
  category: string;
  level: string;
  createdBy: string;
  creatorName?: string;
  createdAt: string;
  updatedAt: string;
  lessonCount: number;
  enrollmentCount: number;
}

export interface Lesson {
  id: string;
  courseId: string;
  title: string;
  description: string;
  order: number;
  createdAt: string;
  updatedAt: string;
  contents: LessonContent[];
}

export interface LessonContent {
  id: string;
  lessonId: string;
  type: string;
  order: number;
  title: string;
  data?: any;
  createdAt: string;
  updatedAt: string;
}

export const authService = {
  login: async (email: string, password: string): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/login', { email, password });
    return response.data;
  },

  register: async (name: string, email: string, password: string, role: string = 'USER'): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/register', { name, email, password, role });
    return response.data;
  },
};

export const courseService = {
  getAll: async (): Promise<Course[]> => {
    const response = await api.get<Course[]>('/course');
    return response.data;
  },

  getById: async (id: string): Promise<Course> => {
    const response = await api.get<Course>(`/course/${id}`);
    return response.data;
  },

  create: async (course: Partial<Course>): Promise<Course> => {
    const response = await api.post<Course>('/course', course);
    return response.data;
  },

  update: async (id: string, course: Partial<Course>): Promise<Course> => {
    const response = await api.put<Course>(`/course/${id}`, course);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/course/${id}`);
  },

  getMyCourses: async (): Promise<Course[]> => {
    const response = await api.get<Course[]>('/course/my-courses');
    return response.data;
  },
};

export const lessonService = {
  getByCourseId: async (courseId: string): Promise<Lesson[]> => {
    const response = await api.get<Lesson[]>(`/lesson/course/${courseId}`);
    return response.data;
  },

  getById: async (id: string): Promise<Lesson> => {
    const response = await api.get<Lesson>(`/lesson/${id}`);
    return response.data;
  },
};

export const enrollmentService = {
  enroll: async (userId: string, courseId: string): Promise<void> => {
    await api.post('/enrollment', { userId, courseId });
  },

  getMyEnrollments: async (): Promise<any[]> => {
    const response = await api.get('/enrollment/my-enrollments');
    return response.data;
  },
};

export default api;


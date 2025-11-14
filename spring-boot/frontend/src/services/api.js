import axios from "axios";
import { API_BASE_URL, getAuthHeaders } from "../config/api";

// Create axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Add request interceptor to include token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// ============================================================================
// AUTH API
// ============================================================================

export const authAPI = {
  signup: async (data) => {
    const response = await api.post("/auth/signup", data);
    return response.data;
  },

  signin: async (data) => {
    const response = await api.post("/auth/signin", data);
    return response.data;
  },

  signupAdmin: async (data) => {
    const response = await api.post("/auth/signup-admin", data);
    return response.data;
  },
};

// ============================================================================
// COURSES API
// ============================================================================

export const coursesAPI = {
  getAll: async () => {
    const response = await api.get("/courses");
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/courses/${id}`);
    return response.data;
  },

  create: async (course) => {
    const response = await api.post("/courses", course);
    return response.data;
  },

  update: async (id, course) => {
    const response = await api.put(`/courses/${id}`, course);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/courses/${id}`);
  },

  deleteAll: async () => {
    await api.delete("/courses");
  },
};

// ============================================================================
// DISCUSSIONS API
// ============================================================================

export const discussionsAPI = {
  getAll: async () => {
    const response = await api.get("/discussions");
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/discussions/${id}`);
    return response.data;
  },

  getByCourseId: async (courseId) => {
    const response = await api.get(`/discussions/course/${courseId}`);
    return response.data;
  },

  create: async (discussion) => {
    const response = await api.post("/discussions", discussion);
    return response.data;
  },

  update: async (id, discussion) => {
    const response = await api.put(`/discussions/${id}`, discussion);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/discussions/${id}`);
  },
};

// ============================================================================
// POSTS API
// ============================================================================

export const postsAPI = {
  getByDiscussionId: async (discussionId) => {
    const response = await api.get(`/posts/discussion/${discussionId}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/posts/${id}`);
    return response.data;
  },

  create: async (post) => {
    const response = await api.post("/posts", post);
    return response.data;
  },

  update: async (id, post) => {
    const response = await api.put(`/posts/${id}`, post);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/posts/${id}`);
  },
};

// ============================================================================
// ENROLLMENTS API
// ============================================================================

export const enrollmentsAPI = {
  enrollUsers: async (courseId, userIds) => {
    const response = await api.post("/enrollments", {
      courseId,
      userIds,
    });
    return response.data;
  },

  getEnrolledUsers: async (courseId) => {
    const response = await api.get(`/enrollments/course/${courseId}`);
    return response.data;
  },

  getMyEnrolledCourses: async () => {
    const response = await api.get("/enrollments/my-courses");
    return response.data;
  },
};

// ============================================================================
// USERS API
// ============================================================================

export const usersAPI = {
  getAll: async () => {
    const response = await api.get("/users");
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },
};

export default api;

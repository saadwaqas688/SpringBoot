import axios from "axios";

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || "http://localhost:5000/api",
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor to add token
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

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Don't redirect on auth endpoints (login/register)
    const isAuthEndpoint = error.config?.url?.includes("/auth/");

    if (error.response?.status === 401 && !isAuthEndpoint) {
      // Only redirect if we're not already on the login page
      // This prevents redirect loops and issues during initial page load
      const isLoginPage = window.location.pathname === "/login" || 
                         window.location.pathname === "/register";
      
      if (!isLoginPage) {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);

export default api;

// API Configuration
export const API_BASE_URL = "http://localhost:8080/api";

// Helper function to get auth token from localStorage
export const getAuthToken = () => {
  return localStorage.getItem("token");
};

// Helper function to get auth headers
export const getAuthHeaders = () => {
  const token = getAuthToken();
  return {
    "Content-Type": "application/json",
    ...(token && { Authorization: `Bearer ${token}` }),
  };
};






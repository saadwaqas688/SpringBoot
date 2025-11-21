import React, { createContext, useState, useContext, useEffect } from "react";
import api from "../services/api";
import signalRService from "../services/signalRService";

const AuthContext = createContext();

export function useAuth() {
  return useContext(AuthContext);
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const userData = localStorage.getItem("user");

    if (token && userData) {
      setUser(JSON.parse(userData));
      api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      // Start SignalR connection if user is already logged in
      signalRService.startConnection(token).catch(console.error);
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      const response = await api.post("/auth/login", { email, password });

      // Handle both camelCase and PascalCase responses
      const token = response.data.token || response.data.Token;
      const user = response.data.user || response.data.User;

      if (!token || !user) {
        console.error("Invalid response structure:", response.data);
        return {
          success: false,
          message: "Invalid response from server",
        };
      }

      localStorage.setItem("token", token);
      localStorage.setItem("user", JSON.stringify(user));
      api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      setUser(user);

      // Start SignalR connection immediately after login to receive real-time notifications
      try {
        await signalRService.startConnection(token);
        console.log("SignalR connection established");
      } catch (signalRError) {
        console.error("Failed to start SignalR connection:", signalRError);
        // Don't fail login if SignalR fails, just log the error
      }

      return { success: true };
    } catch (error) {
      console.error("Login error:", error);
      console.error("Error response:", error.response?.data);
      console.error("Error status:", error.response?.status);
      console.error("Error config:", error.config);

      // Handle network errors
      if (!error.response) {
        return {
          success: false,
          message: "Network error. Please check if the server is running.",
        };
      }

      const errorMessage =
        error.response?.data?.message ||
        error.message ||
        (error.response?.status === 401
          ? "Invalid email or password"
          : "Login failed");

      return {
        success: false,
        message: errorMessage,
      };
    }
  };

  const register = async (username, email, password) => {
    try {
      const response = await api.post("/auth/register", {
        username,
        email,
        password,
      });

      // Handle both camelCase and PascalCase responses
      const token = response.data.token || response.data.Token;
      const user = response.data.user || response.data.User;

      if (!token || !user) {
        console.error("Invalid response structure:", response.data);
        return {
          success: false,
          message: "Invalid response from server",
        };
      }

      localStorage.setItem("token", token);
      localStorage.setItem("user", JSON.stringify(user));
      api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      setUser(user);

      // Start SignalR connection immediately after registration to receive real-time notifications
      try {
        await signalRService.startConnection(token);
        console.log("SignalR connection established");
      } catch (signalRError) {
        console.error("Failed to start SignalR connection:", signalRError);
        // Don't fail registration if SignalR fails, just log the error
      }

      return { success: true };
    } catch (error) {
      console.error("Registration error:", error);
      console.error("Error response:", error.response?.data);
      console.error("Error config:", error.config);

      // Handle network errors
      if (!error.response) {
        return {
          success: false,
          message: "Network error. Please check if the server is running.",
        };
      }

      const errorMessage =
        error.response?.data?.message || error.message || "Registration failed";

      return {
        success: false,
        message: errorMessage,
      };
    }
  };

  const logout = () => {
    // Stop SignalR connection on logout
    signalRService.stopConnection().catch(console.error);
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    delete api.defaults.headers.common["Authorization"];
    setUser(null);
  };

  const value = {
    user,
    login,
    register,
    logout,
    loading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

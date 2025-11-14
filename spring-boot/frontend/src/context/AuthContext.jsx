import React, { createContext, useContext, useState, useEffect } from "react";
import { authAPI } from "../services/api";

const AuthContext = createContext(null);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is logged in on mount
    const token = localStorage.getItem("token");
    const userData = localStorage.getItem("user");

    if (token && userData) {
      try {
        setUser(JSON.parse(userData));
      } catch (error) {
        console.error("Error parsing user data:", error);
        localStorage.removeItem("token");
        localStorage.removeItem("user");
      }
    }
    setLoading(false);
  }, []);

  const signin = async (email, password) => {
    try {
      const response = await authAPI.signin({ email, password });
      const { token, userId, email: userEmail, role } = response;

      localStorage.setItem("token", token);
      const userData = { userId, email: userEmail, role };
      localStorage.setItem("user", JSON.stringify(userData));
      setUser(userData);

      return { success: true };
    } catch (error) {
      console.error("Signin Error:", error);
      const errorMessage =
        error.response?.data?.message ||
        error.response?.data?.error ||
        error.message ||
        "Invalid credentials";
      return {
        success: false,
        error: errorMessage,
      };
    }
  };

  const signup = async (userData) => {
    try {
      const response = await authAPI.signup(userData);
      const { token, userId, email, role } = response;

      localStorage.setItem("token", token);
      const user = { userId, email, role };
      localStorage.setItem("user", JSON.stringify(user));
      setUser(user);

      return { success: true };
    } catch (error) {
      console.error("Signup Error:", error);

      // Handle validation errors
      if (error.response?.data?.errors) {
        const validationErrors = Object.values(error.response.data.errors).join(
          ", "
        );
        return {
          success: false,
          error: `Validation failed: ${validationErrors}`,
        };
      }

      const errorMessage =
        error.response?.data?.message ||
        error.response?.data?.error ||
        error.message ||
        "Signup failed. Please check if the email already exists.";
      return {
        success: false,
        error: errorMessage,
      };
    }
  };

  const signupAdmin = async (userData) => {
    try {
      const response = await authAPI.signupAdmin(userData);
      const { token, userId, email, role } = response;

      localStorage.setItem("token", token);
      const user = { userId, email, role };
      localStorage.setItem("user", JSON.stringify(user));
      setUser(user);

      return { success: true };
    } catch (error) {
      console.error("Signup Admin Error:", error);

      // Handle validation errors
      if (error.response?.data?.errors) {
        const validationErrors = Object.values(error.response.data.errors).join(
          ", "
        );
        return {
          success: false,
          error: `Validation failed: ${validationErrors}`,
        };
      }

      const errorMessage =
        error.response?.data?.message ||
        error.response?.data?.error ||
        error.message ||
        "Admin signup failed. Please check if the email already exists.";
      return {
        success: false,
        error: errorMessage,
      };
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
  };

  const isAdmin = () => {
    return user?.role === "ADMIN";
  };

  const value = {
    user,
    loading,
    signin,
    signup,
    signupAdmin,
    logout,
    isAdmin,
    isAuthenticated: !!user,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

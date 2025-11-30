import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { TAGS } from "./tags";
import { BASE_URL } from "@/config";

// Helper function to get token from Redux state or localStorage
const getToken = (getState?: () => any): string | null => {
  // First try to get from Redux state (preferred method)
  if (getState) {
    const state = getState();
    const token = state?.auth?.accessToken;
    if (token) {
      return token;
    }
  }

  // Fallback to localStorage (useful during initial load before Redux rehydrates)
  // This reads from the explicit localStorage keys we set, not redux-persist
  if (typeof window !== "undefined") {
    try {
      const token = localStorage.getItem("auth_token");
      if (token) {
        return token;
      }
    } catch (error) {
      console.error("Error reading token from localStorage:", error);
    }
  }

  return null;
};

const baseQuery = fetchBaseQuery({
  baseUrl: BASE_URL,
  prepareHeaders: (headers, { getState, endpoint }) => {
    // Get token from Redux state or localStorage
    const token = getToken(getState);

    if (token) {
      headers.set("Authorization", `Bearer ${token}`);
    }

    // Don't set Content-Type for file uploads (browser will set it with boundary)
    if (!endpoint?.includes("upload")) {
      headers.set("Content-Type", "application/json");
    }
    return headers;
  },
});

export const baseAPI = createApi({
  reducerPath: "api",
  baseQuery,
  tagTypes: Object.values(TAGS),
  endpoints: () => ({}),
});

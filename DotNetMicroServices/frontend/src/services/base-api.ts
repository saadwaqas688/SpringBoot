import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { TAGS } from "./tags";
import { BASE_URL } from "@/config";

const baseQuery = fetchBaseQuery({
  baseUrl: BASE_URL,
  prepareHeaders: (headers, { getState, endpoint }) => {
    // Get token from Redux state if available
    const state = getState() as any;
    const token = state?.auth?.accessToken;

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

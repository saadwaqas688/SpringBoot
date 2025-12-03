import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";

export const adminAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getCourseAnalytics: builder.query({
      query: () => ({
        url: API_ENDPOINTS.ADMIN.COURSE_ANALYTICS,
        method: "GET",
      }),
    }),
    getUserAnalytics: builder.query({
      query: () => ({
        url: API_ENDPOINTS.ADMIN.USER_ANALYTICS,
        method: "GET",
      }),
    }),
    getEngagementAnalytics: builder.query({
      query: () => ({
        url: API_ENDPOINTS.ADMIN.ENGAGEMENT_ANALYTICS,
        method: "GET",
      }),
    }),
    getPerformanceAnalytics: builder.query({
      query: () => ({
        url: API_ENDPOINTS.ADMIN.PERFORMANCE_ANALYTICS,
        method: "GET",
      }),
    }),
  }),
});

export const {
  useGetCourseAnalyticsQuery,
  useGetUserAnalyticsQuery,
  useGetEngagementAnalyticsQuery,
  useGetPerformanceAnalyticsQuery,
} = adminAPI;


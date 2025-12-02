import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { PROGRESS } from "./tags";

export const progressAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getCourseProgress: builder.query({
      query: ({ courseId, userId }) => ({
        url: API_ENDPOINTS.PROGRESS.GET_COURSE_PROGRESS(courseId, userId),
        method: "GET",
      }),
      providesTags: [PROGRESS],
    }),
    getLessonProgress: builder.query({
      query: ({ lessonId, userId }) => ({
        url: API_ENDPOINTS.PROGRESS.GET_LESSON_PROGRESS(lessonId, userId),
        method: "GET",
      }),
      providesTags: [PROGRESS],
    }),
    getSlideProgress: builder.query({
      query: ({ slideId, userId }) => ({
        url: API_ENDPOINTS.PROGRESS.GET_SLIDE_PROGRESS(slideId, userId),
        method: "GET",
      }),
      providesTags: [PROGRESS],
    }),
    updateLessonProgress: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.PROGRESS.UPDATE_LESSON_PROGRESS,
        method: "POST",
        body,
      }),
      invalidatesTags: [PROGRESS],
    }),
    updateSlideProgress: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.PROGRESS.UPDATE_SLIDE_PROGRESS,
        method: "POST",
        body,
      }),
      invalidatesTags: [PROGRESS],
    }),
    logActivity: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.PROGRESS.LOG_ACTIVITY,
        method: "POST",
        body,
      }),
      invalidatesTags: [PROGRESS],
    }),
  }),
});

export const {
  useGetCourseProgressQuery,
  useGetLessonProgressQuery,
  useGetSlideProgressQuery,
  useUpdateLessonProgressMutation,
  useUpdateSlideProgressMutation,
  useLogActivityMutation,
} = progressAPI;





import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { LESSONS } from "./tags";

export const lessonsAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getAllLessons: builder.query({
      query: (params) => ({
        url: API_ENDPOINTS.LESSONS.GET_ALL,
        method: "GET",
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          ...params,
        },
      }),
      providesTags: [LESSONS],
    }),
    getLessonById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.LESSONS.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [LESSONS],
    }),
    createLesson: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.LESSONS.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [LESSONS],
    }),
    updateLesson: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.LESSONS.UPDATE(id),
        method: "PUT",
        body,
      }),
      invalidatesTags: [LESSONS],
    }),
    deleteLesson: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.LESSONS.DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: [LESSONS],
    }),
    getLessonsByCourse: builder.query({
      query: ({ courseId, page = 1, pageSize = 10 }) => ({
        url: API_ENDPOINTS.LESSONS.GET_BY_COURSE(courseId),
        method: "GET",
        params: {
          page,
          pageSize,
        },
      }),
      providesTags: [LESSONS],
    }),
  }),
});

export const {
  useGetAllLessonsQuery,
  useGetLessonByIdQuery,
  useCreateLessonMutation,
  useUpdateLessonMutation,
  useDeleteLessonMutation,
  useGetLessonsByCourseQuery,
} = lessonsAPI;













import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { COURSES } from "./tags";

export const coursesAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getAllCourses: builder.query({
      query: (params) => ({
        url: API_ENDPOINTS.COURSES.GET_ALL,
        method: "GET",
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          ...params,
        },
      }),
      providesTags: [COURSES],
    }),
    getCourseById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.COURSES.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [COURSES],
    }),
    createCourse: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.COURSES.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [COURSES],
    }),
    updateCourse: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.COURSES.UPDATE(id),
        method: "PUT",
        body,
      }),
      invalidatesTags: [COURSES],
    }),
    deleteCourse: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.COURSES.DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: [COURSES],
    }),
    searchCourses: builder.query({
      query: (searchTerm) => ({
        url: API_ENDPOINTS.COURSES.SEARCH,
        method: "GET",
        params: { searchTerm },
      }),
      providesTags: [COURSES],
    }),
    getCoursesByStatus: builder.query({
      query: (status) => ({
        url: API_ENDPOINTS.COURSES.GET_BY_STATUS,
        method: "GET",
        params: { status },
      }),
      providesTags: [COURSES],
    }),
    assignUser: builder.mutation({
      query: ({ courseId, userIds }) => ({
        url: `${API_ENDPOINTS.COURSES.GET_BY_ID(courseId)}/assign-user`,
        method: "POST",
        body: { userIds },
      }),
      invalidatesTags: [COURSES],
    }),
    removeUser: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.COURSES.REMOVE_USER,
        method: "POST",
        body,
      }),
      invalidatesTags: [COURSES],
    }),
    getAssignedUsers: builder.query({
      query: (courseId) => ({
        url: API_ENDPOINTS.COURSES.ASSIGNED_USERS(courseId),
        method: "GET",
      }),
      providesTags: [COURSES],
    }),
    getUserProgress: builder.query({
      query: ({ courseId, userId }) => ({
        url: API_ENDPOINTS.COURSES.USER_PROGRESS(courseId),
        method: "GET",
        params: { userId },
      }),
      providesTags: [COURSES],
    }),
    getUserCourses: builder.query({
      query: ({ userId, page = 1, pageSize = 10 }) => ({
        url: API_ENDPOINTS.COURSES.GET_USER_COURSES(userId),
        method: "GET",
        params: { page, pageSize },
      }),
      providesTags: [COURSES],
    }),
  }),
});

export const {
  useGetAllCoursesQuery,
  useGetCourseByIdQuery,
  useCreateCourseMutation,
  useUpdateCourseMutation,
  useDeleteCourseMutation,
  useSearchCoursesQuery,
  useGetCoursesByStatusQuery,
  useAssignUserMutation,
  useRemoveUserMutation,
  useGetAssignedUsersQuery,
  useGetUserProgressQuery,
  useGetUserCoursesQuery,
} = coursesAPI;


import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { POSTS } from "./tags";

export const discussionsAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getAllPosts: builder.query({
      query: (params) => ({
        url: API_ENDPOINTS.POSTS.GET_ALL,
        method: "GET",
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          ...params,
        },
      }),
      providesTags: [POSTS],
    }),
    getPostById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.POSTS.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [POSTS],
    }),
    createPost: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.POSTS.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [POSTS],
    }),
    updatePost: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.POSTS.UPDATE(id),
        method: "PUT",
        body,
      }),
      invalidatesTags: [POSTS],
    }),
    deletePost: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.POSTS.DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: [POSTS],
    }),
    getPostsByLesson: builder.query({
      query: (lessonId) => ({
        url: API_ENDPOINTS.POSTS.GET_BY_LESSON(lessonId),
        method: "GET",
      }),
      providesTags: [POSTS],
    }),
    getCommentsByPost: builder.query({
      query: (postId) => ({
        url: API_ENDPOINTS.POSTS.GET_COMMENTS(postId),
        method: "GET",
      }),
      providesTags: [POSTS],
    }),
  }),
});

export const {
  useGetAllPostsQuery,
  useGetPostByIdQuery,
  useCreatePostMutation,
  useUpdatePostMutation,
  useDeletePostMutation,
  useGetPostsByLessonQuery,
  useGetCommentsByPostQuery,
} = discussionsAPI;


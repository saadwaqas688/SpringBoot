import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { DISCUSSIONS, POSTS } from "./tags";

export const discussionsAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    // Discussions (one-to-one with lesson)
    getAllDiscussions: builder.query({
      query: () => ({
        url: API_ENDPOINTS.DISCUSSIONS.GET_ALL,
        method: "GET",
      }),
      providesTags: [DISCUSSIONS],
    }),
    getDiscussionByLesson: builder.query({
      query: (lessonId) => ({
        url: API_ENDPOINTS.DISCUSSIONS.GET_BY_LESSON(lessonId),
        method: "GET",
      }),
      providesTags: [DISCUSSIONS],
      // Don't throw error on 404 - discussion might not exist yet
      transformResponse: (response: any) => response,
      transformErrorResponse: (response: any) => response,
    }),
    getPostsByDiscussion: builder.query({
      query: (discussionId) => ({
        url: API_ENDPOINTS.DISCUSSIONS.GET_POSTS_BY_DISCUSSION(discussionId),
        method: "GET",
      }),
      providesTags: [POSTS],
    }),
    getDiscussionById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.DISCUSSIONS.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [DISCUSSIONS],
    }),
    createDiscussion: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.DISCUSSIONS.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [DISCUSSIONS],
    }),
    updateDiscussion: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.DISCUSSIONS.UPDATE(id),
        method: "PUT",
        body,
      }),
      invalidatesTags: [DISCUSSIONS],
    }),
    deleteDiscussion: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.DISCUSSIONS.DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: [DISCUSSIONS],
    }),
    // Discussion Posts
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
  // Discussion hooks
  useGetAllDiscussionsQuery,
  useGetDiscussionByLessonQuery,
  useGetDiscussionByIdQuery,
  useCreateDiscussionMutation,
  useUpdateDiscussionMutation,
  useDeleteDiscussionMutation,
  useGetPostsByDiscussionQuery,
  // Discussion Posts hooks
  useGetAllPostsQuery,
  useGetPostByIdQuery,
  useCreatePostMutation,
  useUpdatePostMutation,
  useDeletePostMutation,
  useGetPostsByLessonQuery,
  useGetCommentsByPostQuery,
} = discussionsAPI;

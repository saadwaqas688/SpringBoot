import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { QUIZZES, QUIZ_QUESTIONS, QUIZ_ATTEMPTS } from "./tags";

export const quizzesAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    // Quizzes
    getAllQuizzes: builder.query({
      query: (params) => ({
        url: API_ENDPOINTS.QUIZZES.GET_ALL,
        method: "GET",
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          ...params,
        },
      }),
      providesTags: [QUIZZES],
    }),
    getQuizById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.QUIZZES.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [QUIZZES],
    }),
    createQuiz: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.QUIZZES.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [QUIZZES],
    }),
    updateQuiz: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.QUIZZES.UPDATE(id),
        method: "PUT",
        body,
      }),
      invalidatesTags: [QUIZZES],
    }),
    deleteQuiz: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.QUIZZES.DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: [QUIZZES],
    }),
    getQuizzesByLesson: builder.query({
      query: (lessonId) => ({
        url: API_ENDPOINTS.QUIZZES.GET_BY_LESSON(lessonId),
        method: "GET",
      }),
      providesTags: [QUIZZES],
    }),
    // Quiz Questions
    getAllQuizQuestions: builder.query({
      query: (params) => ({
        url: API_ENDPOINTS.QUIZ_QUESTIONS.GET_ALL,
        method: "GET",
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          ...params,
        },
      }),
      providesTags: [QUIZ_QUESTIONS],
    }),
    getQuizQuestionById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.QUIZ_QUESTIONS.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [QUIZ_QUESTIONS],
    }),
    createQuizQuestion: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.QUIZ_QUESTIONS.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [QUIZ_QUESTIONS],
    }),
    updateQuizQuestion: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.QUIZ_QUESTIONS.UPDATE(id),
        method: "PUT",
        body,
      }),
      invalidatesTags: [QUIZ_QUESTIONS],
    }),
    deleteQuizQuestion: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.QUIZ_QUESTIONS.DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: [QUIZ_QUESTIONS],
    }),
    getQuestionsByQuiz: builder.query({
      query: (quizId) => ({
        url: API_ENDPOINTS.QUIZ_QUESTIONS.GET_BY_QUIZ(quizId),
        method: "GET",
      }),
      providesTags: [QUIZ_QUESTIONS],
    }),
    // Quiz Attempts
    createQuizAttempt: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.QUIZ_ATTEMPTS.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [QUIZ_ATTEMPTS],
    }),
    getQuizAttemptById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.QUIZ_ATTEMPTS.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [QUIZ_ATTEMPTS],
    }),
    addQuizAnswer: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.QUIZ_ATTEMPTS.ADD_ANSWER(id),
        method: "POST",
        body,
      }),
      invalidatesTags: [QUIZ_ATTEMPTS],
    }),
    submitQuizAttempt: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.QUIZ_ATTEMPTS.SUBMIT(id),
        method: "POST",
      }),
      invalidatesTags: [QUIZ_ATTEMPTS],
    }),
    getQuizResults: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.QUIZ_ATTEMPTS.GET_RESULTS(id),
        method: "GET",
      }),
      providesTags: [QUIZ_ATTEMPTS],
    }),
  }),
});

export const {
  useGetAllQuizzesQuery,
  useGetQuizByIdQuery,
  useCreateQuizMutation,
  useUpdateQuizMutation,
  useDeleteQuizMutation,
  useGetQuizzesByLessonQuery,
  useGetAllQuizQuestionsQuery,
  useGetQuizQuestionByIdQuery,
  useCreateQuizQuestionMutation,
  useUpdateQuizQuestionMutation,
  useDeleteQuizQuestionMutation,
  useGetQuestionsByQuizQuery,
  useCreateQuizAttemptMutation,
  useGetQuizAttemptByIdQuery,
  useAddQuizAnswerMutation,
  useSubmitQuizAttemptMutation,
  useGetQuizResultsQuery,
} = quizzesAPI;


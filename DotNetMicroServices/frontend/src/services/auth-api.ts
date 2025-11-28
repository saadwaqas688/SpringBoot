import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { AUTH } from "./tags";

export const authAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    signUp: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.AUTH.SIGN_UP,
        method: "POST",
        body,
      }),
      invalidatesTags: [AUTH],
    }),
    signIn: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.AUTH.SIGN_IN,
        method: "POST",
        body,
      }),
      invalidatesTags: [AUTH],
    }),
    refreshToken: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.AUTH.REFRESH_TOKEN,
        method: "POST",
        body,
      }),
    }),
    forgotPassword: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.AUTH.FORGOT_PASSWORD,
        method: "POST",
        body,
      }),
    }),
    resetPassword: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.AUTH.RESET_PASSWORD,
        method: "POST",
        body,
      }),
    }),
    getProfile: builder.query({
      query: () => ({
        url: API_ENDPOINTS.AUTH.PROFILE,
        method: "GET",
      }),
      providesTags: [AUTH],
    }),
    getMe: builder.query({
      query: () => ({
        url: API_ENDPOINTS.AUTH.ME,
        method: "GET",
      }),
      providesTags: [AUTH],
    }),
  }),
});

export const {
  useSignUpMutation,
  useSignInMutation,
  useRefreshTokenMutation,
  useForgotPasswordMutation,
  useResetPasswordMutation,
  useGetProfileQuery,
  useGetMeQuery,
} = authAPI;


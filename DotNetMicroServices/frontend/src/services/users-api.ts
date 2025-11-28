import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { COURSES } from "./tags";

export const usersAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getAllUsers: builder.query({
      query: (params) => ({
        url: API_ENDPOINTS.USERS.GET_ALL,
        method: "GET",
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          searchTerm: params?.searchTerm || "",
          ...params,
        },
      }),
      providesTags: [COURSES],
    }),
    searchUsers: builder.query({
      query: (searchTerm) => ({
        url: API_ENDPOINTS.USERS.SEARCH,
        method: "GET",
        params: { searchTerm },
      }),
      providesTags: [COURSES],
    }),
    getUserById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.USERS.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [COURSES],
    }),
  }),
});

export const { useGetAllUsersQuery, useSearchUsersQuery, useGetUserByIdQuery } =
  usersAPI;

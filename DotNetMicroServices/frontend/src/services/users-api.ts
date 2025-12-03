import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { COURSES } from "./tags";

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  mobilePhone?: string;
  dateOfBirth?: string;
  gender?: string;
  country?: string;
  state?: string;
  city?: string;
  postalCode?: string;
  role?: string;
  status?: string;
  image?: string;
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  email?: string;
  mobilePhone?: string;
  dateOfBirth?: string;
  gender?: string;
  country?: string;
  state?: string;
  city?: string;
  postalCode?: string;
  role?: string;
  status?: string;
  image?: string;
}

export interface UserInfo {
  id: string;
  name: string;
  email: string;
  image?: string;
  role: string;
  status: string;
  createdAt?: string;
  gender?: string;
  dateOfBirth?: string;
  mobilePhone?: string;
  country?: string;
  state?: string;
  city?: string;
  postalCode?: string;
}

export interface PagedUsersResponse {
  items: UserInfo[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
}

export const usersAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getAllUsers: builder.query<
      PagedUsersResponse,
      { page?: number; pageSize?: number; searchTerm?: string }
    >({
      query: (params) => ({
        url: API_ENDPOINTS.USERS.GET_ALL,
        method: "GET",
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          searchTerm: params?.searchTerm || "",
        },
      }),
      providesTags: [COURSES],
    }),
    createUser: builder.mutation<UserInfo, CreateUserRequest>({
      query: (data) => ({
        url: API_ENDPOINTS.USERS.GET_ALL,
        method: "POST",
        body: data,
      }),
      invalidatesTags: [COURSES],
    }),
    updateUser: builder.mutation<
      UserInfo,
      { id: string; data: UpdateUserRequest }
    >({
      query: ({ id, data }) => ({
        url: `${API_ENDPOINTS.USERS.GET_ALL}/${id}`,
        method: "PUT",
        body: data,
      }),
      invalidatesTags: [COURSES],
    }),
    deleteUser: builder.mutation<boolean, string>({
      query: (id) => ({
        url: `${API_ENDPOINTS.USERS.GET_ALL}/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: [COURSES],
    }),
    updateUserStatus: builder.mutation<boolean, { id: string; status: string }>(
      {
        query: ({ id, status }) => ({
          url: `${API_ENDPOINTS.USERS.GET_ALL}/${id}/status`,
          method: "PUT",
          body: { status },
        }),
        invalidatesTags: [COURSES],
      }
    ),
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

export const {
  useGetAllUsersQuery,
  useCreateUserMutation,
  useUpdateUserMutation,
  useDeleteUserMutation,
  useUpdateUserStatusMutation,
  useSearchUsersQuery,
  useGetUserByIdQuery,
} = usersAPI;

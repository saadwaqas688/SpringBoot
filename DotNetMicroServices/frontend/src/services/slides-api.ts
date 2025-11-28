import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { SLIDES } from "./tags";

export const slidesAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    getSlideById: builder.query({
      query: (id) => ({
        url: API_ENDPOINTS.SLIDES.GET_BY_ID(id),
        method: "GET",
      }),
      providesTags: [SLIDES],
    }),
    createSlide: builder.mutation({
      query: (body) => ({
        url: API_ENDPOINTS.SLIDES.CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: [SLIDES],
    }),
    updateSlide: builder.mutation({
      query: ({ id, ...body }) => ({
        url: API_ENDPOINTS.SLIDES.UPDATE(id),
        method: "PUT",
        body,
      }),
      invalidatesTags: [SLIDES],
    }),
    deleteSlide: builder.mutation({
      query: (id) => ({
        url: API_ENDPOINTS.SLIDES.DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: [SLIDES],
    }),
    getSlidesByLesson: builder.query({
      query: ({ lessonId, page = 1, pageSize = 100 }) => ({
        url: API_ENDPOINTS.SLIDES.GET_BY_LESSON(lessonId),
        method: "GET",
        params: {
          page,
          pageSize,
        },
      }),
      providesTags: [SLIDES],
    }),
  }),
});

export const {
  useGetSlideByIdQuery,
  useCreateSlideMutation,
  useUpdateSlideMutation,
  useDeleteSlideMutation,
  useGetSlidesByLessonQuery,
} = slidesAPI;

import { baseAPI } from "./base-api";
import { API_ENDPOINTS } from "@/constants/api-endpoints";
import { BASE_URL } from "@/config";

export const uploadAPI = baseAPI.injectEndpoints({
  endpoints: (builder) => ({
    uploadImage: builder.mutation<string, File>({
      queryFn: async (file) => {
        try {
          const formData = new FormData();
          formData.append("file", file);

          // Get token from localStorage (redux-persist format)
          let token: string | null = null;
          try {
            const persisted = localStorage.getItem("persist:root");
            if (persisted) {
              const parsed = JSON.parse(persisted);
              const auth = parsed.auth ? JSON.parse(parsed.auth) : {};
              token = auth.accessToken || null;
            }
          } catch (e) {
            console.warn("Could not get token from localStorage", e);
          }

          const headers: HeadersInit = {};
          if (token) {
            headers["Authorization"] = `Bearer ${token}`;
          }
          // Don't set Content-Type - browser will set it with boundary for multipart/form-data

          const response = await fetch(
            `${BASE_URL}${API_ENDPOINTS.UPLOAD.IMAGE}`,
            {
              method: "POST",
              headers,
              body: formData,
            }
          );

          if (!response.ok) {
            const error = await response
              .json()
              .catch(() => ({ message: "Upload failed" }));
            return { error: error.message || "Upload failed" };
          }

          const data = await response.json();
          return { data: data.data || data.success ? data.data : null };
        } catch (error: any) {
          return { error: error.message || "Upload failed" };
        }
      },
    }),
    deleteImage: builder.mutation<boolean, string>({
      query: (fileName) => ({
        url: API_ENDPOINTS.UPLOAD.DELETE_IMAGE(fileName),
        method: "DELETE",
      }),
    }),
  }),
});

// Image upload endpoint now handles both images and videos
export const { useUploadImageMutation, useDeleteImageMutation } = uploadAPI;

// Alias hooks for video uploads (uses the same image endpoint which handles both)
export const useUploadVideoMutation = useUploadImageMutation;
export const useDeleteVideoMutation = useDeleteImageMutation;

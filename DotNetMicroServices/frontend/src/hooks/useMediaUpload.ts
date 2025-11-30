import { useUploadImageMutation, useUploadVideoMutation } from "@/services/upload-api";

export const useMediaUpload = () => {
  const [uploadImage, { isLoading: isUploadingImage }] = useUploadImageMutation();
  const [uploadVideo, { isLoading: isUploadingVideo }] = useUploadVideoMutation();

  const handleImageUpload = async (file: File): Promise<string> => {
    try {
      const result = await uploadImage(file).unwrap();
      return result;
    } catch (error) {
      console.error("Failed to upload image:", error);
      throw error;
    }
  };

  const handleVideoUpload = async (file: File): Promise<string> => {
    try {
      const result = await uploadVideo(file).unwrap();
      return result;
    } catch (error) {
      console.error("Failed to upload video:", error);
      throw error;
    }
  };

  return {
    handleImageUpload,
    handleVideoUpload,
    isUploadingImage,
    isUploadingVideo,
  };
};


import { useState, useEffect } from "react";
import {
  useGetSlidesByLessonQuery,
  useCreateSlideMutation,
  useUpdateSlideMutation,
} from "@/services/slides-api";

export const useSlides = (lessonId: string | null, isQuizLesson: boolean) => {
  const [selectedSlide, setSelectedSlide] = useState<any>(null);

  // Reset selected slide when lessonId changes or is cleared
  useEffect(() => {
    setSelectedSlide(null);
  }, [lessonId]);

  const {
    data: slidesData,
    isLoading: slidesLoading,
    refetch: refetchSlides,
  } = useGetSlidesByLessonQuery(
    {
      lessonId: lessonId || "",
      page: 1,
      pageSize: 100,
    },
    {
      skip: !lessonId || isQuizLesson || lessonId === "",
      // Force refetch when lessonId changes to avoid stale cache
      refetchOnMountOrArgChange: true,
    }
  );

  const [createSlide, { isLoading: isCreatingSlide }] = useCreateSlideMutation();
  const [updateSlide, { isLoading: isUpdatingSlide }] = useUpdateSlideMutation();

  const getSlidesArray = () => {
    return Array.isArray(slidesData?.data)
      ? slidesData.data
      : Array.isArray(slidesData?.data?.items)
      ? slidesData.data.items
      : Array.isArray(slidesData?.items)
      ? slidesData.items
      : Array.isArray(slidesData)
      ? slidesData
      : [];
  };

  return {
    slidesData,
    slidesLoading,
    selectedSlide,
    setSelectedSlide,
    isCreatingSlide,
    isUpdatingSlide,
    createSlide,
    updateSlide,
    refetchSlides,
    getSlidesArray,
  };
};


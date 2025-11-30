import { useParams } from "next/navigation";
import { useGetCourseByIdQuery, useUpdateCourseMutation } from "@/services/courses-api";

export const useCourseData = () => {
  const params = useParams();
  const courseId = params.id as string;

  const { data: courseData, isLoading: courseLoading } = useGetCourseByIdQuery(courseId);
  const [updateCourse, { isLoading: isPublishing }] = useUpdateCourseMutation();

  const course = courseData?.data;

  const handlePublish = async () => {
    if (!courseData?.data) return;
    try {
      const course = courseData.data;
      await updateCourse({
        id: courseId,
        ...course,
        Status: "Published",
      }).unwrap();
      alert("Course published successfully!");
    } catch (error) {
      console.error("Failed to publish course:", error);
      alert("Failed to publish course. Please try again.");
    }
  };

  return {
    courseId,
    course,
    courseLoading,
    isPublishing,
    handlePublish,
  };
};


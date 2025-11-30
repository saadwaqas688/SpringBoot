import { useState, useCallback, useRef, useEffect } from "react";
import {
  useGetLessonsByCourseQuery,
  useCreateLessonMutation,
  useUpdateLessonMutation,
  useDeleteLessonMutation,
} from "@/services/lessons-api";
import { arrayMove, DragEndEvent } from "@dnd-kit/sortable";

const pageSize = 10;
const initialDisplayCount = 10;

export const useLessons = (courseId: string) => {
  const [sortedLessons, setSortedLessons] = useState<any[]>([]);
  const [displayedLessons, setDisplayedLessons] = useState<any[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [refreshKey, setRefreshKey] = useState(0);
  const [editingLesson, setEditingLesson] = useState<any>(null);
  const [editTitle, setEditTitle] = useState("");
  const [selectedLesson, setSelectedLesson] = useState<any>(null);

  const isInitializedRef = useRef(false);
  const scrollLoadingRef = useRef(false);

  const {
    data: lessonsData,
    isLoading: lessonsLoading,
    isFetching,
    refetch: refetchLessons,
  } = useGetLessonsByCourseQuery({
    courseId,
    page: currentPage,
    pageSize,
  });

  const [createLesson, { isLoading: isCreating }] = useCreateLessonMutation();
  const [updateLesson, { isLoading: isUpdating }] = useUpdateLessonMutation();
  const [deleteLesson, { isLoading: isDeleting }] = useDeleteLessonMutation();

  // Reset pagination when courseId changes
  useEffect(() => {
    setCurrentPage(1);
    setDisplayedLessons([]);
    setSortedLessons([]);
    setHasMore(true);
    setRefreshKey(0);
    isInitializedRef.current = false;
    scrollLoadingRef.current = false;
  }, [courseId]);

  // Accumulate lessons from multiple pages
  useEffect(() => {
    if (!lessonsData) {
      if (currentPage === 1) {
        setSortedLessons([]);
        setDisplayedLessons([]);
        setHasMore(false);
        isInitializedRef.current = false;
      }
      return;
    }

    const lessonsArray = Array.isArray(lessonsData?.data)
      ? lessonsData.data
      : Array.isArray(lessonsData?.data?.items)
      ? lessonsData.data.items
      : Array.isArray(lessonsData?.items)
      ? lessonsData.items
      : Array.isArray(lessonsData)
      ? lessonsData
      : [];

    if (lessonsArray.length > 0) {
      setSortedLessons((prevSorted) => {
        if (currentPage === 1) {
          const sorted = [...lessonsArray].sort((a: any, b: any) => {
            const orderA = a.order || a.Order || 0;
            const orderB = b.order || b.Order || 0;
            if (orderA !== orderB) {
              return orderA - orderB;
            }
            const dateA = new Date(a.createdAt || a.CreatedAt || 0).getTime();
            const dateB = new Date(b.createdAt || b.CreatedAt || 0).getTime();
            return dateA - dateB;
          });
          return sorted;
        }

        const existingIds = new Set(prevSorted.map((l: any) => String(l.id || l.Id)));
        const existingMap = new Map(prevSorted.map((l: any) => [String(l.id || l.Id), l]));
        const currentIds = new Set(lessonsArray.map((l: any) => String(l.id || l.Id)));
        const keptExisting = prevSorted.filter((l: any) =>
          currentIds.has(String(l.id || l.Id))
        );

        const combined = [...keptExisting];
        lessonsArray.forEach((lesson: any) => {
          const lessonId = String(lesson.id || lesson.Id);
          const existingIndex = combined.findIndex(
            (l: any) => String(l.id || l.Id) === lessonId
          );
          if (existingIndex >= 0) {
            combined[existingIndex] = lesson;
          } else {
            combined.push(lesson);
          }
        });

        const sorted = combined.sort((a: any, b: any) => {
          const orderA = a.order || a.Order || 0;
          const orderB = b.order || b.Order || 0;
          if (orderA !== orderB) {
            return orderA - orderB;
          }
          const dateA = new Date(a.createdAt || a.CreatedAt || 0).getTime();
          const dateB = new Date(b.createdAt || b.CreatedAt || 0).getTime();
          return dateA - dateB;
        });

        return sorted;
      });

      setHasMore(lessonsArray.length === pageSize);
      scrollLoadingRef.current = false;
    } else if (currentPage === 1) {
      setSortedLessons([]);
      setDisplayedLessons([]);
      setHasMore(false);
      isInitializedRef.current = false;
    }
  }, [lessonsData, currentPage, refreshKey]);

  // Update displayed lessons
  useEffect(() => {
    if (sortedLessons.length > 0) {
      if (!isInitializedRef.current) {
        setDisplayedLessons(sortedLessons.slice(0, initialDisplayCount));
        isInitializedRef.current = true;
      } else {
        setDisplayedLessons((prev) => {
          const currentCount = prev.length;
          const newDisplayed = sortedLessons.slice(
            0,
            Math.max(currentCount, initialDisplayCount)
          );
          if (prev.length === 0 || prev.length !== newDisplayed.length) {
            return newDisplayed;
          }
          return newDisplayed;
        });
      }
    } else if (sortedLessons.length === 0) {
      setDisplayedLessons([]);
      isInitializedRef.current = false;
    }
  }, [sortedLessons]);

  const handleScroll = useCallback(
    (e: React.UIEvent<HTMLDivElement>) => {
      if (scrollLoadingRef.current || isFetching) return;

      const target = e.currentTarget;
      const scrollBottom =
        target.scrollHeight - target.scrollTop - target.clientHeight;

      if (scrollBottom < 100 && hasMore) {
        scrollLoadingRef.current = true;

        if (displayedLessons.length < sortedLessons.length) {
          const nextDisplayCount = Math.min(
            displayedLessons.length + pageSize,
            sortedLessons.length
          );
          setDisplayedLessons(sortedLessons.slice(0, nextDisplayCount));
          scrollLoadingRef.current = false;
        } else {
          setCurrentPage((prev) => prev + 1);
          setTimeout(() => {
            scrollLoadingRef.current = false;
          }, 500);
        }
      }
    },
    [hasMore, isFetching, displayedLessons.length, sortedLessons.length]
  );

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (!over || active.id === over.id) {
      return;
    }

    const oldIndex = sortedLessons.findIndex(
      (lesson) => String(lesson.id || lesson.Id) === active.id
    );
    const newIndex = sortedLessons.findIndex(
      (lesson) => String(lesson.id || lesson.Id) === over.id
    );

    if (oldIndex === -1 || newIndex === -1) {
      return;
    }

    const newSortedLessons = arrayMove(sortedLessons, oldIndex, newIndex);
    setSortedLessons(newSortedLessons);

    try {
      const updatePromises = newSortedLessons.map((lesson: any, index: number) => {
        const newOrder = index + 1;
        const lessonId = lesson.id || lesson.Id;
        const currentOrder = lesson.order || lesson.Order || 0;
        if (currentOrder !== newOrder) {
          return updateLesson({
            id: lessonId,
            Order: newOrder,
            Title: lesson.title || lesson.Title || "",
            Description: lesson.description || lesson.Description || "",
            LessonType: lesson.lessonType || lesson.LessonType || "standard",
            CourseId: courseId,
          }).unwrap();
        }
        return Promise.resolve();
      });

      await Promise.all(updatePromises);
    } catch (error) {
      console.error("Failed to update lesson order:", error);
    }
  };

  const handleCreateLesson = async (lessonType: string) => {
    try {
      const lessonData = {
        CourseId: courseId,
        Title: `Untitled ${lessonType}`,
        LessonType: lessonType,
        Order: sortedLessons.length + 1,
      };
      await createLesson(lessonData).unwrap();
      setCurrentPage(1);
      isInitializedRef.current = false;
      await refetchLessons();
    } catch (error) {
      console.error("Failed to create lesson:", error);
    }
  };

  const handleEditClick = (e: React.MouseEvent, lesson: any) => {
    e.stopPropagation();
    const lessonId = lesson.id || lesson.Id;
    setEditingLesson({ ...lesson, id: lessonId, Id: lessonId });
    setEditTitle(lesson.title || lesson.Title || lesson.name || lesson.Name || "");
  };

  const handleSaveEdit = async () => {
    if (!editingLesson || !editTitle.trim()) return;

    const lessonId = editingLesson.id || editingLesson.Id;
    const lessonIdString = String(lessonId);

    try {
      const updateData = {
        id: lessonId,
        Title: editTitle,
        Description: editingLesson.Description || editingLesson.description || "",
        LessonType: editingLesson.LessonType || editingLesson.lessonType || "standard",
        Order: editingLesson.Order || editingLesson.order || 0,
        CourseId: courseId,
      };

      const updatedLesson = {
        ...editingLesson,
        title: editTitle,
        Title: editTitle,
        id: lessonId,
        Id: lessonId,
      };

      setSortedLessons((prev) => {
        return prev.map((lesson) => {
          const lessonIdMatch = String(lesson.id || lesson.Id);
          if (lessonIdMatch === lessonIdString) {
            return updatedLesson;
          }
          return lesson;
        });
      });

      setDisplayedLessons((prev) => {
        return prev.map((lesson) => {
          const lessonIdMatch = String(lesson.id || lesson.Id);
          if (lessonIdMatch === lessonIdString) {
            return updatedLesson;
          }
          return lesson;
        });
      });

      setEditingLesson(null);
      setEditTitle("");

      await updateLesson(updateData).unwrap();
    } catch (error) {
      console.error("Failed to update lesson:", error);
      await refetchLessons();
      setEditingLesson(editingLesson);
      setEditTitle(editTitle);
    }
  };

  const handleCancelEdit = () => {
    setEditingLesson(null);
    setEditTitle("");
  };

  const handleDeleteClick = (e: React.MouseEvent, lesson: any) => {
    e.stopPropagation();
    setSelectedLesson(lesson);
  };

  const handleDeleteLesson = async () => {
    if (!selectedLesson) return;
    try {
      const lessonId = selectedLesson.id || selectedLesson.Id;
      await deleteLesson(lessonId).unwrap();
      setCurrentPage(1);
      isInitializedRef.current = false;
      await refetchLessons();
      setSelectedLesson(null);
    } catch (error) {
      console.error("Failed to delete lesson:", error);
    }
  };

  const formatDuration = (duration?: number | string): string => {
    if (!duration) return "";
    const num = typeof duration === "string" ? parseFloat(duration) : duration;
    if (isNaN(num)) return "";
    const hours = Math.floor(num / 60);
    const minutes = num % 60;
    if (hours > 0) {
      return `${hours}h ${minutes}m`;
    }
    return `${minutes}m`;
  };

  return {
    displayedLessons,
    sortedLessons,
    lessonsLoading,
    isFetching,
    isCreating,
    isUpdating,
    isDeleting,
    editingLesson,
    editTitle,
    setEditTitle,
    selectedLesson,
    handleScroll,
    handleDragEnd,
    handleCreateLesson,
    handleEditClick,
    handleSaveEdit,
    handleCancelEdit,
    handleDeleteClick,
    handleDeleteLesson,
    formatDuration,
    refetchLessons,
  };
};


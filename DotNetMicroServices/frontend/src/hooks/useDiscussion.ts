import { useState, useRef, useCallback, useEffect } from "react";
import {
  useGetDiscussionByLessonQuery,
  useCreateDiscussionMutation,
  useUpdateDiscussionMutation,
} from "@/services/discussions-api";

export const useDiscussion = (
  lessonId: string | null,
  isDiscussionLesson: boolean
) => {
  const [description, setDescription] = useState("");
  const autoSaveTimerRef = useRef<NodeJS.Timeout | null>(null);
  const hasUnsavedChangesRef = useRef(false);
  const isInitializingRef = useRef(false);

  const {
    data: discussionData,
    isLoading: discussionLoading,
    error: discussionError,
    refetch: refetchDiscussion,
  } = useGetDiscussionByLessonQuery(lessonId || "", {
    skip: !lessonId || !isDiscussionLesson,
    refetchOnMountOrArgChange: true,
  });

  // Handle different response structures
  const discussion = discussionData?.data || discussionData;
  const discussionId = discussion?.id || discussion?.Id || discussion?._id;

  const [createDiscussion, { isLoading: isCreating }] =
    useCreateDiscussionMutation();
  const [updateDiscussion, { isLoading: isUpdating }] =
    useUpdateDiscussionMutation();

  // Initialize description when discussion loads
  useEffect(() => {
    if (discussion && isDiscussionLesson && !isInitializingRef.current) {
      const discussionDescription =
        discussion.description || discussion.Description || "";
      setDescription(discussionDescription);
      hasUnsavedChangesRef.current = false;
    } else if (
      !discussion &&
      isDiscussionLesson &&
      lessonId &&
      !discussionLoading
    ) {
      // Discussion doesn't exist yet, initialize with empty description
      setDescription("");
      hasUnsavedChangesRef.current = false;
    }
  }, [discussion, isDiscussionLesson, lessonId, discussionLoading]);

  // Auto-save handler
  const handleAutoSave = useCallback(async () => {
    if (
      !lessonId ||
      !hasUnsavedChangesRef.current ||
      isInitializingRef.current ||
      isUpdating ||
      isCreating
    ) {
      return;
    }

    try {
      if (discussionId) {
        // Update existing discussion
        await updateDiscussion({
          id: discussionId,
          description,
          lessonId,
        }).unwrap();
        hasUnsavedChangesRef.current = false;
      } else {
        // Create new discussion
        const result = await createDiscussion({
          lessonId,
          description,
        }).unwrap();
        hasUnsavedChangesRef.current = false;
        // Refetch to get the new discussion
        await refetchDiscussion();
      }
    } catch (error) {
      console.error("Auto-save failed:", error);
    }
  }, [
    lessonId,
    description,
    discussionId,
    updateDiscussion,
    createDiscussion,
    refetchDiscussion,
    isUpdating,
    isCreating,
  ]);

  // Schedule auto-save with debounce
  const scheduleAutoSave = useCallback(() => {
    if (autoSaveTimerRef.current) {
      clearTimeout(autoSaveTimerRef.current);
    }

    hasUnsavedChangesRef.current = true;
    autoSaveTimerRef.current = setTimeout(() => {
      handleAutoSave();
    }, 5000); // 5 second debounce
  }, [handleAutoSave]);

  // Handle description change with auto-save
  const handleDescriptionChange = useCallback(
    (newDescription: string) => {
      setDescription(newDescription);
      if (!isInitializingRef.current) {
        scheduleAutoSave();
      }
    },
    [scheduleAutoSave]
  );

  // Reset discussion state
  const resetDiscussion = useCallback(() => {
    setDescription("");
    hasUnsavedChangesRef.current = false;
    isInitializingRef.current = false;
    if (autoSaveTimerRef.current) {
      clearTimeout(autoSaveTimerRef.current);
    }
  }, []);

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (autoSaveTimerRef.current) {
        clearTimeout(autoSaveTimerRef.current);
      }
    };
  }, []);

  return {
    discussion,
    discussionId,
    description,
    setDescription: handleDescriptionChange,
    discussionLoading,
    discussionError,
    isUpdating,
    isCreating,
    scheduleAutoSave,
    resetDiscussion,
    refetchDiscussion,
    isInitializingRef,
  };
};

import { useState, useRef, useCallback } from "react";
import { flushSync } from "react-dom";
import { useUpdateSlideMutation } from "@/services/slides-api";

export const useSlideEditor = () => {
  const [slideTitle, setSlideTitle] = useState("Bulleted list");
  const [bulletPoints, setBulletPoints] = useState<string[]>([
    "Has Several Points",
    "Displays each point with a bullet",
    "Is similar to a powerpoint slide",
  ]);
  const [doneText, setDoneText] = useState("Continue");
  const [isListExpanded, setIsListExpanded] = useState(true);
  const [isNarrationExpanded, setIsNarrationExpanded] = useState(false);
  const [expandableListItems, setExpandableListItems] = useState<
    { title: string; content: string }[]
  >([
    { title: "Item 1", content: "Content for item 1" },
    { title: "Item 2", content: "Content for item 2" },
  ]);
  const [focusMode, setFocusMode] = useState(false);
  const [promptText, setPromptText] = useState("Prompt Text");
  const [expandedItemIndex, setExpandedItemIndex] = useState<number | null>(null);
  const [imageUrl, setImageUrl] = useState<string>("");
  const [imageText, setImageText] = useState<string>("");
  const [imageCollection, setImageCollection] = useState<
    { url: string; alt?: string; text?: string }[]
  >([]);
  const [expandedImageIndex, setExpandedImageIndex] = useState<number | null>(null);
  const [videoUrl, setVideoUrl] = useState<string>("");
  const [videoText, setVideoText] = useState<string>("");
  const [videoThumbnail, setVideoThumbnail] = useState<string>("");
  const [videoCollection, setVideoCollection] = useState<
    { url: string; title?: string; thumbnail?: string }[]
  >([]);
  const [expandedVideoIndex, setExpandedVideoIndex] = useState<number | null>(null);

  const autoSaveTimerRef = useRef<NodeJS.Timeout | null>(null);
  const hasUnsavedChangesRef = useRef(false);
  const isInitializingSlideRef = useRef(false);

  const [updateSlide, { isLoading: isUpdatingSlide }] = useUpdateSlideMutation();

  const handleAutoSaveSlide = useCallback(async (selectedSlide: any, lessonId?: string) => {
    if (
      !selectedSlide ||
      !hasUnsavedChangesRef.current ||
      isInitializingSlideRef.current
    ) {
      return;
    }

    try {
      const slideId = selectedSlide.id || selectedSlide.Id;
      if (!slideId) {
        return;
      }

      const slideType = selectedSlide.type || selectedSlide.Type || "bulleted-list";
      let contentTitle = slideTitle || "Untitled Slide";
      let contentItems: Array<{
        text?: string;
        image?: string;
        video?: string;
        thumbnail?: string;
      }> = [];

      if (slideType === "expandable-list") {
        contentItems = expandableListItems.map((item) => ({
          text: `${item.title}\n${item.content}`,
        }));
      } else if (slideType === "single-image") {
        contentItems = [
          {
            text: imageText || "",
            image: imageUrl || "",
          },
        ];
      } else if (slideType === "image-collection") {
        contentItems = imageCollection.map((img) => ({
          text: img.text || img.alt || "",
          image: img.url || "",
        }));
      } else if (slideType === "single-video") {
        contentItems = [
          {
            text: videoText || "",
            video: videoUrl || "",
            thumbnail: videoThumbnail || "",
          },
        ];
      } else if (slideType === "video-collection") {
        contentItems = videoCollection.map((video) => ({
          text: video.title || "",
          video: video.url || "",
          thumbnail: video.thumbnail || "",
        }));
      } else {
        contentItems = bulletPoints.map((point) => ({ text: point }));
      }

      const updateData: any = {
        id: slideId,
        type: slideType, // Preserve the slide type
        title: contentTitle,
        content: {
          title: contentTitle,
          items: contentItems,
        },
        order: selectedSlide.order || selectedSlide.Order || 0,
      };

      if (lessonId) {
        updateData.lessonId = lessonId;
      }

      await updateSlide(updateData).unwrap();
      hasUnsavedChangesRef.current = false;
    } catch (error) {
      console.error("Auto-save failed:", error);
    }
  }, [
    slideTitle,
    bulletPoints,
    expandableListItems,
    imageUrl,
    imageText,
    imageCollection,
    videoUrl,
    videoText,
    videoThumbnail,
    videoCollection,
    updateSlide,
  ]);

  const scheduleAutoSave = useCallback((selectedSlide: any, lessonId?: string) => {
    if (autoSaveTimerRef.current) {
      clearTimeout(autoSaveTimerRef.current);
    }

    hasUnsavedChangesRef.current = true;
    autoSaveTimerRef.current = setTimeout(() => {
      handleAutoSaveSlide(selectedSlide, lessonId);
    }, 5000);
  }, [handleAutoSaveSlide]);

  const resetEditor = () => {
    setSlideTitle("Bulleted list");
    setBulletPoints([
      "Has Several Points",
      "Displays each point with a bullet",
      "Is similar to a powerpoint slide",
    ]);
    setDoneText("Continue");
    setIsListExpanded(true);
    setIsNarrationExpanded(false);
    setExpandableListItems([
      { title: "Item 1", content: "Content for item 1" },
      { title: "Item 2", content: "Content for item 2" },
    ]);
    setFocusMode(false);
    setPromptText("Prompt Text");
    setExpandedItemIndex(null);
    setImageUrl("");
    setImageText("");
    setImageCollection([]);
    setExpandedImageIndex(null);
    setVideoUrl("");
    setVideoText("");
    setVideoThumbnail("");
    setVideoCollection([]);
    setExpandedVideoIndex(null);
    hasUnsavedChangesRef.current = false;
    isInitializingSlideRef.current = false;
    if (autoSaveTimerRef.current) {
      clearTimeout(autoSaveTimerRef.current);
    }
  };

  return {
    // State
    slideTitle,
    setSlideTitle,
    bulletPoints,
    setBulletPoints,
    doneText,
    setDoneText,
    isListExpanded,
    setIsListExpanded,
    isNarrationExpanded,
    setIsNarrationExpanded,
    expandableListItems,
    setExpandableListItems,
    focusMode,
    setFocusMode,
    promptText,
    setPromptText,
    expandedItemIndex,
    setExpandedItemIndex,
    imageUrl,
    setImageUrl,
    imageText,
    setImageText,
    imageCollection,
    setImageCollection,
    expandedImageIndex,
    setExpandedImageIndex,
    videoUrl,
    setVideoUrl,
    videoText,
    setVideoText,
    videoThumbnail,
    setVideoThumbnail,
    videoCollection,
    setVideoCollection,
    expandedVideoIndex,
    setExpandedVideoIndex,
    // Refs
    isInitializingSlideRef,
    // Functions
    handleAutoSaveSlide,
    scheduleAutoSave,
    resetEditor,
    isUpdatingSlide,
  };
};


"use client";

import { useParams } from "next/navigation";
import { useState, useCallback } from "react";
import React from "react";
import { flushSync } from "react-dom";
import {
  Box,
  Typography,
  Button,
  Paper,
  List,
  ListItem,
  ListItemText,
  IconButton,
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemButton,
  Dialog,
  DialogTitle,
  DialogContent,
  Tabs,
  Tab,
  Grid,
  Card,
  CardContent,
  TextField,
  Collapse,
  Divider,
  Checkbox,
  FormControlLabel,
  Select,
  FormControl,
  InputLabel,
  Input,
  InputAdornment,
} from "@mui/material";
import {
  Add as AddIcon,
  CheckCircle as CheckCircleIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Close as CloseIcon,
  MoreVert as MoreVertIcon,
  Description as DescriptionIcon,
  Settings as SettingsIcon,
  Book as BookIcon,
  DragIndicator as DragIndicatorIcon,
  Image as ImageIcon,
  VideoLibrary as VideoIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  ArrowBack as ArrowBackIcon,
  ArrowForward as ArrowForwardIcon,
  ChevronRight as ChevronRightIcon,
  DeleteOutline as DeleteOutlineIcon,
  Person as PersonIcon,
  Menu as MenuIcon,
  PlayCircle as PlayCircleIcon,
} from "@mui/icons-material";
import {
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { DnDContextWrapper } from "@/components/dnd/DnDContextWrapper";
import { DnDSortableList } from "@/components/dnd/DnDSortableList";
import { DnDDraggableItem } from "@/components/dnd/DnDDraggableItem";
import {
  useGetCourseByIdQuery,
  useUpdateCourseMutation,
} from "@/services/courses-api";
import {
  useGetLessonsByCourseQuery,
  useCreateLessonMutation,
  useUpdateLessonMutation,
  useDeleteLessonMutation,
} from "@/services/lessons-api";
import {
  useGetSlidesByLessonQuery,
  useCreateSlideMutation,
  useUpdateSlideMutation,
} from "@/services/slides-api";
import {
  useGetQuizzesByLessonQuery,
  useGetQuestionsByQuizQuery,
} from "@/services/quizzes-api";
import {
  useUploadImageMutation,
  useUploadVideoMutation,
} from "@/services/upload-api";
import DeleteConfirmationModal from "@/components/DeleteConfirmationModal";
import MediaPreviewModal from "@/components/MediaPreviewModal";
import AssignUsersModal from "@/components/AssignUsersModal";
import QuizUploadModal from "@/components/QuizUploadModal";
import LessonsPanel from "@/components/course/LessonsPanel";
import ContentPreviewPanel from "@/components/course/ContentPreviewPanel";
import { CourseHeader } from "@/components/course/CourseHeader";
import { QuizDisplay } from "@/components/course/QuizDisplay";
import { QuizEditor } from "@/components/course/QuizEditor";
import { DiscussionPreview } from "@/components/course/DiscussionPreview";
import { DiscussionEditor } from "@/components/course/DiscussionEditor";
import { SlideLibraryDialog } from "@/components/course/SlideLibraryDialog";
import { SlidePreviewContainer } from "@/components/course/SlidePreviewContainer";
import { SlidePropertiesPanel } from "@/components/course/SlidePropertiesPanel";
import { MobileMenuButton } from "@/components/course/MobileMenuButton";
import { MobilePropertiesButton } from "@/components/course/MobilePropertiesButton";
import { MobileBackdrop } from "@/components/course/MobileBackdrop";
import { useCourseData } from "@/hooks/useCourseData";
import { useLessons } from "@/hooks/useLessons";
import { useSlides } from "@/hooks/useSlides";
import { useQuiz } from "@/hooks/useQuiz";
import { useDiscussion } from "@/hooks/useDiscussion";
import { useSlideEditor } from "@/hooks/useSlideEditor";
import { useModals } from "@/hooks/useModals";
import { useMediaUpload } from "@/hooks/useMediaUpload";
import { useLessonSelection } from "@/hooks/useLessonSelection";
import styled from "styled-components";

const LessonItem = styled(ListItem)`
  padding: 0.75rem 0;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  align-items: center;

  &:last-child {
    border-bottom: none;
  }
`;

const LessonContent = styled(Box)`
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
`;

const LessonActions = styled(Box)`
  display: flex;
  gap: 0.25rem;
  align-items: center;
`;

const EmptyStateContainer = styled(Paper)`
  flex: 1;
  padding: 4rem;
  min-height: 500px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background-color: #f9fafb;
`;

const EmptyStateIllustration = styled(Box)`
  width: 100%;
  max-width: 400px;
  height: 300px;
  background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 2rem;
  position: relative;
  overflow: hidden;

  &::before {
    content: "📄";
    font-size: 4rem;
    opacity: 0.3;
  }
`;

const LessonTypeMenu = styled(Menu)`
  .MuiPaper-root {
    border-radius: 8px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    min-width: 180px;
  }
`;

const LESSON_TYPES = [
  { value: "standard", label: "Standard", icon: <DescriptionIcon /> },
  { value: "discussion", label: "Discussion", icon: <DescriptionIcon /> },
  { value: "assignment", label: "Assignment", icon: <DescriptionIcon /> },
  { value: "practical", label: "Practical", icon: <DescriptionIcon /> },
  { value: "quiz", label: "Quiz", icon: <DescriptionIcon /> },
];

interface LessonItemContentProps {
  lesson: any;
  lessonId: string;
  isEditing: boolean;
  isSelected: boolean;
  editTitle: string;
  setEditTitle: (title: string) => void;
  handleSaveEdit: () => void;
  handleCancelEdit: () => void;
  handleEditClick: (e: React.MouseEvent, lesson: any) => void;
  handleDeleteClick: (e: React.MouseEvent, lesson: any) => void;
  handleLessonClick: (e: React.MouseEvent, lesson: any) => void;
  formatDuration: (duration?: number | string) => string;
}

const LessonItemContent: React.FC<LessonItemContentProps> = ({
  lesson,
  lessonId,
  isEditing,
  isSelected,
  editTitle,
  setEditTitle,
  handleSaveEdit,
  handleCancelEdit,
  handleEditClick,
  handleDeleteClick,
  handleLessonClick,
  formatDuration,
}) => {
  return (
    <LessonItem
      sx={{
        backgroundColor: isSelected ? "#f3f4f6" : "transparent",
        "&:hover": {
          backgroundColor: isSelected ? "#e5e7eb" : "#f9fafb",
        },
      }}
    >
      {isEditing ? (
        <Box
          sx={{
            display: "flex",
            gap: 1,
            width: "100%",
            alignItems: "center",
          }}
        >
          <input
            type="text"
            value={editTitle}
            onChange={(e) => setEditTitle(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") handleSaveEdit();
              if (e.key === "Escape") handleCancelEdit();
            }}
            autoFocus
            style={{
              flex: 1,
              padding: "8px 12px",
              border: "1px solid #6366f1",
              borderRadius: "6px",
              fontSize: "0.875rem",
              outline: "none",
            }}
          />
          <IconButton
            size="small"
            onClick={(e) => {
              e.stopPropagation();
              handleSaveEdit();
            }}
            onMouseDown={(e) => {
              e.stopPropagation();
            }}
            sx={{
              bgcolor: "#10b981",
              color: "white",
              "&:hover": { bgcolor: "#059669" },
            }}
          >
            <CheckCircleIcon fontSize="small" />
          </IconButton>
          <IconButton
            size="small"
            onClick={(e) => {
              e.stopPropagation();
              handleCancelEdit();
            }}
            onMouseDown={(e) => {
              e.stopPropagation();
            }}
            sx={{
              color: "#6b7280",
              "&:hover": { bgcolor: "#f3f4f6" },
            }}
          >
            <CloseIcon fontSize="small" />
          </IconButton>
        </Box>
      ) : (
        <Box
          sx={{
            display: "flex",
            gap: 1,
            width: "100%",
            alignItems: "center",
          }}
        >
          {!isEditing && (
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                cursor: "grab",
                color: "#9ca3af",
                "&:hover": {
                  color: "#6366f1",
                },
              }}
            >
              <DragIndicatorIcon fontSize="small" />
            </Box>
          )}
          <LessonContent
            sx={{
              flex: 1,
              cursor: "pointer",
            }}
            onClick={(e) => {
              if (!isEditing) {
                e.stopPropagation();
                handleLessonClick(e, lesson);
              }
            }}
          >
            <DescriptionIcon sx={{ color: "#6366f1", fontSize: 20 }} />
            <ListItemText
              primary={
                lesson.title ||
                lesson.Title ||
                lesson.name ||
                lesson.Name ||
                (lesson.lessonType || lesson.LessonType
                  ? `Untitled ${lesson.lessonType || lesson.LessonType}`
                  : "Untitled Lesson")
              }
              secondary={formatDuration(lesson.Duration || lesson.duration)}
              primaryTypographyProps={{
                fontSize: "0.875rem",
                fontWeight: 500,
              }}
              secondaryTypographyProps={{
                fontSize: "0.75rem",
                color: "text.secondary",
              }}
            />
          </LessonContent>
          <LessonActions>
            <IconButton
              size="small"
              onClick={(e) => {
                e.stopPropagation();
                handleEditClick(e, lesson);
              }}
              onMouseDown={(e) => {
                e.stopPropagation();
              }}
              sx={{
                color: "#6366f1",
                "&:hover": { bgcolor: "#eef2ff" },
              }}
            >
              <EditIcon fontSize="small" />
            </IconButton>
            <IconButton
              size="small"
              onClick={(e) => {
                e.stopPropagation();
                handleDeleteClick(e, lesson);
              }}
              onMouseDown={(e) => {
                e.stopPropagation();
              }}
              sx={{
                color: "#ef4444",
                "&:hover": { bgcolor: "#fee2e2" },
              }}
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </LessonActions>
        </Box>
      )}
    </LessonItem>
  );
};

export default function CourseDetailPage() {
  // Get user role from localStorage instead of Redux state (persists on refresh)
  const getUserRoleFromStorage = () => {
    if (typeof window === "undefined") return "";
    try {
      const userInfo = localStorage.getItem("user_info");
      if (userInfo) {
        const user = JSON.parse(userInfo);
        return user?.role || user?.Role || "";
      }
    } catch (error) {
      console.error("Error reading user info from localStorage:", error);
    }
    return "";
  };

  const userRole = getUserRoleFromStorage();
  const isUserRole = userRole.toLowerCase() === "user";

  const { courseId, course, courseLoading, isPublishing, handlePublish } =
    useCourseData();
  const lessons = useLessons(courseId);
  const lessonSelection = useLessonSelection();
  const slides = useSlides(
    lessonSelection.selectedLessonId,
    lessonSelection.isQuizLesson || lessonSelection.isDiscussionLesson
  );
  const quiz = useQuiz(
    lessonSelection.selectedLessonId,
    lessonSelection.isQuizLesson
  );
  const discussion = useDiscussion(
    lessonSelection.selectedLessonId,
    lessonSelection.isDiscussionLesson
  );
  const slideEditor = useSlideEditor();
  const modals = useModals();
  const mediaUpload = useMediaUpload();

  // Local state for complex logic
  const [isSlideEditorOpen, setIsSlideEditorOpen] = useState(false);
  const [mobileLessonsOpen, setMobileLessonsOpen] = useState(false);
  const [mobilePropertiesOpen, setMobilePropertiesOpen] = useState(false);
  const [isLessonTypeMenuOpen, setIsLessonTypeMenuOpen] = useState(false);
  const [lessonTypeMenuAnchor, setLessonTypeMenuAnchor] =
    useState<HTMLElement | null>(null);

  // Set up sensors for drag and drop
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  // Lesson handlers use hooks
  const handleDragEnd = lessons.handleDragEnd;
  const handleScroll = lessons.handleScroll;

  const handleLessonClick = (e: React.MouseEvent, lesson: any) => {
    e.stopPropagation();
    const previousLessonId =
      lessonSelection.selectedLessonForSlides?.id ||
      lessonSelection.selectedLessonForSlides?.Id;
    const newLessonId = lesson.id || lesson.Id;

    // If switching to a different lesson, reset all state
    if (previousLessonId && previousLessonId !== newLessonId) {
      // Reset slide editor state
      slideEditor.resetEditor();
      // Close slide editor
      setIsSlideEditorOpen(false);
      // Clear selected slide
      slides.setSelectedSlide(null);
    }

    lessonSelection.handleLessonClick(lesson);
    if (lessonSelection.isQuizLesson) {
      quiz.resetQuiz();
      slides.setSelectedSlide(null);
      discussion.resetDiscussion();
    } else if (lessonSelection.isDiscussionLesson) {
      quiz.resetQuiz();
      slides.setSelectedSlide(null);
    } else {
      quiz.resetQuiz();
      slides.setSelectedSlide(null);
      discussion.resetDiscussion();
    }
  };

  const handleConfirmDelete = async () => {
    const deletedLessonId =
      lessons.selectedLesson?.id || lessons.selectedLesson?.Id;
    const currentSelectedLessonId =
      lessonSelection.selectedLessonForSlides?.id ||
      lessonSelection.selectedLessonForSlides?.Id;

    await lessons.handleDeleteLesson();
    modals.setIsDeleteModalOpen(false);

    // If the deleted lesson was the currently selected lesson, clear the selection
    if (
      deletedLessonId &&
      currentSelectedLessonId &&
      String(deletedLessonId) === String(currentSelectedLessonId)
    ) {
      lessonSelection.setSelectedLessonForSlides(null);
      setIsSlideEditorOpen(false);
      slides.setSelectedSlide(null);
    }
  };

  const handleOpenSlideLibrary = (lessonOverride?: any) => {
    console.log("=== handleOpenSlideLibrary CALLED ===");
    console.log("lessonOverride parameter:", lessonOverride);
    console.log(
      "lessonSelection.selectedLessonForSlides:",
      lessonSelection.selectedLessonForSlides
    );
    console.log("lessonSelection.isQuizLesson:", lessonSelection.isQuizLesson);
    console.log(
      "modals.isQuizUploadModalOpen (before):",
      modals.isQuizUploadModalOpen
    );
    console.log(
      "modals.isSlideLibraryOpen (before):",
      modals.isSlideLibraryOpen
    );

    // Use provided lesson or fall back to selected lesson
    const currentLesson =
      lessonOverride || lessonSelection.selectedLessonForSlides;
    console.log("currentLesson determined:", currentLesson);

    if (!currentLesson) {
      console.error("ERROR: No current lesson available");
      alert("Please select a lesson first before adding a slide.");
      return;
    }

    // For user role, don't allow opening slide library
    if (isUserRole) {
      return;
    }

    // Ensure the lesson is selected in state if it was passed as override
    if (
      lessonOverride &&
      lessonOverride !== lessonSelection.selectedLessonForSlides
    ) {
      console.log("Setting selected lesson in state:", lessonOverride);
      lessonSelection.setSelectedLessonForSlides(lessonOverride);
    }

    // Check lesson type directly from the lesson
    const lessonType =
      currentLesson.lessonType || currentLesson.LessonType || "standard";
    console.log("Lesson type from currentLesson:", lessonType);

    // If it's a quiz lesson, open quiz upload modal instead
    if (lessonType.toLowerCase() === "quiz") {
      console.log("Opening quiz upload modal");
      modals.setIsQuizUploadModalOpen(true);
      console.log(
        "setIsQuizUploadModalOpen called, state will update in next render"
      );
      return;
    }

    // If it's a discussion lesson, don't open slide library
    if (lessonType.toLowerCase() === "discussion") {
      console.log("Discussion lesson - no slide library needed");
      return;
    }

    console.log("Opening slide library");
    modals.setIsSlideLibraryOpen(true);
    modals.setSelectedSlideTab(0);
    console.log(
      "modals.isSlideLibraryOpen (after):",
      modals.isSlideLibraryOpen
    );
  };

  const handleCloseSlideLibrary = () => {
    modals.setIsSlideLibraryOpen(false);
    modals.setSelectedSlideTab(0);
  };

  const handleAssign = () => {
    modals.setIsAssignModalOpen(true);
  };

  const handleAddLessonClick = (
    buttonRef: React.RefObject<HTMLButtonElement>
  ) => {
    if (buttonRef.current) {
      setLessonTypeMenuAnchor(buttonRef.current);
      setIsLessonTypeMenuOpen(true);
    }
  };

  const handleCloseLessonTypeMenu = () => {
    setIsLessonTypeMenuOpen(false);
    setLessonTypeMenuAnchor(null);
  };

  // Handler for CourseHeader menu (different from lesson type menu)
  const handleCourseMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    modals.setAnchorEl(event.currentTarget);
  };

  const handleCreateSlide = async (slideType: string) => {
    // If no lesson is selected, try to auto-select the first lesson
    let currentLesson = lessonSelection.selectedLessonForSlides;

    if (!currentLesson) {
      const lessonsArray = lessons.displayedLessons || [];
      if (lessonsArray.length > 0) {
        const firstStandardLesson = lessonsArray.find((lesson: any) => {
          const lessonType =
            lesson.lessonType || lesson.LessonType || "standard";
          return lessonType.toLowerCase() !== "quiz";
        });

        if (firstStandardLesson) {
          // Create a synthetic event for handleLessonClick
          const syntheticEvent = {
            stopPropagation: () => {},
          } as React.MouseEvent;
          handleLessonClick(syntheticEvent, firstStandardLesson);
          // Use the lesson directly since we just selected it
          currentLesson = firstStandardLesson;
        } else {
          alert("Please select a lesson first before adding a slide.");
          modals.setIsSlideLibraryOpen(false);
          return;
        }
      } else {
        alert("Please create a lesson first before adding a slide.");
        modals.setIsSlideLibraryOpen(false);
        return;
      }
    }

    // Final check
    if (!currentLesson) {
      console.error("Cannot create slide: No lesson selected");
      alert("Please select a lesson first before adding a slide.");
      modals.setIsSlideLibraryOpen(false);
      return;
    }

    try {
      // Use currentLesson which might be the auto-selected one
      const selectedLesson =
        lessonSelection.selectedLessonForSlides || currentLesson;
      const lessonId = selectedLesson.id || selectedLesson.Id;
      if (!lessonId) {
        console.error("Cannot create slide: Lesson ID is missing");
        alert("Invalid lesson selected. Please try again.");
        return;
      }

      const slidesArray = slides.getSlidesArray();
      const nextOrder = slidesArray.length;

      let contentTitle = "";
      let contentItems: Array<{
        text?: string;
        image?: string;
        video?: string;
        thumbnail?: string;
      }> = [];

      if (slideType === "bulleted-list") {
        contentTitle = "Bulleted list";
        contentItems = [
          { text: "Has Several Points" },
          { text: "Displays each point with a bullet" },
          { text: "Is similar to a powerpoint slide" },
        ];
      } else if (slideType === "comparison") {
        contentTitle = "Comparison";
        contentItems = [];
      } else if (slideType === "expandable-list") {
        contentTitle = "Expandable list";
        contentItems = [];
      } else if (slideType === "single-image") {
        contentTitle = "Single Image";
        contentItems = [{ text: "", image: "", thumbnail: "" }];
      } else if (slideType === "image-collection") {
        contentTitle = "Image Collection";
        // Initialize with one empty image item so preview can render
        contentItems = [{ text: "", image: "" }];
      } else if (slideType === "single-video") {
        contentTitle = "Single Video";
        contentItems = [{ text: "", video: "", thumbnail: "" }];
      } else if (slideType === "video-collection") {
        contentTitle = "Video Collection";
        // Initialize with one empty video item so preview can render
        contentItems = [{ text: "", video: "", thumbnail: "" }];
      }

      const slideData = {
        lessonId: String(lessonId),
        type: slideType,
        title: contentTitle,
        content: {
          title: contentTitle,
          items: contentItems,
        },
        order: nextOrder,
      };

      console.log("Creating slide with data:", slideData);
      const result = await slides.createSlide(slideData).unwrap();
      console.log("Slide created successfully:", result);

      modals.setIsSlideLibraryOpen(false);
      await slides.refetchSlides();

      if (
        slideType === "bulleted-list" ||
        slideType === "expandable-list" ||
        slideType === "single-image" ||
        slideType === "image-collection" ||
        slideType === "single-video" ||
        slideType === "video-collection"
      ) {
        const createdSlide = result?.data || result;
        const selectedLesson =
          lessonSelection.selectedLessonForSlides || currentLesson;
        if (createdSlide && selectedLesson) {
          slideEditor.isInitializingSlideRef.current = true;
          slides.setSelectedSlide({
            ...createdSlide,
            id: createdSlide.id || createdSlide.Id,
            Id: createdSlide.Id || createdSlide.id,
          });
          setIsSlideEditorOpen(true);
          populateSlideEditor(createdSlide);
          setTimeout(() => {
            slideEditor.isInitializingSlideRef.current = false;
          }, 100);
        }
      }
    } catch (error: any) {
      console.error("Failed to create slide:", error);
      const errorMessage =
        error?.data?.message ||
        error?.message ||
        "Failed to create slide. Please try again.";
      alert(errorMessage);
    }
  };

  // Auto-save effect - debounced to 5 seconds
  React.useEffect(() => {
    if (!isSlideEditorOpen || !slides.selectedSlide) {
      return;
    }

    if (slideEditor.isInitializingSlideRef.current) {
      return;
    }

    const lessonId =
      lessonSelection.selectedLessonForSlides?.id ||
      lessonSelection.selectedLessonForSlides?.Id;
    slideEditor.scheduleAutoSave(
      slides.selectedSlide,
      lessonId ? String(lessonId) : undefined
    );

    return () => {
      // Cleanup handled in hook
    };
  }, [
    slideEditor.slideTitle,
    slideEditor.bulletPoints,
    slideEditor.doneText,
    slideEditor.expandableListItems,
    slideEditor.focusMode,
    slideEditor.promptText,
    slideEditor.imageUrl,
    slideEditor.imageText,
    slideEditor.imageCollection,
    slideEditor.videoUrl,
    slideEditor.videoText,
    slideEditor.videoCollection,
    isSlideEditorOpen,
    slides.selectedSlide,
    slideEditor.scheduleAutoSave,
    lessonSelection.selectedLessonForSlides,
  ]);

  // Auto-save effect for discussions - debounced to 5 seconds
  React.useEffect(() => {
    if (
      !lessonSelection.isDiscussionLesson ||
      !lessonSelection.selectedLessonForSlides
    ) {
      return;
    }

    if (discussion.isInitializingRef.current) {
      return;
    }

    // Auto-save is handled in the hook via scheduleAutoSave
    discussion.scheduleAutoSave();

    return () => {
      // Cleanup handled in hook
    };
  }, [
    discussion.description,
    lessonSelection.isDiscussionLesson,
    lessonSelection.selectedLessonForSlides,
    discussion.scheduleAutoSave,
  ]);

  const formatDuration = lessons.formatDuration;

  const getLessonTypeLabel = (lesson: any) => {
    const type = lesson.LessonType || lesson.lessonType || "standard";
    return LESSON_TYPES.find((t) => t.value === type)?.label || type;
  };

  // Helper function to populate slide editor data
  const populateSlideEditor = (slide: any) => {
    // Reset auto-save state
    if (slideEditor.isInitializingSlideRef.current) {
      clearTimeout(slideEditor.isInitializingSlideRef.current as any);
    }
    slideEditor.isInitializingSlideRef.current = true;

    slides.setSelectedSlide(slide);
    setIsSlideEditorOpen(true);

    let slideType =
      slide.type ||
      slide.Type ||
      slide.slideType ||
      slide.SlideType ||
      "bulleted-list";
    // Handle legacy "text" type as bulleted-list
    if (slideType === "text") {
      slideType = "bulleted-list";
    }

    const contentObj = slide.content || slide.Content || {};
    const contentTitle = contentObj?.title || contentObj?.Title || "";
    const contentItems = contentObj?.items || contentObj?.Items || [];

    slideEditor.setSlideTitle(
      contentTitle ||
        slide.title ||
        slide.Title ||
        (slideType === "expandable-list"
          ? "Expandable list"
          : slideType === "single-image"
          ? "Single Image"
          : slideType === "image-collection"
          ? "Image Collection"
          : slideType === "single-video"
          ? "Single Video"
          : slideType === "video-collection"
          ? "Video Collection"
          : "Bulleted list")
    );

    if (slideType === "expandable-list") {
      if (Array.isArray(contentItems) && contentItems.length > 0) {
        const expandedItems = contentItems.map((item: any) => {
          // Handle both lowercase and uppercase property names
          const textValue = item.text || item.Text || "";
          const textParts = textValue.split("\n");
          return {
            title: textParts[0] || "",
            content: textParts.slice(1).join("\n") || "",
          };
        });
        slideEditor.setExpandableListItems(expandedItems);
      } else {
        slideEditor.setExpandableListItems([
          { title: "Item 1", content: "Content for item 1" },
          { title: "Item 2", content: "Content for item 2" },
        ]);
      }
      slideEditor.setFocusMode(false);
      slideEditor.setPromptText("Prompt Text");
      slideEditor.setDoneText("Continue");
      slideEditor.setExpandedItemIndex(null);
    } else if (slideType === "single-image") {
      const firstItem = contentItems[0] || {};
      slideEditor.setImageUrl(firstItem.image || firstItem.Image || "");
      slideEditor.setImageText(firstItem.text || firstItem.Text || "");
    } else if (slideType === "image-collection") {
      if (Array.isArray(contentItems) && contentItems.length > 0) {
        const images = contentItems.map((item: any) => ({
          url: item.image || item.Image || "",
          text: item.text || item.Text || "",
          alt: item.text || item.Text || "",
        }));
        // Filter out completely empty items (no url) but keep at least one empty item for editing
        const filteredImages = images.filter((img) => img.url || img.text);
        const finalImages =
          filteredImages.length > 0
            ? filteredImages
            : [{ url: "", text: "", alt: "" }];
        slideEditor.setImageCollection(finalImages);
        // Auto-expand the first item if it's empty (new slide) so upload option is visible
        if (finalImages.length > 0 && !finalImages[0].url) {
          slideEditor.setExpandedImageIndex(0);
        } else {
          slideEditor.setExpandedImageIndex(null);
        }
      } else {
        // Initialize with one empty item so preview can render
        slideEditor.setImageCollection([{ url: "", text: "", alt: "" }]);
        // Auto-expand the first item so upload option is visible
        slideEditor.setExpandedImageIndex(0);
      }
    } else if (slideType === "single-video") {
      const firstItem = contentItems[0] || {};
      slideEditor.setVideoUrl(firstItem.video || firstItem.Video || "");
      slideEditor.setVideoText(firstItem.text || firstItem.Text || "");
      slideEditor.setVideoThumbnail(
        firstItem.thumbnail || firstItem.Thumbnail || ""
      );
    } else if (slideType === "video-collection") {
      if (Array.isArray(contentItems) && contentItems.length > 0) {
        const videos = contentItems.map((item: any) => ({
          url: item.video || item.Video || "",
          title: item.text || item.Text || "",
          thumbnail: item.thumbnail || item.Thumbnail || "",
        }));
        // Filter out completely empty items (no url) but keep at least one empty item for editing
        const filteredVideos = videos.filter(
          (video) => video.url || video.title || video.thumbnail
        );
        const finalVideos =
          filteredVideos.length > 0
            ? filteredVideos
            : [{ url: "", title: "", thumbnail: "" }];
        slideEditor.setVideoCollection(finalVideos);
        // Auto-expand the first item if it's empty (new slide) so upload option is visible
        if (finalVideos.length > 0 && !finalVideos[0].url) {
          slideEditor.setExpandedVideoIndex(0);
        } else {
          slideEditor.setExpandedVideoIndex(null);
        }
      } else {
        // Initialize with one empty item so preview can render
        slideEditor.setVideoCollection([{ url: "", title: "", thumbnail: "" }]);
        // Auto-expand the first item so upload option is visible
        slideEditor.setExpandedVideoIndex(0);
      }
    } else {
      if (Array.isArray(contentItems) && contentItems.length > 0) {
        // Handle both lowercase and uppercase property names
        const bullets = contentItems.map(
          (item: any) => item.text || item.Text || ""
        );
        slideEditor.setBulletPoints(bullets);
      } else {
        slideEditor.setBulletPoints([
          "Has Several Points",
          "Displays each point with a bullet",
          "Is similar to a powerpoint slide",
        ]);
      }
      slideEditor.setDoneText("Continue");
    }

    slideEditor.setIsListExpanded(true);
    slideEditor.setIsNarrationExpanded(false);

    setTimeout(() => {
      slideEditor.isInitializingSlideRef.current = false;
    }, 100);
  };

  // Handler functions for slide editor - using hook state
  const handleAddBulletPoint = () => {
    slideEditor.setBulletPoints([...slideEditor.bulletPoints, ""]);
  };

  const handleUpdateBulletPoint = (index: number, value: string) => {
    const updated = [...slideEditor.bulletPoints];
    updated[index] = value;
    slideEditor.setBulletPoints(updated);
  };

  const handleDeleteBulletPoint = (index: number) => {
    const updated = slideEditor.bulletPoints.filter((_, i) => i !== index);
    slideEditor.setBulletPoints(updated);
  };

  const handleAddExpandableItem = () => {
    slideEditor.setExpandableListItems([
      ...slideEditor.expandableListItems,
      { title: "", content: "" },
    ]);
  };

  const handleUpdateExpandableItem = (
    index: number,
    field: "title" | "content",
    value: string
  ) => {
    const updated = [...slideEditor.expandableListItems];
    updated[index] = { ...updated[index], [field]: value };
    slideEditor.setExpandableListItems(updated);
  };

  const handleDeleteExpandableItem = (index: number) => {
    const updated = slideEditor.expandableListItems.filter(
      (_, i) => i !== index
    );
    slideEditor.setExpandableListItems(updated);
  };

  const handleToggleExpandableItem = (index: number) => {
    if (slideEditor.focusMode) {
      slideEditor.setExpandedItemIndex(
        slideEditor.expandedItemIndex === index ? null : index
      );
    } else {
      slideEditor.setExpandedItemIndex(
        slideEditor.expandedItemIndex === index ? null : index
      );
    }
  };

  const handleAddImageToCollection = () => {
    const newIndex = slideEditor.imageCollection.length;
    slideEditor.setImageCollection([
      ...slideEditor.imageCollection,
      { url: "", text: "", alt: "" },
    ]);
    // Auto-expand the newly added item so upload option is visible
    slideEditor.setExpandedImageIndex(newIndex);
  };

  const handleRemoveImageFromCollection = (index: number) => {
    const updated = slideEditor.imageCollection.filter((_, i) => i !== index);
    slideEditor.setImageCollection(updated);
    if (slideEditor.expandedImageIndex === index) {
      slideEditor.setExpandedImageIndex(null);
    } else if (
      slideEditor.expandedImageIndex !== null &&
      slideEditor.expandedImageIndex > index
    ) {
      slideEditor.setExpandedImageIndex(slideEditor.expandedImageIndex - 1);
    }
  };

  const handleToggleImageDropdown = (index: number) => {
    slideEditor.setExpandedImageIndex(
      slideEditor.expandedImageIndex === index ? null : index
    );
  };

  const handleImageFileUpload = async (index: number, file: File) => {
    try {
      const uploadedUrl = await mediaUpload.handleImageUpload(file);
      const updated = [...slideEditor.imageCollection];
      updated[index] = { ...updated[index], url: uploadedUrl };
      slideEditor.setImageCollection(updated);
      slideEditor.setExpandedImageIndex(null);
    } catch (error) {
      console.error("Failed to upload image:", error);
    }
  };

  const handleUpdateImageInCollection = (
    index: number,
    url?: string,
    text?: string
  ) => {
    const updated = [...slideEditor.imageCollection];
    updated[index] = {
      ...updated[index],
      ...(url !== undefined && { url }),
      ...(text !== undefined && { text }),
    };
    slideEditor.setImageCollection(updated);
  };

  const handleAddVideoToCollection = () => {
    const newIndex = slideEditor.videoCollection.length;
    slideEditor.setVideoCollection([
      ...slideEditor.videoCollection,
      { url: "", title: "", thumbnail: "" },
    ]);
    // Auto-expand the newly added item so upload option is visible
    slideEditor.setExpandedVideoIndex(newIndex);
  };

  const handleRemoveVideoFromCollection = (index: number) => {
    const updated = slideEditor.videoCollection.filter((_, i) => i !== index);
    slideEditor.setVideoCollection(updated);
    if (slideEditor.expandedVideoIndex === index) {
      slideEditor.setExpandedVideoIndex(null);
    } else if (
      slideEditor.expandedVideoIndex !== null &&
      slideEditor.expandedVideoIndex > index
    ) {
      slideEditor.setExpandedVideoIndex(slideEditor.expandedVideoIndex - 1);
    }
  };

  const handleToggleVideoDropdown = (index: number) => {
    slideEditor.setExpandedVideoIndex(
      slideEditor.expandedVideoIndex === index ? null : index
    );
  };

  const handleVideoFileUpload = async (index: number, file: File) => {
    try {
      const uploadedUrl = await mediaUpload.handleVideoUpload(file);
      const updated = [...slideEditor.videoCollection];
      updated[index] = { ...updated[index], url: uploadedUrl };
      slideEditor.setVideoCollection(updated);
      slideEditor.setExpandedVideoIndex(null);
    } catch (error) {
      console.error("Failed to upload video:", error);
    }
  };

  const handleUpdateVideoInCollection = (
    index: number,
    url?: string,
    thumbnail?: string,
    title?: string
  ) => {
    const updated = [...slideEditor.videoCollection];
    updated[index] = {
      ...updated[index],
      ...(url !== undefined && { url }),
      ...(thumbnail !== undefined && { thumbnail }),
      ...(title !== undefined && { title }),
    };
    slideEditor.setVideoCollection(updated);
  };

  const handleCloseSlideEditor = () => {
    setIsSlideEditorOpen(false);
    slides.setSelectedSlide(null);
  };

  // Helper function to get slides array
  // Auto-select first slide when standard lesson is selected and slides are loaded
  React.useEffect(() => {
    const currentLessonId =
      lessonSelection.selectedLessonForSlides?.id ||
      lessonSelection.selectedLessonForSlides?.Id;

    // Reset editor state when lesson changes
    if (
      currentLessonId &&
      lessonSelection.lastAutoSelectedLessonId &&
      lessonSelection.lastAutoSelectedLessonId !== currentLessonId
    ) {
      slideEditor.resetEditor();
      setIsSlideEditorOpen(false);
      slides.setSelectedSlide(null);
    }

    if (
      currentLessonId &&
      !lessonSelection.isQuizLesson &&
      !lessonSelection.isDiscussionLesson &&
      !slides.slidesLoading &&
      slides.slidesData &&
      !isSlideEditorOpen &&
      lessonSelection.lastAutoSelectedLessonId !== currentLessonId
    ) {
      const slidesArray = slides.getSlidesArray();
      // Verify that slides belong to the current lesson
      const validSlides = slidesArray.filter((slide: any) => {
        const slideLessonId = slide.lessonId || slide.LessonId;
        return String(slideLessonId) === String(currentLessonId);
      });

      if (validSlides.length > 0) {
        const sortedSlides = [...validSlides].sort(
          (a: any, b: any) =>
            (a.order || a.Order || 0) - (b.order || b.Order || 0)
        );
        const firstSlide = sortedSlides[0];
        if (firstSlide) {
          lessonSelection.setLastAutoSelectedLessonId(currentLessonId);
          slides.setSelectedSlide(firstSlide);
          populateSlideEditor(firstSlide);
        }
      }
    }

    if (!currentLessonId) {
      lessonSelection.setLastAutoSelectedLessonId(null);
      slideEditor.resetEditor();
      setIsSlideEditorOpen(false);
      slides.setSelectedSlide(null);
    }
  }, [
    lessonSelection.selectedLessonForSlides?.id,
    lessonSelection.selectedLessonForSlides?.Id,
    lessonSelection.lastAutoSelectedLessonId,
    slides.slidesData,
    slides.slidesLoading,
    lessonSelection.isQuizLesson,
    isSlideEditorOpen,
  ]);

  // Calculate slide type for preview - handle legacy "text" type
  const getPreviewSlideType = (slide: any) => {
    const type =
      slide?.type ||
      slide?.Type ||
      slide?.slideType ||
      slide?.SlideType ||
      "bulleted-list";
    // Handle legacy "text" type as bulleted-list
    return type === "text" ? "bulleted-list" : type;
  };

  const previewSlideType = slides.selectedSlide
    ? getPreviewSlideType(slides.selectedSlide)
    : "bulleted-list";

  const previewIsSingleImage = previewSlideType === "single-image";
  const previewIsImageCollection = previewSlideType === "image-collection";
  const previewIsSingleVideo = previewSlideType === "single-video";
  const previewIsVideoCollection = previewSlideType === "video-collection";
  const previewIsExpandableList = previewSlideType === "expandable-list";
  const previewIsBulletedList = previewSlideType === "bulleted-list";

  // Navigation handlers for slides
  const handlePreviousSlide = async () => {
    if (!slides.selectedSlide || !lessonSelection.selectedLessonForSlides)
      return;
    const slidesArray = slides.getSlidesArray();
    const currentIndex = slidesArray.findIndex(
      (s: any) =>
        String(s.id || s.Id) ===
        String(slides.selectedSlide?.id || slides.selectedSlide?.Id)
    );
    if (currentIndex > 0) {
      const previousSlide = slidesArray[currentIndex - 1];
      if (previousSlide) {
        populateSlideEditor(previousSlide);
      }
    }
  };

  const handleNextSlide = async () => {
    if (!slides.selectedSlide || !lessonSelection.selectedLessonForSlides)
      return;
    const slidesArray = slides.getSlidesArray();
    const currentIndex = slidesArray.findIndex(
      (s: any) =>
        String(s.id || s.Id) ===
        String(slides.selectedSlide?.id || slides.selectedSlide?.Id)
    );
    if (currentIndex < slidesArray.length - 1) {
      const nextSlide = slidesArray[currentIndex + 1];
      if (nextSlide) {
        populateSlideEditor(nextSlide);
      }
    }
  };

  // If slide editor is open OR quiz lesson is selected OR discussion lesson is selected OR (user role with selected slide), show the preview panel
  // For user role, show preview panel but without editor panel
  if (
    (isSlideEditorOpen && slides.selectedSlide) ||
    (lessonSelection.isQuizLesson && lessonSelection.selectedLessonForSlides) ||
    (lessonSelection.isDiscussionLesson &&
      lessonSelection.selectedLessonForSlides) ||
    (isUserRole && slides.selectedSlide)
  ) {
    // Get the slide type - handle legacy "text" type
    // Only access slide properties if selectedSlide exists (not for quiz lessons)
    let slideType = "bulleted-list"; // default
    if (slides.selectedSlide) {
      slideType =
        slides.selectedSlide.type ||
        slides.selectedSlide.Type ||
        slides.selectedSlide.slideType ||
        slides.selectedSlide.SlideType ||
        "bulleted-list";

      // Handle legacy "text" type as bulleted-list
      if (slideType === "text") {
        slideType = "bulleted-list";
      }
    }

    // Check slide types
    const isExpandableList = slideType === "expandable-list";
    const isBulletedList = slideType === "bulleted-list";
    const isSingleImage = slideType === "single-image";
    const isImageCollection = slideType === "image-collection";
    const isSingleVideo = slideType === "single-video";
    const isVideoCollection = slideType === "video-collection";

    return (
      <>
        {/* Modals - Always render modals so they can open/close */}
        <DeleteConfirmationModal
          open={modals.isDeleteModalOpen}
          onClose={() => modals.setIsDeleteModalOpen(false)}
          onConfirm={handleConfirmDelete}
          title="Delete Lesson"
          message="Are you sure you want to delete this lesson? This action cannot be undone."
        />
        <AssignUsersModal
          open={modals.isAssignModalOpen}
          onClose={() => modals.setIsAssignModalOpen(false)}
          courseId={courseId}
        />
        {(() => {
          console.log("=== RENDERING QuizUploadModal (in early return) ===");
          console.log(
            "modals.isQuizUploadModalOpen:",
            modals.isQuizUploadModalOpen
          );
          const lessonIdValue =
            lessonSelection.selectedLessonForSlides?.id ||
            lessonSelection.selectedLessonForSlides?.Id ||
            lessonSelection.selectedLessonId ||
            "";
          console.log("lessonIdValue:", lessonIdValue);
          return (
            <QuizUploadModal
              open={modals.isQuizUploadModalOpen}
              onClose={() => {
                console.log("QuizUploadModal onClose called");
                modals.setIsQuizUploadModalOpen(false);
              }}
              lessonId={lessonIdValue}
              onUpload={async (file, quizScore) => {
                console.log("QuizUploadModal onUpload called");
                quiz.resetQuiz();
              }}
            />
          );
        })()}
        <MediaPreviewModal
          open={modals.isPreviewModalOpen}
          onClose={modals.handleClosePreview}
          url={modals.previewUrl}
          type={modals.previewType}
        />
        <SlideLibraryDialog
          open={modals.isSlideLibraryOpen}
          onClose={() => modals.setIsSlideLibraryOpen(false)}
          selectedTab={modals.selectedSlideTab}
          onTabChange={modals.setSelectedSlideTab}
          onCreateSlide={handleCreateSlide}
        />

        {/* Mobile Backdrop */}
        {(mobileLessonsOpen || mobilePropertiesOpen) && (
          <Box
            sx={{
              display: { xs: "block", lg: "none" },
              position: "fixed",
              top: 0,
              left: 0,
              right: 0,
              bottom: 0,
              bgcolor: "rgba(0, 0, 0, 0.5)",
              zIndex: 1100,
            }}
            onClick={() => {
              setMobileLessonsOpen(false);
              setMobilePropertiesOpen(false);
            }}
          />
        )}

        <Box
          sx={{
            display: "flex",
            flexDirection: { xs: "column", lg: "row" },
            height: { xs: "auto", lg: "calc(100vh - 100px)" },
            gap: { xs: 1, lg: 2 },
            position: "relative",
            width: "100%",
            overflow: { xs: "hidden", lg: "visible" },
          }}
        >
          {/* Mobile Menu Button */}
          <IconButton
            sx={{
              display: { xs: "flex", lg: "none" },
              position: "fixed",
              top: { xs: 70, sm: 80 },
              left: 10,
              zIndex: 1200,
              bgcolor: "#6366f1",
              color: "white",
              width: 40,
              height: 40,
              boxShadow: 2,
              "&:hover": { bgcolor: "#4f46e5" },
            }}
            onClick={() => {
              setMobileLessonsOpen(!mobileLessonsOpen);
              if (mobilePropertiesOpen) setMobilePropertiesOpen(false);
            }}
          >
            <MenuIcon />
          </IconButton>

          {/* Left Panel - Lessons Outline */}
          <LessonsPanel
            key={`lessons-${lessons.displayedLessons?.length || 0}`}
            mobileLessonsOpen={mobileLessonsOpen}
            onCloseMobile={() => setMobileLessonsOpen(false)}
            displayedLessons={lessons.displayedLessons || []}
            selectedLessonForSlides={lessonSelection.selectedLessonForSlides}
            onLessonSelect={(lesson) => {
              lessonSelection.setSelectedLessonForSlides(lesson);
              if (!lesson) {
                setIsSlideEditorOpen(false);
              }
            }}
            onAddLessonClick={handleAddLessonClick}
            slidesData={slides.slidesData}
            selectedSlide={slides.selectedSlide}
            onSlideClick={(slide) => {
              // For user role, don't open editor - just view
              if (isUserRole) {
                return;
              }
              const slideType =
                slide.type || slide.Type || slide.slideType || slide.SlideType;
              if (
                slideType === "bulleted-list" ||
                slideType === "expandable-list" ||
                slideType === "single-image" ||
                slideType === "image-collection" ||
                slideType === "single-video" ||
                slideType === "video-collection" ||
                !slideType
              ) {
                populateSlideEditor(slide);
              }
            }}
            onOpenSlideLibrary={handleOpenSlideLibrary}
            isCreatingSlide={slides.isCreatingSlide}
            onCloseSlideEditor={() => setIsSlideEditorOpen(false)}
            editingLesson={lessons.editingLesson}
            editTitle={lessons.editTitle}
            setEditTitle={lessons.setEditTitle}
            handleEditClick={lessons.handleEditClick}
            handleSaveEdit={lessons.handleSaveEdit}
            handleCancelEdit={lessons.handleCancelEdit}
            handleDeleteClick={(e, lesson) => {
              lessons.handleDeleteClick(e, lesson);
              modals.setIsDeleteModalOpen(true);
            }}
            isUpdating={lessons.isUpdating}
            isDeleting={lessons.isDeleting}
            isUserRole={isUserRole}
          />

          {/* Middle Panel - Slide Preview */}
          <Paper
            sx={{
              flex: 1,
              p: { xs: 1, sm: 2, md: 3 },
              display: "flex",
              flexDirection: "column",
              backgroundColor: "#f9fafb",
              minWidth: 0, // Prevents flex item from overflowing
              width: { xs: "100%", lg: "auto" },
              minHeight: { xs: "calc(100vh - 200px)", lg: "auto" },
              overflow: "hidden",
              position: "relative",
            }}
          >
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: { xs: "flex-start", sm: "center" },
                mb: { xs: 2, md: 3 },
                flexDirection: { xs: "column", sm: "row" },
                gap: { xs: 1, sm: 0 },
              }}
            >
              <Typography
                variant="h5"
                sx={{
                  fontWeight: 600,
                  fontSize: { xs: "1.25rem", sm: "1.5rem", md: "1.75rem" },
                  wordBreak: "break-word",
                }}
              >
                {lessonSelection.selectedLessonForSlides?.title ||
                  lessonSelection.selectedLessonForSlides?.Title ||
                  "Untitled Lesson"}
              </Typography>
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 1,
                  flexShrink: 0,
                }}
              >
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ fontSize: { xs: "0.75rem", sm: "0.875rem" } }}
                >
                  {lessonSelection.isQuizLesson && quiz.quizQuestions.length > 0
                    ? `${quiz.currentQuestionIndex + 1}/${
                        quiz.quizQuestions.length
                      }`
                    : (() => {
                        const slidesArray = slides.getSlidesArray();
                        const currentIndex = slidesArray.findIndex(
                          (s: any) =>
                            String(s.id || s.Id) ===
                            String(
                              slides.selectedSlide?.id ||
                                slides.selectedSlide?.Id
                            )
                        );
                        return `${currentIndex >= 0 ? currentIndex + 1 : 1}/${
                          slidesArray.length || 1
                        }`;
                      })()}
                </Typography>
                {lessonSelection.isQuizLesson && !isUserRole && (
                  <Button
                    size="small"
                    variant="outlined"
                    onClick={() => modals.setIsQuizUploadModalOpen(true)}
                    sx={{ textTransform: "none", ml: 1 }}
                  >
                    Upload Quiz
                  </Button>
                )}
                <IconButton
                  size="small"
                  sx={{ display: { xs: "none", sm: "flex" } }}
                >
                  <MoreVertIcon fontSize="small" />
                </IconButton>
              </Box>
            </Box>

            {/* Slide Preview Content or Quiz Questions or Discussion */}
            {lessonSelection.isDiscussionLesson ? (
              <DiscussionPreview
                description={discussion.description}
                isLoading={discussion.discussionLoading}
                error={discussion.discussionError}
              />
            ) : lessonSelection.isQuizLesson ? (
              (() => {
                // Debug logging
                console.log("=== RENDER QUIZ DEBUG ===");
                console.log("Is Quiz Lesson:", lessonSelection.isQuizLesson);
                console.log("Quiz ID:", quiz.quizId);
                console.log("Quiz Questions:", quiz.quizQuestions);
                console.log(
                  "Questions Count:",
                  quiz.quizQuestions?.length || 0
                );
                console.log("Current Index:", quiz.currentQuestionIndex);
                console.log("Quiz Loading:", quiz.quizLoading);
                console.log("Questions Loading:", quiz.questionsLoading);
                console.log("Quiz Error:", quiz.quizError);
                console.log("Questions Error:", quiz.questionsError);

                // Ensure we have questions and a valid current question
                const hasQuestions =
                  quiz.quizQuestions &&
                  Array.isArray(quiz.quizQuestions) &&
                  quiz.quizQuestions.length > 0;
                const currentQuestion =
                  hasQuestions &&
                  quiz.currentQuestionIndex >= 0 &&
                  quiz.currentQuestionIndex < quiz.quizQuestions.length
                    ? quiz.quizQuestions[quiz.currentQuestionIndex]
                    : null;
                const isLoading = quiz.quizLoading || quiz.questionsLoading;

                console.log("Has Questions:", hasQuestions);
                console.log("Current Question:", currentQuestion);
                console.log("Is Loading:", isLoading);

                if (hasQuestions && currentQuestion) {
                  return (
                    <QuizDisplay
                      question={currentQuestion}
                      questionIndex={quiz.currentQuestionIndex}
                      totalQuestions={quiz.quizQuestions.length}
                      selectedAnswer={
                        quiz.selectedAnswers[
                          currentQuestion?.id ||
                            currentQuestion?.Id ||
                            currentQuestion?._id
                        ]
                      }
                      onAnswerSelect={quiz.handleAnswerSelect}
                      onPrevious={() => quiz.handleQuestionNavigation("prev")}
                      onNext={() => quiz.handleQuestionNavigation("next")}
                      canGoPrevious={quiz.currentQuestionIndex > 0}
                      canGoNext={
                        quiz.currentQuestionIndex <
                        quiz.quizQuestions.length - 1
                      }
                      isUserRole={quiz.isUserRole}
                    />
                  );
                } else if (isLoading) {
                  return (
                    <Box
                      sx={{
                        flex: 1,
                        backgroundColor: "white",
                        borderRadius: 2,
                        p: { xs: 2, sm: 3, md: 4 },
                        display: "flex",
                        flexDirection: "column",
                        justifyContent: "center",
                        alignItems: "center",
                        boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
                        minHeight: { xs: 400, md: "auto" },
                      }}
                    >
                      <Typography
                        variant="h6"
                        sx={{ mb: 2, textAlign: "center" }}
                      >
                        Loading quiz questions...
                      </Typography>
                    </Box>
                  );
                } else if (!quiz.quizId) {
                  return (
                    <Box
                      sx={{
                        flex: 1,
                        backgroundColor: "white",
                        borderRadius: 2,
                        p: { xs: 2, sm: 3, md: 4 },
                        display: "flex",
                        flexDirection: "column",
                        justifyContent: "center",
                        alignItems: "center",
                        boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
                        minHeight: { xs: 400, md: "auto" },
                      }}
                    >
                      <Typography
                        variant="h6"
                        sx={{ mb: 2, textAlign: "center" }}
                      >
                        No quiz uploaded yet
                      </Typography>
                      {!isUserRole && (
                        <Button
                          variant="contained"
                          onClick={() => modals.setIsQuizUploadModalOpen(true)}
                          sx={{
                            bgcolor: "#6366f1",
                            textTransform: "none",
                            "&:hover": {
                              bgcolor: "#4f46e5",
                            },
                          }}
                        >
                          Upload Quiz File
                        </Button>
                      )}
                    </Box>
                  );
                } else {
                  return (
                    <Box
                      sx={{
                        flex: 1,
                        backgroundColor: "white",
                        borderRadius: 2,
                        p: { xs: 2, sm: 3, md: 4 },
                        display: "flex",
                        flexDirection: "column",
                        justifyContent: "center",
                        alignItems: "center",
                        boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
                        minHeight: { xs: 400, md: "auto" },
                      }}
                    >
                      <Typography
                        variant="h6"
                        sx={{ mb: 2, textAlign: "center" }}
                      >
                        No questions found in this quiz
                      </Typography>
                      {quiz.quizError && (
                        <Typography
                          variant="body2"
                          color="error"
                          sx={{ mb: 2 }}
                        >
                          Error:{" "}
                          {"message" in quiz.quizError
                            ? quiz.quizError.message
                            : "Failed to load quiz"}
                        </Typography>
                      )}
                      {quiz.questionsError && (
                        <Typography
                          variant="body2"
                          color="error"
                          sx={{ mb: 2 }}
                        >
                          Error:{" "}
                          {"message" in quiz.questionsError
                            ? quiz.questionsError.message
                            : "Failed to load questions"}
                        </Typography>
                      )}
                      <Button
                        variant="contained"
                        onClick={() => modals.setIsQuizUploadModalOpen(true)}
                        sx={{
                          bgcolor: "#6366f1",
                          textTransform: "none",
                          "&:hover": {
                            bgcolor: "#4f46e5",
                          },
                        }}
                      >
                        Upload Quiz File
                      </Button>
                    </Box>
                  );
                }
              })()
            ) : (
              <SlidePreviewContainer
                selectedSlide={slides.selectedSlide}
                slideTitle={slideEditor.slideTitle}
                slideType={previewSlideType}
                imageUrl={slideEditor.imageUrl}
                imageText={slideEditor.imageText}
                imageCollection={slideEditor.imageCollection}
                videoUrl={slideEditor.videoUrl}
                videoText={slideEditor.videoText}
                videoThumbnail={slideEditor.videoThumbnail}
                videoCollection={slideEditor.videoCollection}
                bulletPoints={slideEditor.bulletPoints}
                expandableListItems={slideEditor.expandableListItems}
                focusMode={slideEditor.focusMode}
                expandedItemIndex={slideEditor.expandedItemIndex}
                promptText={slideEditor.promptText}
                doneText={slideEditor.doneText}
                slidesArray={slides.getSlidesArray()}
                currentSlideIndex={(() => {
                  const slidesArray = slides.getSlidesArray();
                  return slidesArray.findIndex(
                    (s: any) =>
                      String(s.id || s.Id) ===
                      String(
                        slides.selectedSlide?.id || slides.selectedSlide?.Id
                      )
                  );
                })()}
                onImageClick={(url) => modals.handleOpenPreview(url, "image")}
                onVideoClick={(url) => modals.handleOpenPreview(url, "video")}
                onToggleExpandableItem={handleToggleExpandableItem}
                onPreviousSlide={handlePreviousSlide}
                onNextSlide={handleNextSlide}
              />
            )}
          </Paper>

          {/* Mobile Properties Toggle Button */}
          <MobilePropertiesButton
            onClick={() => {
              setMobilePropertiesOpen(!mobilePropertiesOpen);
              if (mobileLessonsOpen) setMobileLessonsOpen(false);
            }}
          />

          {/* Right Panel - Properties Editor or Quiz Editor or Discussion Editor (hidden for user role) */}
          {!isUserRole && (
            <>
              {lessonSelection.isDiscussionLesson ? (
                <DiscussionEditor
                  description={discussion.description}
                  onDescriptionChange={discussion.setDescription}
                  isUpdating={discussion.isUpdating || discussion.isCreating}
                  mobilePropertiesOpen={mobilePropertiesOpen}
                  onCloseMobile={() => setMobilePropertiesOpen(false)}
                />
              ) : lessonSelection.isQuizLesson && quiz.quizId ? (
                <QuizEditor
                  quizQuestions={quiz.quizQuestions}
                  quizId={quiz.quizId}
                  currentQuestionIndex={quiz.currentQuestionIndex}
                  onQuestionUpdated={() => {
                    // Refetch quiz questions after update
                    quiz.refetchQuestions();
                  }}
                  onQuestionDeleted={() => {
                    // Refetch quiz questions after delete
                    quiz.refetchQuestions();
                  }}
                  onQuestionNavigation={quiz.handleQuestionNavigation}
                  mobilePropertiesOpen={mobilePropertiesOpen}
                  onCloseMobile={() => setMobilePropertiesOpen(false)}
                />
              ) : (
                <SlidePropertiesPanel
                  mobilePropertiesOpen={mobilePropertiesOpen}
                  onCloseMobile={() => setMobilePropertiesOpen(false)}
                  slideTitle={slideEditor.slideTitle}
                  slideType={slideType}
                  imageUrl={slideEditor.imageUrl}
                  imageText={slideEditor.imageText}
                  imageCollection={slideEditor.imageCollection}
                  expandedImageIndex={slideEditor.expandedImageIndex}
                  videoUrl={slideEditor.videoUrl}
                  videoText={slideEditor.videoText}
                  videoThumbnail={slideEditor.videoThumbnail}
                  videoCollection={slideEditor.videoCollection}
                  expandedVideoIndex={slideEditor.expandedVideoIndex}
                  bulletPoints={slideEditor.bulletPoints}
                  doneText={slideEditor.doneText}
                  expandableListItems={slideEditor.expandableListItems}
                  focusMode={slideEditor.focusMode}
                  promptText={slideEditor.promptText}
                  isListExpanded={slideEditor.isListExpanded}
                  isNarrationExpanded={slideEditor.isNarrationExpanded}
                  onSlideTitleChange={(title) =>
                    slideEditor.setSlideTitle(title)
                  }
                  onImageUrlChange={(url) => slideEditor.setImageUrl(url)}
                  onImageTextChange={(text) => slideEditor.setImageText(text)}
                  onAddImageToCollection={handleAddImageToCollection}
                  onRemoveImageFromCollection={handleRemoveImageFromCollection}
                  onToggleImageDropdown={handleToggleImageDropdown}
                  onUpdateImageInCollection={handleUpdateImageInCollection}
                  onImageUpload={async (file) => {
                    try {
                      const uploadedUrl = await mediaUpload.handleImageUpload(
                        file
                      );
                      slideEditor.setImageUrl(uploadedUrl);
                    } catch (error) {
                      console.error("Failed to upload image:", error);
                    }
                  }}
                  onImageUploadToCollection={handleImageFileUpload}
                  onVideoUrlChange={(url) => slideEditor.setVideoUrl(url)}
                  onVideoTextChange={(text) => slideEditor.setVideoText(text)}
                  onVideoThumbnailChange={(thumbnail) =>
                    slideEditor.setVideoThumbnail(thumbnail)
                  }
                  onAddVideoToCollection={handleAddVideoToCollection}
                  onRemoveVideoFromCollection={handleRemoveVideoFromCollection}
                  onToggleVideoDropdown={handleToggleVideoDropdown}
                  onUpdateVideoInCollection={handleUpdateVideoInCollection}
                  onVideoUpload={async (file) => {
                    try {
                      const uploadedUrl = await mediaUpload.handleVideoUpload(
                        file
                      );
                      slideEditor.setVideoUrl(uploadedUrl);
                    } catch (error) {
                      console.error("Failed to upload video:", error);
                    }
                  }}
                  onVideoUploadToCollection={handleVideoFileUpload}
                  onThumbnailUpload={async (file) => {
                    try {
                      const uploadedUrl = await mediaUpload.handleImageUpload(
                        file
                      );
                      slideEditor.setVideoThumbnail(uploadedUrl);
                    } catch (error) {
                      console.error("Failed to upload thumbnail:", error);
                    }
                  }}
                  onThumbnailUploadToCollection={async (index, file) => {
                    try {
                      const uploadedUrl = await mediaUpload.handleImageUpload(
                        file
                      );
                      handleUpdateVideoInCollection(
                        index,
                        undefined,
                        uploadedUrl,
                        undefined
                      );
                    } catch (error) {
                      console.error("Failed to upload thumbnail:", error);
                    }
                  }}
                  onAddBulletPoint={handleAddBulletPoint}
                  onUpdateBulletPoint={handleUpdateBulletPoint}
                  onDeleteBulletPoint={handleDeleteBulletPoint}
                  onDoneTextChange={(text) => slideEditor.setDoneText(text)}
                  onAddExpandableItem={handleAddExpandableItem}
                  onUpdateExpandableItem={handleUpdateExpandableItem}
                  onDeleteExpandableItem={handleDeleteExpandableItem}
                  onToggleFocusMode={() =>
                    slideEditor.setFocusMode(!slideEditor.focusMode)
                  }
                  onPromptTextChange={(text) => slideEditor.setPromptText(text)}
                  onToggleListExpanded={() =>
                    slideEditor.setIsListExpanded(!slideEditor.isListExpanded)
                  }
                  onToggleNarrationExpanded={() =>
                    slideEditor.setIsNarrationExpanded(
                      !slideEditor.isNarrationExpanded
                    )
                  }
                  isUploadingImage={mediaUpload.isUploadingImage}
                  isUploadingVideo={mediaUpload.isUploadingVideo}
                  isUploadingThumbnail={mediaUpload.isUploadingImage}
                />
              )}
            </>
          )}

          {/* Quiz Upload Modal - Must be in early return block */}
          {(() => {
            console.log("=== RENDERING QuizUploadModal (in early return) ===");
            console.log(
              "modals.isQuizUploadModalOpen:",
              modals.isQuizUploadModalOpen
            );
            const lessonIdValue =
              lessonSelection.selectedLessonForSlides?.id ||
              lessonSelection.selectedLessonForSlides?.Id ||
              lessonSelection.selectedLessonId ||
              "";
            console.log("lessonIdValue:", lessonIdValue);
            return (
              <QuizUploadModal
                open={modals.isQuizUploadModalOpen}
                onClose={() => {
                  console.log("QuizUploadModal onClose called");
                  modals.setIsQuizUploadModalOpen(false);
                }}
                lessonId={lessonIdValue}
                onUpload={async (file, quizScore) => {
                  console.log("QuizUploadModal onUpload called");
                  quiz.resetQuiz();
                }}
              />
            );
          })()}
        </Box>
      </>
    );
  }

  return (
    <Box>
      <CourseHeader
        courseTitle={course?.Title || course?.title || "Course"}
        isPublishing={isPublishing}
        onPublish={handlePublish}
        onAssign={handleAssign}
        anchorEl={modals.anchorEl}
        onMenuOpen={handleCourseMenuOpen}
        onMenuClose={modals.handleMenuClose}
        isUserRole={isUserRole}
      />

      <Box>
        <Typography variant="h5" component="h2" sx={{ fontWeight: 600, mb: 3 }}>
          Project
        </Typography>

        <Box sx={{ display: "flex", gap: 3, height: "calc(100vh - 200px)" }}>
          <LessonsPanel
            mobileLessonsOpen={false}
            onCloseMobile={() => {}}
            displayedLessons={lessons.displayedLessons || []}
            selectedLessonForSlides={lessonSelection.selectedLessonForSlides}
            onLessonSelect={(lesson) => {
              lessonSelection.setSelectedLessonForSlides(lesson);
              if (!lesson) {
                setIsSlideEditorOpen(false);
              }
            }}
            onAddLessonClick={isUserRole ? () => {} : handleAddLessonClick}
            isUserRole={isUserRole}
            slidesData={slides.slidesData}
            selectedSlide={slides.selectedSlide}
            onSlideClick={(slide) => {
              // For user role, set selected slide and populate preview data (no editor)
              if (isUserRole) {
                slides.setSelectedSlide(slide);
                // Populate preview data without opening editor
                const slideType =
                  slide.type ||
                  slide.Type ||
                  slide.slideType ||
                  slide.SlideType ||
                  "bulleted-list";
                const contentObj = slide.content || slide.Content || {};
                const contentTitle =
                  contentObj?.title || contentObj?.Title || "";
                const contentItems =
                  contentObj?.items || contentObj?.Items || [];

                slideEditor.setSlideTitle(
                  contentTitle ||
                    slide.title ||
                    slide.Title ||
                    (slideType === "expandable-list"
                      ? "Expandable list"
                      : slideType === "single-image"
                      ? "Single Image"
                      : slideType === "image-collection"
                      ? "Image Collection"
                      : slideType === "single-video"
                      ? "Single Video"
                      : slideType === "video-collection"
                      ? "Video Collection"
                      : "Bulleted list")
                );

                // Populate based on slide type
                if (slideType === "bulleted-list") {
                  slideEditor.setBulletPoints(
                    contentItems.map(
                      (item: any) => item.text || item.Text || ""
                    )
                  );
                } else if (slideType === "expandable-list") {
                  const expandedItems = contentItems.map((item: any) => {
                    const textValue = item.text || item.Text || "";
                    const textParts = textValue.split("\n");
                    return {
                      title: textParts[0] || "",
                      content: textParts.slice(1).join("\n") || "",
                    };
                  });
                  slideEditor.setExpandableListItems(expandedItems);
                } else if (slideType === "single-image") {
                  slideEditor.setImageUrl(
                    contentItems[0]?.image || contentItems[0]?.Image || ""
                  );
                  slideEditor.setImageText(
                    contentItems[0]?.text || contentItems[0]?.Text || ""
                  );
                } else if (slideType === "image-collection") {
                  slideEditor.setImageCollection(
                    contentItems.map((item: any) => ({
                      text: item.text || item.Text || "",
                      image: item.image || item.Image || "",
                    }))
                  );
                } else if (slideType === "single-video") {
                  slideEditor.setVideoUrl(
                    contentItems[0]?.video || contentItems[0]?.Video || ""
                  );
                  slideEditor.setVideoText(
                    contentItems[0]?.text || contentItems[0]?.Text || ""
                  );
                  slideEditor.setVideoThumbnail(
                    contentItems[0]?.thumbnail ||
                      contentItems[0]?.Thumbnail ||
                      ""
                  );
                } else if (slideType === "video-collection") {
                  slideEditor.setVideoCollection(
                    contentItems.map((item: any) => ({
                      text: item.text || item.Text || "",
                      video: item.video || item.Video || "",
                      thumbnail: item.thumbnail || item.Thumbnail || "",
                    }))
                  );
                }
                return;
              }
              const slideType =
                slide.type || slide.Type || slide.slideType || slide.SlideType;
              if (
                slideType === "bulleted-list" ||
                slideType === "expandable-list" ||
                slideType === "single-image" ||
                slideType === "image-collection" ||
                slideType === "single-video" ||
                slideType === "video-collection" ||
                !slideType
              ) {
                populateSlideEditor(slide);
              }
            }}
            onOpenSlideLibrary={handleOpenSlideLibrary}
            isCreatingSlide={slides.isCreatingSlide}
            onCloseSlideEditor={() => setIsSlideEditorOpen(false)}
            editingLesson={lessons.editingLesson}
            editTitle={lessons.editTitle}
            setEditTitle={lessons.setEditTitle}
            handleEditClick={lessons.handleEditClick}
            handleSaveEdit={lessons.handleSaveEdit}
            handleCancelEdit={lessons.handleCancelEdit}
            handleDeleteClick={(e, lesson) => {
              lessons.handleDeleteClick(e, lesson);
              modals.setIsDeleteModalOpen(true);
            }}
            isUpdating={lessons.isUpdating}
            isDeleting={lessons.isDeleting}
          />

          <EmptyStateContainer>
            <EmptyStateIllustration />
            <Typography
              variant="h6"
              color="text.secondary"
              sx={{ mb: 1, textAlign: "center" }}
            >
              No slides found in this lesson.
            </Typography>
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ textAlign: "center" }}
            >
              Create your first slide to get started.
            </Typography>
          </EmptyStateContainer>
        </Box>
      </Box>

      {/* Lesson Type Menu - Always rendered, conditionally visible */}
      <LessonTypeMenu
        anchorEl={lessonTypeMenuAnchor}
        open={isLessonTypeMenuOpen}
        onClose={handleCloseLessonTypeMenu}
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "right",
        }}
        transformOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
      >
        {LESSON_TYPES.map((type) => (
          <MenuItem
            key={type.value}
            onClick={() => {
              lessons.handleCreateLesson(type.value);
              handleCloseLessonTypeMenu();
            }}
            sx={{
              "&:hover": {
                backgroundColor: "rgba(99, 102, 241, 0.08)",
              },
            }}
          >
            <ListItemIcon sx={{ minWidth: 36, color: "#6366f1" }}>
              {type.icon}
            </ListItemIcon>
            <Typography variant="body2">{type.label}</Typography>
          </MenuItem>
        ))}
      </LessonTypeMenu>

      <DeleteConfirmationModal
        open={modals.isDeleteModalOpen}
        onClose={() => {
          modals.setIsDeleteModalOpen(false);
          lessons.handleDeleteClick({} as React.MouseEvent, null);
        }}
        onConfirm={handleConfirmDelete}
        isLoading={lessons.isDeleting}
        title="Are you sure you want to delete this lesson?"
        message="This will delete the lesson and all its content. This action is irreversible."
      />

      <SlideLibraryDialog
        open={modals.isSlideLibraryOpen}
        onClose={handleCloseSlideLibrary}
        selectedTab={modals.selectedSlideTab}
        onTabChange={modals.setSelectedSlideTab}
        onCreateSlide={handleCreateSlide}
      />

      <MediaPreviewModal
        open={modals.isPreviewModalOpen}
        onClose={modals.handleClosePreview}
        url={modals.previewUrl}
        type={modals.previewType}
        alt="Media preview"
      />

      <AssignUsersModal
        open={modals.isAssignModalOpen}
        onClose={() => modals.setIsAssignModalOpen(false)}
        courseId={courseId}
      />
      {(() => {
        console.log("=== RENDERING QuizUploadModal ===");
        console.log(
          "modals.isQuizUploadModalOpen:",
          modals.isQuizUploadModalOpen
        );
        console.log(
          "lessonSelection.selectedLessonForSlides:",
          lessonSelection.selectedLessonForSlides
        );
        const lessonIdValue =
          lessonSelection.selectedLessonForSlides?.id ||
          lessonSelection.selectedLessonForSlides?.Id ||
          lessonSelection.selectedLessonId ||
          "";
        console.log("lessonIdValue:", lessonIdValue);
        return (
          <QuizUploadModal
            open={modals.isQuizUploadModalOpen}
            onClose={() => {
              console.log("QuizUploadModal onClose called");
              modals.setIsQuizUploadModalOpen(false);
            }}
            lessonId={lessonIdValue}
            onUpload={async (file, quizScore) => {
              console.log("QuizUploadModal onUpload called");
              quiz.resetQuiz();
            }}
          />
        );
      })()}
    </Box>
  );
}

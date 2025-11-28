"use client";

import { useParams } from "next/navigation";
import { useState } from "react";
import React from "react";
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
  useUploadImageMutation,
  useUploadVideoMutation,
} from "@/services/upload-api";
import DeleteConfirmationModal from "@/components/DeleteConfirmationModal";
import MediaPreviewModal from "@/components/MediaPreviewModal";
import AssignUsersModal from "@/components/AssignUsersModal";
import styled from "styled-components";

const LessonsPanel = styled(Paper)`
  width: 300px;
  min-width: 300px;
  max-width: 300px;
  padding: 1rem;
  overflow: hidden;
  display: flex;
  flex-direction: column;
`;

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
  const params = useParams();
  const courseId = params.id as string;
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [isAssignModalOpen, setIsAssignModalOpen] = useState(false);
  const [selectedLesson, setSelectedLesson] = useState<any>(null);
  const [selectedLessonForSlides, setSelectedLessonForSlides] =
    useState<any>(null);
  const [editingLesson, setEditingLesson] = useState<any>(null);
  const [editTitle, setEditTitle] = useState("");
  const [isSlideLibraryOpen, setIsSlideLibraryOpen] = useState(false);
  const [selectedSlideTab, setSelectedSlideTab] = useState(0);
  const [selectedSlide, setSelectedSlide] = useState<any>(null);
  const [isSlideEditorOpen, setIsSlideEditorOpen] = useState(false);
  // Slide editor state
  const [slideTitle, setSlideTitle] = useState("Bulleted list");
  const [bulletPoints, setBulletPoints] = useState<string[]>([
    "Has Several Points",
    "Displays each point with a bullet",
    "Is similar to a powerpoint slide",
  ]);
  const [doneText, setDoneText] = useState("Continue");
  const [isListExpanded, setIsListExpanded] = useState(true);
  const [isNarrationExpanded, setIsNarrationExpanded] = useState(false);
  // Expandable list state
  const [expandableListItems, setExpandableListItems] = useState<
    { title: string; content: string }[]
  >([
    { title: "Item 1", content: "Content for item 1" },
    { title: "Item 2", content: "Content for item 2" },
  ]);
  const [focusMode, setFocusMode] = useState(false);
  const [promptText, setPromptText] = useState("Prompt Text");
  const [expandedItemIndex, setExpandedItemIndex] = useState<number | null>(
    null
  );
  // Image slide state
  const [imageUrl, setImageUrl] = useState<string>("");
  const [imageText, setImageText] = useState<string>("");
  const [imageCollection, setImageCollection] = useState<
    { url: string; alt?: string; text?: string }[]
  >([]);
  const [expandedImageIndex, setExpandedImageIndex] = useState<number | null>(
    null
  );
  // Video slide state
  const [videoUrl, setVideoUrl] = useState<string>("");
  const [videoText, setVideoText] = useState<string>("");
  const [videoThumbnail, setVideoThumbnail] = useState<string>("");
  const [videoCollection, setVideoCollection] = useState<
    { url: string; title?: string; thumbnail?: string }[]
  >([]);
  const [expandedVideoIndex, setExpandedVideoIndex] = useState<number | null>(
    null
  );
  // Preview modal state
  const [isPreviewModalOpen, setIsPreviewModalOpen] = useState(false);
  const [previewUrl, setPreviewUrl] = useState<string>("");
  const [previewType, setPreviewType] = useState<"image" | "video">("image");
  const [sortedLessons, setSortedLessons] = useState<any[]>([]);
  const [displayedLessons, setDisplayedLessons] = useState<any[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [refreshKey, setRefreshKey] = useState(0); // Force refresh key
  const pageSize = 10;
  const initialDisplayCount = 10;
  const isInitializedRef = React.useRef(false);
  const scrollLoadingRef = React.useRef(false);
  // Auto-save refs
  const autoSaveTimerRef = React.useRef<NodeJS.Timeout | null>(null);
  const hasUnsavedChangesRef = React.useRef(false);
  const isInitializingSlideRef = React.useRef(false);

  // Reset pagination when courseId changes
  React.useEffect(() => {
    setCurrentPage(1);
    setDisplayedLessons([]);
    setSortedLessons([]);
    setHasMore(true);
    setRefreshKey(0);
    isInitializedRef.current = false;
    scrollLoadingRef.current = false;
  }, [courseId]);

  // Set up sensors for drag and drop
  // Configure PointerSensor with activation constraint to prevent clicks from triggering drag
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8, // Require 8px of movement before activating drag
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const { data: courseData, isLoading: courseLoading } =
    useGetCourseByIdQuery(courseId);
  const [updateCourse, { isLoading: isPublishing }] = useUpdateCourseMutation();
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
  const [createSlide, { isLoading: isCreatingSlide }] =
    useCreateSlideMutation();
  const [updateSlide, { isLoading: isUpdatingSlide }] =
    useUpdateSlideMutation();
  const [uploadImage, { isLoading: isUploadingImage }] =
    useUploadImageMutation();
  const [uploadVideo, { isLoading: isUploadingVideo }] =
    useUploadVideoMutation();

  // Fetch slides for selected lesson
  const selectedLessonId =
    selectedLessonForSlides?.id || selectedLessonForSlides?.Id;
  const {
    data: slidesData,
    isLoading: slidesLoading,
    refetch: refetchSlides,
  } = useGetSlidesByLessonQuery(
    {
      lessonId: selectedLessonId || "",
      page: 1,
      pageSize: 100, // Get all slides for the lesson
    },
    {
      skip: !selectedLessonId, // Skip query if no lesson is selected
    }
  );

  const course = courseData?.data;

  // Accumulate lessons from multiple pages - aligned with reference project pattern
  React.useEffect(() => {
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
        // Reset completely if it's the first page (page 1) - ensures fresh data after delete/update
        if (currentPage === 1) {
          // Sort by order, then by creation date
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

        // For subsequent pages, merge: update existing lessons and append new ones
        const existingIds = new Set(
          prevSorted.map((l: any) => String(l.id || l.Id))
        );

        // Create a map of existing lessons for easy lookup
        const existingMap = new Map(
          prevSorted.map((l: any) => [String(l.id || l.Id), l])
        );

        // Update existing lessons or add new ones
        const updatedLessons = lessonsArray.map((lesson: any) => {
          const lessonId = String(lesson.id || lesson.Id);
          if (existingMap.has(lessonId)) {
            // Update existing lesson with new data (important for updates)
            return lesson;
          }
          return lesson;
        });

        // Filter out lessons that no longer exist in the API response
        const currentIds = new Set(
          lessonsArray.map((l: any) => String(l.id || l.Id))
        );
        const keptExisting = prevSorted.filter((l: any) =>
          currentIds.has(String(l.id || l.Id))
        );

        // Merge: keep existing lessons from previous pages, replace/update with new data
        const combined = [...keptExisting];

        // Add/update lessons from current page
        updatedLessons.forEach((lesson: any) => {
          const lessonId = String(lesson.id || lesson.Id);
          const existingIndex = combined.findIndex(
            (l: any) => String(l.id || l.Id) === lessonId
          );
          if (existingIndex >= 0) {
            // Update existing
            combined[existingIndex] = lesson;
          } else {
            // Add new
            combined.push(lesson);
          }
        });

        // Sort by order, then by creation date
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

      // Check if there are more lessons to load - same pattern as reference project
      setHasMore(lessonsArray.length === pageSize);
      scrollLoadingRef.current = false;
    } else if (currentPage === 1) {
      // No lessons on first page
      setSortedLessons([]);
      setDisplayedLessons([]);
      setHasMore(false);
      isInitializedRef.current = false;
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [lessonsData, currentPage, pageSize, refreshKey]);

  // Update displayed lessons - initially show only 10, or update when lessons change
  React.useEffect(() => {
    if (sortedLessons.length > 0) {
      if (!isInitializedRef.current) {
        // Initially display only first 10 lessons
        setDisplayedLessons(sortedLessons.slice(0, initialDisplayCount));
        isInitializedRef.current = true;
      } else {
        // Update displayed lessons to reflect changes in sortedLessons (e.g., after edit)
        // Keep the same number displayed but update the content
        setDisplayedLessons((prev) => {
          const currentCount = prev.length;
          const newDisplayed = sortedLessons.slice(
            0,
            Math.max(currentCount, initialDisplayCount)
          );
          // If displayed lessons count changed, use new count
          if (prev.length === 0 || prev.length !== newDisplayed.length) {
            return newDisplayed;
          }
          // Otherwise update existing displayed lessons with new data
          return newDisplayed;
        });
      }
    } else if (sortedLessons.length === 0) {
      setDisplayedLessons([]);
      isInitializedRef.current = false;
    }
  }, [sortedLessons]);

  // Handle scroll to load more - aligned with reference project pattern
  const handleScroll = React.useCallback(
    (e: React.UIEvent<HTMLDivElement>) => {
      if (scrollLoadingRef.current || isFetching) return;

      const target = e.currentTarget;
      const scrollBottom =
        target.scrollHeight - target.scrollTop - target.clientHeight;

      // Load more when within 100px of bottom
      if (scrollBottom < 100 && hasMore) {
        scrollLoadingRef.current = true;

        // Check if we have more lessons already loaded
        if (displayedLessons.length < sortedLessons.length) {
          // Display more from already loaded lessons
          const nextDisplayCount = Math.min(
            displayedLessons.length + pageSize,
            sortedLessons.length
          );
          setDisplayedLessons(sortedLessons.slice(0, nextDisplayCount));
          scrollLoadingRef.current = false;
        } else {
          // Need to load more from API
          setCurrentPage((prev) => prev + 1);
          // Reset after a short delay to allow API call to start
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

    // Reorder lessons in state
    const newSortedLessons = arrayMove(sortedLessons, oldIndex, newIndex);
    setSortedLessons(newSortedLessons);

    // Update order property for each lesson and save to backend
    try {
      const updatePromises = newSortedLessons.map(
        (lesson: any, index: number) => {
          const newOrder = index + 1;
          const lessonId = lesson.id || lesson.Id;

          // Only update if order has changed
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
        }
      );

      await Promise.all(updatePromises);
      console.log("Lessons reordered successfully");
    } catch (error) {
      console.error("Failed to update lesson order:", error);
      // On error, the state will be updated when API refetches
    }
  };

  // Debug: Log lesson data structure
  React.useEffect(() => {
    if (lessonsData) {
      console.log("Lessons API Response:", lessonsData);
      console.log("Sorted lessons count:", sortedLessons.length);
      console.log("Displayed lessons count:", displayedLessons.length);
      if (sortedLessons.length > 0) {
        console.log("First lesson structure:", sortedLessons[0]);
        console.log("First lesson title:", sortedLessons[0].title);
        console.log("First lesson id:", sortedLessons[0].id);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [lessonsData]);

  const handleAddLessonClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleCreateLesson = async (lessonType: string) => {
    try {
      // Use capital property names to match backend Lesson model
      const lessonData = {
        CourseId: courseId,
        Title: `Untitled ${lessonType}`,
        LessonType: lessonType,
        Order: sortedLessons.length + 1,
      };
      console.log("Creating lesson:", lessonData);
      const result = await createLesson(lessonData).unwrap();
      console.log("Lesson created successfully");

      // Reset pagination to page 1 to see the new lesson
      setCurrentPage(1);
      isInitializedRef.current = false;

      // Explicitly refetch to get the updated list with the new lesson
      await refetchLessons();

      handleMenuClose();
    } catch (error) {
      console.error("Failed to create lesson:", error);
    }
  };

  const handleEditClick = (e: React.MouseEvent, lesson: any) => {
    e.stopPropagation();
    const lessonId = lesson.id || lesson.Id;
    setEditingLesson({ ...lesson, id: lessonId, Id: lessonId });
    setEditTitle(
      lesson.title || lesson.Title || lesson.name || lesson.Name || ""
    );
  };

  const handleSaveEdit = async () => {
    if (!editingLesson || !editTitle.trim()) return;

    const lessonId = editingLesson.id || editingLesson.Id;
    const lessonIdString = String(lessonId);

    try {
      // Use capital property names to match backend Lesson model
      const updateData = {
        id: lessonId,
        Title: editTitle,
        Description:
          editingLesson.Description || editingLesson.description || "",
        LessonType:
          editingLesson.LessonType || editingLesson.lessonType || "standard",
        Order: editingLesson.Order || editingLesson.order || 0,
        CourseId: courseId,
      };
      console.log("Updating lesson:", updateData);

      // Optimistically update the UI immediately for instant feedback
      const updatedLesson = {
        ...editingLesson,
        title: editTitle,
        Title: editTitle,
        id: lessonId,
        Id: lessonId,
      };

      // Optimistically update both sortedLessons and displayedLessons immediately
      // This ensures instant UI feedback before the server responds
      setSortedLessons((prev) => {
        const updated = prev.map((lesson) => {
          const lessonIdMatch = String(lesson.id || lesson.Id);
          if (lessonIdMatch === lessonIdString) {
            return updatedLesson;
          }
          return lesson;
        });
        return updated;
      });

      setDisplayedLessons((prev) => {
        const updated = prev.map((lesson) => {
          const lessonIdMatch = String(lesson.id || lesson.Id);
          if (lessonIdMatch === lessonIdString) {
            return updatedLesson;
          }
          return lesson;
        });
        return updated;
      });

      // Close edit mode immediately for better UX
      setEditingLesson(null);
      setEditTitle("");

      // Perform the update mutation - this will automatically invalidate cache
      await updateLesson(updateData).unwrap();
      console.log("Lesson updated successfully");

      // The optimistic update already shows the change immediately
      // RTK Query's cache invalidation will automatically trigger a refetch in the background
      // We don't need to manually refetch - the automatic refetch will sync with server data
      // The optimistic update ensures instant feedback while the refetch happens asynchronously
    } catch (error) {
      console.error("Failed to update lesson:", error);
      // On error, refetch to restore correct state
      await refetchLessons();
      // Re-open edit mode so user can try again
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
    setIsDeleteModalOpen(true);
  };

  const handleLessonClick = (e: React.MouseEvent, lesson: any) => {
    e.stopPropagation();
    const lessonId = lesson.id || lesson.Id;
    // Toggle selection - if same lesson is clicked, deselect it
    if (
      selectedLessonForSlides &&
      (selectedLessonForSlides.id || selectedLessonForSlides.Id) === lessonId
    ) {
      setSelectedLessonForSlides(null);
    } else {
      setSelectedLessonForSlides({ ...lesson, id: lessonId, Id: lessonId });
    }
  };

  const handleOpenSlideLibrary = () => {
    if (!selectedLessonForSlides) return;
    setIsSlideLibraryOpen(true);
    setSelectedSlideTab(0);
  };

  const handleOpenPreview = (url: string, type: "image" | "video") => {
    if (!url) return;
    setPreviewUrl(url);
    setPreviewType(type);
    setIsPreviewModalOpen(true);
  };

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

  const handleAssign = () => {
    setIsAssignModalOpen(true);
  };

  const handleCreateSlide = async (slideType: string) => {
    if (!selectedLessonForSlides) return;

    try {
      const lessonId = selectedLessonForSlides.id || selectedLessonForSlides.Id;

      // Get current slides to determine order
      const slidesArray = Array.isArray(slidesData?.data)
        ? slidesData.data
        : Array.isArray(slidesData?.data?.items)
        ? slidesData.data.items
        : Array.isArray(slidesData?.items)
        ? slidesData.items
        : Array.isArray(slidesData)
        ? slidesData
        : [];

      const nextOrder = slidesArray.length;

      // Prepare content structure with title and items array
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
        contentItems = [];
      } else if (slideType === "single-video") {
        contentTitle = "Single Video";
        contentItems = [{ text: "", video: "", thumbnail: "" }];
      } else if (slideType === "video-collection") {
        contentTitle = "Video Collection";
        contentItems = [];
      }

      // Prepare payload according to new backend API structure
      // Note: id is generated by backend, so we don't include it in the create payload
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

      const result = await createSlide(slideData).unwrap();
      console.log("Slide created successfully");
      setIsSlideLibraryOpen(false);

      // Refetch slides to update the list
      if (selectedLessonForSlides) {
        const lessonId =
          selectedLessonForSlides.id || selectedLessonForSlides.Id;
        refetchSlides();
      }

      // If slide type supports editor, open it
      if (
        slideType === "bulleted-list" ||
        slideType === "expandable-list" ||
        slideType === "single-image" ||
        slideType === "image-collection" ||
        slideType === "single-video" ||
        slideType === "video-collection"
      ) {
        const createdSlide = result?.data || result;
        // Ensure we have the lesson selected to show slides
        if (selectedLessonForSlides) {
          // Reset auto-save state
          if (autoSaveTimerRef.current) {
            clearTimeout(autoSaveTimerRef.current);
          }
          hasUnsavedChangesRef.current = false;
          isInitializingSlideRef.current = true;

          setSelectedSlide({
            ...createdSlide,
            id: createdSlide.id || createdSlide.Id,
            Id: createdSlide.Id || createdSlide.id,
          });
          setIsSlideEditorOpen(true);

          // Initialize editor state with default values based on slide type
          if (slideType === "single-image") {
            setSlideTitle("Single Image");
            setImageUrl("");
            setImageText("");
          } else if (slideType === "image-collection") {
            setSlideTitle("Image Collection");
            setImageCollection([]);
            setExpandedImageIndex(null);
          } else if (slideType === "single-video") {
            setSlideTitle("Single Video");
            setVideoUrl("");
            setVideoText("");
            setVideoThumbnail("");
          } else if (slideType === "video-collection") {
            setSlideTitle("Video Collection");
            setVideoCollection([]);
            setExpandedVideoIndex(null);
          } else if (slideType === "expandable-list") {
            setSlideTitle("Expandable list");
            setExpandableListItems([
              { title: "Item 1", content: "Content for item 1" },
              { title: "Item 2", content: "Content for item 2" },
            ]);
            setFocusMode(false);
            setPromptText("Prompt Text");
          } else {
            // Default to bulleted list
            setSlideTitle("Bulleted list");
            setBulletPoints([
              "Has Several Points",
              "Displays each point with a bullet",
              "Is similar to a powerpoint slide",
            ]);
            setDoneText("Continue");
          }

          setIsListExpanded(true);
          setIsNarrationExpanded(false);

          // Reset initialization flag after a brief delay
          setTimeout(() => {
            isInitializingSlideRef.current = false;
          }, 100);
        }
      }
    } catch (error) {
      console.error("Failed to create slide:", error);
    }
  };

  const handleCloseSlideLibrary = () => {
    setIsSlideLibraryOpen(false);
    setSelectedSlideTab(0);
  };

  // Auto-save function for slide editor
  const handleAutoSaveSlide = React.useCallback(async () => {
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
        console.error("No slide ID found for auto-save");
        return;
      }

      // Get slide type
      const slideType =
        selectedSlide.type || selectedSlide.Type || "bulleted-list";

      // Build content JSON based on slide type
      let contentText;
      // Build content with new structure: { title: "", items: [...] }
      let contentTitle = slideTitle || "Untitled Slide";
      let contentItems: Array<{
        text?: string;
        image?: string;
        video?: string;
        thumbnail?: string;
      }> = [];

      if (slideType === "expandable-list") {
        // For expandable list, convert items to the new structure
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
        // Bulleted list (default)
        contentItems = bulletPoints.map((point) => ({
          text: point,
        }));
      }

      // Prepare update payload with new structure
      const updateData = {
        id: String(slideId),
        lessonId: selectedLessonForSlides?.id || selectedLessonForSlides?.Id,
        type: slideType,
        title: contentTitle,
        content: {
          title: contentTitle,
          items: contentItems,
        },
        order: selectedSlide.order || selectedSlide.Order || 0,
      };

      await updateSlide(updateData).unwrap();
      console.log("Slide auto-saved successfully");
      hasUnsavedChangesRef.current = false;
    } catch (error) {
      console.error("Failed to auto-save slide:", error);
    }
  }, [
    selectedSlide,
    slideTitle,
    bulletPoints,
    doneText,
    expandableListItems,
    focusMode,
    promptText,
    imageUrl,
    imageText,
    imageCollection,
    videoUrl,
    videoText,
    videoCollection,
    selectedLessonForSlides,
    updateSlide,
  ]);

  // Auto-save effect - debounced to 5 seconds
  React.useEffect(() => {
    // Only auto-save if slide editor is open and slide is selected
    if (!isSlideEditorOpen || !selectedSlide) {
      return;
    }

    // Skip auto-save if we're currently initializing the slide
    if (isInitializingSlideRef.current) {
      return;
    }

    // Clear existing timer
    if (autoSaveTimerRef.current) {
      clearTimeout(autoSaveTimerRef.current);
    }

    // Mark that there are unsaved changes
    hasUnsavedChangesRef.current = true;

    // Set new timer for 5 seconds
    autoSaveTimerRef.current = setTimeout(() => {
      handleAutoSaveSlide();
    }, 5000); // 5 seconds

    // Cleanup timer on unmount or when dependencies change
    return () => {
      if (autoSaveTimerRef.current) {
        clearTimeout(autoSaveTimerRef.current);
      }
    };
  }, [
    slideTitle,
    bulletPoints,
    doneText,
    expandableListItems,
    focusMode,
    promptText,
    imageUrl,
    imageText,
    imageCollection,
    videoUrl,
    videoText,
    videoCollection,
    isSlideEditorOpen,
    selectedSlide,
    handleAutoSaveSlide,
    isInitializingSlideRef,
  ]);

  const handleConfirmDelete = async () => {
    if (selectedLesson) {
      try {
        const lessonId = selectedLesson.id || selectedLesson.Id;
        if (!lessonId) {
          console.error("No lesson ID found for deletion");
          setIsDeleteModalOpen(false);
          setSelectedLesson(null);
          return;
        }
        console.log("Deleting lesson with ID:", lessonId);

        // Perform the delete mutation first
        await deleteLesson(String(lessonId)).unwrap();
        console.log("Lesson deleted successfully");

        // Reset pagination state to refetch from beginning
        // This ensures we get fresh data from the API
        setCurrentPage(1);
        isInitializedRef.current = false;

        // Explicitly refetch lessons to get updated list
        // The cache invalidation should handle this, but explicit refetch ensures it happens
        await refetchLessons();

        setIsDeleteModalOpen(false);
        setSelectedLesson(null);
      } catch (error) {
        console.error("Failed to delete lesson:", error);
        // Refetch on error to restore state
        setCurrentPage(1);
        isInitializedRef.current = false;
        // Keep modal open on error so user can retry if needed
      }
    }
  };

  const formatDuration = (duration?: number | string) => {
    if (!duration) return "1:00";
    if (typeof duration === "string") return duration;
    const minutes = Math.floor(duration / 60);
    const seconds = duration % 60;
    return `${minutes}:${seconds.toString().padStart(2, "0")}`;
  };

  const getLessonTypeLabel = (lesson: any) => {
    const type = lesson.LessonType || lesson.lessonType || "standard";
    return LESSON_TYPES.find((t) => t.value === type)?.label || type;
  };

  // Helper function to populate slide editor data
  const populateSlideEditor = (slide: any) => {
    console.log("Populating slide editor with:", slide);

    // Reset auto-save state
    if (autoSaveTimerRef.current) {
      clearTimeout(autoSaveTimerRef.current);
    }
    hasUnsavedChangesRef.current = false;
    isInitializingSlideRef.current = true;

    setSelectedSlide(slide);
    setIsSlideEditorOpen(true);

    // Get slide type
    const slideType =
      slide.type || slide.Type || slide.slideType || slide.SlideType;

    try {
      // Handle new content structure: content has { title: "", items: [...] }
      const contentObj = slide.content || slide.Content || {};
      console.log("Content object:", contentObj);

      // New structure: content.title and content.items
      const contentTitle = contentObj?.title || contentObj?.Title || "";
      const contentItems = contentObj?.items || contentObj?.Items || [];

      console.log("Content title:", contentTitle);
      console.log("Content items:", contentItems);

      // Set title from content or slide
      setSlideTitle(
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

      // Handle different slide types based on new structure
      if (slideType === "expandable-list") {
        // For expandable list, items contain title\ncontent in text field
        if (Array.isArray(contentItems) && contentItems.length > 0) {
          const expandedItems = contentItems.map((item) => {
            const textParts = item.text?.split("\n") || ["", ""];
            return {
              title: textParts[0] || "",
              content: textParts.slice(1).join("\n") || "",
            };
          });
          setExpandableListItems(expandedItems);
        } else {
          setExpandableListItems([
            { title: "Item 1", content: "Content for item 1" },
            { title: "Item 2", content: "Content for item 2" },
          ]);
        }
        setFocusMode(false);
        setPromptText("Prompt Text");
        setDoneText("Continue");
        setExpandedItemIndex(null);
      } else if (slideType === "single-image") {
        // Single image - first item contains image data
        const firstItem = contentItems[0] || {};
        setImageUrl(firstItem.image || "");
        setImageText(firstItem.text || "");
      } else if (slideType === "image-collection") {
        // Image collection - each item is an image
        if (Array.isArray(contentItems) && contentItems.length > 0) {
          const images = contentItems.map((item) => ({
            url: item.image || "",
            text: item.text || "",
            alt: item.text || "", // Keep alt for backward compatibility
          }));
          setImageCollection(images);
        } else {
          setImageCollection([]);
        }
        setExpandedImageIndex(null);
      } else if (slideType === "single-video") {
        // Single video - first item contains video data
        const firstItem = contentItems[0] || {};
        setVideoUrl(firstItem.video || "");
        setVideoText(firstItem.text || "");
        setVideoThumbnail(firstItem.thumbnail || "");
      } else if (slideType === "video-collection") {
        // Video collection - each item is a video
        if (Array.isArray(contentItems) && contentItems.length > 0) {
          const videos = contentItems.map((item) => ({
            url: item.video || "",
            title: item.text || "",
            thumbnail: item.thumbnail || "",
          }));
          setVideoCollection(videos);
        } else {
          setVideoCollection([]);
        }
        setExpandedVideoIndex(null);
      } else {
        // Bulleted list (default) - each item is a bullet point
        if (Array.isArray(contentItems) && contentItems.length > 0) {
          const bullets = contentItems.map((item) => item.text || "");
          setBulletPoints(bullets);
        } else {
          setBulletPoints([
            "Has Several Points",
            "Displays each point with a bullet",
            "Is similar to a powerpoint slide",
          ]);
        }
        setDoneText("Continue");
      }
    } catch (error) {
      console.error("Error parsing slide content:", error);
      // On error, use defaults based on slide type
      setSlideTitle(slide.title || slide.Title || "Untitled Slide");

      if (slideType === "expandable-list") {
        setExpandableListItems([
          { title: "Item 1", content: "Content for item 1" },
          { title: "Item 2", content: "Content for item 2" },
        ]);
        setFocusMode(false);
        setPromptText("Prompt Text");
        setDoneText("Continue");
        setExpandedItemIndex(null);
      } else if (slideType === "single-image") {
        setImageUrl("");
        setImageText("");
      } else if (slideType === "image-collection") {
        setImageCollection([]);
        setExpandedImageIndex(null);
      } else if (slideType === "single-video") {
        setVideoUrl("");
        setVideoText("");
        setVideoThumbnail("");
      } else if (slideType === "video-collection") {
        setVideoCollection([]);
        setExpandedVideoIndex(null);
      } else {
        // Bulleted list (default)
        setBulletPoints([
          "Has Several Points",
          "Displays each point with a bullet",
          "Is similar to a powerpoint slide",
        ]);
        setDoneText("Continue");
      }
    }

    setIsListExpanded(true);
    setIsNarrationExpanded(false);

    // Reset initialization flag after a brief delay
    setTimeout(() => {
      isInitializingSlideRef.current = false;
    }, 100);
  };

  // Handler functions for slide editor
  const handleAddBulletPoint = () => {
    setBulletPoints([...bulletPoints, ""]);
  };

  const handleUpdateBulletPoint = (index: number, value: string) => {
    const updated = [...bulletPoints];
    updated[index] = value;
    setBulletPoints(updated);
  };

  const handleDeleteBulletPoint = (index: number) => {
    const updated = bulletPoints.filter((_, i) => i !== index);
    setBulletPoints(updated);
  };

  // Handler functions for expandable list
  const handleAddExpandableItem = () => {
    setExpandableListItems([
      ...expandableListItems,
      { title: "", content: "" },
    ]);
  };

  const handleUpdateExpandableItem = (
    index: number,
    field: "title" | "content",
    value: string
  ) => {
    const updated = [...expandableListItems];
    updated[index] = { ...updated[index], [field]: value };
    setExpandableListItems(updated);
  };

  const handleDeleteExpandableItem = (index: number) => {
    const updated = expandableListItems.filter((_, i) => i !== index);
    setExpandableListItems(updated);
  };

  const handleToggleExpandableItem = (index: number) => {
    if (focusMode) {
      // Only one item can be expanded at a time in focus mode
      setExpandedItemIndex(expandedItemIndex === index ? null : index);
    } else {
      // Toggle the item expansion without focus mode restrictions
      setExpandedItemIndex(expandedItemIndex === index ? null : index);
    }
  };

  // Handler functions for image collection
  const handleAddImageToCollection = () => {
    setImageCollection([...imageCollection, { url: "" }]);
  };

  const handleRemoveImageFromCollection = (index: number) => {
    const updated = imageCollection.filter((_, i) => i !== index);
    setImageCollection(updated);
    if (expandedImageIndex === index) {
      setExpandedImageIndex(null);
    } else if (expandedImageIndex !== null && expandedImageIndex > index) {
      setExpandedImageIndex(expandedImageIndex - 1);
    }
  };

  const handleToggleImageDropdown = (index: number) => {
    setExpandedImageIndex(expandedImageIndex === index ? null : index);
  };

  const handleImageFileUpload = async (index: number, file: File) => {
    try {
      const uploadedUrl = await uploadImage(file).unwrap();
      const updated = [...imageCollection];
      updated[index] = { ...updated[index], url: uploadedUrl };
      setImageCollection(updated);
      // Close dropdown after successful upload
      setExpandedImageIndex(null);
    } catch (error) {
      console.error("Failed to upload image:", error);
    }
  };

  const handleUpdateImageInCollection = (
    index: number,
    url?: string,
    text?: string
  ) => {
    const updated = [...imageCollection];
    updated[index] = {
      ...updated[index],
      ...(url !== undefined && { url }),
      ...(text !== undefined && { text }),
    };
    setImageCollection(updated);
  };

  // Handler functions for video collection
  const handleAddVideoToCollection = () => {
    setVideoCollection([...videoCollection, { url: "" }]);
  };

  const handleRemoveVideoFromCollection = (index: number) => {
    const updated = videoCollection.filter((_, i) => i !== index);
    setVideoCollection(updated);
    if (expandedVideoIndex === index) {
      setExpandedVideoIndex(null);
    } else if (expandedVideoIndex !== null && expandedVideoIndex > index) {
      setExpandedVideoIndex(expandedVideoIndex - 1);
    }
  };

  const handleToggleVideoDropdown = (index: number) => {
    setExpandedVideoIndex(expandedVideoIndex === index ? null : index);
  };

  const handleVideoFileUpload = async (index: number, file: File) => {
    try {
      const uploadedUrl = await uploadVideo(file).unwrap();
      const updated = [...videoCollection];
      updated[index] = { ...updated[index], url: uploadedUrl };
      setVideoCollection(updated);
      // Close dropdown after successful upload
      setExpandedVideoIndex(null);
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
    const updated = [...videoCollection];
    updated[index] = {
      ...updated[index],
      ...(url !== undefined && { url }),
      ...(thumbnail !== undefined && { thumbnail }),
      ...(title !== undefined && { title }),
    };
    setVideoCollection(updated);
  };

  const handleCloseSlideEditor = () => {
    // Save any pending changes before closing
    if (hasUnsavedChangesRef.current && selectedSlide) {
      handleAutoSaveSlide();
    }

    // Clear auto-save timer
    if (autoSaveTimerRef.current) {
      clearTimeout(autoSaveTimerRef.current);
      autoSaveTimerRef.current = null;
    }

    // Reset flags
    hasUnsavedChangesRef.current = false;
    isInitializingSlideRef.current = false;

    setIsSlideEditorOpen(false);
    setSelectedSlide(null);
  };

  // Helper function to get slides array
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

  // Navigation handlers for slides
  const handlePreviousSlide = async () => {
    if (!selectedSlide || !selectedLessonForSlides) return;

    // Save current slide before navigating
    if (hasUnsavedChangesRef.current) {
      await handleAutoSaveSlide();
    }

    const slidesArray = getSlidesArray();
    const currentIndex = slidesArray.findIndex(
      (s: any) =>
        String(s.id || s.Id) === String(selectedSlide?.id || selectedSlide?.Id)
    );

    if (currentIndex > 0) {
      const previousSlide = slidesArray[currentIndex - 1];
      if (previousSlide) {
        populateSlideEditor(previousSlide);
      }
    }
  };

  const handleNextSlide = async () => {
    if (!selectedSlide || !selectedLessonForSlides) return;

    // Save current slide before navigating
    if (hasUnsavedChangesRef.current) {
      await handleAutoSaveSlide();
    }

    const slidesArray = getSlidesArray();
    const currentIndex = slidesArray.findIndex(
      (s: any) =>
        String(s.id || s.Id) === String(selectedSlide?.id || selectedSlide?.Id)
    );

    if (currentIndex < slidesArray.length - 1) {
      const nextSlide = slidesArray[currentIndex + 1];
      if (nextSlide) {
        populateSlideEditor(nextSlide);
      }
    }
  };

  // If slide editor is open, show the three-panel editor view
  if (isSlideEditorOpen && selectedSlide) {
    // Get the slide type
    const slideType =
      selectedSlide.type ||
      selectedSlide.Type ||
      selectedSlide.slideType ||
      selectedSlide.SlideType;

    // Check slide types
    const isExpandableList = slideType === "expandable-list";
    const isBulletedList = slideType === "bulleted-list";
    const isSingleImage = slideType === "single-image";
    const isImageCollection = slideType === "image-collection";
    const isSingleVideo = slideType === "single-video";
    const isVideoCollection = slideType === "video-collection";

    return (
      <Box sx={{ display: "flex", height: "calc(100vh - 100px)", gap: 2 }}>
        {/* Left Panel - Lessons Outline */}
        <LessonsPanel
          sx={{
            width: 300,
            minWidth: 300,
            maxWidth: 300,
            height: "100%",
            flexShrink: 0,
          }}
        >
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              mb: 2,
            }}
          >
            <Typography variant="h6">Lessons</Typography>
            <Box sx={{ display: "flex", gap: 0.5 }}>
              <IconButton size="small" sx={{ color: "text.secondary" }}>
                <MoreVertIcon fontSize="small" />
              </IconButton>
              <IconButton
                size="small"
                sx={{ bgcolor: "#6366f1", color: "white" }}
                onClick={handleAddLessonClick}
              >
                <AddIcon fontSize="small" />
              </IconButton>
            </Box>
          </Box>

          <Box sx={{ maxHeight: "calc(100vh - 300px)", overflowY: "auto" }}>
            <List sx={{ p: 0 }}>
              {displayedLessons.map((lesson: any) => {
                const lessonId = String(lesson.id || lesson.Id || "");
                const isSelected =
                  selectedLessonForSlides &&
                  String(
                    selectedLessonForSlides.id ||
                      selectedLessonForSlides.Id ||
                      ""
                  ) === lessonId;
                return (
                  <React.Fragment key={lessonId}>
                    <ListItem
                      onClick={() => {
                        const newSelection = isSelected ? null : lesson;
                        setSelectedLessonForSlides(newSelection);
                        if (!newSelection) {
                          setIsSlideEditorOpen(false);
                        }
                      }}
                      sx={{
                        cursor: "pointer",
                        backgroundColor: isSelected ? "#f3f4f6" : "transparent",
                        "&:hover": { backgroundColor: "#f9fafb" },
                        mb: 0.5,
                      }}
                    >
                      <DescriptionIcon
                        sx={{ color: "#6366f1", fontSize: 20, mr: 1 }}
                      />
                      <ListItemText
                        primary={
                          lesson.title || lesson.Title || "Untitled Lesson"
                        }
                        primaryTypographyProps={{ fontSize: "0.875rem" }}
                      />
                    </ListItem>
                    {isSelected && (
                      <Box sx={{ pl: 3, pr: 1, pb: 1 }}>
                        {(() => {
                          const slidesArray = Array.isArray(slidesData?.data)
                            ? slidesData.data
                            : Array.isArray(slidesData?.data?.items)
                            ? slidesData.data.items
                            : Array.isArray(slidesData?.items)
                            ? slidesData.items
                            : Array.isArray(slidesData)
                            ? slidesData
                            : [];

                          return (
                            <>
                              <List sx={{ p: 0 }}>
                                {slidesArray.map(
                                  (slideItem: any, index: number) => {
                                    const slideItemId = String(
                                      slideItem.id || slideItem.Id || ""
                                    );
                                    const isSlideSelected =
                                      selectedSlide &&
                                      String(
                                        selectedSlide.id ||
                                          selectedSlide.Id ||
                                          ""
                                      ) === slideItemId;
                                    return (
                                      <ListItem
                                        key={slideItemId}
                                        onClick={() => {
                                          // Check slide type - backend uses 'type' or 'Type'
                                          const slideType =
                                            slideItem.type ||
                                            slideItem.Type ||
                                            slideItem.slideType ||
                                            slideItem.SlideType;

                                          console.log(
                                            "Slide clicked:",
                                            slideItem
                                          );
                                          console.log("Slide type:", slideType);

                                          // Open editor for supported slide types
                                          if (
                                            slideType === "bulleted-list" ||
                                            slideType === "expandable-list" ||
                                            slideType === "single-image" ||
                                            slideType === "image-collection" ||
                                            slideType === "single-video" ||
                                            slideType === "video-collection" ||
                                            !slideType
                                          ) {
                                            populateSlideEditor(slideItem);
                                          }
                                        }}
                                        sx={{
                                          py: 0.5,
                                          px: 1,
                                          cursor: "pointer",
                                          backgroundColor: isSlideSelected
                                            ? "#eef2ff"
                                            : "transparent",
                                          borderLeft: isSlideSelected
                                            ? "3px solid #6366f1"
                                            : "3px solid transparent",
                                          "&:hover": {
                                            backgroundColor: "#f3f4f6",
                                          },
                                        }}
                                      >
                                        <ListItemText
                                          primary={`${index + 1}. ${
                                            slideItem.title ||
                                            slideItem.Title ||
                                            "Untitled Slide"
                                          }`}
                                          primaryTypographyProps={{
                                            fontSize: "0.75rem",
                                            fontWeight: isSlideSelected
                                              ? 500
                                              : 400,
                                          }}
                                        />
                                      </ListItem>
                                    );
                                  }
                                )}
                              </List>
                              <Button
                                variant="contained"
                                size="small"
                                startIcon={<AddIcon />}
                                onClick={handleOpenSlideLibrary}
                                disabled={isCreatingSlide}
                                sx={{
                                  mt: 1,
                                  bgcolor: "#6366f1",
                                  "&:hover": { bgcolor: "#4f46e5" },
                                  fontSize: "0.75rem",
                                  textTransform: "none",
                                  width: "100%",
                                }}
                              >
                                New Slide
                              </Button>
                            </>
                          );
                        })()}
                      </Box>
                    )}
                  </React.Fragment>
                );
              })}
            </List>
          </Box>

          <Box sx={{ mt: 2, display: "flex", flexDirection: "column", gap: 1 }}>
            <ListItemButton sx={{ borderRadius: 1, py: 0.5 }}>
              <SettingsIcon sx={{ mr: 1, fontSize: 20, color: "#6366f1" }} />
              <Typography variant="body2" sx={{ color: "#6366f1" }}>
                Settings
              </Typography>
            </ListItemButton>
            <ListItemButton sx={{ borderRadius: 1, py: 0.5 }}>
              <BookIcon sx={{ mr: 1, fontSize: 20, color: "#6366f1" }} />
              <Typography variant="body2" sx={{ color: "#6366f1" }}>
                Overview
              </Typography>
            </ListItemButton>
          </Box>
        </LessonsPanel>

        {/* Middle Panel - Slide Preview */}
        <Paper
          sx={{
            flex: 1,
            p: 3,
            display: "flex",
            flexDirection: "column",
            backgroundColor: "#f9fafb",
          }}
        >
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              mb: 3,
            }}
          >
            <Typography variant="h5" sx={{ fontWeight: 600 }}>
              {selectedLessonForSlides?.title ||
                selectedLessonForSlides?.Title ||
                "Untitled Lesson"}
            </Typography>
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
              <Typography variant="body2" color="text.secondary">
                {(() => {
                  const slidesArray = Array.isArray(slidesData?.data)
                    ? slidesData.data
                    : Array.isArray(slidesData?.data?.items)
                    ? slidesData.data.items
                    : Array.isArray(slidesData?.items)
                    ? slidesData.items
                    : Array.isArray(slidesData)
                    ? slidesData
                    : [];
                  const currentIndex = slidesArray.findIndex(
                    (s: any) =>
                      String(s.id || s.Id) ===
                      String(selectedSlide?.id || selectedSlide?.Id)
                  );
                  return `${currentIndex >= 0 ? currentIndex + 1 : 1}/${
                    slidesArray.length || 1
                  }`;
                })()}
              </Typography>
              <IconButton size="small">
                <MoreVertIcon fontSize="small" />
              </IconButton>
            </Box>
          </Box>

          {/* Slide Preview Content */}
          <Box
            sx={{
              flex: 1,
              backgroundColor: "white",
              borderRadius: 2,
              p: 4,
              display: "flex",
              flexDirection: "column",
              justifyContent: "center",
              boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
            }}
          >
            <Typography
              variant="h4"
              sx={{
                fontWeight: 600,
                mb: 4,
                textAlign: "center",
                color: "#1e293b",
              }}
            >
              {slideTitle}
            </Typography>

            <Box sx={{ maxWidth: 600, mx: "auto", width: "100%" }}>
              {isSingleImage ? (
                // Single Image Preview
                <Box>
                  {imageUrl ? (
                    <Box
                      component="img"
                      src={imageUrl}
                      alt={slideTitle}
                      onClick={() => handleOpenPreview(imageUrl, "image")}
                      sx={{
                        width: "100%",
                        height: "auto",
                        maxHeight: 400,
                        objectFit: "contain",
                        borderRadius: 1,
                        mb: 2,
                        cursor: "pointer",
                        "&:hover": {
                          opacity: 0.9,
                        },
                      }}
                    />
                  ) : (
                    <Box
                      sx={{
                        width: "100%",
                        height: 300,
                        backgroundColor: "#f3f4f6",
                        borderRadius: 1,
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        mb: 2,
                        border: "2px dashed #d1d5db",
                      }}
                    >
                      <Typography color="text.secondary">
                        No image uploaded
                      </Typography>
                    </Box>
                  )}
                  {imageText && (
                    <Typography
                      variant="body1"
                      sx={{
                        textAlign: "center",
                        color: "#374151",
                        fontSize: "0.875rem",
                      }}
                    >
                      {imageText}
                    </Typography>
                  )}
                </Box>
              ) : isImageCollection ? (
                // Image Collection Preview - Scrollable
                <Box
                  sx={{
                    maxHeight: "calc(100vh - 400px)",
                    overflowY: "auto",
                    overflowX: "hidden",
                  }}
                >
                  {imageCollection.length === 0 ? (
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        minHeight: 200,
                      }}
                    >
                      <Typography color="text.secondary">
                        No images added yet
                      </Typography>
                    </Box>
                  ) : (
                    <Grid container spacing={2}>
                      {imageCollection.map((img, index) => (
                        <Grid item xs={4} key={index}>
                          {img.url ? (
                            <Box>
                              <Box
                                component="img"
                                src={img.url}
                                alt={img.alt || `Image ${index + 1}`}
                                onClick={() =>
                                  handleOpenPreview(img.url, "image")
                                }
                                sx={{
                                  width: "100%",
                                  height: 150,
                                  objectFit: "cover",
                                  borderRadius: 1,
                                  border: "1px solid #e5e7eb",
                                  cursor: "pointer",
                                  "&:hover": {
                                    opacity: 0.9,
                                  },
                                }}
                              />
                              {img.text && (
                                <Typography
                                  variant="body2"
                                  sx={{
                                    mt: 1,
                                    textAlign: "center",
                                    color: "#374151",
                                    fontSize: "0.875rem",
                                  }}
                                >
                                  {img.text}
                                </Typography>
                              )}
                            </Box>
                          ) : (
                            <Box
                              sx={{
                                width: "100%",
                                height: 150,
                                backgroundColor: "#f3f4f6",
                                borderRadius: 1,
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "center",
                                border: "2px dashed #d1d5db",
                              }}
                            >
                              <ImageIcon sx={{ color: "#9ca3af" }} />
                            </Box>
                          )}
                        </Grid>
                      ))}
                    </Grid>
                  )}
                  {imageCollection.length > 0 && (
                    <Typography
                      variant="body2"
                      sx={{
                        mt: 2,
                        textAlign: "center",
                        color: "#9ca3af",
                        fontSize: "0.875rem",
                      }}
                    >
                      Select each image for more details
                    </Typography>
                  )}
                </Box>
              ) : isSingleVideo ? (
                // Single Video Preview
                <Box>
                  {videoThumbnail ? (
                    <Box
                      component="img"
                      src={videoThumbnail}
                      alt="Video thumbnail"
                      onClick={() => handleOpenPreview(videoUrl, "video")}
                      sx={{
                        width: "100%",
                        maxHeight: 400,
                        borderRadius: 1,
                        mb: 2,
                        objectFit: "contain",
                        border: "1px solid #e5e7eb",
                        cursor: "pointer",
                        "&:hover": {
                          opacity: 0.9,
                        },
                      }}
                    />
                  ) : videoUrl ? (
                    <Box
                      sx={{
                        width: "100%",
                        maxHeight: 400,
                        borderRadius: 1,
                        mb: 2,
                        overflow: "hidden",
                      }}
                    >
                      <video
                        src={videoUrl}
                        controls
                        onClick={(e) => {
                          e.stopPropagation();
                          handleOpenPreview(videoUrl, "video");
                        }}
                        style={{
                          width: "100%",
                          height: "auto",
                          maxHeight: 400,
                          borderRadius: "8px",
                          cursor: "pointer",
                        }}
                      />
                    </Box>
                  ) : (
                    <Box
                      sx={{
                        width: "100%",
                        height: 300,
                        backgroundColor: "#f3f4f6",
                        borderRadius: 1,
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        mb: 2,
                        border: "2px dashed #d1d5db",
                      }}
                    >
                      <Typography color="text.secondary">
                        No video uploaded
                      </Typography>
                    </Box>
                  )}
                  {videoText && (
                    <Typography
                      variant="body1"
                      sx={{
                        textAlign: "center",
                        color: "#374151",
                        fontSize: "0.875rem",
                      }}
                    >
                      {videoText}
                    </Typography>
                  )}
                </Box>
              ) : isVideoCollection ? (
                // Video Collection Preview - Scrollable
                <Box
                  sx={{
                    maxHeight: "calc(100vh - 400px)",
                    overflowY: "auto",
                    overflowX: "hidden",
                  }}
                >
                  {videoCollection.length === 0 ? (
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        minHeight: 200,
                      }}
                    >
                      <Typography color="text.secondary">
                        No videos added yet
                      </Typography>
                    </Box>
                  ) : (
                    <Grid container spacing={2}>
                      {videoCollection.map((video, index) => (
                        <Grid item xs={4} key={index}>
                          <Box>
                            {video.thumbnail ? (
                              <Box
                                component="img"
                                src={video.thumbnail}
                                alt={video.title || `Video ${index + 1}`}
                                onClick={() =>
                                  handleOpenPreview(video.url, "video")
                                }
                                sx={{
                                  width: "100%",
                                  height: 150,
                                  objectFit: "cover",
                                  borderRadius: 1,
                                  border: "1px solid #e5e7eb",
                                  cursor: "pointer",
                                  "&:hover": {
                                    opacity: 0.9,
                                  },
                                }}
                              />
                            ) : video.url ? (
                              <Box
                                onClick={() =>
                                  handleOpenPreview(video.url, "video")
                                }
                                sx={{
                                  width: "100%",
                                  height: 150,
                                  borderRadius: 1,
                                  border: "1px solid #e5e7eb",
                                  overflow: "hidden",
                                  cursor: "pointer",
                                  "&:hover": {
                                    opacity: 0.9,
                                  },
                                }}
                              >
                                <video
                                  src={video.url}
                                  controls
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    handleOpenPreview(video.url, "video");
                                  }}
                                  style={{
                                    width: "100%",
                                    height: "100%",
                                    objectFit: "cover",
                                    borderRadius: "8px",
                                  }}
                                />
                              </Box>
                            ) : (
                              <Box
                                sx={{
                                  width: "100%",
                                  height: 150,
                                  backgroundColor: "#f3f4f6",
                                  borderRadius: 1,
                                  display: "flex",
                                  alignItems: "center",
                                  justifyContent: "center",
                                  border: "2px dashed #d1d5db",
                                }}
                              >
                                <VideoIcon sx={{ color: "#9ca3af" }} />
                              </Box>
                            )}
                            {video.title && (
                              <Typography
                                variant="body2"
                                sx={{
                                  mt: 1,
                                  textAlign: "center",
                                  color: "#374151",
                                  fontSize: "0.875rem",
                                }}
                              >
                                {video.title}
                              </Typography>
                            )}
                          </Box>
                        </Grid>
                      ))}
                    </Grid>
                  )}
                </Box>
              ) : isExpandableList ? (
                // Expandable List Preview
                <Box>
                  {expandableListItems.map((item, index) => {
                    const isExpanded =
                      focusMode && expandedItemIndex === index
                        ? true
                        : !focusMode && expandedItemIndex === index;

                    return (
                      <Box
                        key={index}
                        sx={{
                          mb: 2,
                          border: "1px solid #e5e7eb",
                          borderRadius: 1,
                          overflow: "hidden",
                        }}
                      >
                        <Box
                          onClick={() => handleToggleExpandableItem(index)}
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            p: 2,
                            cursor: "pointer",
                            backgroundColor: isExpanded
                              ? "#f3f4f6"
                              : "transparent",
                            "&:hover": { backgroundColor: "#f9fafb" },
                          }}
                        >
                          <Typography
                            variant="body1"
                            sx={{
                              fontSize: "1rem",
                              fontWeight: 500,
                              color: "#374151",
                              flex: 1,
                            }}
                          >
                            {item.title || `Item ${index + 1}`}
                          </Typography>
                          {isExpanded ? (
                            <ExpandLessIcon sx={{ color: "#6b7280" }} />
                          ) : (
                            <ChevronRightIcon sx={{ color: "#6b7280" }} />
                          )}
                        </Box>
                        {isExpanded && (
                          <Collapse in={isExpanded}>
                            <Box sx={{ p: 2, pt: 0 }}>
                              <Typography
                                variant="body2"
                                sx={{
                                  color: "#6b7280",
                                  fontSize: "0.875rem",
                                }}
                              >
                                {item.content || "Content for item"}
                              </Typography>
                            </Box>
                          </Collapse>
                        )}
                      </Box>
                    );
                  })}
                  <Typography
                    variant="body2"
                    sx={{
                      mt: 2,
                      textAlign: "center",
                      color: "#9ca3af",
                      fontSize: "0.875rem",
                    }}
                  >
                    {promptText}
                  </Typography>
                </Box>
              ) : (
                // Bulleted List Preview
                bulletPoints.map((point, index) => (
                  <Box
                    key={index}
                    sx={{
                      display: "flex",
                      alignItems: "flex-start",
                      mb: 2,
                    }}
                  >
                    <Typography
                      sx={{
                        fontSize: "1.5rem",
                        lineHeight: 1,
                        mr: 2,
                        color: "#6366f1",
                      }}
                    >
                      •
                    </Typography>
                    <Typography
                      variant="body1"
                      sx={{
                        fontSize: "1.125rem",
                        color: "#374151",
                        flex: 1,
                      }}
                    >
                      {point || `Bullet point ${index + 1}`}
                    </Typography>
                  </Box>
                ))
              )}
            </Box>
          </Box>

          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              mt: 3,
            }}
          >
            <Button
              variant="contained"
              sx={{
                bgcolor: "#6366f1",
                "&:hover": { bgcolor: "#4f46e5" },
                textTransform: "none",
              }}
            >
              {doneText || "Continue"}
            </Button>
            <Box sx={{ display: "flex", gap: 1 }}>
              {(() => {
                const slidesArray = getSlidesArray();
                const currentIndex = slidesArray.findIndex(
                  (s: any) =>
                    String(s.id || s.Id) ===
                    String(selectedSlide?.id || selectedSlide?.Id)
                );
                const isFirstSlide = currentIndex <= 0;
                const isLastSlide = currentIndex >= slidesArray.length - 1;

                return (
                  <>
                    <IconButton
                      onClick={handlePreviousSlide}
                      disabled={isFirstSlide}
                      sx={{
                        "&:disabled": {
                          opacity: 0.5,
                        },
                      }}
                    >
                      <ArrowBackIcon />
                    </IconButton>
                    <IconButton
                      onClick={handleNextSlide}
                      disabled={isLastSlide}
                      sx={{
                        "&:disabled": {
                          opacity: 0.5,
                        },
                      }}
                    >
                      <ArrowForwardIcon />
                    </IconButton>
                  </>
                );
              })()}
            </Box>
          </Box>
        </Paper>

        {/* Right Panel - Properties Editor */}
        <Paper sx={{ width: 350, p: 2, overflowY: "auto", maxHeight: "100%" }}>
          {/* Title Section */}
          <Box sx={{ mb: 3 }}>
            <Typography
              variant="subtitle2"
              sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
            >
              Title
            </Typography>
            <TextField
              fullWidth
              value={slideTitle}
              onChange={(e) => setSlideTitle(e.target.value)}
              placeholder="Enter title"
              size="small"
              sx={{
                "& .MuiOutlinedInput-root": {
                  fontSize: "0.875rem",
                },
              }}
            />
          </Box>

          <Divider sx={{ my: 2 }} />

          {/* Content Section */}
          {isSingleImage ? (
            // Single Image Properties
            <>
              <Box sx={{ mb: 3 }}>
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
                >
                  Image
                </Typography>
                <TextField
                  fullWidth
                  value={imageUrl}
                  onChange={(e) => setImageUrl(e.target.value)}
                  placeholder="Enter image URL or upload"
                  size="small"
                  sx={{
                    mb: 1,
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
                {imageUrl && (
                  <Box
                    component="img"
                    src={imageUrl}
                    alt="Preview"
                    sx={{
                      width: "100%",
                      maxHeight: 150,
                      objectFit: "contain",
                      borderRadius: 1,
                      mt: 1,
                      border: "1px solid #e5e7eb",
                    }}
                  />
                )}
                <Typography
                  variant="caption"
                  sx={{
                    color: "#6b7280",
                    fontSize: "0.75rem",
                    mt: 1,
                    display: "block",
                  }}
                >
                  Note: Supported file types: jpg, jpeg, png, gif, svg, heic
                </Typography>
              </Box>
              <Divider sx={{ my: 2 }} />
              <Box sx={{ mb: 3 }}>
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
                >
                  Content
                </Typography>
                <TextField
                  fullWidth
                  value={imageText}
                  onChange={(e) => setImageText(e.target.value)}
                  placeholder="Text to display below image"
                  multiline
                  rows={3}
                  size="small"
                  sx={{
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
              </Box>
              <Divider sx={{ my: 2 }} />
            </>
          ) : isImageCollection ? (
            // Image Collection Properties with Dropdowns
            <>
              <Box sx={{ mb: 3 }}>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    mb: 1,
                  }}
                >
                  <Typography
                    variant="subtitle2"
                    sx={{ fontWeight: 600, color: "#374151" }}
                  >
                    Images
                  </Typography>
                  <Button
                    size="small"
                    startIcon={<AddIcon />}
                    onClick={handleAddImageToCollection}
                    sx={{
                      textTransform: "none",
                      fontSize: "0.75rem",
                      color: "#6366f1",
                    }}
                  >
                    Add Image
                  </Button>
                </Box>

                {imageCollection.map((img, index) => (
                  <Box key={index} sx={{ mb: 2 }}>
                    <FormControl fullWidth size="small">
                      <InputLabel
                        id={`image-dropdown-${index}`}
                        sx={{ fontSize: "0.875rem" }}
                      >
                        Image {index + 1}
                      </InputLabel>
                      <Select
                        labelId={`image-dropdown-${index}`}
                        label={`Image ${index + 1}`}
                        value={expandedImageIndex === index ? "open" : "closed"}
                        open={expandedImageIndex === index}
                        onOpen={() => handleToggleImageDropdown(index)}
                        onClose={() => handleToggleImageDropdown(index)}
                        MenuProps={{
                          PaperProps: {
                            onClick: (e: React.MouseEvent) => {
                              // Don't close menu when clicking inside the menu content
                              if (
                                (e.target as HTMLElement).closest(
                                  'input[type="file"]'
                                ) ||
                                (e.target as HTMLElement).closest("button") ||
                                (e.target as HTMLElement).closest("label") ||
                                (e.target as HTMLElement).closest(
                                  'input[type="text"]'
                                )
                              ) {
                                e.stopPropagation();
                              }
                            },
                          },
                          disableAutoFocusItem: true,
                        }}
                        sx={{
                          fontSize: "0.875rem",
                          mb: 1,
                        }}
                        renderValue={() => (
                          <Box
                            sx={{
                              display: "flex",
                              alignItems: "center",
                              gap: 1,
                            }}
                          >
                            {img.url ? (
                              <>
                                <Box
                                  component="img"
                                  src={img.url}
                                  alt={`Preview ${index + 1}`}
                                  sx={{
                                    width: 40,
                                    height: 40,
                                    objectFit: "cover",
                                    borderRadius: 0.5,
                                  }}
                                />
                                <Typography variant="body2" sx={{ flex: 1 }}>
                                  Image {index + 1}
                                </Typography>
                              </>
                            ) : (
                              <>
                                <ImageIcon
                                  sx={{ fontSize: 20, color: "#9ca3af" }}
                                />
                                <Typography
                                  variant="body2"
                                  sx={{ flex: 1, color: "#9ca3af" }}
                                >
                                  Upload Image {index + 1}
                                </Typography>
                              </>
                            )}
                          </Box>
                        )}
                      >
                        <MenuItem
                          onClick={(e) => {
                            // Only stop propagation to prevent menu close, don't prevent default
                            e.stopPropagation();
                          }}
                          sx={{
                            p: 0,
                            "&:hover": { backgroundColor: "transparent" },
                          }}
                        >
                          <Box
                            sx={{
                              display: "flex",
                              flexDirection: "column",
                              gap: 1,
                              width: "100%",
                              py: 1,
                              px: 2,
                            }}
                          >
                            <input
                              accept="image/*"
                              style={{ display: "none" }}
                              id={`image-upload-${index}`}
                              type="file"
                              onChange={(e) => {
                                const file = e.target.files?.[0];
                                if (file) {
                                  handleImageFileUpload(index, file);
                                  // Reset input value to allow uploading same file again
                                  e.target.value = "";
                                }
                              }}
                            />
                            <label
                              htmlFor={`image-upload-${index}`}
                              style={{ width: "100%", cursor: "pointer" }}
                              onClick={(e) => {
                                e.stopPropagation();
                              }}
                            >
                              <Button
                                variant="outlined"
                                component="span"
                                startIcon={<ImageIcon />}
                                fullWidth
                                disabled={isUploadingImage}
                                onClick={(e) => {
                                  e.stopPropagation();
                                  // Directly trigger the file input
                                  const fileInput = document.getElementById(
                                    `image-upload-${index}`
                                  ) as HTMLInputElement;
                                  if (fileInput) {
                                    fileInput.click();
                                  }
                                }}
                                sx={{
                                  textTransform: "none",
                                  borderColor: "#6366f1",
                                  color: "#6366f1",
                                  "&:hover": {
                                    borderColor: "#4f46e5",
                                    backgroundColor: "#eef2ff",
                                  },
                                }}
                              >
                                {isUploadingImage
                                  ? "Uploading..."
                                  : "Upload Image"}
                              </Button>
                            </label>
                            <TextField
                              fullWidth
                              size="small"
                              placeholder="Or enter image URL"
                              value={img.url || ""}
                              onChange={(e) =>
                                handleUpdateImageInCollection(
                                  index,
                                  e.target.value
                                )
                              }
                              sx={{
                                mt: 1,
                                "& .MuiOutlinedInput-root": {
                                  fontSize: "0.875rem",
                                },
                              }}
                            />
                            {img.url && (
                              <Box
                                component="img"
                                src={img.url}
                                alt={`Preview ${index + 1}`}
                                sx={{
                                  width: "100%",
                                  maxHeight: 150,
                                  objectFit: "contain",
                                  borderRadius: 1,
                                  border: "1px solid #e5e7eb",
                                  mt: 1,
                                }}
                              />
                            )}
                            <TextField
                              fullWidth
                              size="small"
                              placeholder="Text to display below image"
                              value={img.text || ""}
                              onChange={(e) => {
                                e.stopPropagation();
                                const updated = [...imageCollection];
                                updated[index] = {
                                  ...updated[index],
                                  text: e.target.value,
                                };
                                setImageCollection(updated);
                              }}
                              onClick={(e) => e.stopPropagation()}
                              onKeyDown={(e) => e.stopPropagation()}
                              sx={{
                                mt: 1,
                                "& .MuiOutlinedInput-root": {
                                  fontSize: "0.875rem",
                                },
                              }}
                            />
                            <IconButton
                              size="small"
                              onClick={(e) => {
                                e.stopPropagation();
                                handleRemoveImageFromCollection(index);
                              }}
                              sx={{
                                color: "#ef4444",
                                mt: 1,
                                alignSelf: "flex-end",
                              }}
                            >
                              <DeleteOutlineIcon fontSize="small" />
                            </IconButton>
                          </Box>
                        </MenuItem>
                      </Select>
                    </FormControl>
                  </Box>
                ))}

                {imageCollection.length === 0 && (
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ textAlign: "center", py: 2 }}
                  >
                    Click "Add Image" to add images to this collection
                  </Typography>
                )}

                <Typography
                  variant="caption"
                  sx={{
                    color: "#6b7280",
                    fontSize: "0.75rem",
                    display: "block",
                    mt: 1,
                  }}
                >
                  Note: Supported file types: jpg, jpeg, png, gif, svg, heic
                </Typography>
              </Box>
              <Divider sx={{ my: 2 }} />
            </>
          ) : isSingleVideo ? (
            // Single Video Properties
            <>
              <Box sx={{ mb: 3 }}>
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
                >
                  Video
                </Typography>
                <input
                  accept="video/*"
                  style={{ display: "none" }}
                  id="single-video-upload"
                  type="file"
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (file) {
                      try {
                        const uploadedUrl = await uploadVideo(file).unwrap();
                        setVideoUrl(uploadedUrl);
                        e.target.value = "";
                      } catch (error) {
                        console.error("Failed to upload video:", error);
                      }
                    }
                  }}
                />
                <label htmlFor="single-video-upload" style={{ width: "100%" }}>
                  <Button
                    variant="outlined"
                    component="span"
                    startIcon={<VideoIcon />}
                    fullWidth
                    disabled={isUploadingVideo}
                    sx={{
                      textTransform: "none",
                      borderColor: "#6366f1",
                      color: "#6366f1",
                      mb: 1,
                      "&:hover": {
                        borderColor: "#4f46e5",
                        backgroundColor: "#eef2ff",
                      },
                    }}
                  >
                    {isUploadingVideo ? "Uploading..." : "Upload Video"}
                  </Button>
                </label>
                <TextField
                  fullWidth
                  value={videoUrl}
                  onChange={(e) => setVideoUrl(e.target.value)}
                  placeholder="Or enter video URL"
                  size="small"
                  sx={{
                    mt: 1,
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
                {videoUrl && (
                  <Box
                    sx={{
                      width: "100%",
                      maxHeight: 150,
                      borderRadius: 1,
                      mt: 1,
                      border: "1px solid #e5e7eb",
                      overflow: "hidden",
                    }}
                  >
                    <video
                      src={videoUrl}
                      controls
                      style={{
                        width: "100%",
                        height: "auto",
                        maxHeight: 150,
                        borderRadius: "8px",
                      }}
                    />
                  </Box>
                )}
              </Box>
              <Divider sx={{ my: 2 }} />
              <Box sx={{ mb: 3 }}>
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
                >
                  Thumbnail
                </Typography>
                <input
                  accept="image/*"
                  style={{ display: "none" }}
                  id="video-thumbnail-upload"
                  type="file"
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (file) {
                      try {
                        const uploadedUrl = await uploadImage(file).unwrap();
                        setVideoThumbnail(uploadedUrl);
                        e.target.value = "";
                      } catch (error) {
                        console.error("Failed to upload thumbnail:", error);
                      }
                    }
                  }}
                />
                <label
                  htmlFor="video-thumbnail-upload"
                  style={{ width: "100%" }}
                >
                  <Button
                    variant="outlined"
                    component="span"
                    startIcon={<ImageIcon />}
                    fullWidth
                    disabled={isUploadingImage}
                    sx={{
                      textTransform: "none",
                      borderColor: "#6366f1",
                      color: "#6366f1",
                      mb: 1,
                      "&:hover": {
                        borderColor: "#4f46e5",
                        backgroundColor: "#eef2ff",
                      },
                    }}
                  >
                    {isUploadingImage ? "Uploading..." : "Upload Thumbnail"}
                  </Button>
                </label>
                <TextField
                  fullWidth
                  value={videoThumbnail}
                  onChange={(e) => setVideoThumbnail(e.target.value)}
                  placeholder="Or enter thumbnail image URL"
                  size="small"
                  sx={{
                    mt: 1,
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
                {videoThumbnail && (
                  <Box
                    component="img"
                    src={videoThumbnail}
                    alt="Thumbnail preview"
                    sx={{
                      width: "100%",
                      maxHeight: 150,
                      objectFit: "contain",
                      borderRadius: 1,
                      border: "1px solid #e5e7eb",
                      mt: 1,
                    }}
                  />
                )}
              </Box>
              <Divider sx={{ my: 2 }} />
              <Box sx={{ mb: 3 }}>
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
                >
                  Content
                </Typography>
                <TextField
                  fullWidth
                  value={videoText}
                  onChange={(e) => setVideoText(e.target.value)}
                  placeholder="Text to display below video"
                  multiline
                  rows={3}
                  size="small"
                  sx={{
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
              </Box>
              <Divider sx={{ my: 2 }} />
            </>
          ) : isVideoCollection ? (
            // Video Collection Properties with Dropdowns
            <>
              <Box sx={{ mb: 3 }}>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    mb: 1,
                  }}
                >
                  <Typography
                    variant="subtitle2"
                    sx={{ fontWeight: 600, color: "#374151" }}
                  >
                    Videos
                  </Typography>
                  <Button
                    size="small"
                    startIcon={<AddIcon />}
                    onClick={handleAddVideoToCollection}
                    sx={{
                      textTransform: "none",
                      fontSize: "0.75rem",
                      color: "#6366f1",
                    }}
                  >
                    Add Video
                  </Button>
                </Box>

                {videoCollection.map((video, index) => (
                  <Box key={index} sx={{ mb: 2 }}>
                    <FormControl fullWidth size="small">
                      <InputLabel
                        id={`video-dropdown-${index}`}
                        sx={{ fontSize: "0.875rem" }}
                      >
                        Video {index + 1}
                      </InputLabel>
                      <Select
                        labelId={`video-dropdown-${index}`}
                        label={`Video ${index + 1}`}
                        value={expandedVideoIndex === index ? "open" : "closed"}
                        open={expandedVideoIndex === index}
                        onOpen={() => handleToggleVideoDropdown(index)}
                        onClose={() => handleToggleVideoDropdown(index)}
                        MenuProps={{
                          PaperProps: {
                            onClick: (e: React.MouseEvent) => {
                              // Don't close menu when clicking inside the menu content
                              if (
                                (e.target as HTMLElement).closest(
                                  'input[type="file"]'
                                ) ||
                                (e.target as HTMLElement).closest("button") ||
                                (e.target as HTMLElement).closest("label") ||
                                (e.target as HTMLElement).closest(
                                  'input[type="text"]'
                                )
                              ) {
                                e.stopPropagation();
                              }
                            },
                          },
                          disableAutoFocusItem: true,
                        }}
                        sx={{
                          fontSize: "0.875rem",
                          mb: 1,
                        }}
                        renderValue={() => (
                          <Box
                            sx={{
                              display: "flex",
                              alignItems: "center",
                              gap: 1,
                            }}
                          >
                            {video.url ? (
                              <>
                                <VideoIcon
                                  sx={{ fontSize: 20, color: "#6366f1" }}
                                />
                                <Typography variant="body2" sx={{ flex: 1 }}>
                                  Video {index + 1}
                                </Typography>
                              </>
                            ) : (
                              <>
                                <VideoIcon
                                  sx={{ fontSize: 20, color: "#9ca3af" }}
                                />
                                <Typography
                                  variant="body2"
                                  sx={{ flex: 1, color: "#9ca3af" }}
                                >
                                  Upload Video {index + 1}
                                </Typography>
                              </>
                            )}
                          </Box>
                        )}
                      >
                        <MenuItem
                          onClick={(e) => {
                            // Only stop propagation to prevent menu close, don't prevent default
                            e.stopPropagation();
                          }}
                          sx={{
                            p: 0,
                            "&:hover": { backgroundColor: "transparent" },
                          }}
                        >
                          <Box
                            sx={{
                              display: "flex",
                              flexDirection: "column",
                              gap: 1,
                              width: "100%",
                              py: 1,
                              px: 2,
                            }}
                          >
                            <input
                              accept="video/*"
                              style={{ display: "none" }}
                              id={`video-upload-${index}`}
                              type="file"
                              onChange={(e) => {
                                const file = e.target.files?.[0];
                                if (file) {
                                  handleVideoFileUpload(index, file);
                                  // Reset input value to allow uploading same file again
                                  e.target.value = "";
                                }
                              }}
                              onClick={(e) => {
                                e.stopPropagation();
                              }}
                            />
                            <Button
                              variant="outlined"
                              component="span"
                              startIcon={<VideoIcon />}
                              fullWidth
                              disabled={isUploadingVideo}
                              onClick={(e) => {
                                // Stop propagation to prevent menu from closing
                                e.stopPropagation();
                                // Directly trigger the file input
                                const fileInput = document.getElementById(
                                  `video-upload-${index}`
                                ) as HTMLInputElement;
                                if (fileInput) {
                                  fileInput.click();
                                }
                              }}
                              sx={{
                                textTransform: "none",
                                borderColor: "#6366f1",
                                color: "#6366f1",
                                "&:hover": {
                                  borderColor: "#4f46e5",
                                  backgroundColor: "#eef2ff",
                                },
                              }}
                            >
                              {isUploadingVideo
                                ? "Uploading..."
                                : "Upload Video"}
                            </Button>
                            <TextField
                              fullWidth
                              size="small"
                              placeholder="Or enter video URL"
                              value={video.url || ""}
                              onChange={(e) => {
                                e.stopPropagation();
                                handleUpdateVideoInCollection(
                                  index,
                                  e.target.value
                                );
                              }}
                              onClick={(e) => {
                                e.stopPropagation();
                              }}
                              onKeyDown={(e) => {
                                e.stopPropagation();
                              }}
                              sx={{
                                mt: 1,
                                "& .MuiOutlinedInput-root": {
                                  fontSize: "0.875rem",
                                },
                              }}
                            />
                            <TextField
                              fullWidth
                              size="small"
                              placeholder="Video title/description"
                              value={video.title || ""}
                              onChange={(e) => {
                                e.stopPropagation();
                                handleUpdateVideoInCollection(
                                  index,
                                  undefined,
                                  undefined,
                                  e.target.value
                                );
                              }}
                              onClick={(e) => e.stopPropagation()}
                              onKeyDown={(e) => e.stopPropagation()}
                              sx={{
                                mt: 1,
                                "& .MuiOutlinedInput-root": {
                                  fontSize: "0.875rem",
                                },
                              }}
                            />
                            <input
                              accept="image/*"
                              style={{ display: "none" }}
                              id={`video-thumbnail-upload-${index}`}
                              type="file"
                              onChange={async (e) => {
                                const file = e.target.files?.[0];
                                if (file) {
                                  try {
                                    const uploadedUrl = await uploadImage(
                                      file
                                    ).unwrap();
                                    handleUpdateVideoInCollection(
                                      index,
                                      undefined,
                                      uploadedUrl,
                                      undefined
                                    );
                                    e.target.value = "";
                                  } catch (error) {
                                    console.error(
                                      "Failed to upload thumbnail:",
                                      error
                                    );
                                  }
                                }
                              }}
                              onClick={(e) => {
                                e.stopPropagation();
                              }}
                            />
                            <label
                              htmlFor={`video-thumbnail-upload-${index}`}
                              style={{ width: "100%" }}
                              onClick={(e) => {
                                e.stopPropagation();
                              }}
                            >
                              <Button
                                variant="outlined"
                                component="span"
                                startIcon={<ImageIcon />}
                                fullWidth
                                disabled={isUploadingImage}
                                onClick={(e) => {
                                  e.stopPropagation();
                                  // Directly trigger the file input
                                  const fileInput = document.getElementById(
                                    `video-thumbnail-upload-${index}`
                                  ) as HTMLInputElement;
                                  if (fileInput) {
                                    fileInput.click();
                                  }
                                }}
                                sx={{
                                  textTransform: "none",
                                  borderColor: "#6366f1",
                                  color: "#6366f1",
                                  mt: 1,
                                  "&:hover": {
                                    borderColor: "#4f46e5",
                                    backgroundColor: "#eef2ff",
                                  },
                                }}
                              >
                                {isUploadingImage
                                  ? "Uploading..."
                                  : "Upload Thumbnail"}
                              </Button>
                            </label>
                            <TextField
                              fullWidth
                              size="small"
                              placeholder="Or enter thumbnail image URL"
                              value={video.thumbnail || ""}
                              onChange={(e) => {
                                e.stopPropagation();
                                handleUpdateVideoInCollection(
                                  index,
                                  undefined,
                                  e.target.value,
                                  undefined
                                );
                              }}
                              onClick={(e) => e.stopPropagation()}
                              onKeyDown={(e) => e.stopPropagation()}
                              sx={{
                                mt: 1,
                                "& .MuiOutlinedInput-root": {
                                  fontSize: "0.875rem",
                                },
                              }}
                            />
                            {video.url && (
                              <Box
                                sx={{
                                  width: "100%",
                                  maxHeight: 150,
                                  borderRadius: 1,
                                  border: "1px solid #e5e7eb",
                                  mt: 1,
                                  overflow: "hidden",
                                }}
                              >
                                <video
                                  src={video.url}
                                  controls
                                  onClick={(e) => {
                                    e.stopPropagation();
                                  }}
                                  style={{
                                    width: "100%",
                                    height: "auto",
                                    maxHeight: 150,
                                    borderRadius: "8px",
                                  }}
                                />
                              </Box>
                            )}
                            {video.thumbnail && (
                              <Box
                                component="img"
                                src={video.thumbnail}
                                alt="Thumbnail preview"
                                sx={{
                                  width: "100%",
                                  maxHeight: 150,
                                  objectFit: "contain",
                                  borderRadius: 1,
                                  border: "1px solid #e5e7eb",
                                  mt: 1,
                                }}
                              />
                            )}
                            <IconButton
                              size="small"
                              onClick={(e) => {
                                e.stopPropagation();
                                handleRemoveVideoFromCollection(index);
                              }}
                              sx={{
                                color: "#ef4444",
                                mt: 1,
                                alignSelf: "flex-end",
                              }}
                            >
                              <DeleteOutlineIcon fontSize="small" />
                            </IconButton>
                          </Box>
                        </MenuItem>
                      </Select>
                    </FormControl>
                  </Box>
                ))}

                {videoCollection.length === 0 && (
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ textAlign: "center", py: 2 }}
                  >
                    Click "Add Video" to add videos to this collection
                  </Typography>
                )}
              </Box>
              <Divider sx={{ my: 2 }} />
            </>
          ) : isExpandableList ? (
            // Expandable List Properties
            <>
              <Box sx={{ mb: 3 }}>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    mb: 1,
                    cursor: "pointer",
                  }}
                  onClick={() => setIsListExpanded(!isListExpanded)}
                >
                  <Typography
                    variant="subtitle2"
                    sx={{ fontWeight: 600, color: "#374151" }}
                  >
                    Expandable list
                  </Typography>
                  {isListExpanded ? (
                    <ExpandLessIcon sx={{ fontSize: 20, color: "#6b7280" }} />
                  ) : (
                    <ExpandMoreIcon sx={{ fontSize: 20, color: "#6b7280" }} />
                  )}
                </Box>

                <Collapse in={isListExpanded}>
                  <Box>
                    {expandableListItems.map((item, index) => (
                      <Box
                        key={index}
                        sx={{
                          mb: 2,
                          p: 2,
                          border: "1px solid #e5e7eb",
                          borderRadius: 1,
                        }}
                      >
                        <TextField
                          fullWidth
                          value={item.title}
                          onChange={(e) =>
                            handleUpdateExpandableItem(
                              index,
                              "title",
                              e.target.value
                            )
                          }
                          placeholder={`Item ${index + 1} title`}
                          size="small"
                          sx={{
                            mb: 1,
                            "& .MuiOutlinedInput-root": {
                              fontSize: "0.875rem",
                            },
                          }}
                        />
                        <TextField
                          fullWidth
                          value={item.content}
                          onChange={(e) =>
                            handleUpdateExpandableItem(
                              index,
                              "content",
                              e.target.value
                            )
                          }
                          placeholder={`Item ${index + 1} content`}
                          size="small"
                          multiline
                          rows={2}
                          sx={{
                            mb: 1,
                            "& .MuiOutlinedInput-root": {
                              fontSize: "0.875rem",
                            },
                          }}
                        />
                        <Box
                          sx={{ display: "flex", justifyContent: "flex-end" }}
                        >
                          <IconButton
                            size="small"
                            onClick={() => handleDeleteExpandableItem(index)}
                            sx={{ color: "#ef4444" }}
                          >
                            <DeleteOutlineIcon fontSize="small" />
                          </IconButton>
                        </Box>
                      </Box>
                    ))}
                    <Button
                      variant="outlined"
                      size="small"
                      startIcon={<AddIcon />}
                      onClick={handleAddExpandableItem}
                      fullWidth
                      sx={{
                        mt: 1,
                        borderColor: "#6366f1",
                        color: "#6366f1",
                        textTransform: "none",
                        "&:hover": {
                          borderColor: "#4f46e5",
                          bgcolor: "#eef2ff",
                        },
                      }}
                    >
                      Add an item
                    </Button>
                  </Box>
                </Collapse>
              </Box>

              <Divider sx={{ my: 2 }} />

              {/* Focus Section */}
              <Box sx={{ mb: 3 }}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={focusMode}
                      onChange={(e) => setFocusMode(e.target.checked)}
                      sx={{ color: "#6366f1" }}
                    />
                  }
                  label={
                    <Box>
                      <Typography
                        variant="subtitle2"
                        sx={{ fontWeight: 600, color: "#374151" }}
                      >
                        Focus
                      </Typography>
                      <Typography
                        variant="caption"
                        sx={{ color: "#6b7280", fontSize: "0.75rem" }}
                      >
                        Only reveal one item at a time
                      </Typography>
                    </Box>
                  }
                />
              </Box>

              <Divider sx={{ my: 2 }} />

              {/* Prompt Text Section */}
              <Box sx={{ mb: 3 }}>
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
                >
                  Prompt
                </Typography>
                <TextField
                  fullWidth
                  value={promptText}
                  onChange={(e) => setPromptText(e.target.value)}
                  placeholder="Prompt Text"
                  size="small"
                  sx={{
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
              </Box>

              <Divider sx={{ my: 2 }} />
            </>
          ) : (
            // Bulleted List Properties
            <>
              <Box sx={{ mb: 3 }}>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    mb: 1,
                    cursor: "pointer",
                  }}
                  onClick={() => setIsListExpanded(!isListExpanded)}
                >
                  <Typography
                    variant="subtitle2"
                    sx={{ fontWeight: 600, color: "#374151" }}
                  >
                    List
                  </Typography>
                  {isListExpanded ? (
                    <ExpandLessIcon sx={{ fontSize: 20, color: "#6b7280" }} />
                  ) : (
                    <ExpandMoreIcon sx={{ fontSize: 20, color: "#6b7280" }} />
                  )}
                </Box>

                <Collapse in={isListExpanded}>
                  <Box>
                    {bulletPoints.map((point, index) => (
                      <Box
                        key={index}
                        sx={{
                          display: "flex",
                          alignItems: "center",
                          gap: 1,
                          mb: 1,
                        }}
                      >
                        <TextField
                          fullWidth
                          value={point}
                          onChange={(e) =>
                            handleUpdateBulletPoint(index, e.target.value)
                          }
                          placeholder={`Bullet point ${index + 1}`}
                          size="small"
                          sx={{
                            "& .MuiOutlinedInput-root": {
                              fontSize: "0.875rem",
                            },
                          }}
                        />
                        <IconButton
                          size="small"
                          onClick={() => handleDeleteBulletPoint(index)}
                          sx={{ color: "#ef4444" }}
                        >
                          <DeleteOutlineIcon fontSize="small" />
                        </IconButton>
                      </Box>
                    ))}
                    <Button
                      variant="outlined"
                      size="small"
                      startIcon={<AddIcon />}
                      onClick={handleAddBulletPoint}
                      fullWidth
                      sx={{
                        mt: 1,
                        borderColor: "#6366f1",
                        color: "#6366f1",
                        textTransform: "none",
                        "&:hover": {
                          borderColor: "#4f46e5",
                          bgcolor: "#eef2ff",
                        },
                      }}
                    >
                      Add an item
                    </Button>
                  </Box>
                </Collapse>
              </Box>

              <Divider sx={{ my: 2 }} />
            </>
          )}

          {/* Done Text Section */}
          <Box sx={{ mb: 3 }}>
            <Typography
              variant="subtitle2"
              sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
            >
              Done Text
            </Typography>
            <TextField
              fullWidth
              value={doneText}
              onChange={(e) => setDoneText(e.target.value)}
              placeholder="Continue"
              size="small"
              sx={{
                "& .MuiOutlinedInput-root": {
                  fontSize: "0.875rem",
                },
              }}
            />
          </Box>

          <Divider sx={{ my: 2 }} />

          {/* Narration Section */}
          <Box>
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                mb: 1,
                cursor: "pointer",
              }}
              onClick={() => setIsNarrationExpanded(!isNarrationExpanded)}
            >
              <Typography
                variant="subtitle2"
                sx={{ fontWeight: 600, color: "#374151" }}
              >
                Narration
              </Typography>
              {isNarrationExpanded ? (
                <ExpandLessIcon sx={{ fontSize: 20, color: "#6b7280" }} />
              ) : (
                <ExpandMoreIcon sx={{ fontSize: 20, color: "#6b7280" }} />
              )}
            </Box>
            <Collapse in={isNarrationExpanded}>
              <TextField
                fullWidth
                multiline
                rows={4}
                placeholder="Enter narration text..."
                size="small"
                sx={{
                  "& .MuiOutlinedInput-root": {
                    fontSize: "0.875rem",
                  },
                }}
              />
            </Collapse>
          </Box>
        </Paper>
      </Box>
    );
  }

  return (
    <Box>
      <Box
        sx={{
          mb: 3,
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <Box>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
            Content {">"} Courses
          </Typography>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
            {courseData?.data?.Title || courseData?.data?.title || "Course"}
          </Typography>
        </Box>
        <Box sx={{ display: "flex", gap: 2 }}>
          <Button
            variant="contained"
            sx={{ bgcolor: "#6366f1", textTransform: "none" }}
            startIcon={<PersonIcon />}
            onClick={handleAssign}
          >
            Assign
          </Button>
          <Button
            variant="contained"
            sx={{ bgcolor: "#10b981", textTransform: "none" }}
            startIcon={<CheckCircleIcon />}
            onClick={handlePublish}
            disabled={isPublishing || courseData?.data?.Status === "Published"}
          >
            {isPublishing ? "Publishing..." : "Publish"}
          </Button>
        </Box>
      </Box>

      <Box>
        <Typography variant="h5" component="h2" sx={{ fontWeight: 600, mb: 3 }}>
          Project
        </Typography>

        <Box sx={{ display: "flex", gap: 3, height: "calc(100vh - 200px)" }}>
          <LessonsPanel
            sx={{
              width: 300,
              minWidth: 300,
              maxWidth: 300,
              height: "100%",
              flexShrink: 0,
            }}
          >
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                mb: 2,
                flexShrink: 0,
              }}
            >
              <Typography variant="h6">Lessons</Typography>
              <Box sx={{ display: "flex", gap: 0.5 }}>
                <IconButton size="small" sx={{ color: "text.secondary" }}>
                  <MoreVertIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  sx={{ bgcolor: "#6366f1", color: "white" }}
                  onClick={handleAddLessonClick}
                >
                  <AddIcon fontSize="small" />
                </IconButton>
              </Box>
            </Box>

            <DnDContextWrapper
              sensors={sensors}
              collisionDetection={closestCenter}
              onDragEnd={handleDragEnd}
            >
              <DnDSortableList
                items={displayedLessons.map((lesson: any) =>
                  String(lesson.id || lesson.Id || "")
                )}
                strategy={verticalListSortingStrategy}
              >
                <Box
                  sx={{
                    flex: 1,
                    overflowY: "auto",
                    overflowX: "hidden",
                    minHeight: 0,
                  }}
                  onScroll={handleScroll}
                >
                  <List sx={{ p: 0 }}>
                    {displayedLessons.map((lesson: any) => {
                      const lessonId = String(lesson.id || lesson.Id || "");
                      const editingLessonId = editingLesson
                        ? String(editingLesson.id || editingLesson.Id || "")
                        : null;
                      const isEditing: boolean =
                        editingLessonId !== null &&
                        editingLessonId === lessonId &&
                        editingLessonId !== "";

                      const selectedLessonId = selectedLessonForSlides
                        ? String(
                            selectedLessonForSlides.id ||
                              selectedLessonForSlides.Id ||
                              ""
                          )
                        : null;
                      const isSelected = selectedLessonId === lessonId;

                      const lessonItemContent = (
                        <LessonItemContent
                          lesson={lesson}
                          lessonId={lessonId}
                          isEditing={isEditing}
                          isSelected={isSelected}
                          editTitle={editTitle}
                          setEditTitle={setEditTitle}
                          handleSaveEdit={handleSaveEdit}
                          handleCancelEdit={handleCancelEdit}
                          handleEditClick={handleEditClick}
                          handleDeleteClick={handleDeleteClick}
                          handleLessonClick={handleLessonClick}
                          formatDuration={formatDuration}
                        />
                      );

                      const wrappedLesson = (
                        <DnDDraggableItem
                          key={lessonId}
                          draggableId={lessonId}
                          disabled={isEditing}
                          getItemStyle={(isDragging) => ({
                            opacity: isDragging ? 0.85 : 1,
                            cursor: isDragging ? "grabbing" : "grab",
                          })}
                        >
                          {lessonItemContent}
                        </DnDDraggableItem>
                      );

                      return (
                        <React.Fragment key={lessonId}>
                          {wrappedLesson}
                          {isSelected && (
                            <Box sx={{ pl: 3, pr: 1, pb: 1 }}>
                              {/* Slides list for selected lesson */}
                              {slidesLoading ? (
                                <Box sx={{ p: 2, textAlign: "center" }}>
                                  <Typography
                                    variant="body2"
                                    color="text.secondary"
                                  >
                                    Loading slides...
                                  </Typography>
                                </Box>
                              ) : (
                                <>
                                  {(() => {
                                    const slidesArray = Array.isArray(
                                      slidesData?.data
                                    )
                                      ? slidesData.data
                                      : Array.isArray(slidesData?.data?.items)
                                      ? slidesData.data.items
                                      : Array.isArray(slidesData?.items)
                                      ? slidesData.items
                                      : Array.isArray(slidesData)
                                      ? slidesData
                                      : [];

                                    if (slidesArray.length === 0) {
                                      return (
                                        <Box sx={{ py: 2 }}>
                                          <Typography
                                            variant="body2"
                                            color="text.secondary"
                                            sx={{ mb: 1, fontSize: "0.75rem" }}
                                          >
                                            No slides in current lesson
                                          </Typography>
                                          <Button
                                            variant="contained"
                                            size="small"
                                            startIcon={<AddIcon />}
                                            onClick={handleOpenSlideLibrary}
                                            disabled={isCreatingSlide}
                                            sx={{
                                              bgcolor: "#6366f1",
                                              "&:hover": { bgcolor: "#4f46e5" },
                                              fontSize: "0.75rem",
                                              textTransform: "none",
                                            }}
                                          >
                                            New Slide
                                          </Button>
                                        </Box>
                                      );
                                    }

                                    return (
                                      <Box>
                                        <List sx={{ p: 0 }}>
                                          {slidesArray.map((slide: any) => {
                                            const slideId = String(
                                              slide.id || slide.Id || ""
                                            );
                                            return (
                                              <ListItem
                                                key={slideId}
                                                sx={{
                                                  py: 0.5,
                                                  px: 1,
                                                  cursor: "pointer",
                                                  "&:hover": {
                                                    bgcolor: "#f9fafb",
                                                  },
                                                }}
                                                onClick={() => {
                                                  console.log(
                                                    "Slide clicked in main view:",
                                                    slide
                                                  );

                                                  // Check slide type - backend uses 'type' or 'Type'
                                                  const slideType =
                                                    slide.type ||
                                                    slide.Type ||
                                                    slide.slideType ||
                                                    slide.SlideType;

                                                  console.log(
                                                    "Slide type:",
                                                    slideType
                                                  );

                                                  // Open editor for all bulleted-list slides
                                                  if (
                                                    slideType ===
                                                      "bulleted-list" ||
                                                    !slideType
                                                  ) {
                                                    populateSlideEditor(slide);
                                                  }
                                                }}
                                              >
                                                <DescriptionIcon
                                                  sx={{
                                                    color: "#9ca3af",
                                                    fontSize: 16,
                                                    mr: 1,
                                                  }}
                                                />
                                                <ListItemText
                                                  primary={
                                                    slide.title ||
                                                    slide.Title ||
                                                    "Untitled Slide"
                                                  }
                                                  primaryTypographyProps={{
                                                    fontSize: "0.75rem",
                                                  }}
                                                />
                                              </ListItem>
                                            );
                                          })}
                                        </List>
                                        <Button
                                          variant="contained"
                                          size="small"
                                          startIcon={<AddIcon />}
                                          onClick={handleOpenSlideLibrary}
                                          disabled={isCreatingSlide}
                                          sx={{
                                            mt: 1,
                                            bgcolor: "#6366f1",
                                            "&:hover": { bgcolor: "#4f46e5" },
                                            fontSize: "0.75rem",
                                            textTransform: "none",
                                          }}
                                        >
                                          New Slide
                                        </Button>
                                      </Box>
                                    );
                                  })()}
                                </>
                              )}
                            </Box>
                          )}
                        </React.Fragment>
                      );
                    })}
                    {displayedLessons.length === 0 && !lessonsLoading && (
                      <Typography
                        variant="body2"
                        color="text.secondary"
                        sx={{ p: 2, textAlign: "center" }}
                      >
                        No lessons yet. Add your first lesson!
                      </Typography>
                    )}
                    {isFetching && displayedLessons.length > 0 && (
                      <Box sx={{ p: 2, textAlign: "center" }}>
                        <Typography variant="body2" color="text.secondary">
                          Loading more lessons...
                        </Typography>
                      </Box>
                    )}
                    {!hasMore && displayedLessons.length > 0 && (
                      <Box sx={{ p: 2, textAlign: "center" }}>
                        <Typography variant="body2" color="text.secondary">
                          No more lessons to load
                        </Typography>
                      </Box>
                    )}
                  </List>
                </Box>
              </DnDSortableList>
            </DnDContextWrapper>

            <Box
              sx={{ mt: 2, display: "flex", flexDirection: "column", gap: 1 }}
            >
              <ListItemButton
                sx={{
                  borderRadius: 1,
                  py: 0.5,
                  "&:hover": { backgroundColor: "rgba(99, 102, 241, 0.04)" },
                }}
              >
                <ListItemIcon sx={{ minWidth: 36 }}>
                  <SettingsIcon sx={{ fontSize: 18, color: "#6366f1" }} />
                </ListItemIcon>
                <Typography variant="body2" sx={{ color: "#6366f1" }}>
                  Settings
                </Typography>
              </ListItemButton>
              <ListItemButton
                sx={{
                  borderRadius: 1,
                  py: 0.5,
                  "&:hover": { backgroundColor: "rgba(99, 102, 241, 0.04)" },
                }}
              >
                <ListItemIcon sx={{ minWidth: 36 }}>
                  <BookIcon sx={{ fontSize: 18, color: "#6366f1" }} />
                </ListItemIcon>
                <Typography variant="body2" sx={{ color: "#6366f1" }}>
                  Overview
                </Typography>
              </ListItemButton>
            </Box>
          </LessonsPanel>

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

      <LessonTypeMenu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
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
            onClick={() => handleCreateLesson(type.value)}
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
        open={isDeleteModalOpen}
        onClose={() => {
          setIsDeleteModalOpen(false);
          setSelectedLesson(null);
        }}
        onConfirm={handleConfirmDelete}
        isLoading={isDeleting}
        title="Are you sure you want to delete this lesson?"
        message="This will delete the lesson and all its content. This action is irreversible."
      />

      {/* Slide Library Modal */}
      <Dialog
        open={isSlideLibraryOpen}
        onClose={handleCloseSlideLibrary}
        maxWidth="md"
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: 2,
            maxHeight: "90vh",
          },
        }}
      >
        <DialogTitle
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            pb: 1,
            borderBottom: "1px solid #e5e7eb",
          }}
        >
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Slide Library
          </Typography>
          <IconButton
            onClick={handleCloseSlideLibrary}
            sx={{ color: "text.secondary" }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>

        <DialogContent sx={{ p: 0 }}>
          <Box sx={{ display: "flex", height: "calc(90vh - 120px)" }}>
            {/* Left Sidebar - Tabs */}
            <Box
              sx={{
                width: 200,
                borderRight: "1px solid #e5e7eb",
                pt: 2,
                backgroundColor: "#f9fafb",
              }}
            >
              <Tabs
                orientation="vertical"
                value={selectedSlideTab}
                onChange={(_, newValue) => setSelectedSlideTab(newValue)}
                sx={{
                  "& .MuiTab-root": {
                    textTransform: "none",
                    alignItems: "flex-start",
                    minHeight: 48,
                    fontSize: "0.875rem",
                    color: "#6b7280",
                    "&.Mui-selected": {
                      color: "#6366f1",
                      backgroundColor: "#eef2ff",
                    },
                  },
                }}
              >
                <Tab
                  icon={<DescriptionIcon />}
                  iconPosition="start"
                  label="Text"
                  sx={{ pl: 2 }}
                />
                <Tab
                  icon={<ImageIcon />}
                  iconPosition="start"
                  label="Image"
                  sx={{ pl: 2 }}
                />
                <Tab
                  icon={<VideoIcon />}
                  iconPosition="start"
                  label="Video"
                  sx={{ pl: 2 }}
                />
              </Tabs>
            </Box>

            {/* Right Content Area */}
            <Box sx={{ flex: 1, p: 3, overflowY: "auto" }}>
              {selectedSlideTab === 0 && (
                <Grid container spacing={2}>
                  {/* Bulleted list */}
                  <Grid item xs={4}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={() => handleCreateSlide("bulleted-list")}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                          }}
                        >
                          <DescriptionIcon
                            sx={{ fontSize: 48, color: "#6366f1" }}
                          />
                        </Box>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: 600, mb: 0.5 }}
                        >
                          Bulleted list
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          Show a list of bullet points
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>

                  {/* Comparison */}
                  <Grid item xs={4}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={() => handleCreateSlide("comparison")}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                          }}
                        >
                          <DescriptionIcon
                            sx={{ fontSize: 48, color: "#6366f1" }}
                          />
                        </Box>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: 600, mb: 0.5 }}
                        >
                          Comparison
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          Compare two text blocks
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>

                  {/* Expandable list */}
                  <Grid item xs={4}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={() => handleCreateSlide("expandable-list")}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                          }}
                        >
                          <DescriptionIcon
                            sx={{ fontSize: 48, color: "#6366f1" }}
                          />
                        </Box>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: 600, mb: 0.5 }}
                        >
                          Expandable list
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          Show a list of concepts
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                </Grid>
              )}

              {selectedSlideTab === 1 && (
                <Grid container spacing={2}>
                  {/* Single Image */}
                  <Grid item xs={4}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={() => handleCreateSlide("single-image")}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                          }}
                        >
                          <ImageIcon sx={{ fontSize: 48, color: "#6366f1" }} />
                        </Box>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: 600, mb: 0.5 }}
                        >
                          Single Image
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          Display a single image with text
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>

                  {/* Image Collection */}
                  <Grid item xs={4}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={() => handleCreateSlide("image-collection")}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                            gap: 1,
                          }}
                        >
                          <ImageIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                          <ImageIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                          <ImageIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                        </Box>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: 600, mb: 0.5 }}
                        >
                          Image Collection
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          Display a collection of three images
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                </Grid>
              )}

              {selectedSlideTab === 2 && (
                <Grid container spacing={2}>
                  {/* Single Video */}
                  <Grid item xs={4}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={() => handleCreateSlide("single-video")}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                          }}
                        >
                          <VideoIcon sx={{ fontSize: 48, color: "#6366f1" }} />
                        </Box>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: 600, mb: 0.5 }}
                        >
                          Single Video
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          Display a single video with text
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>

                  {/* Video Collection */}
                  <Grid item xs={4}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={() => handleCreateSlide("video-collection")}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                            gap: 1,
                          }}
                        >
                          <VideoIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                          <VideoIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                          <VideoIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                        </Box>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: 600, mb: 0.5 }}
                        >
                          Video Collection
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          Display a collection of three videos
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                </Grid>
              )}
            </Box>
          </Box>
        </DialogContent>
      </Dialog>

      {/* Media Preview Modal */}
      <MediaPreviewModal
        open={isPreviewModalOpen}
        onClose={() => setIsPreviewModalOpen(false)}
        url={previewUrl}
        type={previewType}
        alt="Media preview"
      />

      {/* Assign Users Modal */}
      <AssignUsersModal
        open={isAssignModalOpen}
        onClose={() => setIsAssignModalOpen(false)}
        courseId={courseId}
      />
    </Box>
  );
}

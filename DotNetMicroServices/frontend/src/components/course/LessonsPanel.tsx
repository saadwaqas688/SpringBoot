"use client";

import React, { useEffect, useRef } from "react";
import {
  Box,
  Typography,
  IconButton,
  List,
  ListItem,
  ListItemText,
  ListItemButton,
} from "@mui/material";
import {
  Add as AddIcon,
  Close as CloseIcon,
  MoreVert as MoreVertIcon,
  Description as DescriptionIcon,
  Settings as SettingsIcon,
  Book as BookIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  CheckCircle as CheckCircleIcon,
} from "@mui/icons-material";
import { Paper, TextField } from "@mui/material";
import styled from "styled-components";

const StyledLessonsPanel = styled(Paper)`
  width: 300px;
  min-width: 300px;
  max-width: 300px;
  padding: 1rem;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  
  @media (max-width: 1200px) {
    width: 250px;
    min-width: 250px;
    max-width: 250px;
  }
  
  @media (max-width: 960px) {
    position: fixed;
    left: 0;
    top: 0;
    height: 100vh;
    z-index: 1300;
    transform: translateX(-100%);
    transition: transform 0.3s ease-in-out;
    
    &.open {
      transform: translateX(0);
    }
  }
`;

interface LessonsPanelProps {
  mobileLessonsOpen: boolean;
  onCloseMobile: () => void;
  displayedLessons?: any[];
  selectedLessonForSlides: any;
  onLessonSelect: (lesson: any | null) => void;
  onAddLessonClick: (buttonRef: React.RefObject<HTMLButtonElement>) => void;
  slidesData: any;
  selectedSlide: any;
  onSlideClick: (slide: any) => void;
  onOpenSlideLibrary: (lesson?: any) => void;
  isCreatingSlide: boolean;
  onCloseSlideEditor: () => void;
  editingLesson?: any;
  editTitle?: string;
  setEditTitle?: (title: string) => void;
  handleEditClick?: (e: React.MouseEvent, lesson: any) => void;
  handleSaveEdit?: () => void;
  handleCancelEdit?: () => void;
  handleDeleteClick?: (e: React.MouseEvent, lesson: any) => void;
  isUpdating?: boolean;
  isDeleting?: boolean;
  isUserRole?: boolean;
}

const LessonsPanel: React.FC<LessonsPanelProps> = ({
  mobileLessonsOpen,
  onCloseMobile,
  displayedLessons,
  selectedLessonForSlides,
  onLessonSelect,
  onAddLessonClick,
  slidesData,
  selectedSlide,
  onSlideClick,
  onOpenSlideLibrary,
  isCreatingSlide,
  onCloseSlideEditor,
  editingLesson,
  editTitle,
  setEditTitle,
  handleEditClick,
  handleSaveEdit,
  handleCancelEdit,
  handleDeleteClick,
  isUpdating,
  isDeleting,
  isUserRole = false,
}) => {
  // Ref for the add lesson button
  const addLessonButtonRef = useRef<HTMLButtonElement>(null);

  // Debug logging
  useEffect(() => {
    console.log("LessonsPanel - displayedLessons:", displayedLessons);
    console.log("LessonsPanel - displayedLessons length:", displayedLessons?.length);
    console.log("LessonsPanel - displayedLessons isArray:", Array.isArray(displayedLessons));
    if (displayedLessons && displayedLessons.length > 0) {
      console.log("LessonsPanel - First lesson:", displayedLessons[0]);
      console.log("LessonsPanel - Will render lessons list");
    } else {
      console.log("LessonsPanel - Will render 'No lessons available'");
    }
  }, [displayedLessons]);

  return (
    <StyledLessonsPanel
      className={mobileLessonsOpen ? "open" : ""}
      sx={{
        width: { xs: 280, lg: 300 },
        minWidth: { xs: 280, lg: 300 },
        maxWidth: { xs: 280, lg: 300 },
        height: { xs: "100vh", lg: "100%" },
        flexShrink: 0,
        display: { xs: mobileLessonsOpen ? "flex" : "none", lg: "flex" },
        overflowY: "auto",
        overflowX: "hidden",
        background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important",
        backgroundColor: "transparent !important",
        boxShadow: "none",
        color: "white",
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
        <Typography variant="h6" sx={{ color: "white !important", fontWeight: 600 }}>
          Lessons
        </Typography>
        <Box sx={{ display: "flex", gap: 0.5, alignItems: "center" }}>
          <IconButton
            sx={{ display: { xs: "flex", lg: "none" }, color: "white" }}
            onClick={onCloseMobile}
          >
            <CloseIcon />
          </IconButton>
          <IconButton size="small" sx={{ color: "white" }}>
            <MoreVertIcon fontSize="small" />
          </IconButton>
          {!isUserRole && (
            <IconButton
              ref={addLessonButtonRef}
              size="small"
              sx={{ 
                bgcolor: "rgba(255, 255, 255, 0.2)", 
                color: "white",
                "&:hover": {
                  bgcolor: "rgba(255, 255, 255, 0.3)",
                }
              }}
              onClick={(e) => {
                e.stopPropagation();
                onAddLessonClick(addLessonButtonRef);
              }}
            >
              <AddIcon fontSize="small" />
            </IconButton>
          )}
        </Box>
      </Box>

      <Box
        sx={{
          maxHeight: { xs: "calc(100vh - 200px)", lg: "calc(100vh - 300px)" },
          overflowY: "auto",
          overflowX: "hidden",
          flex: 1,
          "&::-webkit-scrollbar": {
            width: "6px",
          },
          "&::-webkit-scrollbar-thumb": {
            backgroundColor: "rgba(255, 255, 255, 0.3)",
            borderRadius: "3px",
          },
        }}
      >
        <List sx={{ p: 0, width: "100%", color: "white" }}>
          {!displayedLessons || displayedLessons.length === 0 ? (
            <Box sx={{ p: 2, textAlign: "center" }}>
              <Typography variant="body2" sx={{ color: "rgba(255, 255, 255, 0.9) !important" }}>
                No lessons available
              </Typography>
            </Box>
          ) : (
            displayedLessons.map((lesson: any, index: number) => {
              console.log(`Rendering lesson ${index}:`, lesson);
              const lessonId = String(lesson.id || lesson.Id || "");
              const isSelected =
                selectedLessonForSlides &&
                String(
                  selectedLessonForSlides.id ||
                    selectedLessonForSlides.Id ||
                    ""
                ) === lessonId;
              const isEditing = editingLesson && (editingLesson.id || editingLesson.Id) === (lesson.id || lesson.Id);
              const currentLessonTitle = lesson.title || lesson.Title || "Untitled Lesson";
              
              return (
                <React.Fragment key={lessonId}>
                  <ListItem
                    onClick={() => {
                      // Don't trigger selection when clicking on edit/delete icons
                      if (isEditing) return;
                      const newSelection = isSelected ? null : lesson;
                      onLessonSelect(newSelection);
                      if (!newSelection) {
                        onCloseSlideEditor();
                      }
                    }}
                    sx={{
                      cursor: isEditing ? "default" : "pointer",
                      backgroundColor: isSelected
                        ? "rgba(255, 255, 255, 0.2)"
                        : "transparent",
                      "&:hover": { backgroundColor: isEditing ? "transparent" : "rgba(255, 255, 255, 0.1)" },
                      mb: 0.5,
                      color: "white !important",
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                    }}
                  >
                    <DescriptionIcon
                      sx={{ color: "white !important", fontSize: 20, mr: 1, flexShrink: 0 }}
                    />
                    {isEditing ? (
                      <TextField
                        value={editTitle || currentLessonTitle}
                        onChange={(e) => {
                          if (setEditTitle) {
                            setEditTitle(e.target.value);
                          }
                        }}
                        onKeyDown={(e) => {
                          if (e.key === "Enter" && handleSaveEdit) {
                            handleSaveEdit();
                          } else if (e.key === "Escape" && handleCancelEdit) {
                            handleCancelEdit();
                          }
                        }}
                        autoFocus
                        size="small"
                        sx={{
                          flex: 1,
                          "& .MuiOutlinedInput-root": {
                            color: "white",
                            fontSize: "0.875rem",
                            "& fieldset": {
                              borderColor: "rgba(255, 255, 255, 0.5)",
                            },
                            "&:hover fieldset": {
                              borderColor: "rgba(255, 255, 255, 0.7)",
                            },
                            "&.Mui-focused fieldset": {
                              borderColor: "white",
                            },
                          },
                        }}
                        onClick={(e) => e.stopPropagation()}
                      />
                    ) : (
                      <ListItemText
                        primary={currentLessonTitle}
                        primaryTypographyProps={{
                          fontSize: "0.875rem",
                          color: "white !important",
                          fontWeight: isSelected ? 600 : 400,
                        }}
                        sx={{
                          flex: 1,
                          "& .MuiListItemText-primary": {
                            color: "white !important",
                          },
                        }}
                      />
                    )}
                    {!isUserRole && (
                      <Box
                        sx={{
                          display: "flex",
                          gap: 0.5,
                          alignItems: "center",
                          flexShrink: 0,
                        }}
                        onClick={(e) => e.stopPropagation()}
                      >
                        {isEditing ? (
                          <IconButton
                            size="small"
                            onClick={(e) => {
                              e.stopPropagation();
                              if (handleSaveEdit) {
                                handleSaveEdit();
                              }
                            }}
                            disabled={isUpdating}
                            sx={{
                              color: "white",
                              "&:hover": {
                                bgcolor: "rgba(255, 255, 255, 0.2)",
                              },
                              "&:disabled": {
                                color: "rgba(255, 255, 255, 0.5)",
                              },
                            }}
                          >
                            <CheckCircleIcon fontSize="small" />
                          </IconButton>
                        ) : (
                          <>
                            <IconButton
                              size="small"
                              onClick={(e) => {
                                e.stopPropagation();
                                if (handleEditClick) {
                                  handleEditClick(e, lesson);
                                }
                              }}
                              disabled={isUpdating || isDeleting}
                              sx={{
                                color: "white",
                                "&:hover": {
                                  bgcolor: "rgba(255, 255, 255, 0.2)",
                                },
                                "&:disabled": {
                                  color: "rgba(255, 255, 255, 0.5)",
                                },
                              }}
                            >
                              <EditIcon fontSize="small" />
                            </IconButton>
                            <IconButton
                              size="small"
                              onClick={(e) => {
                                e.stopPropagation();
                                if (handleDeleteClick) {
                                  handleDeleteClick(e, lesson);
                                }
                              }}
                              disabled={isUpdating || isDeleting}
                              sx={{
                                color: "white",
                                "&:hover": {
                                  bgcolor: "rgba(255, 255, 255, 0.2)",
                                },
                                "&:disabled": {
                                  color: "rgba(255, 255, 255, 0.5)",
                                },
                              }}
                            >
                              <DeleteIcon fontSize="small" />
                            </IconButton>
                          </>
                        )}
                      </Box>
                    )}
                  </ListItem>
                  {isSelected && (
                    <Box sx={{ pl: 3, pr: 1, pb: 1 }}>
                      {(() => {
                        const lessonId = lesson.id || lesson.Id;
                        let slidesArray = Array.isArray(slidesData?.data)
                          ? slidesData.data
                          : Array.isArray(slidesData?.data?.items)
                          ? slidesData.data.items
                          : Array.isArray(slidesData?.items)
                          ? slidesData.items
                          : Array.isArray(slidesData)
                          ? slidesData
                          : [];

                        // Filter slides to only show slides that belong to this lesson
                        slidesArray = slidesArray.filter((slide: any) => {
                          const slideLessonId = slide.lessonId || slide.LessonId;
                          return String(slideLessonId) === String(lessonId);
                        });

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
                                        const slideType =
                                          slideItem.type ||
                                          slideItem.Type ||
                                          slideItem.slideType ||
                                          slideItem.SlideType;

                                        if (
                                          slideType === "bulleted-list" ||
                                          slideType === "expandable-list" ||
                                          slideType === "single-image" ||
                                          slideType === "image-collection" ||
                                          slideType === "single-video" ||
                                          slideType === "video-collection" ||
                                          !slideType
                                        ) {
                                          onSlideClick(slideItem);
                                        }
                                      }}
                                      sx={{
                                        py: 0.5,
                                        px: 1,
                                        cursor: "pointer",
                                        backgroundColor: isSlideSelected
                                          ? "rgba(255, 255, 255, 0.3)"
                                          : "transparent",
                                        borderLeft: isSlideSelected
                                          ? "3px solid white"
                                          : "3px solid transparent",
                                        "&:hover": {
                                          backgroundColor: "rgba(255, 255, 255, 0.2)",
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
                                          fontWeight: isSlideSelected ? 500 : 400,
                                          color: "white !important",
                                        }}
                                        sx={{
                                          "& .MuiListItemText-primary": {
                                            color: "white !important",
                                          },
                                        }}
                                      />
                                    </ListItem>
                                  );
                                }
                              )}
                            </List>
                            {!isUserRole && (
                              <Box sx={{ display: "flex", justifyContent: "center", mt: 1 }}>
                                <IconButton
                                  size="small"
                                  onClick={(e) => {
                                    console.log("=== ADD QUIZ/SLIDE BUTTON CLICKED ===");
                                    console.log("Event:", e);
                                    console.log("Current lesson from scope:", lesson);
                                    console.log("selectedLessonForSlides prop:", selectedLessonForSlides);
                                    console.log("isCreatingSlide:", isCreatingSlide);
                                    console.log("Button disabled:", isCreatingSlide || !selectedLessonForSlides);
                                    
                                    e.stopPropagation();
                                    e.preventDefault();
                                    
                                    // Use the lesson from the current scope to ensure we have the correct lesson
                                    // even if the state hasn't updated yet
                                    const currentLesson = lesson;
                                    console.log("Using currentLesson:", currentLesson);
                                    
                                    if (!currentLesson) {
                                      console.error("ERROR: No lesson available in scope");
                                      return;
                                    }
                                    
                                    const lessonType = currentLesson.lessonType || currentLesson.LessonType || "standard";
                                    console.log("Lesson type detected:", lessonType);
                                    console.log("Calling onOpenSlideLibrary with lesson:", currentLesson);
                                    
                                    // Pass the lesson directly to onOpenSlideLibrary
                                    // This ensures the modal opens immediately with the correct lesson
                                    try {
                                      onOpenSlideLibrary(currentLesson);
                                      console.log("onOpenSlideLibrary called successfully");
                                    } catch (error) {
                                      console.error("ERROR calling onOpenSlideLibrary:", error);
                                    }
                                  }}
                                  disabled={isCreatingSlide || !selectedLessonForSlides}
                                  sx={{
                                    bgcolor: "rgba(255, 255, 255, 0.2)",
                                    color: "white",
                                    "&:hover": {
                                      bgcolor: "rgba(255, 255, 255, 0.3)",
                                    },
                                    "&:disabled": {
                                      bgcolor: "rgba(255, 255, 255, 0.1)",
                                      color: "rgba(255, 255, 255, 0.5)",
                                    },
                                  }}
                                >
                                  <AddIcon />
                                </IconButton>
                              </Box>
                            )}
                          </>
                        );
                      })()}
                    </Box>
                  )}
                </React.Fragment>
              );
            })
          )}
        </List>
      </Box>

      <Box sx={{ mt: 2, display: "flex", flexDirection: "column", gap: 1 }}>
        <ListItemButton
          sx={{
            borderRadius: 1,
            py: 0.5,
            color: "white",
            "&:hover": { backgroundColor: "rgba(255, 255, 255, 0.1)" },
          }}
        >
          <SettingsIcon sx={{ mr: 1, fontSize: 20, color: "white" }} />
          <Typography variant="body2" sx={{ color: "white" }}>
            Settings
          </Typography>
        </ListItemButton>
        <ListItemButton
          sx={{
            borderRadius: 1,
            py: 0.5,
            color: "white",
            "&:hover": { backgroundColor: "rgba(255, 255, 255, 0.1)" },
          }}
        >
          <BookIcon sx={{ mr: 1, fontSize: 20, color: "white" }} />
          <Typography variant="body2" sx={{ color: "white" }}>
            Overview
          </Typography>
        </ListItemButton>
      </Box>
    </StyledLessonsPanel>
  );
};

export default LessonsPanel;


"use client";

import React from "react";
import {
  Box,
  Typography,
  Button,
  Paper,
  IconButton,
  Grid,
  Collapse,
} from "@mui/material";
import {
  MoreVert as MoreVertIcon,
  ArrowBack as ArrowBackIcon,
  ArrowForward as ArrowForwardIcon,
  PlayCircle as PlayCircleIcon,
  ExpandLess as ExpandLessIcon,
  ChevronRight as ChevronRightIcon,
  Image as ImageIcon,
  VideoFile as VideoIcon,
} from "@mui/icons-material";

interface ContentPreviewPanelProps {
  selectedLessonForSlides: any;
  isQuizLesson: boolean;
  sortedQuizQuestions: any[];
  currentQuestionIndex: number;
  selectedAnswers: Record<string, string>;
  onAnswerSelect: (questionId: string, index: number) => void;
  onQuestionNavigation: (direction: "prev" | "next") => void;
  onOpenQuizUpload: () => void;
  quizLoading: boolean;
  questionsLoading: boolean;
  quizId: string | null;
  quizError: any;
  questionsError: any;
  selectedSlide: any;
  slideTitle: string;
  previewIsSingleImage: boolean;
  previewIsImageCollection: boolean;
  previewIsSingleVideo: boolean;
  previewIsVideoCollection: boolean;
  previewIsExpandableList: boolean;
  previewIsBulletedList: boolean;
  imageUrl: string;
  imageText: string;
  imageCollection: Array<{ url: string; alt?: string; text?: string }>;
  videoUrl: string;
  videoText: string;
  videoThumbnail: string;
  videoCollection: Array<{ url: string; title?: string; thumbnail?: string }>;
  expandableListItems: Array<{ title: string; content: string }>;
  expandedItemIndex: number | null;
  focusMode: boolean;
  onToggleExpandableItem: (index: number) => void;
  bulletPoints: string[];
  promptText: string;
  onOpenPreview: (url: string, type: "image" | "video") => void;
  slidesData: any;
  onPreviousSlide: () => void;
  onNextSlide: () => void;
  onEditSlide: () => void;
  onDeleteSlide: () => void;
}

const ContentPreviewPanel: React.FC<ContentPreviewPanelProps> = ({
  selectedLessonForSlides,
  isQuizLesson,
  sortedQuizQuestions,
  currentQuestionIndex,
  selectedAnswers,
  onAnswerSelect,
  onQuestionNavigation,
  onOpenQuizUpload,
  quizLoading,
  questionsLoading,
  quizId,
  quizError,
  questionsError,
  selectedSlide,
  slideTitle,
  previewIsSingleImage,
  previewIsImageCollection,
  previewIsSingleVideo,
  previewIsVideoCollection,
  previewIsExpandableList,
  previewIsBulletedList,
  imageUrl,
  imageText,
  imageCollection,
  videoUrl,
  videoText,
  videoThumbnail,
  videoCollection,
  expandableListItems,
  expandedItemIndex,
  focusMode,
  onToggleExpandableItem,
  bulletPoints,
  promptText,
  onOpenPreview,
  slidesData,
  onPreviousSlide,
  onNextSlide,
  onEditSlide,
  onDeleteSlide,
}) => {
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

  const getCurrentSlideIndex = () => {
    const slidesArray = getSlidesArray();
    return slidesArray.findIndex(
      (s: any) =>
        String(s.id || s.Id) === String(selectedSlide?.id || selectedSlide?.Id)
    );
  };

  return (
    <Paper
      sx={{
        flex: 1,
        p: { xs: 1, sm: 2, md: 3 },
        display: "flex",
        flexDirection: "column",
        backgroundColor: "#f9fafb",
        minWidth: 0,
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
          {selectedLessonForSlides?.title ||
            selectedLessonForSlides?.Title ||
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
            {isQuizLesson && sortedQuizQuestions.length > 0
              ? `${currentQuestionIndex + 1}/${sortedQuizQuestions.length}`
              : (() => {
                  const slidesArray = getSlidesArray();
                  const currentIndex = getCurrentSlideIndex();
                  return `${currentIndex >= 0 ? currentIndex + 1 : 1}/${
                    slidesArray.length || 1
                  }`;
                })()}
          </Typography>
          {isQuizLesson && (
            <Button
              size="small"
              variant="outlined"
              onClick={onOpenQuizUpload}
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

      {/* Slide Preview Content or Quiz Questions */}
      {isQuizLesson ? (
        sortedQuizQuestions.length > 0 ? (
          // Quiz Questions Display
          <Box
            sx={{
              flex: 1,
              borderRadius: 2,
              p: { xs: 2, sm: 3, md: 4 },
              display: "flex",
              flexDirection: "column",
              justifyContent: "center",
              boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
              overflow: "auto",
              minHeight: { xs: 400, md: "auto" },
              background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
              position: "relative",
            }}
          >
            {sortedQuizQuestions[currentQuestionIndex] && (() => {
              const question = sortedQuizQuestions[currentQuestionIndex];
              const questionId = question.id || question.Id || "";
              const options = question.options || question.Options || [];
              const selectedAnswer = selectedAnswers[questionId];
              const optionLabels = ["A", "B", "C", "D"];

              return (
                <Box
                  sx={{ maxWidth: 800, mx: "auto", width: "100%", color: "white" }}
                >
                  <Typography
                    variant="h5"
                    sx={{
                      fontWeight: 600,
                      mb: 4,
                      color: "white",
                      fontSize: { xs: "1.25rem", sm: "1.5rem", md: "1.75rem" },
                    }}
                  >
                    {question.question || question.Question}
                  </Typography>

                  <Box
                    sx={{
                      display: "flex",
                      flexDirection: "column",
                      gap: 2,
                      mb: 4,
                    }}
                  >
                    {options.map((option: any, index: number) => {
                      const optionValue = option.value || option.Value || "";
                      const isCorrect =
                        option.isCorrect || option.IsCorrect || false;
                      const isSelected =
                        selectedAnswer === index.toString();

                      return (
                        <Box
                          key={index}
                          onClick={() => onAnswerSelect(questionId, index)}
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            gap: 2,
                            p: 2,
                            borderRadius: 2,
                            backgroundColor: isCorrect
                              ? "rgba(34, 197, 94, 0.3)"
                              : isSelected
                              ? "rgba(255, 255, 255, 0.2)"
                              : "rgba(255, 255, 255, 0.1)",
                            cursor: "pointer",
                            transition: "all 0.2s",
                            border: isCorrect
                              ? "2px solid rgba(34, 197, 94, 0.6)"
                              : "none",
                            "&:hover": {
                              backgroundColor: isCorrect
                                ? "rgba(34, 197, 94, 0.4)"
                                : "rgba(255, 255, 255, 0.2)",
                            },
                          }}
                        >
                          <Box
                            sx={{
                              width: 32,
                              height: 32,
                              borderRadius: "50%",
                              backgroundColor: isCorrect
                                ? "#22c55e"
                                : isSelected
                                ? "#a78bfa"
                                : "rgba(255, 255, 255, 0.3)",
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              border: isCorrect
                                ? "2px solid #16a34a"
                                : isSelected
                                ? "2px solid #8b5cf6"
                                : "2px solid rgba(255, 255, 255, 0.5)",
                            }}
                          >
                            {(isCorrect || isSelected) && (
                              <Box
                                component="span"
                                sx={{
                                  color: "white",
                                  fontSize: 20,
                                  fontWeight: "bold",
                                }}
                              >
                                ✓
                              </Box>
                            )}
                          </Box>
                          <Typography
                            sx={{
                              fontWeight: 500,
                              fontSize: { xs: "0.875rem", sm: "1rem" },
                              color: "white",
                            }}
                          >
                            {optionLabels[index]}. {optionValue}
                            {isCorrect && (
                              <Typography
                                component="span"
                                sx={{
                                  ml: 1,
                                  fontSize: "0.75rem",
                                  color: "#86efac",
                                  fontWeight: 600,
                                }}
                              >
                                (Correct)
                              </Typography>
                            )}
                          </Typography>
                        </Box>
                      );
                    })}
                  </Box>

                  {/* Navigation */}
                  <Box
                    sx={{
                      display: "flex",
                      justifyContent: "center",
                      alignItems: "center",
                      gap: 2,
                      mt: 4,
                    }}
                  >
                    <IconButton
                      onClick={() => onQuestionNavigation("prev")}
                      disabled={currentQuestionIndex === 0}
                      sx={{
                        backgroundColor: "rgba(255, 255, 255, 0.2)",
                        color: "white",
                        "&:hover": {
                          backgroundColor: "rgba(255, 255, 255, 0.3)",
                        },
                        "&.Mui-disabled": {
                          backgroundColor: "rgba(255, 255, 255, 0.1)",
                          color: "rgba(255, 255, 255, 0.5)",
                        },
                      }}
                    >
                      <ArrowBackIcon />
                    </IconButton>
                    <Typography
                      sx={{
                        color: "white",
                        fontWeight: 500,
                        minWidth: 60,
                        textAlign: "center",
                      }}
                    >
                      {currentQuestionIndex + 1} / {sortedQuizQuestions.length}
                    </Typography>
                    <IconButton
                      onClick={() => onQuestionNavigation("next")}
                      disabled={
                        currentQuestionIndex === sortedQuizQuestions.length - 1
                      }
                      sx={{
                        backgroundColor: "rgba(255, 255, 255, 0.2)",
                        color: "white",
                        "&:hover": {
                          backgroundColor: "rgba(255, 255, 255, 0.3)",
                        },
                        "&.Mui-disabled": {
                          backgroundColor: "rgba(255, 255, 255, 0.1)",
                          color: "rgba(255, 255, 255, 0.5)",
                        },
                      }}
                    >
                      <ArrowForwardIcon />
                    </IconButton>
                  </Box>
                </Box>
              );
            })()}
          </Box>
        ) : quizLoading || questionsLoading ? (
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
            <Typography variant="h6" sx={{ mb: 2, textAlign: "center" }}>
              Loading quiz questions...
            </Typography>
          </Box>
        ) : !quizId ? (
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
            <Typography variant="h6" sx={{ mb: 2, textAlign: "center" }}>
              No quiz uploaded yet
            </Typography>
            <Button
              variant="contained"
              onClick={onOpenQuizUpload}
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
        ) : (
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
            <Typography variant="h6" sx={{ mb: 2, textAlign: "center" }}>
              No questions found in this quiz
            </Typography>
            {quizError && (
              <Typography variant="body2" color="error" sx={{ mb: 2 }}>
                Error:{" "}
                {"message" in quizError ? quizError.message : "Failed to load quiz"}
              </Typography>
            )}
            {questionsError && (
              <Typography variant="body2" color="error" sx={{ mb: 2 }}>
                Error:{" "}
                {"message" in questionsError
                  ? questionsError.message
                  : "Failed to load questions"}
              </Typography>
            )}
            <Button
              variant="contained"
              onClick={onOpenQuizUpload}
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
        )
      ) : selectedSlide ? (
        // Regular Slide Preview Content
        <Box
          sx={{
            flex: 1,
            backgroundColor: "white",
            borderRadius: 2,
            p: { xs: 2, sm: 3, md: 4 },
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
            overflow: "auto",
            minHeight: { xs: 400, md: "auto" },
          }}
        >
          <Typography
            variant="h4"
            sx={{
              fontWeight: 600,
              mb: { xs: 2, md: 4 },
              textAlign: "center",
              color: "#1e293b",
              fontSize: { xs: "1.5rem", sm: "2rem", md: "2.125rem" },
            }}
          >
            {slideTitle}
          </Typography>

          <Box sx={{ maxWidth: { xs: "100%", md: 600 }, mx: "auto", width: "100%" }}>
            {previewIsSingleImage ? (
              // Single Image Preview
              <Box>
                {imageUrl ? (
                  <Box
                    component="img"
                    src={imageUrl}
                    alt={slideTitle}
                    onClick={() => onOpenPreview(imageUrl, "image")}
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
            ) : previewIsImageCollection ? (
              // Image Collection Preview
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
                      <Grid item xs={12} sm={6} md={4} key={index}>
                        {img.url ? (
                          <Box>
                            <Box
                              component="img"
                              src={img.url}
                              alt={img.alt || `Image ${index + 1}`}
                              onClick={() => onOpenPreview(img.url, "image")}
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
              </Box>
            ) : previewIsSingleVideo ? (
              // Single Video Preview
              <Box>
                {videoThumbnail ? (
                  <Box
                    component="img"
                    src={videoThumbnail}
                    alt="Video thumbnail"
                    onClick={() => onOpenPreview(videoUrl, "video")}
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
                      preload="metadata"
                      playsInline
                      onClick={(e) => {
                        e.stopPropagation();
                        onOpenPreview(videoUrl, "video");
                      }}
                      onError={(e) => {
                        console.error("Video failed to load:", videoUrl, e);
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
            ) : previewIsVideoCollection ? (
              // Video Collection Preview
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
                      <Grid item xs={12} sm={6} md={4} key={index}>
                        <Box>
                          {video.thumbnail ? (
                            <Box
                              sx={{
                                position: "relative",
                                width: "100%",
                                height: 150,
                                cursor: "pointer",
                                "&:hover .play-overlay": {
                                  opacity: 1,
                                },
                              }}
                              onClick={(e) => {
                                e.preventDefault();
                                e.stopPropagation();
                                if (video.url) {
                                  onOpenPreview(video.url, "video");
                                }
                              }}
                            >
                              <Box
                                component="img"
                                src={video.thumbnail}
                                alt={video.title || `Video ${index + 1}`}
                                sx={{
                                  width: "100%",
                                  height: "100%",
                                  objectFit: "cover",
                                  borderRadius: 1,
                                  border: "1px solid #e5e7eb",
                                }}
                              />
                              <Box
                                className="play-overlay"
                                sx={{
                                  position: "absolute",
                                  top: 0,
                                  left: 0,
                                  right: 0,
                                  bottom: 0,
                                  display: "flex",
                                  alignItems: "center",
                                  justifyContent: "center",
                                  backgroundColor: "rgba(0, 0, 0, 0.3)",
                                  borderRadius: 1,
                                  opacity: 0.7,
                                  transition: "opacity 0.2s",
                                  pointerEvents: "none",
                                }}
                              >
                                <PlayCircleIcon
                                  sx={{
                                    fontSize: 48,
                                    color: "white",
                                  }}
                                />
                              </Box>
                            </Box>
                          ) : video.url ? (
                            <Box
                              onClick={(e) => {
                                e.preventDefault();
                                e.stopPropagation();
                                if (video.url) {
                                  onOpenPreview(video.url, "video");
                                }
                              }}
                              sx={{
                                width: "100%",
                                height: 150,
                                borderRadius: 1,
                                border: "1px solid #e5e7eb",
                                overflow: "hidden",
                                cursor: "pointer",
                                position: "relative",
                                "&:hover .play-overlay": {
                                  opacity: 1,
                                },
                              }}
                            >
                              <video
                                src={video.url}
                                muted
                                preload="metadata"
                                playsInline
                                onError={(e) => {
                                  console.error("Video failed to load:", video.url, e);
                                }}
                                style={{
                                  width: "100%",
                                  height: "100%",
                                  objectFit: "cover",
                                  borderRadius: "8px",
                                }}
                              />
                              <Box
                                className="play-overlay"
                                sx={{
                                  position: "absolute",
                                  top: 0,
                                  left: 0,
                                  right: 0,
                                  bottom: 0,
                                  display: "flex",
                                  alignItems: "center",
                                  justifyContent: "center",
                                  backgroundColor: "rgba(0, 0, 0, 0.3)",
                                  borderRadius: 1,
                                  opacity: 0.7,
                                  transition: "opacity 0.2s",
                                }}
                              >
                                <PlayCircleIcon
                                  sx={{
                                    fontSize: 48,
                                    color: "white",
                                  }}
                                />
                              </Box>
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
            ) : previewIsExpandableList ? (
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
                        onClick={() => onToggleExpandableItem(index)}
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
      ) : (
        // No slide selected
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
          <Typography variant="h6" sx={{ textAlign: "center", color: "text.secondary" }}>
            Select a slide to preview
          </Typography>
        </Box>
      )}

      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mt: { xs: 2, md: 3 },
          flexWrap: { xs: "wrap", sm: "nowrap" },
          gap: { xs: 1, sm: 0 },
        }}
      >
        <Button
          variant="contained"
          fullWidth={false}
          sx={{
            bgcolor: "#6366f1",
            "&:hover": { bgcolor: "#4f46e5" },
            textTransform: "none",
            minWidth: { xs: "120px", sm: "auto" },
            fontSize: { xs: "0.875rem", sm: "1rem" },
          }}
          onClick={onEditSlide}
        >
          Edit
        </Button>
        <Box
          sx={{
            display: "flex",
            gap: 1,
            alignItems: "center",
            flex: { xs: "1 1 100%", sm: "0 0 auto" },
            justifyContent: { xs: "center", sm: "flex-end" },
            mt: { xs: 1, sm: 0 },
          }}
        >
          {(() => {
            const slidesArray = getSlidesArray();
            const currentIndex = getCurrentSlideIndex();
            const isFirstSlide = currentIndex <= 0;
            const isLastSlide = currentIndex >= slidesArray.length - 1;

            return (
              <>
                <IconButton
                  onClick={onPreviousSlide}
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
                  onClick={onNextSlide}
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
        <Button
          variant="outlined"
          color="error"
          fullWidth={false}
          sx={{
            textTransform: "none",
            minWidth: { xs: "120px", sm: "auto" },
            fontSize: { xs: "0.875rem", sm: "1rem" },
          }}
          onClick={onDeleteSlide}
        >
          Delete
        </Button>
      </Box>
    </Paper>
  );
};

export default ContentPreviewPanel;


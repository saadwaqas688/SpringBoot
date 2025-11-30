import { Paper, Box, Typography, Button } from "@mui/material";
import { SlidePreview } from "./slide-preview/SlidePreview";
import { SlideNavigation } from "./SlideNavigation";
import { EmptySlideState } from "./EmptySlideState";

interface SlidePreviewContainerProps {
  selectedSlide: any;
  slideTitle: string;
  slideType: string;
  // Image props
  imageUrl?: string;
  imageText?: string;
  imageCollection?: Array<{ url: string; alt?: string; text?: string }>;
  // Video props
  videoUrl?: string;
  videoText?: string;
  videoThumbnail?: string;
  videoCollection?: Array<{ url: string; title?: string; thumbnail?: string }>;
  // List props
  bulletPoints?: string[];
  expandableListItems?: Array<{ title: string; content: string }>;
  focusMode?: boolean;
  expandedItemIndex?: number | null;
  promptText?: string;
  doneText?: string;
  // Navigation
  slidesArray: any[];
  currentSlideIndex: number;
  onImageClick: (url: string) => void;
  onVideoClick: (url: string) => void;
  onToggleExpandableItem: (index: number) => void;
  onPreviousSlide: () => void;
  onNextSlide: () => void;
}

export const SlidePreviewContainer: React.FC<SlidePreviewContainerProps> = ({
  selectedSlide,
  slideTitle,
  slideType,
  imageUrl,
  imageText,
  imageCollection,
  videoUrl,
  videoText,
  videoThumbnail,
  videoCollection,
  bulletPoints,
  expandableListItems,
  focusMode,
  expandedItemIndex,
  promptText,
  doneText,
  slidesArray,
  currentSlideIndex,
  onImageClick,
  onVideoClick,
  onToggleExpandableItem,
  onPreviousSlide,
  onNextSlide,
}) => {
  if (!selectedSlide) {
    return <EmptySlideState />;
  }

  const isFirstSlide = currentSlideIndex <= 0;
  const isLastSlide = currentSlideIndex >= slidesArray.length - 1;

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
      {selectedSlide && (
        <SlidePreview
          slideTitle={slideTitle}
          slideType={slideType}
          imageUrl={imageUrl}
          imageText={imageText}
          imageCollection={imageCollection}
          videoUrl={videoUrl}
          videoText={videoText}
          videoThumbnail={videoThumbnail}
          videoCollection={videoCollection}
          bulletPoints={bulletPoints}
          expandableListItems={expandableListItems}
          focusMode={focusMode}
          expandedItemIndex={expandedItemIndex}
          promptText={promptText}
          onImageClick={onImageClick}
          onVideoClick={onVideoClick}
          onToggleExpandableItem={onToggleExpandableItem}
        />
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
            py: { xs: 1, sm: 1.5 },
          }}
        >
          {doneText || "Continue"}
        </Button>
        <SlideNavigation
          onPrevious={onPreviousSlide}
          onNext={onNextSlide}
          isFirstSlide={isFirstSlide}
          isLastSlide={isLastSlide}
        />
      </Box>
    </Paper>
  );
};


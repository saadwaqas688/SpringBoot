import { Box, Typography } from "@mui/material";
import { SingleImagePreview } from "./SingleImagePreview";
import { ImageCollectionPreview } from "./ImageCollectionPreview";
import { SingleVideoPreview } from "./SingleVideoPreview";
import { VideoCollectionPreview } from "./VideoCollectionPreview";
import { BulletedListPreview } from "./BulletedListPreview";
import { ExpandableListPreview } from "./ExpandableListPreview";

interface SlidePreviewProps {
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
  onImageClick: (url: string) => void;
  onVideoClick: (url: string) => void;
  onToggleExpandableItem: (index: number) => void;
}

export const SlidePreview: React.FC<SlidePreviewProps> = ({
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
  onImageClick,
  onVideoClick,
  onToggleExpandableItem,
}) => {
  const isSingleImage = slideType === "single-image";
  const isImageCollection = slideType === "image-collection";
  const isSingleVideo = slideType === "single-video";
  const isVideoCollection = slideType === "video-collection";
  const isExpandableList = slideType === "expandable-list";
  const isBulletedList =
    slideType === "bulleted-list" || slideType === "text" || !slideType;

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

      <Box
        sx={{ maxWidth: { xs: "100%", md: 600 }, mx: "auto", width: "100%" }}
      >
        {isSingleImage && (
          <SingleImagePreview
            imageUrl={imageUrl || ""}
            imageText={imageText || ""}
            slideTitle={slideTitle}
            onImageClick={onImageClick}
          />
        )}

        {isImageCollection && (
          <ImageCollectionPreview
            images={imageCollection || []}
            onImageClick={onImageClick}
          />
        )}

        {isSingleVideo && (
          <SingleVideoPreview
            videoUrl={videoUrl || ""}
            videoText={videoText || ""}
            videoThumbnail={videoThumbnail || ""}
            onVideoClick={onVideoClick}
          />
        )}

        {isVideoCollection && (
          <VideoCollectionPreview
            videos={videoCollection || []}
            onVideoClick={onVideoClick}
          />
        )}

        {isExpandableList && expandableListItems && (
          <ExpandableListPreview
            items={expandableListItems}
            focusMode={focusMode || false}
            expandedItemIndex={expandedItemIndex ?? null}
            promptText={promptText || ""}
            onToggleItem={onToggleExpandableItem}
          />
        )}

        {isBulletedList && bulletPoints && (
          <BulletedListPreview bulletPoints={bulletPoints} />
        )}
      </Box>
    </Box>
  );
};

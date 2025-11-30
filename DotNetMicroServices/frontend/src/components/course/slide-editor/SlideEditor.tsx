import { Box, Typography, TextField, Divider, Collapse } from "@mui/material";
import {
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
} from "@mui/icons-material";
import { SingleImageEditor } from "./SingleImageEditor";
import { SingleVideoEditor } from "./SingleVideoEditor";
import { BulletedListEditor } from "./BulletedListEditor";
import { ExpandableListEditor } from "./ExpandableListEditor";
import { ImageCollectionEditor } from "./ImageCollectionEditor";
import { VideoCollectionEditor } from "./VideoCollectionEditor";

interface SlideEditorProps {
  slideTitle: string;
  slideType: string;
  // Image props
  imageUrl?: string;
  imageText?: string;
  imageCollection?: Array<{ url: string; alt?: string; text?: string }>;
  expandedImageIndex?: number | null;
  // Video props
  videoUrl?: string;
  videoText?: string;
  videoThumbnail?: string;
  videoCollection?: Array<{ url: string; title?: string; thumbnail?: string }>;
  expandedVideoIndex?: number | null;
  // List props
  bulletPoints?: string[];
  doneText?: string;
  expandableListItems?: Array<{ title: string; content: string }>;
  focusMode?: boolean;
  promptText?: string;
  isListExpanded?: boolean;
  isNarrationExpanded?: boolean;
  // Handlers
  onSlideTitleChange: (title: string) => void;
  // Image handlers
  onImageUrlChange?: (url: string) => void;
  onImageTextChange?: (text: string) => void;
  onAddImageToCollection?: () => void;
  onRemoveImageFromCollection?: (index: number) => void;
  onToggleImageDropdown?: (index: number) => void;
  onUpdateImageInCollection?: (index: number, url?: string, text?: string) => void;
  onImageUpload?: (file: File) => Promise<void>;
  onImageUploadToCollection?: (index: number, file: File) => Promise<void>;
  // Video handlers
  onVideoUrlChange?: (url: string) => void;
  onVideoTextChange?: (text: string) => void;
  onVideoThumbnailChange?: (thumbnail: string) => void;
  onAddVideoToCollection?: () => void;
  onRemoveVideoFromCollection?: (index: number) => void;
  onToggleVideoDropdown?: (index: number) => void;
  onUpdateVideoInCollection?: (index: number, url?: string, thumbnail?: string, title?: string) => void;
  onVideoUpload?: (file: File) => Promise<void>;
  onVideoUploadToCollection?: (index: number, file: File) => Promise<void>;
  onThumbnailUpload?: (file: File) => Promise<void>;
  onThumbnailUploadToCollection?: (index: number, file: File) => Promise<void>;
  // List handlers
  onAddBulletPoint?: () => void;
  onUpdateBulletPoint?: (index: number, value: string) => void;
  onDeleteBulletPoint?: (index: number) => void;
  onDoneTextChange?: (text: string) => void;
  onAddExpandableItem?: () => void;
  onUpdateExpandableItem?: (index: number, field: "title" | "content", value: string) => void;
  onDeleteExpandableItem?: (index: number) => void;
  onToggleFocusMode?: () => void;
  onPromptTextChange?: (text: string) => void;
  onToggleListExpanded?: () => void;
  onToggleNarrationExpanded?: () => void;
  // Loading states
  isUploadingImage?: boolean;
  isUploadingVideo?: boolean;
  isUploadingThumbnail?: boolean;
}

export const SlideEditor: React.FC<SlideEditorProps> = ({
  slideTitle,
  slideType,
  imageUrl,
  imageText,
  imageCollection,
  expandedImageIndex,
  videoUrl,
  videoText,
  videoThumbnail,
  videoCollection,
  expandedVideoIndex,
  bulletPoints,
  doneText,
  expandableListItems,
  focusMode,
  promptText,
  isListExpanded,
  isNarrationExpanded,
  onSlideTitleChange,
  onImageUrlChange,
  onImageTextChange,
  onAddImageToCollection,
  onRemoveImageFromCollection,
  onToggleImageDropdown,
  onUpdateImageInCollection,
  onImageUpload,
  onImageUploadToCollection,
  onVideoUrlChange,
  onVideoTextChange,
  onVideoThumbnailChange,
  onAddVideoToCollection,
  onRemoveVideoFromCollection,
  onToggleVideoDropdown,
  onUpdateVideoInCollection,
  onVideoUpload,
  onVideoUploadToCollection,
  onThumbnailUpload,
  onThumbnailUploadToCollection,
  onAddBulletPoint,
  onUpdateBulletPoint,
  onDeleteBulletPoint,
  onDoneTextChange,
  onAddExpandableItem,
  onUpdateExpandableItem,
  onDeleteExpandableItem,
  onToggleFocusMode,
  onPromptTextChange,
  onToggleListExpanded,
  onToggleNarrationExpanded,
  isUploadingImage,
  isUploadingVideo,
  isUploadingThumbnail,
}) => {
  const isSingleImage = slideType === "single-image";
  const isImageCollection = slideType === "image-collection";
  const isSingleVideo = slideType === "single-video";
  const isVideoCollection = slideType === "video-collection";
  const isExpandableList = slideType === "expandable-list";
  const isBulletedList = slideType === "bulleted-list" || !slideType;

  return (
    <>
      {/* Title Section */}
      <Box sx={{ mb: { xs: 2, md: 3 } }}>
        <Typography
          variant="subtitle2"
          sx={{
            fontWeight: 600,
            mb: 1,
            color: "#374151",
            fontSize: { xs: "0.875rem", md: "0.9375rem" },
          }}
        >
          Title
        </Typography>
        <TextField
          fullWidth
          value={slideTitle}
          onChange={(e) => onSlideTitleChange(e.target.value)}
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
      {isSingleImage && onImageUrlChange && onImageTextChange && onImageUpload && (
        <SingleImageEditor
          imageUrl={imageUrl || ""}
          imageText={imageText || ""}
          onImageUrlChange={onImageUrlChange}
          onImageTextChange={onImageTextChange}
          onImageUpload={onImageUpload}
          isUploading={isUploadingImage || false}
        />
      )}

      {isImageCollection &&
        imageCollection !== undefined &&
        onAddImageToCollection &&
        onRemoveImageFromCollection &&
        onToggleImageDropdown &&
        onUpdateImageInCollection &&
        onImageUploadToCollection && (
          <ImageCollectionEditor
            images={imageCollection}
            expandedImageIndex={expandedImageIndex || null}
            onAddImage={onAddImageToCollection}
            onRemoveImage={onRemoveImageFromCollection}
            onToggleImageDropdown={onToggleImageDropdown}
            onUpdateImage={onUpdateImageInCollection}
            onImageUpload={onImageUploadToCollection}
            isUploading={isUploadingImage || false}
          />
        )}

      {isSingleVideo &&
        onVideoUrlChange &&
        onVideoTextChange &&
        onVideoThumbnailChange &&
        onVideoUpload &&
        onThumbnailUpload && (
          <SingleVideoEditor
            videoUrl={videoUrl || ""}
            videoText={videoText || ""}
            videoThumbnail={videoThumbnail || ""}
            onVideoUrlChange={onVideoUrlChange}
            onVideoTextChange={onVideoTextChange}
            onVideoThumbnailChange={onVideoThumbnailChange}
            onVideoUpload={onVideoUpload}
            onThumbnailUpload={onThumbnailUpload}
            isUploadingVideo={isUploadingVideo || false}
            isUploadingThumbnail={isUploadingThumbnail || false}
          />
        )}

      {isVideoCollection &&
        videoCollection !== undefined &&
        onAddVideoToCollection &&
        onRemoveVideoFromCollection &&
        onToggleVideoDropdown &&
        onUpdateVideoInCollection &&
        onVideoUploadToCollection &&
        onThumbnailUploadToCollection && (
          <VideoCollectionEditor
            videos={videoCollection}
            expandedVideoIndex={expandedVideoIndex || null}
            onAddVideo={onAddVideoToCollection}
            onRemoveVideo={onRemoveVideoFromCollection}
            onToggleVideoDropdown={onToggleVideoDropdown}
            onUpdateVideo={onUpdateVideoInCollection}
            onVideoUpload={onVideoUploadToCollection}
            onThumbnailUpload={onThumbnailUploadToCollection}
            isUploadingVideo={isUploadingVideo || false}
            isUploadingThumbnail={isUploadingThumbnail || false}
          />
        )}

      {isExpandableList &&
        expandableListItems !== undefined &&
        onAddExpandableItem &&
        onUpdateExpandableItem &&
        onDeleteExpandableItem &&
        onToggleFocusMode &&
        onPromptTextChange &&
        onToggleListExpanded && (
          <ExpandableListEditor
            items={expandableListItems}
            focusMode={focusMode || false}
            promptText={promptText || ""}
            isListExpanded={isListExpanded || false}
            onAddItem={onAddExpandableItem}
            onUpdateItem={onUpdateExpandableItem}
            onDeleteItem={onDeleteExpandableItem}
            onToggleFocusMode={onToggleFocusMode}
            onPromptTextChange={onPromptTextChange}
            onToggleListExpanded={onToggleListExpanded}
          />
        )}

      {isBulletedList &&
        bulletPoints !== undefined &&
        onAddBulletPoint &&
        onUpdateBulletPoint &&
        onDeleteBulletPoint &&
        onDoneTextChange && (
          <BulletedListEditor
            bulletPoints={bulletPoints}
            doneText={doneText || "Continue"}
            onAddBulletPoint={onAddBulletPoint}
            onUpdateBulletPoint={onUpdateBulletPoint}
            onDeleteBulletPoint={onDeleteBulletPoint}
            onDoneTextChange={onDoneTextChange}
          />
        )}

      {/* Narration Section - Common for all types */}
      {onToggleNarrationExpanded && (
        <>
          <Divider sx={{ my: 2 }} />
          <Box sx={{ mb: 3 }}>
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                mb: 1,
                cursor: "pointer",
              }}
              onClick={onToggleNarrationExpanded}
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
            <Collapse in={isNarrationExpanded || false}>
              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ fontSize: "0.875rem" }}
              >
                Narration settings coming soon
              </Typography>
            </Collapse>
          </Box>
        </>
      )}
    </>
  );
};


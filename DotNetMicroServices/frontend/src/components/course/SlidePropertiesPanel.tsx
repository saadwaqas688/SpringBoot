import { Paper, Box, IconButton } from "@mui/material";
import { Close as CloseIcon } from "@mui/icons-material";
import { SlideEditor } from "./slide-editor/SlideEditor";

interface SlidePropertiesPanelProps {
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
  // Mobile
  mobilePropertiesOpen: boolean;
  onCloseMobile: () => void;
  // Handlers - same as SlideEditor
  onSlideTitleChange: (title: string) => void;
  onImageUrlChange?: (url: string) => void;
  onImageTextChange?: (text: string) => void;
  onAddImageToCollection?: () => void;
  onRemoveImageFromCollection?: (index: number) => void;
  onToggleImageDropdown?: (index: number) => void;
  onUpdateImageInCollection?: (index: number, url?: string, text?: string) => void;
  onImageUpload?: (file: File) => Promise<void>;
  onImageUploadToCollection?: (index: number, file: File) => Promise<void>;
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
  isUploadingImage?: boolean;
  isUploadingVideo?: boolean;
  isUploadingThumbnail?: boolean;
}

export const SlidePropertiesPanel: React.FC<SlidePropertiesPanelProps> = ({
  mobilePropertiesOpen,
  onCloseMobile,
  ...slideEditorProps
}) => {
  return (
    <Paper
      sx={{
        width: { xs: "100%", md: 350, lg: 350 },
        minWidth: { xs: "100%", md: 350, lg: 350 },
        maxWidth: { xs: "100%", md: 350, lg: 350 },
        p: { xs: 1.5, md: 2 },
        overflowY: "auto",
        overflowX: "hidden",
        maxHeight: { xs: "85vh", lg: "100%" },
        flexShrink: 0,
        display: { xs: mobilePropertiesOpen ? "block" : "none", lg: "block" },
        position: { xs: "fixed", lg: "relative" },
        right: { xs: 0, lg: "auto" },
        top: { xs: 60, lg: "auto" },
        zIndex: { xs: 1200, lg: "auto" },
        height: { xs: "calc(100vh - 60px)", lg: "100%" },
        boxShadow: { xs: "0 4px 12px rgba(0,0,0,0.15)", lg: "none" },
        backgroundColor: { xs: "white", lg: "inherit" },
      }}
    >
      {/* Mobile Close Button */}
      <Box
        sx={{
          display: { xs: "flex", lg: "none" },
          justifyContent: "flex-end",
          mb: 1,
          pb: 1,
          borderBottom: "1px solid #e5e7eb",
        }}
      >
        <IconButton
          onClick={onCloseMobile}
          sx={{
            color: "#6b7280",
            "&:hover": { bgcolor: "#f3f4f6" },
          }}
        >
          <CloseIcon />
        </IconButton>
      </Box>

      <SlideEditor {...slideEditorProps} />
    </Paper>
  );
};

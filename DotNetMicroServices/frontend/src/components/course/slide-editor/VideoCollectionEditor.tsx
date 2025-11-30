import {
  Box,
  Typography,
  Button,
  Divider,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
  TextField,
} from "@mui/material";
import {
  Add as AddIcon,
  DeleteOutline as DeleteOutlineIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  VideoLibrary as VideoIcon,
} from "@mui/icons-material";

interface VideoItem {
  url: string;
  title?: string;
  thumbnail?: string;
}

interface VideoCollectionEditorProps {
  videos: VideoItem[];
  expandedVideoIndex: number | null;
  onAddVideo: () => void;
  onRemoveVideo: (index: number) => void;
  onToggleVideoDropdown: (index: number) => void;
  onUpdateVideo: (index: number, url?: string, thumbnail?: string, title?: string) => void;
  onVideoUpload: (index: number, file: File) => Promise<void>;
  onThumbnailUpload: (index: number, file: File) => Promise<void>;
  isUploadingVideo: boolean;
  isUploadingThumbnail: boolean;
}

export const VideoCollectionEditor: React.FC<VideoCollectionEditorProps> = ({
  videos,
  expandedVideoIndex,
  onAddVideo,
  onRemoveVideo,
  onToggleVideoDropdown,
  onUpdateVideo,
  onVideoUpload,
  onThumbnailUpload,
  isUploadingVideo,
  isUploadingThumbnail,
}) => {
  return (
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
            onClick={onAddVideo}
            sx={{
              textTransform: "none",
              fontSize: "0.75rem",
              color: "#6366f1",
            }}
          >
            Add Video
          </Button>
        </Box>

        {videos.map((video, index) => (
          <Box key={index} sx={{ mb: 2 }}>
            <FormControl fullWidth size="small">
              <InputLabel>Video {index + 1}</InputLabel>
              <Select
                value={expandedVideoIndex === index ? "expanded" : "collapsed"}
                label={`Video ${index + 1}`}
                onChange={() => onToggleVideoDropdown(index)}
                endAdornment={
                  <IconButton
                    size="small"
                    onClick={(e) => {
                      e.stopPropagation();
                      onRemoveVideo(index);
                    }}
                    sx={{
                      color: "#ef4444",
                      mr: 1,
                    }}
                  >
                    <DeleteOutlineIcon fontSize="small" />
                  </IconButton>
                }
                IconComponent={
                  expandedVideoIndex === index ? ExpandLessIcon : ExpandMoreIcon
                }
              >
                <MenuItem value={expandedVideoIndex === index ? "expanded" : "collapsed"}>
                  {video.url ? "Video uploaded" : "No video"}
                </MenuItem>
              </Select>
            </FormControl>

            {expandedVideoIndex === index && (
              <Box sx={{ mt: 2, p: 2, border: "1px solid #e5e7eb", borderRadius: 1 }}>
                <TextField
                  fullWidth
                  value={video.url}
                  onChange={(e) => onUpdateVideo(index, e.target.value)}
                  placeholder="Video URL"
                  size="small"
                  sx={{ mb: 1 }}
                />
                <input
                  type="file"
                  accept="video/*"
                  id={`video-upload-${index}`}
                  style={{ display: "none" }}
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (file) {
                      await onVideoUpload(index, file);
                    }
                  }}
                />
                <label htmlFor={`video-upload-${index}`}>
                  <Button
                    variant="outlined"
                    component="span"
                    startIcon={<VideoIcon />}
                    fullWidth
                    disabled={isUploadingVideo}
                    sx={{ textTransform: "none", mb: 1 }}
                  >
                    {isUploadingVideo ? "Uploading..." : "Upload Video"}
                  </Button>
                </label>
                {video.url && (
                  <Box
                    sx={{
                      width: "100%",
                      maxHeight: 150,
                      borderRadius: 1,
                      mt: 1,
                      overflow: "hidden",
                    }}
                  >
                    <video
                      src={video.url}
                      style={{
                        width: "100%",
                        maxHeight: 150,
                        objectFit: "contain",
                        borderRadius: "8px",
                      }}
                      controls
                    />
                  </Box>
                )}
                <TextField
                  fullWidth
                  value={video.thumbnail || ""}
                  onChange={(e) => onUpdateVideo(index, undefined, e.target.value)}
                  placeholder="Thumbnail URL"
                  size="small"
                  sx={{ mt: 1, mb: 1 }}
                />
                <input
                  type="file"
                  accept="image/*"
                  id={`video-thumbnail-upload-${index}`}
                  style={{ display: "none" }}
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (file) {
                      await onThumbnailUpload(index, file);
                    }
                  }}
                />
                <label htmlFor={`video-thumbnail-upload-${index}`}>
                  <Button
                    variant="outlined"
                    component="span"
                    fullWidth
                    disabled={isUploadingThumbnail}
                    sx={{ textTransform: "none", mb: 1 }}
                  >
                    {isUploadingThumbnail ? "Uploading..." : "Upload Thumbnail"}
                  </Button>
                </label>
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
                      mt: 1,
                      border: "1px solid #e5e7eb",
                    }}
                  />
                )}
                <TextField
                  fullWidth
                  value={video.title || ""}
                  onChange={(e) => onUpdateVideo(index, undefined, undefined, e.target.value)}
                  placeholder="Video title"
                  size="small"
                  sx={{ mt: 1 }}
                />
              </Box>
            )}
          </Box>
        ))}

        {videos.length === 0 && (
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
  );
};


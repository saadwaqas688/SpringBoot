import { Box, Typography, TextField, Divider, Button } from "@mui/material";
import { VideoLibrary as VideoIcon } from "@mui/icons-material";

interface SingleVideoEditorProps {
  videoUrl: string;
  videoText: string;
  videoThumbnail: string;
  onVideoUrlChange: (url: string) => void;
  onVideoTextChange: (text: string) => void;
  onVideoThumbnailChange: (thumbnail: string) => void;
  onVideoUpload: (file: File) => Promise<void>;
  onThumbnailUpload: (file: File) => Promise<void>;
  isUploadingVideo: boolean;
  isUploadingThumbnail: boolean;
}

export const SingleVideoEditor: React.FC<SingleVideoEditorProps> = ({
  videoUrl,
  videoText,
  videoThumbnail,
  onVideoUrlChange,
  onVideoTextChange,
  onVideoThumbnailChange,
  onVideoUpload,
  onThumbnailUpload,
  isUploadingVideo,
  isUploadingThumbnail,
}) => {
  return (
    <>
      <Box sx={{ mb: 3 }}>
        <Typography
          variant="subtitle2"
          sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
        >
          Video
        </Typography>
        <TextField
          fullWidth
          value={videoUrl}
          onChange={(e) => onVideoUrlChange(e.target.value)}
          placeholder="Enter video URL or upload"
          size="small"
          sx={{
            mb: 1,
            "& .MuiOutlinedInput-root": {
              fontSize: "0.875rem",
            },
          }}
        />
        <input
          type="file"
          accept="video/*"
          id="single-video-upload"
          style={{ display: "none" }}
          onChange={async (e) => {
            const file = e.target.files?.[0];
            if (file) {
              await onVideoUpload(file);
              e.target.value = "";
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
              mb: 1,
            }}
          >
            {isUploadingVideo ? "Uploading..." : "Upload Video"}
          </Button>
        </label>
        {videoUrl && (
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
              src={videoUrl}
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
      </Box>
      <Divider sx={{ my: 2 }} />
      <Box sx={{ mb: 3 }}>
        <Typography
          variant="subtitle2"
          sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
        >
          Thumbnail
        </Typography>
        <TextField
          fullWidth
          value={videoThumbnail}
          onChange={(e) => onVideoThumbnailChange(e.target.value)}
          placeholder="Enter thumbnail URL or upload"
          size="small"
          sx={{
            mb: 1,
            "& .MuiOutlinedInput-root": {
              fontSize: "0.875rem",
            },
          }}
        />
        <input
          type="file"
          accept="image/*"
          id="video-thumbnail-upload"
          style={{ display: "none" }}
          onChange={async (e) => {
            const file = e.target.files?.[0];
            if (file) {
              await onThumbnailUpload(file);
              e.target.value = "";
            }
          }}
        />
        <label htmlFor="video-thumbnail-upload" style={{ width: "100%" }}>
          <Button
            variant="outlined"
            component="span"
            fullWidth
            disabled={isUploadingThumbnail}
            sx={{
              textTransform: "none",
              mb: 1,
            }}
          >
            {isUploadingThumbnail ? "Uploading..." : "Upload Thumbnail"}
          </Button>
        </label>
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
              mt: 1,
              border: "1px solid #e5e7eb",
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
          onChange={(e) => onVideoTextChange(e.target.value)}
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
  );
};


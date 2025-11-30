import { Box, Typography } from "@mui/material";

interface SingleVideoPreviewProps {
  videoUrl: string;
  videoText: string;
  videoThumbnail: string;
  onVideoClick: (url: string) => void;
}

export const SingleVideoPreview: React.FC<SingleVideoPreviewProps> = ({
  videoUrl,
  videoText,
  videoThumbnail,
  onVideoClick,
}) => {
  return (
    <Box>
      {videoThumbnail ? (
        <Box
          component="img"
          src={videoThumbnail}
          alt="Video thumbnail"
          onClick={() => onVideoClick(videoUrl)}
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
              onVideoClick(videoUrl);
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
          <Typography color="text.secondary">No video uploaded</Typography>
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
  );
};


import { Box, Typography, Grid } from "@mui/material";
import { PlayCircle as PlayCircleIcon } from "@mui/icons-material";

interface VideoItem {
  url: string;
  title?: string;
  thumbnail?: string;
}

interface VideoCollectionPreviewProps {
  videos: VideoItem[];
  onVideoClick: (url: string) => void;
}

export const VideoCollectionPreview: React.FC<VideoCollectionPreviewProps> = ({
  videos,
  onVideoClick,
}) => {
  if (videos.length === 0) {
    return (
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: 200,
        }}
      >
        <Typography color="text.secondary">No videos added yet</Typography>
      </Box>
    );
  }

  return (
    <Box
      sx={{
        maxHeight: "calc(100vh - 400px)",
        overflowY: "auto",
        overflowX: "hidden",
      }}
    >
      <Grid container spacing={2}>
        {videos.map((video, index) => (
          <Grid item xs={12} sm={6} md={4} key={index}>
            {video.thumbnail ? (
              <Box
                sx={{
                  position: "relative",
                  width: "100%",
                  maxHeight: 150,
                  cursor: "pointer",
                  "&:hover .play-overlay": {
                    opacity: 1,
                  },
                }}
                onClick={() => video.url && onVideoClick(video.url)}
              >
                <Box
                  component="img"
                  src={video.thumbnail}
                  alt={video.title || `Video ${index + 1}`}
                  sx={{
                    width: "100%",
                    height: 150,
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
                  }}
                >
                  <PlayCircleIcon
                    sx={{
                      fontSize: 48,
                      color: "white",
                      pointerEvents: "none",
                    }}
                  />
                </Box>
              </Box>
            ) : video.url ? (
              <Box
                sx={{
                  position: "relative",
                  width: "100%",
                  maxHeight: 150,
                  cursor: "pointer",
                }}
                onClick={() => onVideoClick(video.url)}
              >
                <video
                  src={video.url}
                  style={{
                    width: "100%",
                    height: 150,
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
                <Typography color="text.secondary" variant="body2">
                  No video
                </Typography>
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
          </Grid>
        ))}
      </Grid>
      {videos.length > 0 && (
        <Typography
          variant="body2"
          sx={{
            mt: 2,
            textAlign: "center",
            color: "#9ca3af",
            fontSize: "0.875rem",
          }}
        >
          Select each video to play
        </Typography>
      )}
    </Box>
  );
};


import { Box, Typography, Grid } from "@mui/material";
import { Image as ImageIcon } from "@mui/icons-material";

interface ImageItem {
  url: string;
  alt?: string;
  text?: string;
}

interface ImageCollectionPreviewProps {
  images: ImageItem[];
  onImageClick: (url: string) => void;
}

export const ImageCollectionPreview: React.FC<ImageCollectionPreviewProps> = ({
  images,
  onImageClick,
}) => {
  if (images.length === 0) {
    return (
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: 200,
        }}
      >
        <Typography color="text.secondary">No images added yet</Typography>
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
        {images.map((img, index) => (
          <Grid item xs={12} sm={6} md={4} key={index}>
            {img.url ? (
              <Box>
                <Box
                  component="img"
                  src={img.url}
                  alt={img.alt || `Image ${index + 1}`}
                  onClick={() => onImageClick(img.url)}
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
      {images.length > 0 && (
        <Typography
          variant="body2"
          sx={{
            mt: 2,
            textAlign: "center",
            color: "#9ca3af",
            fontSize: "0.875rem",
          }}
        >
          Select each image for more details
        </Typography>
      )}
    </Box>
  );
};


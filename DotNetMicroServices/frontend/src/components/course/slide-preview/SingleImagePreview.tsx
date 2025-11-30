import { Box, Typography } from "@mui/material";
import { Image as ImageIcon } from "@mui/icons-material";

interface SingleImagePreviewProps {
  imageUrl: string;
  imageText: string;
  slideTitle: string;
  onImageClick: (url: string) => void;
}

export const SingleImagePreview: React.FC<SingleImagePreviewProps> = ({
  imageUrl,
  imageText,
  slideTitle,
  onImageClick,
}) => {
  return (
    <Box>
      {imageUrl ? (
        <Box
          component="img"
          src={imageUrl}
          alt={slideTitle}
          onClick={() => onImageClick(imageUrl)}
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
          <Typography color="text.secondary">No image uploaded</Typography>
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
  );
};


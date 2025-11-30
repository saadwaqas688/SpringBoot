import { Box, Typography, TextField, Divider } from "@mui/material";

interface SingleImageEditorProps {
  imageUrl: string;
  imageText: string;
  onImageUrlChange: (url: string) => void;
  onImageTextChange: (text: string) => void;
  onImageUpload: (file: File) => Promise<void>;
  isUploading: boolean;
}

export const SingleImageEditor: React.FC<SingleImageEditorProps> = ({
  imageUrl,
  imageText,
  onImageUrlChange,
  onImageTextChange,
  onImageUpload,
  isUploading,
}) => {
  return (
    <>
      <Box sx={{ mb: 3 }}>
        <Typography
          variant="subtitle2"
          sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
        >
          Image
        </Typography>
        <TextField
          fullWidth
          value={imageUrl}
          onChange={(e) => onImageUrlChange(e.target.value)}
          placeholder="Enter image URL or upload"
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
          id="single-image-upload"
          style={{ display: "none" }}
          onChange={async (e) => {
            const file = e.target.files?.[0];
            if (file) {
              await onImageUpload(file);
              e.target.value = "";
            }
          }}
        />
        <label htmlFor="single-image-upload" style={{ width: "100%" }}>
          <Box
            component="span"
            sx={{
              display: "inline-block",
              width: "100%",
            }}
          >
            <Box
              component="span"
              sx={{
                display: "block",
                width: "100%",
                textAlign: "center",
                py: 1,
                border: "1px dashed #d1d5db",
                borderRadius: 1,
                cursor: "pointer",
                "&:hover": {
                  backgroundColor: "#f9fafb",
                },
              }}
            >
              {isUploading ? "Uploading..." : "Upload Image"}
            </Box>
          </Box>
        </label>
        {imageUrl && (
          <Box
            component="img"
            src={imageUrl}
            alt="Preview"
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
        <Typography
          variant="caption"
          sx={{
            color: "#6b7280",
            fontSize: "0.75rem",
            mt: 1,
            display: "block",
          }}
        >
          Note: Supported file types: jpg, jpeg, png, gif, svg, heic
        </Typography>
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
          value={imageText}
          onChange={(e) => onImageTextChange(e.target.value)}
          placeholder="Text to display below image"
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


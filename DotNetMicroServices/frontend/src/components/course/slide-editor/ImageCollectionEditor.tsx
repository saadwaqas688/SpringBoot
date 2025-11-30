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
} from "@mui/icons-material";

interface ImageItem {
  url: string;
  alt?: string;
  text?: string;
}

interface ImageCollectionEditorProps {
  images: ImageItem[];
  expandedImageIndex: number | null;
  onAddImage: () => void;
  onRemoveImage: (index: number) => void;
  onToggleImageDropdown: (index: number) => void;
  onUpdateImage: (index: number, url?: string, text?: string) => void;
  onImageUpload: (index: number, file: File) => Promise<void>;
  isUploading: boolean;
}

export const ImageCollectionEditor: React.FC<ImageCollectionEditorProps> = ({
  images,
  expandedImageIndex,
  onAddImage,
  onRemoveImage,
  onToggleImageDropdown,
  onUpdateImage,
  onImageUpload,
  isUploading,
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
            Images
          </Typography>
          <Button
            size="small"
            startIcon={<AddIcon />}
            onClick={onAddImage}
            sx={{
              textTransform: "none",
              fontSize: "0.75rem",
              color: "#6366f1",
            }}
          >
            Add Image
          </Button>
        </Box>

        {images.map((img, index) => (
          <Box key={index} sx={{ mb: 2 }}>
            <FormControl fullWidth size="small">
              <InputLabel>Image {index + 1}</InputLabel>
              <Select
                value={expandedImageIndex === index ? "expanded" : "collapsed"}
                label={`Image ${index + 1}`}
                onChange={() => onToggleImageDropdown(index)}
                endAdornment={
                  <IconButton
                    size="small"
                    onClick={(e) => {
                      e.stopPropagation();
                      onRemoveImage(index);
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
                  expandedImageIndex === index ? ExpandLessIcon : ExpandMoreIcon
                }
              >
                <MenuItem value={expandedImageIndex === index ? "expanded" : "collapsed"}>
                  {img.url ? "Image uploaded" : "No image"}
                </MenuItem>
              </Select>
            </FormControl>

            {expandedImageIndex === index && (
              <Box sx={{ mt: 2, p: 2, border: "1px solid #e5e7eb", borderRadius: 1 }}>
                <TextField
                  fullWidth
                  value={img.url}
                  onChange={(e) => onUpdateImage(index, e.target.value)}
                  placeholder="Image URL"
                  size="small"
                  sx={{ mb: 1 }}
                />
                <input
                  type="file"
                  accept="image/*"
                  id={`image-upload-${index}`}
                  style={{ display: "none" }}
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (file) {
                      await onImageUpload(index, file);
                    }
                  }}
                />
                <label htmlFor={`image-upload-${index}`}>
                  <Button
                    variant="outlined"
                    component="span"
                    fullWidth
                    disabled={isUploading}
                    sx={{ textTransform: "none", mb: 1 }}
                  >
                    {isUploading ? "Uploading..." : "Upload Image"}
                  </Button>
                </label>
                {img.url && (
                  <Box
                    component="img"
                    src={img.url}
                    alt={img.alt || `Image ${index + 1}`}
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
                  value={img.text || ""}
                  onChange={(e) => onUpdateImage(index, undefined, e.target.value)}
                  placeholder="Image text"
                  size="small"
                  sx={{ mt: 1 }}
                />
              </Box>
            )}
          </Box>
        ))}

        {images.length === 0 && (
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ textAlign: "center", py: 2 }}
          >
            Click "Add Image" to add images to this collection
          </Typography>
        )}
      </Box>
      <Divider sx={{ my: 2 }} />
    </>
  );
};


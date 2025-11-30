import { Box, Typography, TextField, Button, Divider, IconButton } from "@mui/material";
import { Add as AddIcon, Delete as DeleteIcon } from "@mui/icons-material";

interface BulletedListEditorProps {
  bulletPoints: string[];
  doneText: string;
  onAddBulletPoint: () => void;
  onUpdateBulletPoint: (index: number, value: string) => void;
  onDeleteBulletPoint: (index: number) => void;
  onDoneTextChange: (text: string) => void;
}

export const BulletedListEditor: React.FC<BulletedListEditorProps> = ({
  bulletPoints,
  doneText,
  onAddBulletPoint,
  onUpdateBulletPoint,
  onDeleteBulletPoint,
  onDoneTextChange,
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
            Bullet Points
          </Typography>
          <Button
            size="small"
            startIcon={<AddIcon />}
            onClick={onAddBulletPoint}
            sx={{
              textTransform: "none",
              fontSize: "0.75rem",
              color: "#6366f1",
            }}
          >
            Add Point
          </Button>
        </Box>
        {bulletPoints.map((point, index) => (
          <Box
            key={index}
            sx={{
              display: "flex",
              gap: 1,
              mb: 1,
              alignItems: "flex-start",
            }}
          >
            <Typography
              sx={{
                fontSize: "1.25rem",
                lineHeight: 1,
                mt: 1,
                color: "#6366f1",
              }}
            >
              •
            </Typography>
            <TextField
              fullWidth
              value={point}
              onChange={(e) => onUpdateBulletPoint(index, e.target.value)}
              placeholder={`Bullet point ${index + 1}`}
              size="small"
              sx={{
                "& .MuiOutlinedInput-root": {
                  fontSize: "0.875rem",
                },
              }}
            />
            <IconButton
              size="small"
              onClick={() => onDeleteBulletPoint(index)}
              sx={{
                color: "#ef4444",
                mt: 0.5,
              }}
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </Box>
        ))}
      </Box>
      <Divider sx={{ my: 2 }} />
      <Box sx={{ mb: 3 }}>
        <Typography
          variant="subtitle2"
          sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
        >
          Done Button Text
        </Typography>
        <TextField
          fullWidth
          value={doneText}
          onChange={(e) => onDoneTextChange(e.target.value)}
          placeholder="Continue"
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


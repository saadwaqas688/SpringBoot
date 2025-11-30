import {
  Box,
  Typography,
  TextField,
  Button,
  Divider,
  IconButton,
  Collapse,
  Checkbox,
  FormControlLabel,
} from "@mui/material";
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
} from "@mui/icons-material";

interface ExpandableItem {
  title: string;
  content: string;
}

interface ExpandableListEditorProps {
  items: ExpandableItem[];
  focusMode: boolean;
  promptText: string;
  isListExpanded: boolean;
  onAddItem: () => void;
  onUpdateItem: (index: number, field: "title" | "content", value: string) => void;
  onDeleteItem: (index: number) => void;
  onToggleFocusMode: () => void;
  onPromptTextChange: (text: string) => void;
  onToggleListExpanded: () => void;
}

export const ExpandableListEditor: React.FC<ExpandableListEditorProps> = ({
  items,
  focusMode,
  promptText,
  isListExpanded,
  onAddItem,
  onUpdateItem,
  onDeleteItem,
  onToggleFocusMode,
  onPromptTextChange,
  onToggleListExpanded,
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
            cursor: "pointer",
          }}
          onClick={onToggleListExpanded}
        >
          <Typography
            variant="subtitle2"
            sx={{ fontWeight: 600, color: "#374151" }}
          >
            Expandable list
          </Typography>
          {isListExpanded ? (
            <ExpandLessIcon sx={{ fontSize: 20, color: "#6b7280" }} />
          ) : (
            <ExpandMoreIcon sx={{ fontSize: 20, color: "#6b7280" }} />
          )}
        </Box>

        <Collapse in={isListExpanded}>
          <Box>
            {items.map((item, index) => (
              <Box
                key={index}
                sx={{
                  mb: 2,
                  p: 2,
                  border: "1px solid #e5e7eb",
                  borderRadius: 1,
                }}
              >
                <TextField
                  fullWidth
                  value={item.title}
                  onChange={(e) => onUpdateItem(index, "title", e.target.value)}
                  placeholder={`Item ${index + 1} title`}
                  size="small"
                  sx={{
                    mb: 1,
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
                <TextField
                  fullWidth
                  value={item.content}
                  onChange={(e) =>
                    onUpdateItem(index, "content", e.target.value)
                  }
                  placeholder={`Item ${index + 1} content`}
                  multiline
                  rows={3}
                  size="small"
                  sx={{
                    mb: 1,
                    "& .MuiOutlinedInput-root": {
                      fontSize: "0.875rem",
                    },
                  }}
                />
                <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
                  <IconButton
                    size="small"
                    onClick={() => onDeleteItem(index)}
                    sx={{
                      color: "#ef4444",
                    }}
                  >
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </Box>
              </Box>
            ))}
            <Button
              size="small"
              startIcon={<AddIcon />}
              onClick={onAddItem}
              sx={{
                textTransform: "none",
                fontSize: "0.75rem",
                color: "#6366f1",
                mb: 2,
              }}
            >
              Add Item
            </Button>
          </Box>
        </Collapse>
      </Box>
      <Divider sx={{ my: 2 }} />
      <Box sx={{ mb: 3 }}>
        <FormControlLabel
          control={
            <Checkbox
              checked={focusMode}
              onChange={onToggleFocusMode}
              size="small"
            />
          }
          label={
            <Typography variant="body2" sx={{ fontSize: "0.875rem" }}>
              Focus mode (only one item expanded at a time)
            </Typography>
          }
        />
      </Box>
      <Divider sx={{ my: 2 }} />
      <Box sx={{ mb: 3 }}>
        <Typography
          variant="subtitle2"
          sx={{ fontWeight: 600, mb: 1, color: "#374151" }}
        >
          Prompt Text
        </Typography>
        <TextField
          fullWidth
          value={promptText}
          onChange={(e) => onPromptTextChange(e.target.value)}
          placeholder="Prompt Text"
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


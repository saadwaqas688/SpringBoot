import { Box, Typography, Collapse } from "@mui/material";
import {
  ExpandLess as ExpandLessIcon,
  ChevronRight as ChevronRightIcon,
} from "@mui/icons-material";

interface ExpandableItem {
  title: string;
  content: string;
}

interface ExpandableListPreviewProps {
  items: ExpandableItem[];
  focusMode: boolean;
  expandedItemIndex: number | null;
  promptText: string;
  onToggleItem: (index: number) => void;
}

export const ExpandableListPreview: React.FC<ExpandableListPreviewProps> = ({
  items,
  focusMode,
  expandedItemIndex,
  promptText,
  onToggleItem,
}) => {
  return (
    <Box>
      {items.map((item, index) => {
        const isExpanded =
          focusMode && expandedItemIndex === index
            ? true
            : !focusMode && expandedItemIndex === index;

        return (
          <Box
            key={index}
            sx={{
              mb: 2,
              border: "1px solid #e5e7eb",
              borderRadius: 1,
              overflow: "hidden",
            }}
          >
            <Box
              onClick={() => onToggleItem(index)}
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                p: 2,
                cursor: "pointer",
                backgroundColor: isExpanded ? "#f3f4f6" : "transparent",
                "&:hover": { backgroundColor: "#f9fafb" },
              }}
            >
              <Typography
                variant="body1"
                sx={{
                  fontSize: "1rem",
                  fontWeight: 500,
                  color: "#374151",
                  flex: 1,
                }}
              >
                {item.title || `Item ${index + 1}`}
              </Typography>
              {isExpanded ? (
                <ExpandLessIcon sx={{ color: "#6b7280" }} />
              ) : (
                <ChevronRightIcon sx={{ color: "#6b7280" }} />
              )}
            </Box>
            {isExpanded && (
              <Collapse in={isExpanded}>
                <Box sx={{ p: 2, pt: 0 }}>
                  <Typography
                    variant="body2"
                    sx={{
                      color: "#6b7280",
                      fontSize: "0.875rem",
                    }}
                  >
                    {item.content || "Content for item"}
                  </Typography>
                </Box>
              </Collapse>
            )}
          </Box>
        );
      })}
      <Typography
        variant="body2"
        sx={{
          mt: 2,
          textAlign: "center",
          color: "#9ca3af",
          fontSize: "0.875rem",
        }}
      >
        {promptText}
      </Typography>
    </Box>
  );
};


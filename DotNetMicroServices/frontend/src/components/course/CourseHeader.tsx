import React from "react";
import { Box, Typography, Button, IconButton, Menu, MenuItem } from "@mui/material";
import {
  MoreVert as MoreVertIcon,
  Publish as PublishIcon,
  PersonAdd as PersonAddIcon,
} from "@mui/icons-material";

interface CourseHeaderProps {
  courseTitle: string;
  isPublishing: boolean;
  onPublish: () => void;
  onAssign: () => void;
  anchorEl: HTMLElement | null;
  onMenuOpen: (event: React.MouseEvent<HTMLElement>) => void;
  onMenuClose: () => void;
  isUserRole?: boolean;
}

export const CourseHeader: React.FC<CourseHeaderProps> = ({
  courseTitle,
  isPublishing,
  onPublish,
  onAssign,
  anchorEl,
  onMenuOpen,
  onMenuClose,
  isUserRole = false,
}) => {
  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        mb: 3,
        flexWrap: "wrap",
        gap: 2,
      }}
    >
      <Typography variant="h4" sx={{ fontWeight: 600, flex: 1, minWidth: 200 }}>
        {courseTitle || "Untitled Course"}
      </Typography>
      {!isUserRole && (
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          <Button
            variant="contained"
            startIcon={<PublishIcon />}
            onClick={onPublish}
            disabled={isPublishing}
            sx={{
              bgcolor: "#6366f1",
              "&:hover": { bgcolor: "#4f46e5" },
              textTransform: "none",
            }}
          >
            {isPublishing ? "Publishing..." : "Publish"}
          </Button>
          <Button
            variant="outlined"
            startIcon={<PersonAddIcon />}
            onClick={onAssign}
            sx={{
              borderColor: "#6366f1",
              color: "#6366f1",
              "&:hover": { borderColor: "#4f46e5", bgcolor: "#eef2ff" },
              textTransform: "none",
            }}
          >
            Assign
          </Button>
          <IconButton onClick={onMenuOpen} sx={{ color: "#6366f1" }}>
            <MoreVertIcon />
          </IconButton>
          <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={onMenuClose}>
            <MenuItem onClick={onMenuClose}>Settings</MenuItem>
            <MenuItem onClick={onMenuClose}>Export</MenuItem>
          </Menu>
        </Box>
      )}
    </Box>
  );
};


"use client";

import React from "react";
import {
  Paper,
  Box,
  Typography,
  TextField,
  IconButton,
  CircularProgress,
} from "@mui/material";
import { Close as CloseIcon } from "@mui/icons-material";

interface DiscussionEditorProps {
  description: string;
  onDescriptionChange: (description: string) => void;
  isUpdating?: boolean;
  mobilePropertiesOpen: boolean;
  onCloseMobile: () => void;
}

export const DiscussionEditor: React.FC<DiscussionEditorProps> = ({
  description,
  onDescriptionChange,
  isUpdating = false,
  mobilePropertiesOpen,
  onCloseMobile,
}) => {
  return (
    <Paper
      sx={{
        width: { xs: "100%", md: 350, lg: 350 },
        minWidth: { xs: "100%", md: 350, lg: 350 },
        maxWidth: { xs: "100%", md: 350, lg: 350 },
        p: { xs: 1.5, md: 2 },
        overflowY: "auto",
        overflowX: "hidden",
        maxHeight: { xs: "85vh", lg: "100%" },
        flexShrink: 0,
        display: { xs: mobilePropertiesOpen ? "block" : "none", lg: "block" },
        position: { xs: "fixed", lg: "relative" },
        right: { xs: 0, lg: "auto" },
        top: { xs: 60, lg: "auto" },
        zIndex: { xs: 1200, lg: "auto" },
        height: { xs: "calc(100vh - 60px)", lg: "100%" },
        boxShadow: { xs: "0 4px 12px rgba(0,0,0,0.15)", lg: "none" },
        backgroundColor: { xs: "white", lg: "inherit" },
      }}
    >
      {/* Mobile Close Button */}
      <Box
        sx={{
          display: { xs: "flex", lg: "none" },
          justifyContent: "flex-end",
          mb: 1,
          pb: 1,
          borderBottom: "1px solid #e5e7eb",
        }}
      >
        <IconButton
          onClick={onCloseMobile}
          size="small"
          sx={{
            color: "#6b7280",
            "&:hover": { bgcolor: "#f3f4f6" },
          }}
        >
          <CloseIcon fontSize="small" />
        </IconButton>
      </Box>

      {/* Editor Header */}
      <Box sx={{ mb: 2 }}>
        <Typography
          variant="h6"
          sx={{
            fontSize: "1rem",
            fontWeight: 600,
            color: "#111827",
            mb: 0.5,
          }}
        >
          Discussion Content
        </Typography>
        <Typography
          variant="body2"
          sx={{
            fontSize: "0.75rem",
            color: "#6b7280",
          }}
        >
          Edit the discussion description
        </Typography>
      </Box>

      {/* Description Editor */}
      <Box sx={{ mb: 2 }}>
        <TextField
          fullWidth
          multiline
          rows={12}
          value={description}
          onChange={(e) => onDescriptionChange(e.target.value)}
          placeholder="Enter discussion description..."
          variant="outlined"
          sx={{
            "& .MuiOutlinedInput-root": {
              backgroundColor: "#ffffff",
              "&:hover fieldset": {
                borderColor: "#6366f1",
              },
              "&.Mui-focused fieldset": {
                borderColor: "#6366f1",
              },
            },
            "& .MuiInputBase-input": {
              fontSize: "0.875rem",
              lineHeight: 1.6,
            },
          }}
          disabled={isUpdating}
        />
      </Box>

      {/* Auto-save indicator */}
      {isUpdating && (
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1,
            mt: 2,
            p: 1,
            bgcolor: "#f3f4f6",
            borderRadius: 1,
          }}
        >
          <CircularProgress size={16} />
          <Typography variant="caption" color="text.secondary">
            Saving...
          </Typography>
        </Box>
      )}

      {!isUpdating && description && (
        <Box
          sx={{
            mt: 2,
            p: 1,
            bgcolor: "#f0fdf4",
            borderRadius: 1,
            border: "1px solid #86efac",
          }}
        >
          <Typography variant="caption" color="success.main">
            Changes will auto-save in a few seconds
          </Typography>
        </Box>
      )}
    </Paper>
  );
};

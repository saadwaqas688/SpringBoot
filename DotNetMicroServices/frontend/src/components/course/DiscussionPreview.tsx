"use client";

import React from "react";
import { Paper, Box, Typography, CircularProgress } from "@mui/material";
import { Message as MessageIcon } from "@mui/icons-material";

interface DiscussionPreviewProps {
  description: string;
  isLoading?: boolean;
  error?: any;
}

export const DiscussionPreview: React.FC<DiscussionPreviewProps> = ({
  description,
  isLoading = false,
  error,
}) => {
  if (isLoading) {
    return (
      <Paper
        sx={{
          flex: 1,
          p: { xs: 2, sm: 3, md: 4 },
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
          backgroundColor: "#ffffff",
          borderRadius: 2,
          boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
          minHeight: { xs: 400, md: "auto" },
        }}
      >
        <CircularProgress sx={{ mb: 2 }} />
        <Typography variant="body2" color="text.secondary">
          Loading discussion...
        </Typography>
      </Paper>
    );
  }

  if (error) {
    return (
      <Paper
        sx={{
          flex: 1,
          p: { xs: 2, sm: 3, md: 4 },
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
          backgroundColor: "#ffffff",
          borderRadius: 2,
          boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
          minHeight: { xs: 400, md: "auto" },
        }}
      >
        <Typography variant="h6" color="error" sx={{ mb: 1 }}>
          Error loading discussion
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {error?.message || "An error occurred while loading the discussion"}
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper
      sx={{
        flex: 1,
        p: { xs: 2, sm: 3, md: 4 },
        display: "flex",
        flexDirection: "column",
        backgroundColor: "#ffffff",
        borderRadius: 2,
        boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
        minHeight: { xs: 400, md: "auto" },
      }}
    >
      {/* Header */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 1.5,
          mb: 3,
          pb: 2,
          borderBottom: "2px solid #e5e7eb",
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            width: 48,
            height: 48,
            borderRadius: "50%",
            bgcolor: "#eef2ff",
            color: "#6366f1",
          }}
        >
          <MessageIcon sx={{ fontSize: 24 }} />
        </Box>
        <Box>
          <Typography
            variant="h5"
            sx={{
              fontWeight: 600,
              color: "#111827",
              fontSize: { xs: "1.25rem", sm: "1.5rem" },
            }}
          >
            Discussion
          </Typography>
          <Typography
            variant="body2"
            sx={{
              color: "#6b7280",
              fontSize: "0.875rem",
            }}
          >
            Topic for discussion
          </Typography>
        </Box>
      </Box>

      {/* Content */}
      <Box
        sx={{
          flex: 1,
          overflowY: "auto",
          "&::-webkit-scrollbar": {
            width: "8px",
          },
          "&::-webkit-scrollbar-track": {
            background: "#f3f4f6",
            borderRadius: "4px",
          },
          "&::-webkit-scrollbar-thumb": {
            background: "#d1d5db",
            borderRadius: "4px",
            "&:hover": {
              background: "#9ca3af",
            },
          },
        }}
      >
        {description ? (
          <Typography
            variant="body1"
            sx={{
              color: "#374151",
              fontSize: { xs: "0.9375rem", sm: "1rem" },
              lineHeight: 1.7,
              whiteSpace: "pre-wrap",
              wordBreak: "break-word",
            }}
          >
            {description}
          </Typography>
        ) : (
          <Box
            sx={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              justifyContent: "center",
              minHeight: 200,
              textAlign: "center",
              p: 3,
            }}
          >
            <MessageIcon
              sx={{
                fontSize: 48,
                color: "#d1d5db",
                mb: 2,
              }}
            />
            <Typography
              variant="h6"
              sx={{
                color: "#9ca3af",
                mb: 1,
                fontSize: "1rem",
              }}
            >
              No description yet
            </Typography>
            <Typography
              variant="body2"
              sx={{
                color: "#9ca3af",
                fontSize: "0.875rem",
              }}
            >
              Start typing in the editor to add discussion content
            </Typography>
          </Box>
        )}
      </Box>
    </Paper>
  );
};

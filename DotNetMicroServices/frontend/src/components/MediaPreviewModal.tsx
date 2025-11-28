"use client";

import { useEffect, useState } from "react";
import {
  Dialog,
  DialogContent,
  IconButton,
  Box,
  Typography,
} from "@mui/material";
import { Close as CloseIcon } from "@mui/icons-material";

interface MediaPreviewModalProps {
  open: boolean;
  onClose: () => void;
  url: string;
  type: "image" | "video";
  alt?: string;
}

export default function MediaPreviewModal({
  open,
  onClose,
  url,
  type,
  alt = "Preview",
}: MediaPreviewModalProps) {
  const [hasError, setHasError] = useState(false);

  // Handle Escape key to close modal
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape" && open) {
        onClose();
      }
    };
    window.addEventListener("keydown", handleEscape);
    return () => window.removeEventListener("keydown", handleEscape);
  }, [open, onClose]);

  // Reset error state when URL changes
  useEffect(() => {
    setHasError(false);
  }, [url]);

  // Don't render if no URL or modal is closed
  if (!open || !url) return null;

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth={false}
      fullWidth
      PaperProps={{
        sx: {
          maxWidth: "90vw",
          maxHeight: "90vh",
          margin: 0,
          backgroundColor: "rgba(0, 0, 0, 0.9)",
          boxShadow: "none",
          position: "relative",
        },
      }}
      sx={{
        "& .MuiBackdrop-root": {
          backgroundColor: "rgba(0, 0, 0, 0.8)",
        },
      }}
    >
      <DialogContent
        sx={{
          p: 0,
          position: "relative",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "50vh",
          backgroundColor: "transparent",
        }}
      >
        <IconButton
          onClick={onClose}
          aria-label="Close preview"
          sx={{
            position: "absolute",
            top: 8,
            right: 8,
            backgroundColor: "rgba(0, 0, 0, 0.5)",
            color: "white",
            zIndex: 1,
            "&:hover": {
              backgroundColor: "rgba(0, 0, 0, 0.7)",
            },
          }}
        >
          <CloseIcon />
        </IconButton>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            width: "100%",
            height: "100%",
            position: "relative",
            p: 2,
          }}
        >
          {hasError ? (
            <Typography
              variant="h6"
              sx={{
                color: "white",
                textAlign: "center",
                p: 3,
              }}
            >
              Failed to load {type}. Please check the URL.
            </Typography>
          ) : type === "image" ? (
            <Box
              component="img"
              src={url}
              alt={alt}
              onClick={(e) => e.stopPropagation()}
              onError={() => {
                console.error("Image failed to load:", url);
                setHasError(true);
              }}
              onLoad={() => {
                console.log("Image loaded successfully:", url);
                setHasError(false);
              }}
              sx={{
                maxWidth: "100%",
                maxHeight: "85vh",
                width: "auto",
                height: "auto",
                objectFit: "contain",
                borderRadius: 1,
                cursor: "pointer",
                display: "block",
              }}
            />
          ) : (
            <Box
              component="video"
              src={url}
              controls
              autoPlay
              muted={false}
              onClick={(e) => e.stopPropagation()}
              onError={() => {
                console.error("Video failed to load:", url);
                setHasError(true);
              }}
              onLoadedData={() => {
                console.log("Video loaded successfully:", url);
                setHasError(false);
              }}
              sx={{
                maxWidth: "100%",
                maxHeight: "85vh",
                width: "auto",
                height: "auto",
                borderRadius: 1,
                outline: "none",
                cursor: "pointer",
                display: "block",
              }}
            >
              Your browser does not support the video tag.
            </Box>
          )}
        </Box>
      </DialogContent>
    </Dialog>
  );
}

"use client";

import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Typography,
  IconButton,
  CircularProgress,
} from "@mui/material";
import { Delete as DeleteIcon } from "@mui/icons-material";
import styled from "styled-components";

const IconContainer = styled(Box)`
  display: flex;
  justify-content: center;
  margin-bottom: 1.5rem;
`;

interface DeleteConfirmationModalProps {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title?: string;
  message?: string;
  isLoading?: boolean;
}

export default function DeleteConfirmationModal({
  open,
  onClose,
  onConfirm,
  title = "Are you sure you want to delete?",
  message = "This will delete all course progress and data. This course will also be removed from all analytics. This action is irreversible.",
  isLoading = false,
}: DeleteConfirmationModalProps) {
  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="sm"
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: 2,
        },
      }}
    >
      <DialogContent sx={{ pt: 4, pb: 2 }}>
        <IconContainer>
          <DeleteIcon sx={{ fontSize: 64, color: "#ef4444" }} />
        </IconContainer>
        <Typography
          variant="h6"
          component="h2"
          sx={{
            textAlign: "center",
            fontWeight: 600,
            color: "#1e293b",
            mb: 2,
          }}
        >
          {title}
        </Typography>
        <Typography
          variant="body2"
          sx={{
            textAlign: "center",
            color: "#64748b",
            lineHeight: 1.6,
          }}
        >
          {message}
        </Typography>
      </DialogContent>

      <DialogActions sx={{ px: 3, pb: 3, gap: 2, justifyContent: "center" }}>
        <Button
          onClick={onClose}
          disabled={isLoading}
          variant="outlined"
          sx={{
            textTransform: "none",
            borderColor: "#6366f1",
            color: "#6366f1",
            minWidth: 120,
            "&:hover": {
              borderColor: "#4f46e5",
              backgroundColor: "rgba(99, 102, 241, 0.04)",
            },
          }}
        >
          Cancel
        </Button>
        <Button
          onClick={onConfirm}
          disabled={isLoading}
          variant="contained"
          sx={{
            textTransform: "none",
            bgcolor: "#6366f1",
            minWidth: 120,
            "&:hover": {
              bgcolor: "#4f46e5",
            },
          }}
        >
          {isLoading ? (
            <CircularProgress size={24} color="inherit" />
          ) : (
            "Yes, Sure!"
          )}
        </Button>
      </DialogActions>
    </Dialog>
  );
}


"use client";

import { useState, useRef, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Typography,
  IconButton,
  TextField,
  Paper,
  CircularProgress,
  Alert,
} from "@mui/material";
import {
  Close as CloseIcon,
  CloudUpload as CloudUploadIcon,
  Download as DownloadIcon,
  Description as DescriptionIcon,
} from "@mui/icons-material";
import { useUploadQuizFileMutation } from "@/services/upload-api";
import styled from "styled-components";

const UploadArea = styled(Box)<{ hasFile: boolean }>`
  border: 2px dashed ${(props) => (props.hasFile ? "#6366f1" : "#d1d5db")};
  border-radius: 8px;
  padding: 3rem 2rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: ${(props) => (props.hasFile ? "#f3f4f6" : "#ffffff")};

  &:hover {
    border-color: #6366f1;
    background-color: #f9fafb;
  }
`;

const NoteBox = styled(Paper)`
  padding: 12px;
  background-color: #f3f4f6;
  border-radius: 8px;
  margin-top: 12px;
`;

interface QuizUploadModalProps {
  open: boolean;
  onClose: () => void;
  lessonId?: string;
  onUpload?: (file: File, quizScore: number) => void;
}

export default function QuizUploadModal({
  open,
  onClose,
  lessonId,
  onUpload,
}: QuizUploadModalProps) {
  console.log("=== QuizUploadModal RENDERED ===");
  console.log("open prop:", open);
  console.log("lessonId prop:", lessonId);
  console.log("onClose prop:", onClose);
  
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [quizScore, setQuizScore] = useState<string>("");
  const [error, setError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [uploadQuizFile, { isLoading: isUploading }] = useUploadQuizFileMutation();

  // Log when open prop changes
  useEffect(() => {
    console.log("=== QuizUploadModal useEffect - open prop changed ===");
    console.log("New open value:", open);
    console.log("Component will show modal:", open === true);
  }, [open]);
  
  // Log on every render
  useEffect(() => {
    console.log("=== QuizUploadModal useEffect - component rendered ===");
    console.log("Current open prop:", open);
  });

  const handleDownloadTemplate = () => {
    // Create a simple Excel template structure
    const templateData = [
      ["Question", "Option A", "Option B", "Option C", "Option D", "Correct Answer"],
      ["Sample Question 1", "Option A", "Option B", "Option C", "Option D", "A"],
      ["Sample Question 2", "Option A", "Option B", "Option C", "Option D", "B"],
    ];

    // Convert to CSV format (simpler than Excel)
    const csvContent = templateData.map((row) => row.join(",")).join("\n");
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", "quiz_template.csv");
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleFileSelect = (file: File) => {
    // Validate file type
    const validTypes = [
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
      "application/vnd.ms-excel", // .xls
      "application/vnd.ms-excel.sheet.macroEnabled.12", // .xlsm
      "text/csv", // .csv
      "application/csv", // .csv (alternative MIME type)
    ];
    
    const fileName = file.name.toLowerCase();
    const isValidType =
      validTypes.includes(file.type) ||
      fileName.endsWith(".xlsx") ||
      fileName.endsWith(".xls") ||
      fileName.endsWith(".xlsm") ||
      fileName.endsWith(".csv");

    if (!isValidType) {
      alert("Please upload only .xlsx, .xls, .xlsm, or .csv files");
      return;
    }

    // Validate file size (50MB max)
    const maxSize = 50 * 1024 * 1024; // 50MB in bytes
    if (file.size > maxSize) {
      alert("File size must be less than 50MB");
      return;
    }

    setSelectedFile(file);
  };

  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      handleFileSelect(file);
    }
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    const file = e.dataTransfer.files[0];
    if (file) {
      handleFileSelect(file);
    }
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
  };

  const handleUploadClick = () => {
    fileInputRef.current?.click();
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      setError("Please select a file to upload");
      return;
    }

    if (!lessonId) {
      setError("Lesson ID is required");
      return;
    }

    const score = parseInt(quizScore);
    if (isNaN(score) || score <= 0) {
      setError("Please enter a valid quiz score");
      return;
    }

    setError(null);

    try {
      const result = await uploadQuizFile({
        lessonId,
        file: selectedFile,
        quizScore: score,
      }).unwrap();

      // Call the optional onUpload callback
      if (onUpload) {
        onUpload(selectedFile, score);
      }

      // Reset form
      setSelectedFile(null);
      setQuizScore("");
      setError(null);
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || "Failed to upload quiz file. Please try again.");
    }
  };

  const handleClose = () => {
    setSelectedFile(null);
    setQuizScore("");
    setError(null);
    onClose();
  };

  console.log("=== QuizUploadModal RETURN (rendering Dialog) ===");
  console.log("Dialog open prop:", open);
  console.log("Dialog will render:", open === true);
  
  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          pb: 2,
          borderBottom: "1px solid #e5e7eb",
        }}
      >
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          Upload your questions to create a quiz
        </Typography>
        <IconButton onClick={handleClose} size="small">
          <CloseIcon />
        </IconButton>
      </DialogTitle>

      <DialogContent sx={{ pt: 3 }}>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
            {error}
          </Alert>
        )}
        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
          {/* Step 1: Download */}
          <Box>
            <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
              Step 1: Download
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Create your question pool with ease using our template.
            </Typography>
            <Button
              variant="contained"
              startIcon={<DownloadIcon />}
              onClick={handleDownloadTemplate}
              sx={{
                bgcolor: "#6366f1",
                textTransform: "none",
                "&:hover": {
                  bgcolor: "#4f46e5",
                },
              }}
            >
              Download Template
            </Button>
          </Box>

          {/* Step 2: Upload */}
          <Box>
            <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
              Step 2: Upload
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Upload your edited file containing your questions.
            </Typography>
            <input
              type="file"
              ref={fileInputRef}
              onChange={handleFileInputChange}
              accept=".xlsx,.xls,.xlsm,.csv,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel,application/vnd.ms-excel.sheet.macroEnabled.12,text/csv,application/csv"
              style={{ display: "none" }}
            />
            <UploadArea
              hasFile={!!selectedFile}
              onClick={handleUploadClick}
              onDrop={handleDrop}
              onDragOver={handleDragOver}
            >
              {selectedFile ? (
                <Box>
                  <DescriptionIcon sx={{ fontSize: 48, color: "#6366f1", mb: 1 }} />
                  <Typography variant="body1" sx={{ fontWeight: 500, mb: 0.5 }}>
                    {selectedFile.name}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {(selectedFile.size / 1024 / 1024).toFixed(2)} MB
                  </Typography>
                  <Button
                    variant="outlined"
                    size="small"
                    onClick={(e) => {
                      e.stopPropagation();
                      handleUploadClick();
                    }}
                    sx={{ mt: 2, textTransform: "none" }}
                  >
                    Change File
                  </Button>
                </Box>
              ) : (
                <Box>
                  <CloudUploadIcon sx={{ fontSize: 64, color: "#6366f1", mb: 2 }} />
                  <Typography variant="body1" sx={{ fontWeight: 500 }}>
                    Upload File here
                  </Typography>
                </Box>
              )}
            </UploadArea>
            <NoteBox>
              <Typography variant="body2" color="text.secondary">
                Note: Only .xlsx, .xls, .xlsm, or .csv files accepted. Max 50MB.
              </Typography>
            </NoteBox>
          </Box>

          {/* Quiz Score */}
          <Box>
            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
              Quiz Score
            </Typography>
            <TextField
              fullWidth
              placeholder="Enter quiz score (e.g., 100)"
              value={quizScore}
              onChange={(e) => setQuizScore(e.target.value)}
              type="number"
              inputProps={{ min: 1 }}
              sx={{
                "& .MuiOutlinedInput-root": {
                  fontSize: "0.875rem",
                },
              }}
            />
          </Box>
        </Box>
      </DialogContent>

      <DialogActions sx={{ p: 2, borderTop: "1px solid #e5e7eb" }}>
        <Button onClick={handleClose} sx={{ textTransform: "none" }}>
          Cancel
        </Button>
        <Button
          onClick={handleUpload}
          variant="contained"
          disabled={!selectedFile || !quizScore || isUploading}
          sx={{
            bgcolor: "#6366f1",
            textTransform: "none",
            "&:hover": {
              bgcolor: "#4f46e5",
            },
          }}
        >
          {isUploading ? (
            <>
              <CircularProgress size={16} sx={{ mr: 1 }} color="inherit" />
              Uploading...
            </>
          ) : (
            "Upload file"
          )}
        </Button>
      </DialogActions>
    </Dialog>
  );
}


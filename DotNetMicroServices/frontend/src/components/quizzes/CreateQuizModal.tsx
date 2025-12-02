"use client";

import React, { useState, useRef } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  Typography,
  FormControlLabel,
  Switch,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  IconButton,
  LinearProgress,
} from "@mui/material";
import {
  Close as CloseIcon,
  UploadFile as UploadFileIcon,
  CheckCircle as CheckCircleIcon,
} from "@mui/icons-material";
import styled from "styled-components";
import { useUploadQuizFileMutation } from "@/services/upload-api";
import { useGetAllCoursesQuery } from "@/services/courses-api";

const StyledDialog = styled(Dialog)`
  & .MuiDialog-paper {
    max-width: 600px;
    width: 100%;
    border-radius: 12px;
  }
`;

const DialogHeader = styled(Box)`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid #e5e7eb;
`;

const DialogTitleText = styled(Typography)`
  font-size: 1.5rem;
  font-weight: 600;
  color: #6366f1;
`;

const UploadArea = styled(Box)<{ isDragOver?: boolean }>`
  border: 2px dashed ${(props) => (props.isDragOver ? "#6366f1" : "#d1d5db")};
  border-radius: 8px;
  padding: 48px 24px;
  text-align: center;
  background: ${(props) => (props.isDragOver ? "#f3f4f6" : "transparent")};
  cursor: pointer;
  transition: all 0.2s;

  &:hover {
    border-color: #6366f1;
    background: #f3f4f6;
  }
`;

const UploadIcon = styled(Box)`
  color: #8b5cf6;
  margin-bottom: 16px;
  display: flex;
  justify-content: center;
`;

const NoteBox = styled(Box)`
  background: #fef3c7;
  border-left: 4px solid #f59e0b;
  padding: 12px 16px;
  margin-top: 16px;
  border-radius: 4px;
`;

const FormField = styled(Box)`
  margin-bottom: 24px;
`;

const SwitchContainer = styled(Box)`
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
  padding: 12px 0;
`;

const SwitchLabel = styled(Box)`
  display: flex;
  flex-direction: column;
`;

const SwitchHelperText = styled(Typography)`
  font-size: 0.75rem;
  color: #6b7280;
  margin-top: 4px;
`;

interface CreateQuizModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

export default function CreateQuizModal({
  open,
  onClose,
  onSuccess,
}: CreateQuizModalProps) {
  const [file, setFile] = useState<File | null>(null);
  const [quizScore, setQuizScore] = useState("");
  const [universalAccess, setUniversalAccess] = useState(false);
  const [reattempt, setReattempt] = useState(false);
  const [addCourse, setAddCourse] = useState(true);
  const [selectedCourse, setSelectedCourse] = useState("");
  const [isDragOver, setIsDragOver] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [uploadQuizFile, { isLoading: isUploading }] =
    useUploadQuizFileMutation();

  // Fetch courses from API
  const { data: coursesResponse, isLoading: isLoadingCourses } =
    useGetAllCoursesQuery({
      page: 1,
      pageSize: 100,
    });

  // Transform courses data
  const coursesList =
    coursesResponse?.data?.items || coursesResponse?.data || [];
  const courses = coursesList.map((course: any) => ({
    id: course.id || course._id,
    name: course.title || course.name || "Untitled Course",
  }));

  const handleFileSelect = (selectedFile: File) => {
    // Validate file type
    const validTypes = [
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
      "application/vnd.ms-excel.sheet.macroEnabled.12", // .xlsm
      "application/vnd.ms-excel", // .xls
    ];

    if (!validTypes.includes(selectedFile.type)) {
      alert("Only .xlsx or .xlsm files are accepted");
      return;
    }

    // Validate file size (50MB)
    if (selectedFile.size > 50 * 1024 * 1024) {
      alert("File size should be less than 50MB");
      return;
    }

    setFile(selectedFile);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);

    const droppedFile = e.dataTransfer.files[0];
    if (droppedFile) {
      handleFileSelect(droppedFile);
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
  };

  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    if (selectedFile) {
      handleFileSelect(selectedFile);
    }
  };

  const handleSubmit = async () => {
    if (!file) {
      alert("Please upload a file");
      return;
    }

    if (!quizScore || isNaN(Number(quizScore))) {
      alert("Please enter a valid quiz score");
      return;
    }

    if (addCourse && !selectedCourse) {
      alert("Please select a course");
      return;
    }

    try {
      // TODO: Implement quiz creation API call
      // For now, use the upload quiz file mutation
      const lessonId = selectedCourse || ""; // Use course ID as lesson ID for now
      const score = Number(quizScore);

      const result = await uploadQuizFile({
        lessonId,
        file,
        quizScore: score,
      }).unwrap();

      console.log("Quiz created successfully:", result);

      // Reset form
      handleClose();

      if (onSuccess) {
        onSuccess();
      }
    } catch (error: any) {
      console.error("Failed to create quiz:", error);
      alert(error?.data?.message || error?.message || "Failed to create quiz");
    }
  };

  const handleClose = () => {
    setFile(null);
    setQuizScore("");
    setUniversalAccess(false);
    setReattempt(false);
    setAddCourse(true);
    setSelectedCourse("");
    setIsDragOver(false);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
    onClose();
  };

  return (
    <StyledDialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogHeader>
        <DialogTitleText>Create Quiz</DialogTitleText>
        <IconButton onClick={handleClose} size="small">
          <CloseIcon />
        </IconButton>
      </DialogHeader>

      <DialogContent sx={{ padding: "24px" }}>
        {/* File Upload Section */}
        <FormField>
          <input
            ref={fileInputRef}
            type="file"
            accept=".xlsx,.xlsm,.xls"
            style={{ display: "none" }}
            onChange={handleFileInputChange}
          />
          <UploadArea
            isDragOver={isDragOver}
            onDrop={handleDrop}
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            onClick={() => fileInputRef.current?.click()}
          >
            {file ? (
              <>
                <CheckCircleIcon
                  sx={{ fontSize: 48, color: "#10b981", mb: 2 }}
                />
                <Typography variant="body1" fontWeight={600}>
                  {file.name}
                </Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ mt: 1 }}
                >
                  Click to change file
                </Typography>
              </>
            ) : (
              <>
                <UploadIcon>
                  <UploadFileIcon sx={{ fontSize: 64 }} />
                </UploadIcon>
                <Typography variant="body1" fontWeight={600}>
                  Upload File here
                </Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ mt: 1 }}
                >
                  Drag and drop or click to browse
                </Typography>
              </>
            )}
          </UploadArea>
          <NoteBox>
            <Typography variant="caption">
              <strong>Note:</strong> Only .xlsx or .xlsm files accepted. Max
              50MB.
            </Typography>
          </NoteBox>
        </FormField>

        {/* Quiz Score */}
        <FormField>
          <TextField
            label="Quiz Score"
            type="number"
            fullWidth
            value={quizScore}
            onChange={(e) => setQuizScore(e.target.value)}
            inputProps={{ min: 0, step: 1 }}
          />
        </FormField>

        {/* Universal Access Toggle */}
        <SwitchContainer>
          <SwitchLabel>
            <Typography variant="body1" fontWeight={500}>
              Universal Access
            </Typography>
          </SwitchLabel>
          <Switch
            checked={universalAccess}
            onChange={(e) => setUniversalAccess(e.target.checked)}
            color="primary"
          />
        </SwitchContainer>

        {/* Re-attempt Toggle */}
        <SwitchContainer>
          <SwitchLabel>
            <Typography variant="body1" fontWeight={500}>
              Re-attempt
            </Typography>
            <SwitchHelperText>
              Allow re-attempt up to 5 days of submission in case quiz is failed
            </SwitchHelperText>
          </SwitchLabel>
          <Switch
            checked={reattempt}
            onChange={(e) => setReattempt(e.target.checked)}
            color="primary"
          />
        </SwitchContainer>

        {/* Add Course Toggle */}
        <SwitchContainer>
          <SwitchLabel>
            <Typography variant="body1" fontWeight={500}>
              Add Course
            </Typography>
          </SwitchLabel>
          <Switch
            checked={addCourse}
            onChange={(e) => setAddCourse(e.target.checked)}
            color="primary"
          />
        </SwitchContainer>

        {/* Course Selection */}
        {addCourse && (
          <FormField>
            <FormControl fullWidth>
              <InputLabel>Select Course</InputLabel>
              <Select
                value={selectedCourse}
                onChange={(e) => setSelectedCourse(e.target.value)}
                label="Select Course"
              >
                {courses.map((course) => (
                  <MenuItem key={course.id} value={course.id}>
                    {course.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </FormField>
        )}

        {isUploading && (
          <Box sx={{ mt: 2 }}>
            <LinearProgress />
            <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
              Uploading and processing quiz...
            </Typography>
          </Box>
        )}
      </DialogContent>

      <DialogActions
        sx={{ padding: "16px 24px", borderTop: "1px solid #e5e7eb" }}
      >
        <Button onClick={handleClose} disabled={isUploading}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={isUploading || !file || !quizScore}
          sx={{
            background: "linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)",
            "&:hover": {
              background: "linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%)",
            },
          }}
        >
          {isUploading ? "Creating..." : "Create Quiz"}
        </Button>
      </DialogActions>
    </StyledDialog>
  );
}

"use client";

import React, { useState } from "react";
import {
  Paper,
  Box,
  Typography,
  TextField,
  Button,
  IconButton,
  Divider,
  Checkbox,
  FormControlLabel,
  CircularProgress,
  Alert,
} from "@mui/material";
import {
  Delete as DeleteIcon,
  Save as SaveIcon,
  Edit as EditIcon,
  Add as AddIcon,
  Close as CloseIcon,
} from "@mui/icons-material";
import {
  useUpdateQuizQuestionMutation,
  useDeleteQuizQuestionMutation,
  useCreateQuizQuestionMutation,
} from "@/services/quizzes-api";

interface QuizEditorProps {
  quizQuestions: any[];
  quizId: string;
  currentQuestionIndex: number;
  onQuestionUpdated?: () => void;
  onQuestionDeleted?: () => void;
  onQuestionNavigation?: (direction: "prev" | "next") => void;
  mobilePropertiesOpen: boolean;
  onCloseMobile: () => void;
}

export const QuizEditor: React.FC<QuizEditorProps> = ({
  quizQuestions,
  quizId,
  currentQuestionIndex,
  onQuestionUpdated,
  onQuestionDeleted,
  onQuestionNavigation,
  mobilePropertiesOpen,
  onCloseMobile,
}) => {
  const [editingQuestionId, setEditingQuestionId] = useState<string | null>(null);
  const [editedQuestions, setEditedQuestions] = useState<{ [key: string]: any }>({});
  const [updateQuestion, { isLoading: isUpdating }] = useUpdateQuizQuestionMutation();
  const [deleteQuestion, { isLoading: isDeleting }] = useDeleteQuizQuestionMutation();
  const [createQuestion, { isLoading: isCreating }] = useCreateQuizQuestionMutation();
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleEditClick = (question: any) => {
    const questionId = question.id || question.Id || question._id;
    setEditingQuestionId(questionId);
    setEditedQuestions({
      ...editedQuestions,
      [questionId]: {
        question: question.question || question.Question || "",
        options: (question.options || question.Options || []).map((opt: any) => ({
          value: opt.value || opt.Value || "",
          isCorrect: opt.isCorrect || opt.IsCorrect || false,
        })),
        order: question.order || question.Order || 0,
      },
    });
    setError(null);
    setSuccess(null);
  };

  const handleCancelEdit = () => {
    setEditingQuestionId(null);
    setError(null);
    setSuccess(null);
  };

  const handleQuestionChange = (questionId: string, field: string, value: any) => {
    setEditedQuestions({
      ...editedQuestions,
      [questionId]: {
        ...editedQuestions[questionId],
        [field]: value,
      },
    });
  };

  const handleOptionChange = (questionId: string, optionIndex: number, field: string, value: any) => {
    const updatedOptions = [...(editedQuestions[questionId]?.options || [])];
    updatedOptions[optionIndex] = {
      ...updatedOptions[optionIndex],
      [field]: value,
    };
    handleQuestionChange(questionId, "options", updatedOptions);
  };

  const handleAddOption = (questionId: string) => {
    const updatedOptions = [...(editedQuestions[questionId]?.options || []), { value: "", isCorrect: false }];
    handleQuestionChange(questionId, "options", updatedOptions);
  };

  const handleRemoveOption = (questionId: string, optionIndex: number) => {
    const updatedOptions = (editedQuestions[questionId]?.options || []).filter(
      (_: any, index: number) => index !== optionIndex
    );
    handleQuestionChange(questionId, "options", updatedOptions);
  };

  const handleSaveQuestion = async (questionId: string) => {
    try {
      setError(null);
      setSuccess(null);

      const editedQuestion = editedQuestions[questionId];
      if (!editedQuestion) {
        setError("No changes to save");
        return;
      }

      // Validate question
      if (!editedQuestion.question || editedQuestion.question.trim() === "") {
        setError("Question text is required");
        return;
      }

      // Validate options
      if (!editedQuestion.options || editedQuestion.options.length < 2) {
        setError("At least 2 options are required");
        return;
      }

      // Check if at least one option is correct
      const hasCorrectAnswer = editedQuestion.options.some((opt: any) => opt.isCorrect);
      if (!hasCorrectAnswer) {
        setError("At least one option must be marked as correct");
        return;
      }

      // Validate option values
      for (let i = 0; i < editedQuestion.options.length; i++) {
        if (!editedQuestion.options[i].value || editedQuestion.options[i].value.trim() === "") {
          setError(`Option ${i + 1} text is required`);
          return;
        }
      }

      const updateData = {
        id: questionId,
        quizId: quizId,
        question: editedQuestion.question,
        options: editedQuestion.options.map((opt: any) => ({
          value: opt.value,
          isCorrect: opt.isCorrect,
        })),
        order: editedQuestion.order,
        type: "quiz",
      };

      await updateQuestion(updateData).unwrap();
      setEditingQuestionId(null);
      setSuccess("Question updated successfully");
      if (onQuestionUpdated) {
        onQuestionUpdated();
      }
      setTimeout(() => setSuccess(null), 3000);
    } catch (err: any) {
      console.error("Error updating question:", err);
      setError(err?.data?.message || err?.message || "Failed to update question");
    }
  };

  const handleDeleteQuestion = async (questionId: string) => {
    if (!confirm("Are you sure you want to delete this question? This action cannot be undone.")) {
      return;
    }

    try {
      setError(null);
      setSuccess(null);
      await deleteQuestion(questionId).unwrap();
      setSuccess("Question deleted successfully");
      if (onQuestionDeleted) {
        onQuestionDeleted();
      }
      setTimeout(() => setSuccess(null), 3000);
    } catch (err: any) {
      console.error("Error deleting question:", err);
      setError(err?.data?.message || err?.message || "Failed to delete question");
    }
  };

  const handleAddNewQuestion = async () => {
    try {
      setError(null);
      setSuccess(null);

      // Get the highest order number
      const maxOrder = sortedQuestions.length > 0
        ? Math.max(...sortedQuestions.map((q: any) => q.order || q.Order || 0))
        : 0;

      const newQuestion = {
        quizId: quizId,
        question: "",
        options: [
          { value: "", isCorrect: false },
          { value: "", isCorrect: false },
        ],
        order: maxOrder + 1,
        type: "quiz",
      };

      const created = await createQuestion(newQuestion).unwrap();
      setSuccess("New question added successfully");
      
      // Start editing the new question immediately
      const newQuestionId = created?.data?.id || created?.data?.Id || created?.id || created?.Id;
      if (newQuestionId) {
        setEditingQuestionId(newQuestionId);
        setEditedQuestions({
          ...editedQuestions,
          [newQuestionId]: {
            question: "",
            options: [
              { value: "", isCorrect: false },
              { value: "", isCorrect: false },
            ],
            order: maxOrder + 1,
          },
        });
      }

      if (onQuestionUpdated) {
        onQuestionUpdated();
      }
      
      // The question list will be refreshed via onQuestionUpdated callback
      // The new question will be added at the end with the highest order number
      
      setTimeout(() => setSuccess(null), 3000);
    } catch (err: any) {
      console.error("Error creating question:", err);
      setError(err?.data?.message || err?.message || "Failed to create question");
    }
  };

  const sortedQuestions = [...quizQuestions].sort((a: any, b: any) => 
    (a.order || a.Order || 0) - (b.order || b.Order || 0)
  );

  // Get the current question based on index
  const currentQuestion = sortedQuestions[currentQuestionIndex] || null;
  const totalQuestions = sortedQuestions.length;
  const canGoPrevious = currentQuestionIndex > 0;
  const canGoNext = currentQuestionIndex < totalQuestions - 1;

  if (!currentQuestion) {
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
        <Box sx={{ textAlign: "center", py: 4 }}>
          <Typography variant="body2" color="text.secondary">
            No questions available. Upload a quiz to get started.
          </Typography>
        </Box>
      </Paper>
    );
  }

  const questionId = currentQuestion.id || currentQuestion.Id || currentQuestion._id;
  const isEditing = editingQuestionId === questionId;
  const editedQuestion = editedQuestions[questionId];
  const question = isEditing && editedQuestion ? editedQuestion : currentQuestion;
  const options = question.options || question.Options || [];

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
        <IconButton onClick={onCloseMobile} size="small">
          <CloseIcon />
        </IconButton>
      </Box>

      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          Question Editor
        </Typography>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <Typography variant="body2" color="text.secondary">
            {currentQuestionIndex + 1}/{totalQuestions}
          </Typography>
          <IconButton
            size="small"
            onClick={handleAddNewQuestion}
            disabled={isCreating || isUpdating || isDeleting || isEditing}
            color="primary"
            title="Add new question"
            sx={{
              bgcolor: "#6366f1",
              color: "white",
              "&:hover": {
                bgcolor: "#4f46e5",
              },
              "&:disabled": {
                bgcolor: "rgba(99, 102, 241, 0.3)",
                color: "rgba(255, 255, 255, 0.5)",
              },
            }}
          >
            <AddIcon fontSize="small" />
          </IconButton>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      {success && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>
          {success}
        </Alert>
      )}

      <Box
        sx={{
          p: 2,
          border: "1px solid #e5e7eb",
          borderRadius: 2,
          bgcolor: isEditing ? "#f9fafb" : "white",
        }}
      >
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", mb: 2 }}>
          <Typography variant="subtitle2" sx={{ fontWeight: 600, color: "#6366f1" }}>
            Question {currentQuestionIndex + 1}
          </Typography>
          <Box sx={{ display: "flex", gap: 0.5 }}>
            {isEditing ? (
              <>
                <IconButton
                  size="small"
                  onClick={() => handleSaveQuestion(questionId)}
                  disabled={isUpdating || isDeleting}
                  color="primary"
                  title="Save changes"
                >
                  <SaveIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={handleCancelEdit}
                  disabled={isUpdating || isDeleting}
                  title="Cancel editing"
                >
                  <CloseIcon fontSize="small" />
                </IconButton>
              </>
            ) : (
              <>
                <IconButton
                  size="small"
                  onClick={() => handleEditClick(currentQuestion)}
                  disabled={isUpdating || isDeleting}
                  color="primary"
                  title="Edit question"
                >
                  <EditIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => handleDeleteQuestion(questionId)}
                  disabled={isUpdating || isDeleting}
                  color="error"
                  title="Delete question"
                >
                  <DeleteIcon fontSize="small" />
                </IconButton>
              </>
            )}
          </Box>
        </Box>

        <TextField
          fullWidth
          label="Question"
          value={isEditing ? editedQuestion?.question || "" : question.question || question.Question || ""}
          onChange={(e) => handleQuestionChange(questionId, "question", e.target.value)}
          disabled={!isEditing}
          multiline
          rows={3}
          sx={{ mb: 2 }}
        />

        <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
          Options:
        </Typography>

        <Box sx={{ display: "flex", flexDirection: "column", gap: 1, mb: 2 }}>
          {options.map((option: any, optIndex: number) => (
            <Box
              key={optIndex}
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 1,
                p: 1,
                border: "1px solid #e5e7eb",
                borderRadius: 1,
                bgcolor: option.isCorrect || option.IsCorrect ? "#dcfce7" : "white",
              }}
            >
              <FormControlLabel
                control={
                  <Checkbox
                    checked={option.isCorrect || option.IsCorrect || false}
                    onChange={(e) =>
                      handleOptionChange(questionId, optIndex, "isCorrect", e.target.checked)
                    }
                    disabled={!isEditing}
                    size="small"
                  />
                }
                label="Correct"
                sx={{ mr: 0, minWidth: 80 }}
              />
              <TextField
                fullWidth
                size="small"
                value={option.value || option.Value || ""}
                onChange={(e) => handleOptionChange(questionId, optIndex, "value", e.target.value)}
                disabled={!isEditing}
                placeholder={`Option ${optIndex + 1}`}
              />
              {isEditing && (
                <IconButton
                  size="small"
                  onClick={() => handleRemoveOption(questionId, optIndex)}
                  disabled={options.length <= 2}
                  color="error"
                  title="Remove option"
                >
                  <DeleteIcon fontSize="small" />
                </IconButton>
              )}
            </Box>
          ))}
        </Box>

        {isEditing && (
          <Button
            startIcon={<AddIcon />}
            size="small"
            onClick={() => handleAddOption(questionId)}
            sx={{ mt: 1, textTransform: "none" }}
            fullWidth
          >
            Add Option
          </Button>
        )}

        {(isUpdating || isDeleting || isCreating) && (
          <Box sx={{ display: "flex", justifyContent: "center", mt: 1 }}>
            <CircularProgress size={20} />
          </Box>
        )}
      </Box>

      {/* Navigation */}
      {totalQuestions > 1 && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mt: 2,
            pt: 2,
            borderTop: "1px solid #e5e7eb",
          }}
        >
          <Button
            variant="outlined"
            size="small"
            onClick={() => onQuestionNavigation?.("prev")}
            disabled={!canGoPrevious || isEditing || isCreating}
            sx={{ textTransform: "none" }}
          >
            Previous
          </Button>
          <Typography variant="body2" color="text.secondary">
            {currentQuestionIndex + 1} of {totalQuestions}
          </Typography>
          <Button
            variant="outlined"
            size="small"
            onClick={() => onQuestionNavigation?.("next")}
            disabled={!canGoNext || isEditing || isCreating}
            sx={{ textTransform: "none" }}
          >
            Next
          </Button>
        </Box>
      )}
    </Paper>
  );
};


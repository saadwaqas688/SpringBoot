import React from "react";
import { Box, Typography, IconButton } from "@mui/material";
import {
  ArrowBack as ArrowBackIcon,
  ArrowForward as ArrowForwardIcon,
  CheckCircle as CheckCircleIcon,
} from "@mui/icons-material";

interface QuizDisplayProps {
  question: any;
  questionIndex: number;
  totalQuestions: number;
  selectedAnswer: string | undefined;
  onAnswerSelect: (questionId: string, optionIndex: number) => void;
  onPrevious: () => void;
  onNext: () => void;
  canGoPrevious: boolean;
  canGoNext: boolean;
  isUserRole?: boolean;
}

export const QuizDisplay: React.FC<QuizDisplayProps> = ({
  question,
  questionIndex,
  totalQuestions,
  selectedAnswer,
  onAnswerSelect,
  onPrevious,
  onNext,
  canGoPrevious,
  canGoNext,
  isUserRole = false,
}) => {
  const questionId = question?.id || question?.Id || question?._id || "";
  const options = question?.options || question?.Options || [];
  
  // Safety check - if no question or options, don't render
  if (!question || !options || options.length === 0) {
    return null;
  }
  const optionLabels = ["A", "B", "C", "D"];

  return (
    <Box
      sx={{
        flex: 1,
        borderRadius: 2,
        p: { xs: 2, sm: 3, md: 4 },
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
        overflow: "auto",
        minHeight: { xs: 400, md: "auto" },
        background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
        position: "relative",
      }}
    >
      <Box sx={{ maxWidth: 800, mx: "auto", width: "100%", color: "white" }}>
        <Typography
          variant="h5"
          sx={{
            fontWeight: 600,
            mb: 4,
            color: "white",
            fontSize: { xs: "1.25rem", sm: "1.5rem", md: "1.75rem" },
          }}
        >
          {question.question || question.Question}
        </Typography>

        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mb: 4 }}>
          {options.map((option: any, index: number) => {
            const optionValue = option.value || option.Value || "";
            const isCorrect = option.isCorrect || option.IsCorrect || false;
            const isSelected = selectedAnswer === index.toString();
            
            // For user role, hide correct answer indicators
            const showCorrectIndicator = !isUserRole && isCorrect;

            return (
              <Box
                key={index}
                onClick={() => onAnswerSelect(questionId, index)}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 2,
                  p: 2,
                  borderRadius: 2,
                  backgroundColor: showCorrectIndicator
                    ? "rgba(34, 197, 94, 0.3)"
                    : isSelected
                    ? "rgba(255, 255, 255, 0.2)"
                    : "rgba(255, 255, 255, 0.1)",
                  cursor: "pointer",
                  transition: "all 0.2s",
                  border: showCorrectIndicator ? "2px solid rgba(34, 197, 94, 0.6)" : "none",
                  "&:hover": {
                    backgroundColor: showCorrectIndicator
                      ? "rgba(34, 197, 94, 0.4)"
                      : "rgba(255, 255, 255, 0.2)",
                  },
                }}
              >
                <Box
                  sx={{
                    width: 32,
                    height: 32,
                    borderRadius: "50%",
                    backgroundColor: showCorrectIndicator
                      ? "#22c55e"
                      : isSelected
                      ? "#a78bfa"
                      : "rgba(255, 255, 255, 0.3)",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    border: showCorrectIndicator
                      ? "2px solid #16a34a"
                      : isSelected
                      ? "2px solid #8b5cf6"
                      : "2px solid rgba(255, 255, 255, 0.5)",
                  }}
                >
                  {(showCorrectIndicator || isSelected) ? (
                    <CheckCircleIcon sx={{ color: "white", fontSize: 20 }} />
                  ) : null}
                </Box>
                <Typography
                  sx={{
                    fontWeight: 500,
                    fontSize: { xs: "0.875rem", sm: "1rem" },
                    color: "white",
                  }}
                >
                  {optionLabels[index]}. {optionValue}
                  {showCorrectIndicator && (
                    <Typography
                      component="span"
                      sx={{
                        ml: 1,
                        fontSize: "0.75rem",
                        color: "#86efac",
                        fontWeight: 600,
                      }}
                    >
                      (Correct)
                    </Typography>
                  )}
                </Typography>
              </Box>
            );
          })}
        </Box>

        {/* Navigation */}
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            gap: 2,
            mt: 4,
          }}
        >
          <IconButton
            onClick={onPrevious}
            disabled={!canGoPrevious}
            sx={{
              bgcolor: "rgba(255, 255, 255, 0.2)",
              color: "white",
              "&:hover": { bgcolor: "rgba(255, 255, 255, 0.3)" },
              "&:disabled": { opacity: 0.5 },
            }}
          >
            <ArrowBackIcon />
          </IconButton>
          <Typography sx={{ color: "white", minWidth: 60, textAlign: "center" }}>
            {questionIndex + 1}/{totalQuestions}
          </Typography>
          <IconButton
            onClick={onNext}
            disabled={!canGoNext}
            sx={{
              bgcolor: "rgba(255, 255, 255, 0.2)",
              color: "white",
              "&:hover": { bgcolor: "rgba(255, 255, 255, 0.3)" },
              "&:disabled": { opacity: 0.5 },
            }}
          >
            <ArrowForwardIcon />
          </IconButton>
        </Box>
      </Box>
    </Box>
  );
};


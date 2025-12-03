"use client";

import React from "react";
import {
  Card,
  CardContent,
  CardMedia,
  Typography,
  Chip,
  Box,
  IconButton,
  Tooltip,
} from "@mui/material";
import {
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
} from "@mui/icons-material";
import styled from "styled-components";

const StyledCard = styled(Card)`
  height: 100%;
  display: flex;
  flex-direction: column;
  border-radius: 12px;
  overflow: hidden;
  transition: transform 0.2s, box-shadow 0.2s;
  cursor: pointer;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  }
`;

const ImageContainer = styled(Box)`
  position: relative;
  width: 100%;
  padding-top: 56.25%; // 16:9 aspect ratio
  overflow: hidden;
  background: #f3f4f6;
`;

const CardImage = styled("img")`
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
`;

const ActionButtonsContainer = styled(Box)`
  position: absolute;
  top: 8px;
  right: 8px;
  display: flex;
  gap: 4px;
  z-index: 2;
`;

const ActionButton = styled(IconButton)`
  background: rgba(255, 255, 255, 0.9);
  width: 32px;
  height: 32px;
  padding: 0;

  &:hover {
    background: rgba(255, 255, 255, 1);
  }
`;

const StatusChip = styled(Chip)<{ status: string }>`
  margin-top: 12px;
  font-weight: 600;
  height: 24px;
  ${(props) => {
    switch (props.status?.toLowerCase()) {
      case "draft":
        return `
          background-color: #fbbf24;
          color: #92400e;
        `;
      case "published":
        return `
          background-color: #3b82f6;
          color: white;
        `;
      case "completed":
        return `
          background-color: #10b981;
          color: white;
        `;
      default:
        return `
          background-color: #6b7280;
          color: white;
        `;
    }
  }}
`;

const TitleText = styled(Typography)`
  font-weight: 600;
  font-size: 1.125rem;
  color: #1f2937;
  margin-top: 12px;
  margin-bottom: 8px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
`;

const DescriptionText = styled(Typography)`
  color: #6b7280;
  font-size: 0.875rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-bottom: 12px;
`;

const DetailRow = styled(Box)`
  display: flex;
  align-items: center;
  gap: 8px;
  color: #6b7280;
  font-size: 0.875rem;
  margin-top: 8px;
`;

interface QuizCardProps {
  quiz: {
    id: string;
    title: string;
    description: string;
    status: string;
    dueDate?: string;
    endDate?: string;
    questionsCount?: number;
    lessonsCount?: number;
    image?: string;
  };
  onMenuOpen: (event: React.MouseEvent<HTMLElement>, quizId: string) => void;
  onDelete: (quizId: string) => void;
  onEdit: (quizId: string) => void;
}

export default function QuizCard({
  quiz,
  onMenuOpen,
  onDelete,
  onEdit,
}: QuizCardProps) {
  const getStatusLabel = (status: string) => {
    switch (status?.toLowerCase()) {
      case "draft":
        return "Draft";
      case "published":
        return "Published";
      case "completed":
        return "Completed";
      default:
        return status;
    }
  };

  return (
    <StyledCard>
      <ImageContainer>
        {quiz.image ? (
          <CardImage src={quiz.image} alt={quiz.title} />
        ) : (
          <Box
            sx={{
              position: "absolute",
              top: 0,
              left: 0,
              width: "100%",
              height: "100%",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              background: "linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)",
              color: "white",
            }}
          >
            <Typography variant="h4" fontWeight={600}>
              {quiz.title.charAt(0).toUpperCase()}
            </Typography>
          </Box>
        )}
        <ActionButtonsContainer>
          <Tooltip title="Edit">
            <ActionButton
              size="small"
              onClick={(e) => {
                e.stopPropagation();
                onEdit(quiz.id);
              }}
            >
              <EditIcon sx={{ fontSize: 16, color: "#6366f1" }} />
            </ActionButton>
          </Tooltip>
          <Tooltip title="Delete">
            <ActionButton
              size="small"
              onClick={(e) => {
                e.stopPropagation();
                onDelete(quiz.id);
              }}
            >
              <DeleteIcon sx={{ fontSize: 16, color: "#ef4444" }} />
            </ActionButton>
          </Tooltip>
        </ActionButtonsContainer>
      </ImageContainer>

      <CardContent sx={{ flexGrow: 1, padding: "16px" }}>
        <StatusChip
          label={getStatusLabel(quiz.status)}
          size="small"
          status={quiz.status}
        />

        <TitleText variant="h6">{quiz.title}</TitleText>

        <DescriptionText variant="body2">{quiz.description}</DescriptionText>

        <Box sx={{ marginTop: "auto" }}>
          {quiz.status?.toLowerCase() === "completed" ? (
            <>
              <DetailRow>
                <Typography variant="caption" color="text.secondary">
                  Ends on: {quiz.endDate || "N/A"}
                </Typography>
              </DetailRow>
              {quiz.lessonsCount !== undefined && (
                <DetailRow>
                  <Typography variant="caption" color="text.secondary">
                    {quiz.lessonsCount} Lessons
                  </Typography>
                </DetailRow>
              )}
            </>
          ) : (
            <>
              <DetailRow>
                <Typography variant="caption" color="text.secondary">
                  Due date: {quiz.dueDate || "N/A"}
                </Typography>
              </DetailRow>
              {quiz.questionsCount !== undefined && (
                <DetailRow>
                  <Typography variant="caption" color="text.secondary">
                    {quiz.questionsCount} Questions
                  </Typography>
                </DetailRow>
              )}
            </>
          )}
        </Box>
      </CardContent>
    </StyledCard>
  );
}


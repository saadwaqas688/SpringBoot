"use client";

import React, { useState } from "react";
import {
  Box,
  Typography,
  Button,
  TextField,
  InputAdornment,
  Grid,
  Paper,
  Chip,
  IconButton,
  Menu,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from "@mui/material";
import {
  Search as SearchIcon,
  FilterList as FilterIcon,
  Add as AddIcon,
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  FilePresent as FileIcon,
} from "@mui/icons-material";
import { useRouter } from "next/navigation";
import styled from "styled-components";
import CreateQuizModal from "@/components/quizzes/CreateQuizModal";
import QuizCard from "@/components/quizzes/QuizCard";
import {
  useGetAllQuizzesQuery,
  useDeleteQuizMutation,
} from "@/services/quizzes-api";
import { useGetAllCoursesQuery } from "@/services/courses-api";

const PageContainer = styled(Box)`
  padding: 24px;
  background: #f5f5f5;
  min-height: 100vh;
`;

const HeaderSection = styled(Box)`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  flex-wrap: wrap;
  gap: 16px;
`;

const PageTitle = styled(Typography)`
  font-size: 2rem;
  font-weight: 700;
  color: #1f2937;
`;

const ActionButtons = styled(Box)`
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
`;

const SearchAndFilterSection = styled(Box)`
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
  flex-wrap: wrap;
`;

const StyledButton = styled(Button)`
  text-transform: none;
  font-weight: 600;
  padding: 10px 24px;
  border-radius: 8px;
`;

export default function QuizzesPage() {
  const router = useRouter();
  const [searchQuery, setSearchQuery] = useState("");
  const [filterQuery, setFilterQuery] = useState("");
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [selectedQuiz, setSelectedQuiz] = useState<string | null>(null);
  const [menuAnchor, setMenuAnchor] = useState<null | HTMLElement>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [quizToDelete, setQuizToDelete] = useState<string | null>(null);

  // Fetch quizzes from API
  const {
    data: quizzesResponse,
    isLoading: isLoadingQuizzes,
    refetch: refetchQuizzes,
  } = useGetAllQuizzesQuery({ page: 1, pageSize: 100 });

  const [deleteQuiz, { isLoading: isDeleting }] = useDeleteQuizMutation();

  // Extract quizzes from response
  const quizzesList =
    quizzesResponse?.data?.items || quizzesResponse?.data || [];

  // Transform API data to match component expectations
  const quizzes = quizzesList.map((quiz: any) => ({
    id: quiz.id || quiz._id,
    title: quiz.title || quiz.name || "Untitled Quiz",
    description: quiz.description || "",
    status: quiz.status || "draft",
    dueDate: quiz.dueDate || quiz.deadline,
    endDate: quiz.endDate,
    questionsCount: quiz.questionsCount || quiz.totalQuestions || 0,
    lessonsCount: quiz.lessonsCount,
    image: quiz.thumbnail || quiz.image,
    lessonId: quiz.lessonId,
    courseId: quiz.courseId,
  }));

  const filteredQuizzes = quizzes.filter((quiz) => {
    const matchesSearch =
      searchQuery === "" ||
      quiz.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      quiz.description.toLowerCase().includes(searchQuery.toLowerCase());
    return matchesSearch;
  });

  const handleMenuOpen = (
    event: React.MouseEvent<HTMLElement>,
    quizId: string
  ) => {
    setMenuAnchor(event.currentTarget);
    setSelectedQuiz(quizId);
  };

  const handleMenuClose = () => {
    setMenuAnchor(null);
    setSelectedQuiz(null);
  };

  const handleDelete = (quizId: string) => {
    setQuizToDelete(quizId);
    setDeleteDialogOpen(true);
    handleMenuClose();
  };

  const confirmDelete = async () => {
    if (quizToDelete) {
      try {
        await deleteQuiz(quizToDelete).unwrap();
        await refetchQuizzes();
        setDeleteDialogOpen(false);
        setQuizToDelete(null);
      } catch (error: any) {
        console.error("Failed to delete quiz:", error);
        alert(error?.data?.message || "Failed to delete quiz");
        setDeleteDialogOpen(false);
        setQuizToDelete(null);
      }
    }
  };

  const handleEdit = (quizId: string) => {
    // TODO: Navigate to edit page or open edit modal
    console.log("Editing quiz:", quizId);
    handleMenuClose();
  };

  const handleViewReports = () => {
    // TODO: Navigate to reports page
    router.push("/quizzes/reports");
  };

  return (
    <PageContainer>
      <HeaderSection>
        <PageTitle>Rapid Refresh Quizzes</PageTitle>
        <ActionButtons>
          <StyledButton
            variant="outlined"
            color="primary"
            onClick={handleViewReports}
          >
            View All Reports
          </StyledButton>
          <StyledButton
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => setCreateModalOpen(true)}
            sx={{
              background: "linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)",
              "&:hover": {
                background: "linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%)",
              },
            }}
          >
            Create Quiz
          </StyledButton>
        </ActionButtons>
      </HeaderSection>

      <SearchAndFilterSection>
        <TextField
          placeholder="Filters"
          value={filterQuery}
          onChange={(e) => setFilterQuery(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <FilterIcon sx={{ color: "#9ca3af" }} />
              </InputAdornment>
            ),
          }}
          sx={{
            flex: "1 1 200px",
            "& .MuiOutlinedInput-root": {
              backgroundColor: "white",
            },
          }}
        />
        <TextField
          placeholder="Search by title"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon sx={{ color: "#9ca3af" }} />
              </InputAdornment>
            ),
          }}
          sx={{
            flex: "1 1 300px",
            "& .MuiOutlinedInput-root": {
              backgroundColor: "white",
            },
          }}
        />
      </SearchAndFilterSection>

      {isLoadingQuizzes ? (
        <Box sx={{ textAlign: "center", py: 8 }}>
          <Typography>Loading quizzes...</Typography>
        </Box>
      ) : filteredQuizzes.length === 0 ? (
        <Box sx={{ textAlign: "center", py: 8 }}>
          <Typography color="text.secondary">
            {searchQuery || filterQuery
              ? "No quizzes found matching your search"
              : "No quizzes available. Create your first quiz!"}
          </Typography>
        </Box>
      ) : (
        <Grid container spacing={3}>
          {filteredQuizzes.map((quiz) => (
            <Grid item xs={12} sm={6} md={4} lg={3} key={quiz.id}>
              <QuizCard
                quiz={quiz}
                onMenuOpen={handleMenuOpen}
                onDelete={handleDelete}
                onEdit={handleEdit}
              />
            </Grid>
          ))}
        </Grid>
      )}

      {/* Context Menu */}
      <Menu
        anchorEl={menuAnchor}
        open={Boolean(menuAnchor)}
        onClose={handleMenuClose}
      >
        <MenuItem onClick={() => selectedQuiz && handleEdit(selectedQuiz)}>
          <EditIcon sx={{ mr: 1, fontSize: 20 }} />
          Edit
        </MenuItem>
        <MenuItem onClick={() => selectedQuiz && handleDelete(selectedQuiz)}>
          <DeleteIcon sx={{ mr: 1, fontSize: 20, color: "#ef4444" }} />
          Delete
        </MenuItem>
      </Menu>

      {/* Delete Confirmation Dialog */}
      <Dialog
        open={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
      >
        <DialogTitle>Delete Quiz</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete this quiz? This action cannot be
            undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setDeleteDialogOpen(false)}
            disabled={isDeleting}
          >
            Cancel
          </Button>
          <Button
            onClick={confirmDelete}
            color="error"
            variant="contained"
            disabled={isDeleting}
          >
            {isDeleting ? "Deleting..." : "Delete"}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Create Quiz Modal */}
      <CreateQuizModal
        open={createModalOpen}
        onClose={() => setCreateModalOpen(false)}
        onSuccess={() => {
          setCreateModalOpen(false);
          refetchQuizzes();
        }}
      />
    </PageContainer>
  );
}

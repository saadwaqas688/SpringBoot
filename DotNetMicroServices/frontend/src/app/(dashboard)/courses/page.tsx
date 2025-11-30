"use client";

import { useState } from "react";
import {
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  CardMedia,
  Grid,
  Chip,
  TextField,
  InputAdornment,
  Tabs,
  Tab,
  IconButton,
} from "@mui/material";
import {
  Search as SearchIcon,
  Add as AddIcon,
  CheckCircle as CheckCircleIcon,
  LibraryBooks as LibraryBooksIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
} from "@mui/icons-material";
import { useRouter } from "next/navigation";
import {
  useGetAllCoursesQuery,
  useGetUserCoursesQuery,
  useDeleteCourseMutation,
} from "@/services/courses-api";
import CreateCourseModal from "@/components/CreateCourseModal";
import DeleteConfirmationModal from "@/components/DeleteConfirmationModal";
import MediaPreviewModal from "@/components/MediaPreviewModal";
import styled from "styled-components";

const PageHeader = styled(Box)`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
`;

const SearchBar = styled(Box)`
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
`;

const CourseCard = styled(Card)`
  height: 100%;
  transition: transform 0.2s, box-shadow 0.2s;
  position: relative;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);

    .action-icons {
      opacity: 1;
    }
  }
`;

const CourseImage = styled(CardMedia)`
  height: 200px;
  background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 3rem;
  position: relative;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  cursor: pointer;
  transition: opacity 0.2s;

  &:hover {
    opacity: 0.9;
  }
`;

const ActionIcons = styled(Box)`
  position: absolute;
  top: 8px;
  right: 8px;
  display: flex;
  gap: 8px;
  opacity: 0;
  transition: opacity 0.2s;
  z-index: 1;
`;

const ActionIconButton = styled(IconButton)`
  background-color: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(4px);
  color: #6366f1;
  padding: 6px;

  &:hover {
    background-color: rgba(255, 255, 255, 1);
    color: #4f46e5;
  }

  &.delete {
    color: #ef4444;
    &:hover {
      color: #dc2626;
    }
  }
`;

export default function CoursesPage() {
  const router = useRouter();
  
  // Get user info from localStorage instead of Redux state (persists on refresh)
  const getUserFromStorage = () => {
    if (typeof window === "undefined") return null;
    try {
      const userInfo = localStorage.getItem("user_info");
      if (userInfo) {
        return JSON.parse(userInfo);
      }
    } catch (error) {
      console.error("Error reading user info from localStorage:", error);
    }
    return null;
  };
  
  const user = getUserFromStorage();
  const userRole = user?.role || user?.Role || "";
  const isUserRole = userRole.toLowerCase() === "user";
  const userId = user?.id || user?.Id || user?._id || "";
  
  const [searchTerm, setSearchTerm] = useState("");
  const [tabValue, setTabValue] = useState(0);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [selectedCourse, setSelectedCourse] = useState<any>(null);
  const [isPreviewModalOpen, setIsPreviewModalOpen] = useState(false);
  const [previewUrl, setPreviewUrl] = useState<string>("");
  
  // Fetch courses based on user role
  const { data: allCoursesData, isLoading: isLoadingAll, error: errorAll } = useGetAllCoursesQuery(
    {
      page: 1,
      pageSize: 12,
    },
    { skip: isUserRole } // Skip if user role
  );
  
  const { data: userCoursesData, isLoading: isLoadingUser, error: errorUser } = useGetUserCoursesQuery(
    {
      userId: userId,
      page: 1,
      pageSize: 12,
    },
    { skip: !isUserRole || !userId } // Skip if not user role or no userId
  );
  
  const isLoading = isUserRole ? isLoadingUser : isLoadingAll;
  const error = isUserRole ? errorUser : errorAll;
  const courses = isUserRole 
    ? (userCoursesData?.data || [])
    : (allCoursesData?.data?.items || []);
  
  const [deleteCourse, { isLoading: isDeleting }] = useDeleteCourseMutation();

  const handleEdit = (e: React.MouseEvent, course: any) => {
    e.stopPropagation();
    e.preventDefault();
    console.log("Edit clicked for course:", course);
    setSelectedCourse(course);
    setIsEditModalOpen(true);
  };

  const handleDelete = (e: React.MouseEvent, course: any) => {
    e.stopPropagation();
    e.preventDefault();
    console.log("Delete clicked for course:", course);
    setSelectedCourse(course);
    setIsDeleteModalOpen(true);
  };

  const handleImagePreview = (e: React.MouseEvent, imageUrl: string) => {
    e.stopPropagation();
    e.preventDefault();
    if (imageUrl) {
      setPreviewUrl(imageUrl);
      setIsPreviewModalOpen(true);
    }
  };

  const handleConfirmDelete = async () => {
    console.log("handleConfirmDelete called, selectedCourse:", selectedCourse);
    // Handle both Id (capital I) and id (lowercase) for compatibility
    const courseId =
      selectedCourse?.Id || selectedCourse?.id || selectedCourse?.courseId;
    console.log("Extracted courseId:", courseId);

    if (courseId) {
      try {
        console.log("Calling deleteCourse mutation with ID:", courseId);
        const courseIdString = String(courseId);
        console.log("Course ID as string:", courseIdString);

        const result = await deleteCourse(courseIdString).unwrap();
        console.log("Delete mutation result:", result);
        console.log("Course deleted successfully");

        setIsDeleteModalOpen(false);
        setSelectedCourse(null);
      } catch (error) {
        console.error("Failed to delete course:", error);
        console.error("Error details:", JSON.stringify(error, null, 2));
        // Keep modal open on error so user can retry
      }
    } else {
      console.error(
        "No course ID found for deletion. Course object:",
        selectedCourse
      );
      setIsDeleteModalOpen(false);
      setSelectedCourse(null);
    }
  };

  const handleCloseEditModal = () => {
    setIsEditModalOpen(false);
    setSelectedCourse(null);
  };

  return (
    <Box>
      <Box sx={{ mb: 2 }}>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
          Content {">"} Courses
        </Typography>
        <PageHeader>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
            Courses
          </Typography>
          {!isUserRole && (
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button
                variant="contained"
                sx={{ bgcolor: "#4f46e5", textTransform: "none" }}
                startIcon={<LibraryBooksIcon />}
              >
                Browse Course Library
              </Button>
              <Button
                variant="outlined"
                sx={{ textTransform: "none" }}
                startIcon={<CheckCircleIcon />}
              >
                View Archived Courses
              </Button>
              <Button
                variant="contained"
                sx={{ bgcolor: "#6366f1", textTransform: "none" }}
                startIcon={<AddIcon />}
                onClick={() => setIsCreateModalOpen(true)}
              >
                Create Course
              </Button>
            </Box>
          )}
        </PageHeader>
      </Box>

      {!isUserRole && (
        <Tabs value={tabValue} onChange={(e, v) => setTabValue(v)} sx={{ mb: 3 }}>
          <Tab label="Standard" />
          <Tab label="Paths" />
        </Tabs>
      )}

      <SearchBar>
        <TextField
          fullWidth
          placeholder="Search"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
          sx={{ maxWidth: 400 }}
        />
        <Button variant="outlined" sx={{ textTransform: "none" }}>
          Filter
        </Button>
      </SearchBar>

      {isLoading ? (
        <Typography>Loading courses...</Typography>
      ) : error ? (
        <Typography color="error">Error loading courses</Typography>
      ) : (
        <Grid container spacing={3}>
          {courses.map((course: any) => (
            <Grid
              item
              xs={12}
              sm={6}
              md={4}
              lg={3}
              key={course.Id || course.id}
            >
              <CourseCard>
                <CourseImage
                  sx={{
                    backgroundImage:
                      course.Thumbnail || course.thumbnail || course.imageUrl
                        ? `url(${
                            course.Thumbnail ||
                            course.thumbnail ||
                            course.imageUrl
                          })`
                        : undefined,
                  }}
                  onClick={(e) => {
                    const imageUrl =
                      course.Thumbnail || course.thumbnail || course.imageUrl;
                    if (imageUrl) {
                      handleImagePreview(e, imageUrl);
                    }
                  }}
                >
                  {!(
                    course.Thumbnail ||
                    course.thumbnail ||
                    course.imageUrl
                  ) && <>{(course.Title || course.title)?.charAt(0) || "C"}</>}
                  {!isUserRole && (
                    <ActionIcons className="action-icons">
                      <ActionIconButton
                        size="small"
                        onClick={(e) => handleEdit(e, course)}
                        aria-label="Edit course"
                      >
                        <EditIcon fontSize="small" />
                      </ActionIconButton>
                      <ActionIconButton
                        size="small"
                        className="delete"
                        onClick={(e) => handleDelete(e, course)}
                        aria-label="Delete course"
                      >
                        <DeleteIcon fontSize="small" />
                      </ActionIconButton>
                    </ActionIcons>
                  )}
                </CourseImage>
                <CardContent>
                  <Typography variant="h6" component="h3" gutterBottom noWrap>
                    {course.Title || course.title || "Untitled Course"}
                  </Typography>
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ mb: 2, minHeight: 40 }}
                    noWrap
                  >
                    {course.Description ||
                      course.description ||
                      "No description"}
                  </Typography>
                  <Box
                    sx={{
                      display: "flex",
                      justifyContent: "space-between",
                      alignItems: "center",
                      mb: 2,
                    }}
                  >
                    <Chip
                      label={course.Status || course.status || "Draft"}
                      color={
                        (course.Status || course.status) === "Published"
                          ? "success"
                          : "default"
                      }
                      size="small"
                    />
                    <Typography variant="caption" color="text.secondary">
                      = {course.lessonCount || 0}
                    </Typography>
                  </Box>
                  <Button
                    fullWidth
                    variant="contained"
                    sx={{ bgcolor: "#6366f1", textTransform: "none" }}
                    onClick={() => {
                      const courseId = course.Id || course.id;
                      if (courseId) {
                        router.push(`/courses/${courseId}`);
                      }
                    }}
                  >
                    Preview
                  </Button>
                </CardContent>
              </CourseCard>
            </Grid>
          ))}
          {courses.length === 0 && (
            <Grid item xs={12}>
              <Typography
                textAlign="center"
                color="text.secondary"
                sx={{ py: 4 }}
              >
                {isUserRole 
                  ? "No enrolled courses found." 
                  : "No courses found. Create your first course!"}
              </Typography>
            </Grid>
          )}
        </Grid>
      )}

      <CreateCourseModal
        open={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
      />

      <CreateCourseModal
        open={isEditModalOpen}
        onClose={handleCloseEditModal}
        courseId={
          selectedCourse?.Id || selectedCourse?.id || selectedCourse?.courseId
        }
        initialData={
          selectedCourse
            ? {
                title: selectedCourse?.Title || selectedCourse?.title || "",
                description:
                  selectedCourse?.Description ||
                  selectedCourse?.description ||
                  "",
                imageUrl:
                  selectedCourse?.Thumbnail ||
                  selectedCourse?.imageUrl ||
                  selectedCourse?.image ||
                  "",
              }
            : undefined
        }
      />

      <DeleteConfirmationModal
        open={isDeleteModalOpen}
        onClose={() => {
          setIsDeleteModalOpen(false);
          setSelectedCourse(null);
        }}
        onConfirm={handleConfirmDelete}
        isLoading={isDeleting}
      />

      <MediaPreviewModal
        open={isPreviewModalOpen}
        onClose={() => setIsPreviewModalOpen(false)}
        url={previewUrl}
        type="image"
        alt="Course thumbnail"
      />
    </Box>
  );
}

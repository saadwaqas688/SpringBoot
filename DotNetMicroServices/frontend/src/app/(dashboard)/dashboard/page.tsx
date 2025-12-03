"use client";

import { Typography, Box, Grid, CircularProgress, Alert } from "@mui/material";
import { StatCard } from "@/components/dashboard/StatCard";
import {
  Book as BookIcon,
  Quiz as QuizIcon,
  Forum as ForumIcon,
  People as PeopleIcon,
} from "@mui/icons-material";
import { useGetAllQuizzesQuery } from "@/services/quizzes-api";
import { useGetAllDiscussionsQuery } from "@/services/discussions-api";
import {
  useGetUserAnalyticsQuery,
  useGetCourseAnalyticsQuery,
} from "@/services/admin-api";

export default function DashboardPage() {
  // Fetch data for statistics using admin analytics for courses and users
  const {
    data: courseAnalyticsData,
    isLoading: coursesLoading,
    error: coursesError,
  } = useGetCourseAnalyticsQuery();

  const {
    data: quizzesData,
    isLoading: quizzesLoading,
    error: quizzesError,
  } = useGetAllQuizzesQuery({ page: 1, pageSize: 10000 });

  const {
    data: discussionsData,
    isLoading: discussionsLoading,
    error: discussionsError,
  } = useGetAllDiscussionsQuery();

  const {
    data: userAnalyticsData,
    isLoading: usersLoading,
    error: usersError,
  } = useGetUserAnalyticsQuery();

  // Calculate totals
  const totalCourses = courseAnalyticsData?.data?.totalCourses || 0;

  const totalQuizzes =
    quizzesData?.data?.totalCount ||
    quizzesData?.data?.items?.length ||
    (Array.isArray(quizzesData?.data) ? quizzesData.data.length : 0) ||
    0;

  const totalDiscussions =
    discussionsData?.data?.totalCount ||
    (Array.isArray(discussionsData?.data) ? discussionsData.data.length : 0) ||
    0;

  const totalUsers = userAnalyticsData?.data?.totalUsers || 0;

  const isLoading =
    coursesLoading || quizzesLoading || discussionsLoading || usersLoading;
  const hasError =
    coursesError || quizzesError || discussionsError || usersError;

  return (
    <Box sx={{ p: 3 }}>
      <Typography
        variant="h4"
        component="h1"
        sx={{
          fontWeight: 600,
          color: "#1f2937",
          mb: 3,
        }}
      >
        Dashboard
      </Typography>

      {isLoading && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            minHeight: 400,
          }}
        >
          <CircularProgress />
        </Box>
      )}

      {hasError && !isLoading && (
        <Alert severity="error" sx={{ mb: 3 }}>
          Error loading dashboard data. Please try again later.
        </Alert>
      )}

      {!isLoading && !hasError && (
        <Grid container spacing={3}>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Total Courses"
              value={totalCourses}
              icon={<BookIcon />}
              iconColor="#3b82f6"
              backgroundColor="#dbeafe"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Total Quiz"
              value={totalQuizzes}
              icon={<QuizIcon />}
              iconColor="#f59e0b"
              backgroundColor="#fef3c7"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Total Discussions"
              value={totalDiscussions}
              icon={<ForumIcon />}
              iconColor="#10b981"
              backgroundColor="#d1fae5"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Total Users"
              value={totalUsers}
              icon={<PeopleIcon />}
              iconColor="#8b5cf6"
              backgroundColor="#ede9fe"
            />
          </Grid>
        </Grid>
      )}
    </Box>
  );
}

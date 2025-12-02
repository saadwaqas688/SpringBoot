"use client";

import { useState } from "react";
import {
  Box,
  Typography,
  Paper,
  TextField,
  InputAdornment,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  IconButton,
  CircularProgress,
} from "@mui/material";
import {
  Search as SearchIcon,
  ArrowUpward as ArrowUpwardIcon,
  ArrowDownward as ArrowDownwardIcon,
} from "@mui/icons-material";
import { useRouter } from "next/navigation";
import { useGetAllDiscussionsQuery } from "@/services/discussions-api";
import Link from "next/link";

type SortField = "courseTitle" | "discussionTitle" | "contributions";
type SortDirection = "asc" | "desc";

export default function DiscussionsPage() {
  const router = useRouter();
  const [searchTerm, setSearchTerm] = useState("");
  const [sortField, setSortField] = useState<SortField | null>(null);
  const [sortDirection, setSortDirection] = useState<SortDirection>("asc");

  const { data: discussionsData, isLoading } = useGetAllDiscussionsQuery();
  const discussions = discussionsData?.data || [];

  // Filter discussions based on search term
  const filteredDiscussions = discussions.filter((discussion: any) => {
    if (!searchTerm) return true;
    const searchLower = searchTerm.toLowerCase();
    return (
      discussion.discussionTitle?.toLowerCase().includes(searchLower) ||
      discussion.id?.toLowerCase().includes(searchLower) ||
      discussion.courseTitle?.toLowerCase().includes(searchLower)
    );
  });

  // Sort discussions
  const sortedDiscussions = [...filteredDiscussions].sort((a: any, b: any) => {
    if (!sortField) return 0;

    let aValue: any;
    let bValue: any;

    switch (sortField) {
      case "courseTitle":
        aValue = a.courseTitle || "";
        bValue = b.courseTitle || "";
        break;
      case "discussionTitle":
        aValue = a.discussionTitle || "";
        bValue = b.discussionTitle || "";
        break;
      case "contributions":
        aValue = a.contributions || 0;
        bValue = b.contributions || 0;
        break;
      default:
        return 0;
    }

    if (typeof aValue === "string") {
      return sortDirection === "asc"
        ? aValue.localeCompare(bValue)
        : bValue.localeCompare(aValue);
    } else {
      return sortDirection === "asc" ? aValue - bValue : bValue - aValue;
    }
  });

  const handleSort = (field: SortField) => {
    if (sortField === field) {
      setSortDirection(sortDirection === "asc" ? "desc" : "asc");
    } else {
      setSortField(field);
      setSortDirection("asc");
    }
  };

  const handleViewDiscussion = (discussionId: string) => {
    router.push(`/discussions/${discussionId}`);
  };

  const SortIcon = ({ field }: { field: SortField }) => {
    if (sortField !== field) return null;
    return sortDirection === "asc" ? (
      <ArrowUpwardIcon fontSize="small" />
    ) : (
      <ArrowDownwardIcon fontSize="small" />
    );
  };

  return (
    <Box sx={{ p: 3 }}>
      {/* Breadcrumbs */}
      <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
        Facilitate {">"} Discussion
      </Typography>

      {/* Page Title */}
      <Typography
        variant="h4"
        component="h1"
        sx={{ fontWeight: 700, mb: 3, color: "#6366f1" }}
      >
        Discussion
      </Typography>

      {/* Search Bar */}
      <Paper sx={{ mb: 3, p: 2 }}>
        <TextField
          fullWidth
          placeholder="Search by title or ID"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
        />
      </Paper>

      {/* Discussions Table */}
      <Paper>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow sx={{ backgroundColor: "#f3f4f6" }}>
                <TableCell>
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                      cursor: "pointer",
                      userSelect: "none",
                    }}
                    onClick={() => handleSort("courseTitle")}
                  >
                    <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                      Courses
                    </Typography>
                    <SortIcon field="courseTitle" />
                  </Box>
                </TableCell>
                <TableCell>
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                      cursor: "pointer",
                      userSelect: "none",
                    }}
                    onClick={() => handleSort("discussionTitle")}
                  >
                    <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                      Discussion Title
                    </Typography>
                    <SortIcon field="discussionTitle" />
                  </Box>
                </TableCell>
                <TableCell>
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                      cursor: "pointer",
                      userSelect: "none",
                    }}
                    onClick={() => handleSort("contributions")}
                  >
                    <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                      Contributions
                    </Typography>
                    <SortIcon field="contributions" />
                  </Box>
                </TableCell>
                <TableCell>
                  <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                    Actions
                  </Typography>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={4} align="center">
                    <CircularProgress />
                  </TableCell>
                </TableRow>
              ) : sortedDiscussions.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4} align="center">
                    <Typography color="text.secondary">
                      No discussions found
                    </Typography>
                  </TableCell>
                </TableRow>
              ) : (
                sortedDiscussions.map((discussion: any) => (
                  <TableRow key={discussion.id} hover>
                    <TableCell>
                      <Typography variant="body2">
                        {discussion.courseTitle || "Untitled Course"}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">
                        {discussion.discussionTitle || "Untitled discussion"}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">
                        {discussion.contributions || 0}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="contained"
                        size="small"
                        onClick={() => handleViewDiscussion(discussion.id)}
                        sx={{
                          backgroundColor: "#6366f1",
                          textTransform: "none",
                          "&:hover": {
                            backgroundColor: "#4f46e5",
                          },
                        }}
                      >
                        View discussion
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Box>
  );
}

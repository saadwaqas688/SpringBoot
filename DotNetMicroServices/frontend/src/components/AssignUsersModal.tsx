"use client";

import { useState, useEffect } from "react";
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
  InputAdornment,
  Checkbox,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  ListItemIcon,
  CircularProgress,
  Pagination,
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  Tabs,
  Tab,
} from "@mui/material";
import {
  Close as CloseIcon,
  Search as SearchIcon,
  Person as PersonIcon,
} from "@mui/icons-material";
import { useGetAllUsersQuery } from "@/services/users-api";
import { useAssignUserMutation } from "@/services/courses-api";
import styled from "styled-components";

const StyledDialog = styled(Dialog)`
  .MuiDialog-paper {
    max-width: 600px;
    width: 100%;
  }
`;

const SearchContainer = styled(Box)`
  margin-bottom: 1.5rem;
`;

const UsersList = styled(List)`
  max-height: 400px;
  overflow-y: auto;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0;
`;

const StyledListItem = styled(ListItem)`
  border-bottom: 1px solid #e5e7eb;

  &:last-child {
    border-bottom: none;
  }
`;

const PaginationContainer = styled(Box)`
  display: flex;
  justify-content: center;
  margin-top: 1rem;
`;

interface AssignUsersModalProps {
  open: boolean;
  onClose: () => void;
  courseId: string;
}

export default function AssignUsersModal({
  open,
  onClose,
  courseId,
}: AssignUsersModalProps) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedUsers, setSelectedUsers] = useState<string[]>([]);
  const [assignmentType, setAssignmentType] = useState<"all" | "selected">(
    "selected"
  );
  const [currentPage, setCurrentPage] = useState(1);
  const [tabValue, setTabValue] = useState(0);
  const pageSize = 10;

  const {
    data: usersData,
    isLoading,
    isFetching,
  } = useGetAllUsersQuery({
    page: currentPage,
    pageSize,
    searchTerm: searchTerm || undefined,
  });

  const [assignUsers, { isLoading: isAssigning }] = useAssignUserMutation();

  const users = usersData?.data?.items || usersData?.items || [];
  const totalPages =
    usersData?.data?.totalPages ||
    usersData?.totalPages ||
    Math.ceil(
      (usersData?.data?.totalCount || usersData?.totalCount || 0) / pageSize
    );

  useEffect(() => {
    if (!open) {
      setSearchTerm("");
      setSelectedUsers([]);
      setCurrentPage(1);
      setAssignmentType("selected");
      setTabValue(0);
    }
  }, [open]);

  const handleUserToggle = (userId: string) => {
    setSelectedUsers((prev) =>
      prev.includes(userId)
        ? prev.filter((id) => id !== userId)
        : [...prev, userId]
    );
  };

  const handleSelectAll = () => {
    if (selectedUsers.length === users.length) {
      setSelectedUsers([]);
    } else {
      setSelectedUsers(users.map((user: any) => user.id || user.Id));
    }
  };

  const handleAssign = async () => {
    try {
      if (assignmentType === "selected") {
        // Assign to selected users
        if (selectedUsers.length === 0) {
          alert("Please select at least one user");
          return;
        }
        // Assign each user individually
        // The API expects { courseId, userId } for each assignment
        const assignments = selectedUsers.map((userId) =>
          assignUsers({
            courseId,
            userId,
          }).unwrap()
        );
        await Promise.all(assignments);
      } else {
        // For "all groups" - this might need special handling
        // You may need to get all users first or use a different endpoint
        alert("Assigning to all groups is not yet implemented");
        return;
      }
      alert("Users assigned successfully!");
      onClose();
    } catch (error) {
      console.error("Failed to assign users:", error);
      alert("Failed to assign users. Please try again.");
    }
  };

  return (
    <StyledDialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          pb: 1,
          borderBottom: "1px solid #e5e7eb",
        }}
      >
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          Assign this course to
        </Typography>
        <IconButton onClick={onClose} size="small">
          <CloseIcon />
        </IconButton>
      </DialogTitle>

      <DialogContent sx={{ pt: 2 }}>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Select who you would like to assign this course to
        </Typography>

        <FormControl component="fieldset" sx={{ mb: 2, width: "100%" }}>
          <RadioGroup
            value={assignmentType}
            onChange={(e) =>
              setAssignmentType(e.target.value as "all" | "selected")
            }
            row
          >
            <FormControlLabel
              value="all"
              control={<Radio />}
              label="All Groups"
            />
            <FormControlLabel
              value="selected"
              control={<Radio />}
              label="Only Selected Groups"
            />
          </RadioGroup>
        </FormControl>

        {assignmentType === "selected" && (
          <>
            <SearchContainer>
              <TextField
                fullWidth
                placeholder="Search Group or Users"
                value={searchTerm}
                onChange={(e) => {
                  setSearchTerm(e.target.value);
                  setCurrentPage(1);
                }}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon />
                    </InputAdornment>
                  ),
                }}
                size="small"
              />
            </SearchContainer>

            <Tabs
              value={tabValue}
              onChange={(e, newValue) => setTabValue(newValue)}
              sx={{ mb: 2 }}
            >
              <Tab label="Groups" />
              <Tab label="Users" />
            </Tabs>

            {tabValue === 1 && (
              <>
                {users.length > 0 && (
                  <Box sx={{ mb: 1 }}>
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={
                            selectedUsers.length === users.length &&
                            users.length > 0
                          }
                          indeterminate={
                            selectedUsers.length > 0 &&
                            selectedUsers.length < users.length
                          }
                          onChange={handleSelectAll}
                        />
                      }
                      label="Select All"
                    />
                  </Box>
                )}

                {isLoading || isFetching ? (
                  <Box
                    sx={{
                      display: "flex",
                      justifyContent: "center",
                      alignItems: "center",
                      minHeight: 200,
                    }}
                  >
                    <CircularProgress />
                  </Box>
                ) : users.length === 0 ? (
                  <Box
                    sx={{
                      display: "flex",
                      justifyContent: "center",
                      alignItems: "center",
                      minHeight: 200,
                      color: "text.secondary",
                    }}
                  >
                    <Typography>No users found</Typography>
                  </Box>
                ) : (
                  <>
                    <UsersList>
                      {users.map((user: any) => {
                        const userId = user.id || user.Id;
                        const userName =
                          user.name ||
                          user.Name ||
                          user.username ||
                          user.Username ||
                          user.email ||
                          user.Email ||
                          "Unknown User";
                        const userEmail = user.email || user.Email || "";

                        return (
                          <StyledListItem key={userId} disablePadding>
                            <ListItemButton
                              onClick={() => handleUserToggle(userId)}
                              dense
                            >
                              <ListItemIcon>
                                <Checkbox
                                  edge="start"
                                  checked={selectedUsers.includes(userId)}
                                  tabIndex={-1}
                                  disableRipple
                                />
                              </ListItemIcon>
                              <ListItemIcon>
                                <PersonIcon />
                              </ListItemIcon>
                              <ListItemText
                                primary={userName}
                                secondary={userEmail}
                              />
                            </ListItemButton>
                          </StyledListItem>
                        );
                      })}
                    </UsersList>

                    {totalPages > 1 && (
                      <PaginationContainer>
                        <Pagination
                          count={totalPages}
                          page={currentPage}
                          onChange={(e, page) => setCurrentPage(page)}
                          color="primary"
                        />
                      </PaginationContainer>
                    )}
                  </>
                )}
              </>
            )}

            {tabValue === 0 && (
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  minHeight: 200,
                  color: "text.secondary",
                }}
              >
                <Typography>Groups functionality coming soon</Typography>
              </Box>
            )}
          </>
        )}
      </DialogContent>

      <DialogActions sx={{ p: 2, borderTop: "1px solid #e5e7eb" }}>
        <Button onClick={onClose} sx={{ textTransform: "none" }}>
          Cancel
        </Button>
        <Button
          onClick={handleAssign}
          variant="contained"
          disabled={
            isAssigning ||
            (assignmentType === "selected" && selectedUsers.length === 0)
          }
          sx={{
            bgcolor: "#6366f1",
            textTransform: "none",
            "&:hover": { bgcolor: "#4f46e5" },
          }}
        >
          {isAssigning ? <CircularProgress size={20} /> : "Done"}
        </Button>
      </DialogActions>
    </StyledDialog>
  );
}

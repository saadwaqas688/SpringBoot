"use client";

import React, { useState, useCallback } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  InputAdornment,
  Tabs,
  Tab,
  Alert,
  CircularProgress,
} from "@mui/material";
import { Search as SearchIcon, Add as AddIcon } from "@mui/icons-material";
import { UserTable } from "@/components/users/UserTable";
import { AddUserModal } from "@/components/users/AddUserModal";
import { EditUserModal } from "@/components/users/EditUserModal";
import DeleteConfirmationModal from "@/components/DeleteConfirmationModal";
import {
  useGetAllUsersQuery,
  useDeleteUserMutation,
  useUpdateUserStatusMutation,
  UserInfo,
} from "@/services/users-api";

export default function UserManagementPage() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedTab, setSelectedTab] = useState(0);
  const [selectedUsers, setSelectedUsers] = useState<string[]>([]);
  const [addModalOpen, setAddModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<UserInfo | null>(null);
  const [notification, setNotification] = useState<{
    message: string;
    type: "success" | "error";
  } | null>(null);

  const {
    data: usersData,
    isLoading,
    error,
    refetch,
  } = useGetAllUsersQuery({
    page,
    pageSize,
    searchTerm: searchTerm || undefined,
  });

  const [deleteUser, { isLoading: isDeleting }] = useDeleteUserMutation();
  const [updateUserStatus, { isLoading: isUpdatingStatus }] =
    useUpdateUserStatusMutation();

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
    setPage(1); // Reset to first page on search
  };

  const handleAddUser = () => {
    setAddModalOpen(true);
  };

  const handleEditUser = (user: UserInfo) => {
    setSelectedUser(user);
    setEditModalOpen(true);
  };

  const handleDeleteUser = (userId: string) => {
    const user = usersData?.data?.items.find((u) => u.id === userId);
    if (user) {
      setSelectedUser(user);
      setDeleteModalOpen(true);
    }
  };

  const handleConfirmDelete = async () => {
    if (!selectedUser) return;

    try {
      await deleteUser(selectedUser.id).unwrap();
      setNotification({
        message: "User deleted successfully",
        type: "success",
      });
      setDeleteModalOpen(false);
      setSelectedUser(null);
      refetch();
      setTimeout(() => setNotification(null), 3000);
    } catch (error: any) {
      setNotification({
        message: error?.data?.message || "Failed to delete user",
        type: "error",
      });
      setTimeout(() => setNotification(null), 3000);
    }
  };

  const handleStatusChange = async (userId: string, status: string) => {
    try {
      await updateUserStatus({ id: userId, status }).unwrap();
      setNotification({
        message: "User status updated successfully",
        type: "success",
      });
      refetch();
      setTimeout(() => setNotification(null), 3000);
    } catch (error: any) {
      setNotification({
        message: error?.data?.message || "Failed to update user status",
        type: "error",
      });
      setTimeout(() => setNotification(null), 3000);
    }
  };

  const handleSelectUser = (userId: string) => {
    setSelectedUsers((prev) =>
      prev.includes(userId)
        ? prev.filter((id) => id !== userId)
        : [...prev, userId]
    );
  };

  const handleSelectAll = (selected: boolean) => {
    if (selected) {
      setSelectedUsers(usersData?.data?.items.map((u) => u.id) || []);
    } else {
      setSelectedUsers([]);
    }
  };

  const handleSuccess = () => {
    refetch();
  };

  const users = usersData?.data?.items || [];
  const totalCount = usersData?.data?.totalCount || 0;

  return (
    <Box sx={{ p: 3 }}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 3,
        }}
      >
        <Typography
          variant="h4"
          component="h1"
          sx={{ fontWeight: 600, color: "#1f2937" }}
        >
          User Management
        </Typography>
      </Box>

      <Box sx={{ mb: 3 }}>
        <Tabs
          value={selectedTab}
          onChange={(_, newValue) => setSelectedTab(newValue)}
          sx={{ borderBottom: 1, borderColor: "divider" }}
        >
          <Tab label="Users" />
          <Tab label="User Groups" />
          <Tab label="External Users" />
        </Tabs>
      </Box>

      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 2,
          gap: 2,
        }}
      >
        <TextField
          placeholder="Search"
          value={searchTerm}
          onChange={handleSearch}
          size="small"
          sx={{ flexGrow: 1, maxWidth: 400 }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
        />
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleAddUser}
        >
          Add New User +
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Error loading users. Please try again later.
        </Alert>
      )}

      {notification && (
        <Alert
          severity={notification.type}
          sx={{ mb: 2 }}
          onClose={() => setNotification(null)}
        >
          {notification.message}
        </Alert>
      )}

      {isLoading ? (
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
      ) : (
        <UserTable
          users={users}
          page={page}
          pageSize={pageSize}
          totalCount={totalCount}
          onPageChange={setPage}
          onPageSizeChange={(newPageSize) => {
            setPageSize(newPageSize);
            setPage(1);
          }}
          onEdit={handleEditUser}
          onDelete={handleDeleteUser}
          onStatusChange={handleStatusChange}
          selectedUsers={selectedUsers}
          onSelectUser={handleSelectUser}
          onSelectAll={handleSelectAll}
        />
      )}

      <AddUserModal
        open={addModalOpen}
        onClose={() => setAddModalOpen(false)}
        onSuccess={handleSuccess}
      />

      <EditUserModal
        open={editModalOpen}
        user={selectedUser}
        onClose={() => {
          setEditModalOpen(false);
          setSelectedUser(null);
        }}
        onSuccess={handleSuccess}
      />

      <DeleteConfirmationModal
        open={deleteModalOpen}
        onClose={() => {
          setDeleteModalOpen(false);
          setSelectedUser(null);
        }}
        onConfirm={handleConfirmDelete}
        title="Delete User"
        message={`Are you sure you want to delete ${selectedUser?.name}? This action cannot be undone.`}
        isLoading={isDeleting}
      />
    </Box>
  );
}


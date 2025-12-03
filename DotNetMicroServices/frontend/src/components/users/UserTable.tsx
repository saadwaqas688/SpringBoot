"use client";

import React, { useState } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Checkbox,
  IconButton,
  Switch,
  Avatar,
  Chip,
  TablePagination,
  Box,
  Typography,
} from "@mui/material";
import { Edit as EditIcon, Delete as DeleteIcon } from "@mui/icons-material";
import { UserInfo } from "@/services/users-api";
import { format } from "date-fns";

interface UserTableProps {
  users: UserInfo[];
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  onEdit: (user: UserInfo) => void;
  onDelete: (userId: string) => void;
  onStatusChange: (userId: string, status: string) => void;
  selectedUsers: string[];
  onSelectUser: (userId: string) => void;
  onSelectAll: (selected: boolean) => void;
}

export const UserTable: React.FC<UserTableProps> = ({
  users,
  page,
  pageSize,
  totalCount,
  onPageChange,
  onPageSizeChange,
  onEdit,
  onDelete,
  onStatusChange,
  selectedUsers,
  onSelectUser,
  onSelectAll,
}) => {
  const handleSelectAll = (event: React.ChangeEvent<HTMLInputElement>) => {
    onSelectAll(event.target.checked);
  };

  const handleSelectUser = (userId: string) => {
    onSelectUser(userId);
  };

  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return "";
    try {
      const date = new Date(dateString);
      return format(date, "dd/MM/yyyy");
    } catch {
      return "";
    }
  };

  const allSelected = users.length > 0 && selectedUsers.length === users.length;
  const someSelected =
    selectedUsers.length > 0 && selectedUsers.length < users.length;

  return (
    <TableContainer component={Paper} sx={{ boxShadow: "none" }}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell padding="checkbox">
              <Checkbox
                indeterminate={someSelected}
                checked={allSelected}
                onChange={handleSelectAll}
              />
            </TableCell>
            <TableCell sx={{ fontWeight: 600 }}>User ID</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>User</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>Role</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>Created On</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>Status</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.length === 0 ? (
            <TableRow>
              <TableCell colSpan={7} align="center" sx={{ py: 4 }}>
                <Typography color="text.secondary">No users found</Typography>
              </TableCell>
            </TableRow>
          ) : (
            users.map((user) => (
              <TableRow key={user.id} hover>
                <TableCell padding="checkbox">
                  <Checkbox
                    checked={selectedUsers.includes(user.id)}
                    onChange={() => handleSelectUser(user.id)}
                  />
                </TableCell>
                <TableCell>
                  <Typography variant="body2" sx={{ fontFamily: "monospace" }}>
                    {user.id}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                    <Avatar
                      sx={{
                        bgcolor: user.image ? "transparent" : "primary.main",
                        width: 40,
                        height: 40,
                      }}
                      src={user.image}
                    >
                      {!user.image && getInitials(user.name)}
                    </Avatar>
                    <Box>
                      <Typography variant="body2" sx={{ fontWeight: 500 }}>
                        {user.name}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {user.email}
                      </Typography>
                    </Box>
                  </Box>
                </TableCell>
                <TableCell>
                  <Chip
                    label={user.role.toUpperCase()}
                    size="small"
                    sx={{
                      bgcolor: "rgba(0, 0, 0, 0.05)",
                      fontWeight: 500,
                    }}
                  />
                </TableCell>
                <TableCell>{formatDate(user.createdAt)}</TableCell>
                <TableCell>
                  <Chip
                    label={user.status === "active" ? "Active" : "Inactive"}
                    size="small"
                    color={user.status === "active" ? "success" : "default"}
                    sx={{
                      fontWeight: 500,
                    }}
                  />
                </TableCell>
                <TableCell>
                  <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                    <Switch
                      checked={user.status === "active"}
                      onChange={(e) =>
                        onStatusChange(
                          user.id,
                          e.target.checked ? "active" : "inactive"
                        )
                      }
                      size="small"
                    />
                    <IconButton
                      size="small"
                      onClick={() => onEdit(user)}
                      sx={{ color: "primary.main" }}
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => onDelete(user.id)}
                      sx={{ color: "error.main" }}
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </Box>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
      <TablePagination
        component="div"
        count={totalCount}
        page={page - 1}
        onPageChange={(_, newPage) => onPageChange(newPage + 1)}
        rowsPerPage={pageSize}
        onRowsPerPageChange={(e) =>
          onPageSizeChange(parseInt(e.target.value, 10))
        }
        rowsPerPageOptions={[10, 25, 50, 100]}
        labelRowsPerPage="Rows per page:"
        labelDisplayedRows={({ from, to, count }) =>
          `Showing ${from} of ${count !== -1 ? count : `more than ${to}`}`
        }
      />
    </TableContainer>
  );
};


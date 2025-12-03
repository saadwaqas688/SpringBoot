"use client";

import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Grid,
  Box,
  IconButton,
  MenuItem,
} from "@mui/material";
import { Close as CloseIcon } from "@mui/icons-material";
import {
  useUpdateUserMutation,
  UserInfo,
  UpdateUserRequest,
} from "@/services/users-api";

interface EditUserModalProps {
  open: boolean;
  user: UserInfo | null;
  onClose: () => void;
  onSuccess?: () => void;
}

export const EditUserModal: React.FC<EditUserModalProps> = ({
  open,
  user,
  onClose,
  onSuccess,
}) => {
  const [updateUser, { isLoading }] = useUpdateUserMutation();
  const [formData, setFormData] = useState<UpdateUserRequest>({
    firstName: "",
    lastName: "",
    email: "",
    mobilePhone: "",
    dateOfBirth: "",
    gender: "",
    country: "",
    state: "",
    city: "",
    postalCode: "",
    role: "",
    status: "",
  });

  const [errors, setErrors] = useState<
    Partial<Record<keyof UpdateUserRequest, string>>
  >({});

  useEffect(() => {
    if (user) {
      const nameParts = user.name.split(" ");
      const firstName = nameParts[0] || "";
      const lastName = nameParts.slice(1).join(" ") || "";

      setFormData({
        firstName,
        lastName,
        email: user.email || "",
        mobilePhone: user.mobilePhone || "",
        dateOfBirth: user.dateOfBirth
          ? new Date(user.dateOfBirth).toISOString().split("T")[0]
          : "",
        gender: user.gender || "",
        country: user.country || "",
        state: user.state || "",
        city: user.city || "",
        postalCode: user.postalCode || "",
        role: user.role || "",
        status: user.status || "active",
      });
    }
  }, [user]);

  const handleChange =
    (field: keyof UpdateUserRequest) =>
    (e: React.ChangeEvent<HTMLInputElement>) => {
      setFormData((prev) => ({ ...prev, [field]: e.target.value }));
      if (errors[field]) {
        setErrors((prev) => ({ ...prev, [field]: undefined }));
      }
    };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof UpdateUserRequest, string>> = {};

    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = "Invalid email format";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    if (!user || !validate()) return;

    try {
      await updateUser({ id: user.id, data: formData }).unwrap();
      onSuccess?.();
      handleClose();
    } catch (error: any) {
      console.error("Error updating user:", error);
    }
  };

  const handleClose = () => {
    setFormData({
      firstName: "",
      lastName: "",
      email: "",
      mobilePhone: "",
      dateOfBirth: "",
      gender: "",
      country: "",
      state: "",
      city: "",
      postalCode: "",
      role: "",
      status: "",
    });
    setErrors({});
    onClose();
  };

  if (!user) return null;

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <Box sx={{ color: "primary.main", fontWeight: 600 }}>Edit User</Box>
          <IconButton onClick={handleClose} size="small">
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>
      <DialogContent>
        <Grid container spacing={2} sx={{ mt: 1 }}>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="First Name"
              value={formData.firstName}
              onChange={handleChange("firstName")}
              placeholder="Enter first name"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Last Name"
              value={formData.lastName}
              onChange={handleChange("lastName")}
              placeholder="Enter last name"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={formData.email}
              onChange={handleChange("email")}
              error={!!errors.email}
              helperText={errors.email}
              placeholder="Enter email"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Mobile"
              value={formData.mobilePhone}
              onChange={handleChange("mobilePhone")}
              placeholder="Enter mobile"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Date of Birth"
              type="date"
              value={formData.dateOfBirth}
              onChange={handleChange("dateOfBirth")}
              InputLabelProps={{ shrink: true }}
              placeholder="MM/DD/YYYY"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              select
              label="Gender"
              value={formData.gender}
              onChange={handleChange("gender")}
              placeholder="Select gender"
            >
              <MenuItem value="male">Male</MenuItem>
              <MenuItem value="female">Female</MenuItem>
              <MenuItem value="other">Other</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Country"
              value={formData.country}
              onChange={handleChange("country")}
              placeholder="Enter country"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="State"
              value={formData.state}
              onChange={handleChange("state")}
              placeholder="Enter state"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="City"
              value={formData.city}
              onChange={handleChange("city")}
              placeholder="Enter city"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Postcode"
              value={formData.postalCode}
              onChange={handleChange("postalCode")}
              placeholder="Enter postcode"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              select
              label="Role"
              value={formData.role}
              onChange={handleChange("role")}
              placeholder="Select role"
            >
              <MenuItem value="user">User</MenuItem>
              <MenuItem value="learner">Learner</MenuItem>
              <MenuItem value="facilitator">Facilitator</MenuItem>
              <MenuItem value="employee">Employee</MenuItem>
              <MenuItem value="inspector">Inspector</MenuItem>
              <MenuItem value="admin">Admin</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              select
              label="Status"
              value={formData.status}
              onChange={handleChange("status")}
              placeholder="Select status"
            >
              <MenuItem value="active">Active</MenuItem>
              <MenuItem value="inactive">Inactive</MenuItem>
            </TextField>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions sx={{ p: 2 }}>
        <Button onClick={handleClose} variant="outlined">
          Cancel
        </Button>
        <Button onClick={handleSubmit} variant="contained" disabled={isLoading}>
          Update
        </Button>
      </DialogActions>
    </Dialog>
  );
};


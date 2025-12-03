"use client";

import React, { useState } from "react";
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
import { useCreateUserMutation } from "@/services/users-api";
import { CreateUserRequest } from "@/services/users-api";

interface AddUserModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

export const AddUserModal: React.FC<AddUserModalProps> = ({
  open,
  onClose,
  onSuccess,
}) => {
  const [createUser, { isLoading }] = useCreateUserMutation();
  const [formData, setFormData] = useState<CreateUserRequest>({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    mobilePhone: "",
    dateOfBirth: "",
    gender: "",
    country: "",
    state: "",
    city: "",
    postalCode: "",
    role: "user",
    status: "active",
  });

  const [errors, setErrors] = useState<
    Partial<Record<keyof CreateUserRequest, string>>
  >({});

  const handleChange =
    (field: keyof CreateUserRequest) =>
    (e: React.ChangeEvent<HTMLInputElement>) => {
      setFormData((prev) => ({ ...prev, [field]: e.target.value }));
      if (errors[field]) {
        setErrors((prev) => ({ ...prev, [field]: undefined }));
      }
    };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof CreateUserRequest, string>> = {};

    if (!formData.firstName.trim()) {
      newErrors.firstName = "First name is required";
    }
    if (!formData.lastName.trim()) {
      newErrors.lastName = "Last name is required";
    }
    if (!formData.email.trim()) {
      newErrors.email = "Email is required";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = "Invalid email format";
    }
    if (!formData.password) {
      newErrors.password = "Password is required";
    } else if (formData.password.length < 6) {
      newErrors.password = "Password must be at least 6 characters";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    if (!validate()) return;

    try {
      await createUser(formData).unwrap();
      onSuccess?.();
      handleClose();
    } catch (error: any) {
      console.error("Error creating user:", error);
    }
  };

  const handleClose = () => {
    setFormData({
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      mobilePhone: "",
      dateOfBirth: "",
      gender: "",
      country: "",
      state: "",
      city: "",
      postalCode: "",
      role: "user",
      status: "active",
    });
    setErrors({});
    onClose();
  };

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
          <Box sx={{ color: "primary.main", fontWeight: 600 }}>
            Add New User
          </Box>
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
              error={!!errors.firstName}
              helperText={errors.firstName}
              placeholder="Enter first name"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Last Name"
              value={formData.lastName}
              onChange={handleChange("lastName")}
              error={!!errors.lastName}
              helperText={errors.lastName}
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
          <Grid item xs={12}>
            <TextField
              fullWidth
              label="Password"
              type="password"
              value={formData.password}
              onChange={handleChange("password")}
              error={!!errors.password}
              helperText={errors.password}
              placeholder="Enter password"
            />
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions sx={{ p: 2 }}>
        <Button onClick={handleClose} variant="outlined">
          Cancel
        </Button>
        <Button onClick={handleSubmit} variant="contained" disabled={isLoading}>
          Add
        </Button>
      </DialogActions>
    </Dialog>
  );
};


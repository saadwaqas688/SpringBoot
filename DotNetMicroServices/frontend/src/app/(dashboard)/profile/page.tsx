"use client";

import React, { useState, useEffect } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  Avatar,
  IconButton,
  MenuItem,
  Grid,
  Paper,
  Divider,
  CircularProgress,
  Tooltip,
} from "@mui/material";
import {
  Edit as EditIcon,
  CalendarToday as CalendarIcon,
  CameraAlt as CameraIcon,
} from "@mui/icons-material";
import { useRouter } from "next/navigation";
import { useAppSelector } from "@/redux-store";
import styled from "styled-components";
import { useGetMeQuery, useUpdateProfileMutation } from "@/services/auth-api";
import { useUploadImageMutation } from "@/services/upload-api";

const ProfileContainer = styled(Box)`
  min-height: 100vh;
  background: #f5f5f5;
  padding: 24px;
`;

const PageTitle = styled(Typography)`
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 24px;
`;

const BannerContainer = styled(Paper)`
  position: relative;
  height: 200px;
  background: linear-gradient(135deg, #fbbf24 0%, #3b82f6 100%);
  border-radius: 8px;
  margin-bottom: 24px;
  overflow: hidden;
  display: flex;
  align-items: flex-end;
  padding: 0;
`;

const BannerContent = styled(Box)`
  position: relative;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: flex-end;
  padding: 24px;
`;

const ProfilePictureContainer = styled(Box)`
  position: absolute;
  bottom: -60px;
  left: 24px;
  z-index: 2;
  cursor: pointer;

  &:hover .camera-overlay {
    opacity: 1;
  }
`;

const AvatarWrapper = styled(Box)`
  position: relative;
  display: inline-block;
`;

const CameraOverlay = styled(Box)`
  position: absolute;
  bottom: 0;
  right: 0;
  background: rgba(0, 0, 0, 0.6);
  border-radius: 50%;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s;
  border: 3px solid white;

  &:hover {
    background: rgba(0, 0, 0, 0.8);
  }
`;

const HiddenInput = styled("input")`
  display: none;
`;

const ProfileInfo = styled(Box)`
  margin-left: 140px;
  margin-bottom: 16px;
  flex: 1;
`;

const UserName = styled(Typography)`
  font-size: 1.5rem;
  font-weight: 600;
  color: white;
  margin-bottom: 4px;
`;

const EditButton = styled(IconButton)`
  position: absolute;
  top: 16px;
  right: 16px;
  background: rgba(255, 255, 255, 0.9);
  &:hover {
    background: rgba(255, 255, 255, 1);
  }
`;

const FormContainer = styled(Paper)`
  padding: 32px;
  border-radius: 8px;
`;

const SectionTitle = styled(Typography)`
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 24px;
`;

const StyledTextField = styled(TextField)`
  & .MuiOutlinedInput-root {
    background-color: white;
  }
`;

const ButtonContainer = styled(Box)`
  display: flex;
  justify-content: flex-end;
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid #e5e7eb;
`;

export default function ProfilePage() {
  const router = useRouter();
  const userFromRedux = useAppSelector((state) => state.auth.user);
  const { data: profileData, isLoading, refetch } = useGetMeQuery();
  const [updateProfile, { isLoading: isUpdating }] = useUpdateProfileMutation();

  // Get user info from Redux or API
  const user =
    profileData?.data ||
    userFromRedux ||
    (() => {
      if (typeof window !== "undefined") {
        try {
          const userInfo = localStorage.getItem("user_info");
          if (userInfo) return JSON.parse(userInfo);
        } catch (error) {
          console.error("Error reading user info:", error);
        }
      }
      return null;
    })();

  const [formData, setFormData] = useState({
    name: "",
    gender: "",
    dateOfBirth: "",
    mobilePhone: "",
    email: "",
    country: "",
    state: "",
    city: "",
    postalCode: "",
    image: "",
  });

  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [uploadImage, { isLoading: isUploadingImage }] =
    useUploadImageMutation();

  useEffect(() => {
    if (user) {
      // Format date for date input (YYYY-MM-DD)
      let formattedDate = "";
      if (user.dateOfBirth || user.DateOfBirth) {
        const dob = user.dateOfBirth || user.DateOfBirth;
        if (typeof dob === "string") {
          formattedDate = dob.split("T")[0]; // Extract date part from ISO string
        } else if (dob instanceof Date) {
          formattedDate = dob.toISOString().split("T")[0];
        }
      }

      const userImageUrl = user.image || user.Image || "";

      setFormData({
        name: user.name || user.Name || "",
        gender: user.gender || user.Gender || "",
        dateOfBirth: formattedDate,
        mobilePhone: user.mobilePhone || user.MobilePhone || "",
        email: user.email || user.Email || "",
        country: user.country || user.Country || "",
        state: user.state || user.State || "",
        city: user.city || user.City || "",
        postalCode: user.postalCode || user.PostalCode || "",
        image: userImageUrl,
      });

      setImagePreview(userImageUrl);
    }
  }, [user]);

  const userName = user?.name || user?.Name || "User";
  const userEmail = user?.email || user?.Email || "";
  const userImage = user?.image || user?.Image;
  const userInitial = userName.charAt(0).toUpperCase();

  const handleInputChange =
    (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
      setFormData((prev) => ({
        ...prev,
        [field]: e.target.value,
      }));
    };

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validate file type
    if (!file.type.startsWith("image/")) {
      alert("Please select an image file");
      return;
    }

    // Validate file size (max 5MB)
    if (file.size > 5 * 1024 * 1024) {
      alert("Image size should be less than 5MB");
      return;
    }

    try {
      // Show preview immediately
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);

      // Upload image
      const imageUrl = await uploadImage(file).unwrap();

      // Update form data with the uploaded image URL
      setFormData((prev) => ({
        ...prev,
        image: imageUrl,
      }));

      // Update preview with the uploaded URL
      setImagePreview(imageUrl);

      console.log("Image uploaded successfully:", imageUrl);
    } catch (error: any) {
      console.error("Failed to upload image:", error);
      alert(
        error?.data?.message ||
          error?.message ||
          "Failed to upload image. Please try again."
      );
      // Reset preview on error
      setImagePreview(userImage || null);
    }

    // Reset input so the same file can be selected again
    e.target.value = "";
  };

  const handleSave = async () => {
    try {
      // Build update data object - only include fields that have values
      const updateData: any = {};

      // Name is always sent if present
      if (formData.name && formData.name.trim()) {
        updateData.name = formData.name.trim();
      }

      // Optional fields - only send if they have actual values (not empty strings)
      if (formData.gender && formData.gender.trim()) {
        updateData.gender = formData.gender.trim();
      }

      if (formData.dateOfBirth && formData.dateOfBirth.trim()) {
        // Convert date string to ISO format for API
        updateData.dateOfBirth = new Date(formData.dateOfBirth).toISOString();
      }

      if (formData.mobilePhone && formData.mobilePhone.trim()) {
        updateData.mobilePhone = formData.mobilePhone.trim();
      }

      // Email is not updatable - it's set during signup only

      if (formData.country && formData.country.trim()) {
        updateData.country = formData.country.trim();
      }

      if (formData.state && formData.state.trim()) {
        updateData.state = formData.state.trim();
      }

      if (formData.city && formData.city.trim()) {
        updateData.city = formData.city.trim();
      }

      if (formData.postalCode && formData.postalCode.trim()) {
        updateData.postalCode = formData.postalCode.trim();
      }

      // Include image if it was updated
      if (formData.image && formData.image.trim()) {
        updateData.image = formData.image.trim();
      }

      console.log("=== Profile Update Debug ===");
      console.log("Form Data:", formData);
      console.log("Update Data being sent:", updateData);

      const result = await updateProfile(updateData).unwrap();

      console.log("Update result:", result);

      // Refetch user data to get updated info
      await refetch();

      // Update Redux store if needed
      if (result?.data) {
        // The API will invalidate the cache, so data will refresh
      }

      // Show success message
      alert("Profile updated successfully!");
    } catch (error: any) {
      console.error("Error updating profile:", error);
      alert(
        error?.data?.message || error?.message || "Failed to update profile"
      );
    }
  };

  const handleBack = () => {
    router.back();
  };

  const genders = ["male", "female", "other"];

  return (
    <ProfileContainer>
      <PageTitle>Profile</PageTitle>

      <BannerContainer>
        <BannerContent>
          <ProfilePictureContainer>
            <Tooltip title="Click to upload profile picture">
              <AvatarWrapper
                onClick={() =>
                  !isUploadingImage &&
                  document.getElementById("profile-image-upload")?.click()
                }
                sx={{ opacity: isUploadingImage ? 0.7 : 1 }}
              >
                <Avatar
                  src={imagePreview || userImage}
                  sx={{
                    width: 120,
                    height: 120,
                    bgcolor: "#10b981",
                    border: "4px solid white",
                    fontSize: "3rem",
                    fontWeight: 600,
                  }}
                >
                  {userInitial}
                </Avatar>
                {isUploadingImage && (
                  <Box
                    sx={{
                      position: "absolute",
                      top: "50%",
                      left: "50%",
                      transform: "translate(-50%, -50%)",
                      zIndex: 3,
                    }}
                  >
                    <CircularProgress size={40} sx={{ color: "white" }} />
                  </Box>
                )}
                {!isUploadingImage && (
                  <CameraOverlay className="camera-overlay">
                    <CameraIcon sx={{ color: "white", fontSize: 20 }} />
                  </CameraOverlay>
                )}
                <HiddenInput
                  id="profile-image-upload"
                  type="file"
                  accept="image/*"
                  onChange={handleImageUpload}
                  disabled={isUploadingImage}
                />
              </AvatarWrapper>
            </Tooltip>
          </ProfilePictureContainer>
          <ProfileInfo>
            <UserName>{userName}</UserName>
          </ProfileInfo>
          <EditButton size="small">
            <EditIcon />
          </EditButton>
        </BannerContent>
      </BannerContainer>

      <FormContainer>
        <SectionTitle>Personal Information</SectionTitle>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <StyledTextField
              fullWidth
              label="Name"
              required
              value={formData.name}
              onChange={handleInputChange("name")}
              variant="outlined"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              select
              label="Gender"
              value={formData.gender}
              onChange={handleInputChange("gender")}
              variant="outlined"
            >
              {genders.map((option) => (
                <MenuItem key={option} value={option}>
                  {option.charAt(0).toUpperCase() + option.slice(1)}
                </MenuItem>
              ))}
            </StyledTextField>
          </Grid>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              label="Date of Birth"
              type="date"
              value={formData.dateOfBirth}
              onChange={handleInputChange("dateOfBirth")}
              variant="outlined"
              InputLabelProps={{
                shrink: true,
              }}
              InputProps={{
                endAdornment: <CalendarIcon sx={{ color: "#9ca3af", mr: 1 }} />,
              }}
            />
          </Grid>
        </Grid>

        <Divider sx={{ my: 4 }} />

        <SectionTitle>Contact Information</SectionTitle>
        <Grid container spacing={3}>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              label="Mobile Phone"
              value={formData.mobilePhone}
              onChange={handleInputChange("mobilePhone")}
              variant="outlined"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              label="Email"
              required
              type="email"
              value={formData.email}
              variant="outlined"
              disabled
              helperText="Email cannot be changed"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              select
              label="Country"
              value={formData.country}
              onChange={handleInputChange("country")}
              variant="outlined"
              displayEmpty
            >
              <MenuItem value="">
                <em>Select Country</em>
              </MenuItem>
              {/* Add countries list here */}
            </StyledTextField>
          </Grid>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              select
              label="State"
              value={formData.state}
              onChange={handleInputChange("state")}
              variant="outlined"
              displayEmpty
            >
              <MenuItem value="">
                <em>Select State</em>
              </MenuItem>
              {/* Add states list here */}
            </StyledTextField>
          </Grid>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              label="City"
              value={formData.city}
              onChange={handleInputChange("city")}
              variant="outlined"
              placeholder="Select City"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <StyledTextField
              fullWidth
              label="Postal Code"
              value={formData.postalCode}
              onChange={handleInputChange("postalCode")}
              variant="outlined"
              placeholder="Enter your postal code"
            />
          </Grid>
        </Grid>

        <ButtonContainer>
          <Button
            variant="outlined"
            onClick={handleBack}
            sx={{
              minWidth: 120,
              textTransform: "none",
              mr: 2,
            }}
          >
            Back
          </Button>
          <Button
            variant="contained"
            color="primary"
            onClick={handleSave}
            disabled={isUpdating}
            sx={{
              minWidth: 120,
              textTransform: "none",
            }}
          >
            {isUpdating ? "Saving..." : "Save"}
          </Button>
        </ButtonContainer>
      </FormContainer>
    </ProfileContainer>
  );
}

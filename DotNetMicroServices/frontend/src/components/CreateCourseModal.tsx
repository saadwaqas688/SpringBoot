"use client";

import { useState, useRef } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Box,
  Typography,
  IconButton,
  CircularProgress,
} from "@mui/material";
import {
  Close as CloseIcon,
  CloudUpload as CloudUploadIcon,
} from "@mui/icons-material";
import {
  useCreateCourseMutation,
  useUpdateCourseMutation,
} from "@/services/courses-api";
import { useUploadImageMutation } from "@/services/upload-api";
import { useEffect } from "react";
import MediaPreviewModal from "@/components/MediaPreviewModal";
import styled from "styled-components";

const UploadArea = styled(Box).withConfig({
  shouldForwardProp: (prop) => prop !== "hasImage",
})<{ hasImage: boolean }>`
  border: 2px dashed ${(props) => (props.hasImage ? "#6366f1" : "#e0e0e0")};
  border-radius: 8px;
  padding: 3rem 2rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: ${(props) => (props.hasImage ? "#f3f4f6" : "#ffffff")};

  &:hover {
    border-color: #6366f1;
    background-color: #f9fafb;
  }
`;

const UploadIcon = styled(Box)`
  color: #6366f1;
  margin-bottom: 1rem;
  display: flex;
  justify-content: center;
`;

const ImagePreview = styled.img`
  width: 100%;
  max-height: 200px;
  object-fit: cover;
  border-radius: 8px;
  margin-bottom: 1rem;
  cursor: pointer;
  transition: opacity 0.2s;

  &:hover {
    opacity: 0.9;
  }
`;

interface CreateCourseModalProps {
  open: boolean;
  onClose: () => void;
  courseId?: string;
  initialData?: {
    title?: string;
    description?: string;
    imageUrl?: string;
  };
}

export default function CreateCourseModal({
  open,
  onClose,
  courseId,
  initialData,
}: CreateCourseModalProps) {
  const isEditMode = !!courseId;
  const [title, setTitle] = useState(initialData?.title || "");
  const [description, setDescription] = useState(
    initialData?.description || ""
  );
  const [image, setImage] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(
    initialData?.imageUrl || null
  );
  const [errors, setErrors] = useState<{
    title?: string;
    description?: string;
  }>({});
  const [isPreviewModalOpen, setIsPreviewModalOpen] = useState(false);

  const fileInputRef = useRef<HTMLInputElement>(null);
  const [createCourse, { isLoading: isCreating }] = useCreateCourseMutation();
  const [updateCourse, { isLoading: isUpdating }] = useUpdateCourseMutation();
  const [uploadImage, { isLoading: isUploading }] = useUploadImageMutation();
  const isLoading = isCreating || isUpdating || isUploading;

  // Load initial data when editing
  useEffect(() => {
    if (open) {
      if (isEditMode && initialData) {
        console.log("Loading edit data:", {
          courseId,
          initialData,
          isEditMode,
          open,
        });
        const newTitle = initialData.title || "";
        const newDescription = initialData.description || "";
        const newImagePreview = initialData.imageUrl || null;

        console.log("Setting form values:", {
          newTitle,
          newDescription,
          newImagePreview,
        });
        setTitle(newTitle);
        setDescription(newDescription);
        setImagePreview(newImagePreview);
        setImage(null); // Reset file input
        setErrors({});
      } else if (!isEditMode) {
        // Reset form when creating
        setTitle("");
        setDescription("");
        setImage(null);
        setImagePreview(null);
        setErrors({});
      }
    } else {
      // Reset when modal closes
      if (!isEditMode) {
        setTitle("");
        setDescription("");
        setImage(null);
        setImagePreview(null);
        setErrors({});
      }
    }
  }, [
    open,
    isEditMode,
    courseId,
    initialData?.title,
    initialData?.description,
    initialData?.imageUrl,
  ]);

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImage(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleUploadClick = () => {
    fileInputRef.current?.click();
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    const file = e.dataTransfer.files[0];
    if (file && file.type.startsWith("image/")) {
      setImage(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
  };

  const validateForm = () => {
    const newErrors: typeof errors = {};

    if (!title.trim()) {
      newErrors.title = "Course title is required";
    }

    if (!description.trim()) {
      newErrors.description = "Description is required";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    if (!validateForm()) {
      return;
    }

    try {
      let thumbnailUrl: string | undefined = undefined;

      // Upload image if a new one was selected
      if (image) {
        try {
          thumbnailUrl = await uploadImage(image).unwrap();
          console.log("Image uploaded successfully:", thumbnailUrl);
        } catch (uploadError) {
          console.error("Failed to upload image:", uploadError);
          alert("Failed to upload image. Please try again.");
          return;
        }
      } else if (imagePreview && !image) {
        // Use existing image URL if no new image was selected
        thumbnailUrl = imagePreview;
      }

      const courseData: any = {
        title,
        description,
        thumbnail: thumbnailUrl,
      };

      if (isEditMode && courseId) {
        console.log("Updating course with ID:", courseId, "Data:", courseData);
        const courseIdString = String(courseId);
        // Send with capital property names to match backend Course model
        const updateData = {
          id: courseIdString,
          Title: courseData.title,
          Description: courseData.description,
          Thumbnail: courseData.thumbnail,
        };
        await updateCourse(updateData).unwrap();
        console.log("Course updated successfully");
      } else {
        console.log("Creating course with data:", courseData);
        // Send with capital property names to match backend Course model
        const createData = {
          Title: courseData.title,
          Description: courseData.description,
          Thumbnail: courseData.thumbnail,
        };
        await createCourse(createData).unwrap();
        console.log("Course created successfully");
      }

      // Reset form
      setTitle("");
      setDescription("");
      setImage(null);
      setImagePreview(null);
      setErrors({});
      onClose();
    } catch (error) {
      console.error(
        `Failed to ${isEditMode ? "update" : "create"} course:`,
        error
      );
      // You can add error handling/toast notification here
    }
  };

  const handleClose = () => {
    if (!isLoading) {
      setTitle("");
      setDescription("");
      setImage(null);
      setImagePreview(null);
      setErrors({});
      onClose();
    }
  };

  return (
    <>
      <Dialog
        open={open}
        onClose={handleClose}
        maxWidth="sm"
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: 2,
          },
        }}
      >
        <DialogTitle
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            pb: 2,
            fontWeight: 600,
            color: "#6366f1",
          }}
        >
          {isEditMode ? "Edit Course" : "Create Course"}
          <IconButton
            onClick={handleClose}
            disabled={isLoading}
            sx={{ color: "text.secondary" }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>

        <DialogContent>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 3, pt: 1 }}>
            {/* Upload Image Section */}
            <Box>
              <Typography variant="body2" sx={{ mb: 1, fontWeight: 500 }}>
                Upload Image
              </Typography>
              <input
                type="file"
                ref={fileInputRef}
                onChange={handleImageChange}
                accept="image/*"
                style={{ display: "none" }}
              />
              <UploadArea
                hasImage={!!imagePreview}
                onClick={handleUploadClick}
                onDrop={handleDrop}
                onDragOver={handleDragOver}
              >
                {imagePreview ? (
                  <>
                    <ImagePreview
                      src={imagePreview}
                      alt="Course preview"
                      onClick={(e) => {
                        e.stopPropagation();
                        if (imagePreview) {
                          setIsPreviewModalOpen(true);
                        }
                      }}
                    />
                    <Button
                      variant="outlined"
                      size="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleUploadClick();
                      }}
                      sx={{ textTransform: "none" }}
                    >
                      Change Image
                    </Button>
                  </>
                ) : (
                  <>
                    <UploadIcon>
                      <CloudUploadIcon sx={{ fontSize: 48 }} />
                    </UploadIcon>
                    <Typography variant="body2" color="text.secondary">
                      Upload Image here
                    </Typography>
                  </>
                )}
              </UploadArea>
            </Box>

            {/* Course Title */}
            <TextField
              label="Course Title"
              required
              fullWidth
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              error={!!errors.title}
              helperText={errors.title}
              placeholder="Enter Course Title"
              disabled={isLoading}
            />

            {/* Description */}
            <TextField
              label="Description"
              required
              fullWidth
              multiline
              rows={4}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              error={!!errors.description}
              helperText={errors.description}
              placeholder="Add a Description"
              disabled={isLoading}
            />
          </Box>
        </DialogContent>

        <DialogActions sx={{ px: 3, pb: 3, gap: 2 }}>
          <Button
            onClick={handleClose}
            disabled={isLoading}
            variant="outlined"
            sx={{
              textTransform: "none",
              borderColor: "#6366f1",
              color: "#6366f1",
              "&:hover": {
                borderColor: "#4f46e5",
                backgroundColor: "rgba(99, 102, 241, 0.04)",
              },
            }}
          >
            Cancel
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={isLoading}
            variant="contained"
            sx={{
              textTransform: "none",
              bgcolor: "#6366f1",
              "&:hover": {
                bgcolor: "#4f46e5",
              },
            }}
          >
            {isLoading ? (
              <CircularProgress size={24} color="inherit" />
            ) : isEditMode ? (
              "Update"
            ) : (
              "Create"
            )}
          </Button>
        </DialogActions>
      </Dialog>

      <MediaPreviewModal
        open={isPreviewModalOpen}
        onClose={() => setIsPreviewModalOpen(false)}
        url={imagePreview || ""}
        type="image"
        alt="Course preview"
      />
    </>
  );
}

import { useState } from "react";

export const useModals = () => {
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [isAssignModalOpen, setIsAssignModalOpen] = useState(false);
  const [isQuizUploadModalOpen, setIsQuizUploadModalOpen] = useState(false);
  const [isSlideLibraryOpen, setIsSlideLibraryOpen] = useState(false);
  const [isSlideEditorOpen, setIsSlideEditorOpen] = useState(false);
  const [isPreviewModalOpen, setIsPreviewModalOpen] = useState(false);
  const [mobileLessonsOpen, setMobileLessonsOpen] = useState(false);
  const [mobilePropertiesOpen, setMobilePropertiesOpen] = useState(false);
  const [selectedSlideTab, setSelectedSlideTab] = useState(0);
  const [previewUrl, setPreviewUrl] = useState<string>("");
  const [previewType, setPreviewType] = useState<"image" | "video">("image");
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  // Wrapper for setIsQuizUploadModalOpen with logging
  const setIsQuizUploadModalOpenWithLog = (value: boolean) => {
    console.log("=== setIsQuizUploadModalOpen CALLED ===");
    console.log("New value:", value);
    console.log("Current value:", isQuizUploadModalOpen);
    
    // Directly set the state
    setIsQuizUploadModalOpen(value);
    
    // Force a re-render check
    console.log("State setter called, component should re-render");
  };

  const handleOpenPreview = (url: string, type: "image" | "video") => {
    if (!url) return;
    setPreviewUrl(url);
    setPreviewType(type);
    setIsPreviewModalOpen(true);
  };

  const handleClosePreview = () => {
    setIsPreviewModalOpen(false);
    setPreviewUrl("");
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  return {
    // Delete modal
    isDeleteModalOpen,
    setIsDeleteModalOpen,
    // Assign modal
    isAssignModalOpen,
    setIsAssignModalOpen,
    // Quiz upload modal
    isQuizUploadModalOpen,
    setIsQuizUploadModalOpen: setIsQuizUploadModalOpenWithLog,
    // Slide library
    isSlideLibraryOpen,
    setIsSlideLibraryOpen,
    selectedSlideTab,
    setSelectedSlideTab,
    // Slide editor
    isSlideEditorOpen,
    setIsSlideEditorOpen,
    // Preview modal
    isPreviewModalOpen,
    setIsPreviewModalOpen,
    previewUrl,
    previewType,
    handleOpenPreview,
    handleClosePreview,
    // Mobile
    mobileLessonsOpen,
    setMobileLessonsOpen,
    mobilePropertiesOpen,
    setMobilePropertiesOpen,
    // Menu
    anchorEl,
    setAnchorEl,
    handleMenuClose,
  };
};


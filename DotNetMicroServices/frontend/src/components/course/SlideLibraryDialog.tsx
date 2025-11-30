import React from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  IconButton,
  Box,
  Tabs,
  Tab,
  Grid,
  Card,
  CardContent,
  Typography,
} from "@mui/material";
import {
  Close as CloseIcon,
  Description as DescriptionIcon,
  Image as ImageIcon,
  VideoLibrary as VideoIcon,
} from "@mui/icons-material";

interface SlideLibraryDialogProps {
  open: boolean;
  onClose: () => void;
  selectedTab: number;
  onTabChange: (value: number) => void;
  onCreateSlide: (slideType: string) => void;
}

const SLIDE_TYPES = {
  text: [
    { type: "bulleted-list", label: "Bulleted list", description: "Show a list of bullet points" },
    { type: "comparison", label: "Comparison", description: "Compare two text blocks" },
    { type: "expandable-list", label: "Expandable list", description: "Show a list of concepts" },
  ],
  image: [
    { type: "single-image", label: "Single Image", description: "Display a single image with text" },
    { type: "image-collection", label: "Image Collection", description: "Display a collection of three images" },
  ],
  video: [
    { type: "single-video", label: "Single Video", description: "Display a single video with text" },
    { type: "video-collection", label: "Video Collection", description: "Display a collection of three videos" },
  ],
};

export const SlideLibraryDialog: React.FC<SlideLibraryDialogProps> = ({
  open,
  onClose,
  selectedTab,
  onTabChange,
  onCreateSlide,
}) => {
  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: 2,
          maxHeight: { xs: "95vh", sm: "90vh" },
          margin: { xs: 1, sm: 2 },
          width: { xs: "calc(100% - 16px)", sm: "auto" },
        },
      }}
    >
      <DialogTitle
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          pb: 1,
          borderBottom: "1px solid #e5e7eb",
          fontWeight: 600,
        }}
      >
        Slide Library
        <IconButton onClick={onClose} sx={{ color: "text.secondary" }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>

      <DialogContent sx={{ p: 0, overflow: "hidden" }}>
        <Box
          sx={{
            display: "flex",
            flexDirection: { xs: "column", sm: "row" },
            height: { xs: "auto", sm: "calc(90vh - 120px)" },
            maxHeight: { xs: "80vh", sm: "calc(90vh - 120px)" },
          }}
        >
          {/* Left Sidebar - Tabs */}
          <Box
            sx={{
              width: { xs: "100%", sm: 200 },
              borderRight: "1px solid #e5e7eb",
              borderBottom: { xs: "1px solid #e5e7eb", sm: "none" },
              pt: 2,
              backgroundColor: "#f9fafb",
              flexShrink: 0,
            }}
          >
            <Tabs
              orientation="vertical"
              value={selectedTab}
              onChange={(_, newValue) => onTabChange(newValue)}
              variant="standard"
              sx={{
                width: "100%",
                "& .MuiTabs-flexContainer": {
                  flexDirection: "column",
                  alignItems: "stretch",
                },
                "& .MuiTab-root": {
                  textTransform: "none",
                  alignItems: "flex-start",
                  minHeight: 48,
                  fontSize: "0.875rem",
                  color: "#6b7280",
                  justifyContent: "flex-start",
                  px: 2,
                  "&.Mui-selected": {
                    color: "#6366f1",
                    backgroundColor: "#eef2ff",
                  },
                },
              }}
            >
              <Tab
                icon={<DescriptionIcon />}
                iconPosition="start"
                label="Text"
                sx={{ pl: 2 }}
              />
              <Tab
                icon={<ImageIcon />}
                iconPosition="start"
                label="Image"
                sx={{ pl: 2 }}
              />
              <Tab
                icon={<VideoIcon />}
                iconPosition="start"
                label="Video"
                sx={{ pl: 2 }}
              />
            </Tabs>
          </Box>

          {/* Right Content Area */}
          <Box
            sx={{
              flex: 1,
              p: { xs: 2, sm: 3 },
              overflowY: "auto",
              overflowX: "hidden",
              maxHeight: { xs: "60vh", sm: "calc(90vh - 120px)" },
            }}
          >
            {selectedTab === 0 && (
              <Grid container spacing={2}>
                {SLIDE_TYPES.text.map((slide) => (
                  <Grid item xs={12} sm={6} md={4} key={slide.type}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={(e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        console.log("Card clicked, slide type:", slide.type);
                        onCreateSlide(slide.type);
                      }}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                          }}
                        >
                          <DescriptionIcon sx={{ fontSize: 48, color: "#6366f1" }} />
                        </Box>
                        <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 0.5 }}>
                          {slide.label}
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          {slide.description}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            )}

            {selectedTab === 1 && (
              <Grid container spacing={2}>
                {SLIDE_TYPES.image.map((slide) => (
                  <Grid item xs={12} sm={6} md={4} key={slide.type}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={(e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        console.log("Card clicked, slide type:", slide.type);
                        onCreateSlide(slide.type);
                      }}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                            gap: slide.type === "image-collection" ? 1 : 0,
                          }}
                        >
                          {slide.type === "image-collection" ? (
                            <>
                              <ImageIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                              <ImageIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                              <ImageIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                            </>
                          ) : (
                            <ImageIcon sx={{ fontSize: 48, color: "#6366f1" }} />
                          )}
                        </Box>
                        <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 0.5 }}>
                          {slide.label}
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          {slide.description}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            )}

            {selectedTab === 2 && (
              <Grid container spacing={2}>
                {SLIDE_TYPES.video.map((slide) => (
                  <Grid item xs={12} sm={6} md={4} key={slide.type}>
                    <Card
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.2s",
                        "&:hover": {
                          boxShadow: 4,
                          transform: "translateY(-2px)",
                        },
                      }}
                      onClick={(e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        console.log("Card clicked, slide type:", slide.type);
                        onCreateSlide(slide.type);
                      }}
                    >
                      <CardContent>
                        <Box
                          sx={{
                            height: 120,
                            backgroundColor: "#e0e7ff",
                            borderRadius: 1,
                            mb: 2,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            border: "1px solid #c7d2fe",
                            gap: slide.type === "video-collection" ? 1 : 0,
                          }}
                        >
                          {slide.type === "video-collection" ? (
                            <>
                              <VideoIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                              <VideoIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                              <VideoIcon sx={{ fontSize: 32, color: "#6366f1" }} />
                            </>
                          ) : (
                            <VideoIcon sx={{ fontSize: 48, color: "#6366f1" }} />
                          )}
                        </Box>
                        <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 0.5 }}>
                          {slide.label}
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.75rem" }}
                        >
                          {slide.description}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            )}
          </Box>
        </Box>
      </DialogContent>
    </Dialog>
  );
};


import { Box, Typography } from "@mui/material";

export const EmptySlideState: React.FC = () => {
  return (
    <Box
      sx={{
        flex: 1,
        backgroundColor: "white",
        borderRadius: 2,
        p: { xs: 2, sm: 3, md: 4 },
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
        minHeight: { xs: 400, md: "auto" },
      }}
    >
      <Typography variant="h6" sx={{ textAlign: "center", color: "text.secondary" }}>
        Select a slide to preview
      </Typography>
    </Box>
  );
};


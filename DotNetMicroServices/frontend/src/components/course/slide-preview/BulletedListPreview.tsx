import { Box, Typography } from "@mui/material";

interface BulletedListPreviewProps {
  bulletPoints: string[];
}

export const BulletedListPreview: React.FC<BulletedListPreviewProps> = ({
  bulletPoints,
}) => {
  return (
    <Box>
      {bulletPoints.map((point, index) => (
        <Box
          key={index}
          sx={{
            display: "flex",
            alignItems: "flex-start",
            mb: 2,
          }}
        >
          <Typography
            sx={{
              fontSize: "1.5rem",
              lineHeight: 1,
              mr: 2,
              color: "#6366f1",
            }}
          >
            •
          </Typography>
          <Typography
            variant="body1"
            sx={{
              fontSize: "1.125rem",
              color: "#374151",
              flex: 1,
            }}
          >
            {point || `Bullet point ${index + 1}`}
          </Typography>
        </Box>
      ))}
    </Box>
  );
};


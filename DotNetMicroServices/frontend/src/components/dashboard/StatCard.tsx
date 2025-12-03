import { Box, Typography } from "@mui/material";
import { ReactNode } from "react";

interface StatCardProps {
  title: string;
  value: number | string;
  icon: ReactNode;
  iconColor?: string;
  backgroundColor?: string;
}

export const StatCard: React.FC<StatCardProps> = ({
  title,
  value,
  icon,
  iconColor = "#6b7280",
  backgroundColor = "#f9fafb",
}) => {
  return (
    <Box
      sx={{
        backgroundColor: "white",
        borderRadius: 2,
        p: 3,
        boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
        display: "flex",
        alignItems: "center",
        gap: 2,
        transition: "transform 0.2s, box-shadow 0.2s",
        "&:hover": {
          transform: "translateY(-2px)",
          boxShadow: "0 4px 12px rgba(0,0,0,0.15)",
        },
      }}
    >
      <Box
        sx={{
          width: 56,
          height: 56,
          borderRadius: 2,
          backgroundColor,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          flexShrink: 0,
        }}
      >
        <Box sx={{ color: iconColor, fontSize: 28 }}>{icon}</Box>
      </Box>
      <Box sx={{ flex: 1, minWidth: 0 }}>
        <Typography
          variant="body2"
          sx={{
            color: "#6b7280",
            fontSize: "0.875rem",
            fontWeight: 500,
            mb: 0.5,
          }}
        >
          {title}
        </Typography>
        <Typography
          variant="h4"
          sx={{
            color: "#1f2937",
            fontSize: "1.875rem",
            fontWeight: 700,
            lineHeight: 1.2,
          }}
        >
          {value.toLocaleString()}
        </Typography>
      </Box>
    </Box>
  );
};


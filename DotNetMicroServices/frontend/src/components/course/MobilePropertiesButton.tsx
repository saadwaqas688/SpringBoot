import { IconButton } from "@mui/material";
import { Settings as SettingsIcon } from "@mui/icons-material";

interface MobilePropertiesButtonProps {
  onClick: () => void;
  top?: number | string;
}

export const MobilePropertiesButton: React.FC<MobilePropertiesButtonProps> = ({
  onClick,
  top = 70,
}) => {
  return (
    <IconButton
      sx={{
        display: { xs: "flex", lg: "none" },
        position: "fixed",
        top: { xs: top, sm: 80 },
        right: 10,
        zIndex: 1200,
        bgcolor: "#6366f1",
        color: "white",
        width: 40,
        height: 40,
        boxShadow: 2,
        "&:hover": { bgcolor: "#4f46e5" },
      }}
      onClick={onClick}
    >
      <SettingsIcon />
    </IconButton>
  );
};


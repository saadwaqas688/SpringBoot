import { IconButton } from "@mui/material";
import { Menu as MenuIcon } from "@mui/icons-material";

interface MobileMenuButtonProps {
  onClick: () => void;
  top?: number | string;
}

export const MobileMenuButton: React.FC<MobileMenuButtonProps> = ({
  onClick,
  top = 70,
}) => {
  return (
    <IconButton
      sx={{
        display: { xs: "flex", lg: "none" },
        position: "fixed",
        top: { xs: top, sm: 80 },
        left: 10,
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
      <MenuIcon />
    </IconButton>
  );
};


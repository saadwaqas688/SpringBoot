import { Box } from "@mui/material";

interface MobileBackdropProps {
  open: boolean;
  onClose: () => void;
}

export const MobileBackdrop: React.FC<MobileBackdropProps> = ({
  open,
  onClose,
}) => {
  if (!open) return null;

  return (
    <Box
      sx={{
        display: { xs: "block", lg: "none" },
        position: "fixed",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        bgcolor: "rgba(0, 0, 0, 0.5)",
        zIndex: 1100,
      }}
      onClick={onClose}
    />
  );
};


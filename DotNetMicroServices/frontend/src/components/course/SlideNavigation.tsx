import { Box, IconButton } from "@mui/material";
import {
  ArrowBack as ArrowBackIcon,
  ArrowForward as ArrowForwardIcon,
} from "@mui/icons-material";

interface SlideNavigationProps {
  onPrevious: () => void;
  onNext: () => void;
  isFirstSlide: boolean;
  isLastSlide: boolean;
}

export const SlideNavigation: React.FC<SlideNavigationProps> = ({
  onPrevious,
  onNext,
  isFirstSlide,
  isLastSlide,
}) => {
  return (
    <Box sx={{ display: "flex", gap: 1 }}>
      <IconButton
        onClick={onPrevious}
        disabled={isFirstSlide}
        sx={{
          "&:disabled": {
            opacity: 0.5,
          },
        }}
      >
        <ArrowBackIcon />
      </IconButton>
      <IconButton
        onClick={onNext}
        disabled={isLastSlide}
        sx={{
          "&:disabled": {
            opacity: 0.5,
          },
        }}
      >
        <ArrowForwardIcon />
      </IconButton>
    </Box>
  );
};


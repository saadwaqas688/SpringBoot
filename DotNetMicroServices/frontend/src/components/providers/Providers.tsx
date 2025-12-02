"use client";

import { Provider } from "react-redux";
import { PersistGate } from "redux-persist/integration/react";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import { store, persistor } from "@/redux-store";

const theme = createTheme({
  palette: {
    primary: {
      main: "#6366f1", // Indigo/purple
      light: "#818cf8",
      dark: "#4f46e5",
    },
    secondary: {
      main: "#8b5cf6", // Purple
      light: "#a78bfa",
      dark: "#7c3aed",
    },
    background: {
      default: "#f5f5f5",
      paper: "#ffffff",
    },
  },
  typography: {
    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
  },
  shape: {
    borderRadius: 8,
  },
});

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <Provider store={store}>
      <PersistGate loading={null} persistor={persistor}>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          {children}
        </ThemeProvider>
      </PersistGate>
    </Provider>
  );
}





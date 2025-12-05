import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  user: any | null;
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  accessToken: null,
  refreshToken: null,
  user: null,
  isAuthenticated: false,
};

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{
        accessToken: string;
        refreshToken?: string;
        user?: any;
      }>
    ) => {
      state.accessToken = action.payload.accessToken;
      state.refreshToken = action.payload.refreshToken || null;
      state.user = action.payload.user || null;
      state.isAuthenticated = true;

      // Also store in localStorage as backup (redux-persist handles this, but this is extra safety)
      if (typeof window !== "undefined") {
        try {
          if (action.payload.accessToken) {
            localStorage.setItem("auth_token", action.payload.accessToken);
          }
          if (action.payload.user) {
            localStorage.setItem("user_info", JSON.stringify(action.payload.user));
          }
        } catch (error) {
          console.error("Error storing auth data in localStorage:", error);
        }
      }
    },
    clearCredentials: (state) => {
      state.accessToken = null;
      state.refreshToken = null;
      state.user = null;
      state.isAuthenticated = false;

      // Clear localStorage as well
      if (typeof window !== "undefined") {
        try {
          localStorage.removeItem("auth_token");
          localStorage.removeItem("user_info");
        } catch (error) {
          console.error("Error clearing auth data from localStorage:", error);
        }
      }
    },
    updateUser: (state, action: PayloadAction<any>) => {
      state.user = action.payload;
    },
  },
});

export const { setCredentials, clearCredentials, updateUser } =
  authSlice.actions;
export default authSlice.reducer;














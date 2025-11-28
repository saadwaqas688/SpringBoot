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
    },
    clearCredentials: (state) => {
      state.accessToken = null;
      state.refreshToken = null;
      state.user = null;
      state.isAuthenticated = false;
    },
    updateUser: (state, action: PayloadAction<any>) => {
      state.user = action.payload;
    },
  },
});

export const { setCredentials, clearCredentials, updateUser } =
  authSlice.actions;
export default authSlice.reducer;


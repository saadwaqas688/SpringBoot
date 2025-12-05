"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import {
  Box,
  Container,
  Paper,
  TextField,
  Button,
  Typography,
  Link,
  InputAdornment,
  IconButton,
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import { useForm } from "react-hook-form";
import { useSignInMutation } from "@/services/auth-api";
import { useAppDispatch } from "@/redux-store";
import { setCredentials } from "@/redux-store";
import styled from "styled-components";

const SignInContainer = styled(Box)`
  min-height: 100vh;
  display: flex;
  background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%);
  position: relative;
  overflow: hidden;
`;

const BlobShape = styled(Box)`
  position: absolute;
  border-radius: 30% 70% 70% 30% / 30% 30% 70% 70%;
  background: rgba(99, 102, 241, 0.1);
  width: 400px;
  height: 400px;
  animation: blob 20s infinite;

  @keyframes blob {
    0%,
    100% {
      transform: translate(0, 0) scale(1);
    }
    33% {
      transform: translate(30px, -50px) scale(1.1);
    }
    66% {
      transform: translate(-20px, 20px) scale(0.9);
    }
  }
`;

const LeftSection = styled(Box)`
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  position: relative;
  z-index: 1;
`;

const RightSection = styled(Box)`
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  position: relative;
  z-index: 1;
`;

const LogoContainer = styled(Box)`
  text-align: center;
`;

const LogoText = styled(Typography)`
  font-size: 3rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
`;

const LogoSquare = styled(Box)`
  display: inline-block;
  width: 60px;
  height: 60px;
  border: 3px solid #6366f1;
  border-radius: 8px;
  position: relative;
  margin-left: 10px;
  vertical-align: middle;
`;

const Checkmark = styled(Box)`
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 20px;
  height: 20px;
  border: 3px solid #ef4444;
  border-top: none;
  border-right: none;
  transform: translate(-50%, -60%) rotate(-45deg);
`;

const FormPaper = styled(Paper)`
  padding: 3rem;
  max-width: 450px;
  width: 100%;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
`;

const SignInButton = styled(Button)`
  background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
  color: white;
  padding: 12px;
  font-size: 1rem;
  font-weight: 600;
  text-transform: none;
  border-radius: 8px;
  margin-top: 1rem;

  &:hover {
    background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
  }
`;

interface SignInForm {
  email: string;
  password: string;
}

export default function SignInPage() {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const [showPassword, setShowPassword] = useState(false);
  const [signIn, { isLoading }] = useSignInMutation();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<SignInForm>();

  const onSubmit = async (data: SignInForm) => {
    try {
      const result = await signIn(data).unwrap();
      if (result.success && result.data) {
        const token = result.data.token || result.data.accessToken || result.data.Token;
        const user = result.data.user || result.data.User;

        // Store in Redux (which will persist to localStorage via redux-persist)
        dispatch(
          setCredentials({
            accessToken: token,
            refreshToken: result.data.refreshToken,
            user: user,
          })
        );

        // Also explicitly store in localStorage as backup
        if (typeof window !== "undefined" && token && user) {
          try {
            localStorage.setItem("auth_token", token);
            localStorage.setItem("user_info", JSON.stringify(user));
          } catch (error) {
            console.error("Error storing auth data in localStorage:", error);
          }
        }

        router.push("/dashboard");
      }
    } catch (error: any) {
      console.error("Sign in error:", error);
      alert(error?.data?.message || "Sign in failed");
    }
  };

  return (
    <SignInContainer>
      <BlobShape style={{ top: "10%", left: "5%" }} />
      <BlobShape style={{ bottom: "10%", right: "5%", animationDelay: "2s" }} />

      <LeftSection>
        <LogoContainer>
          <Box>
            <LogoText component="span">Compliance</LogoText>
            <LogoSquare>
              <Checkmark />
            </LogoSquare>
          </Box>
          <LogoText component="span" style={{ marginTop: "-10px" }}>
            Sheet
          </LogoText>
        </LogoContainer>
      </LeftSection>

      <RightSection>
        <FormPaper elevation={3}>
          <Typography
            variant="h4"
            component="h1"
            gutterBottom
            sx={{ color: "#6366f1", fontWeight: 600, mb: 1 }}
          >
            Sign In
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Let&apos;s Sign You In
          </Typography>

          <form onSubmit={handleSubmit(onSubmit)}>
            <TextField
              fullWidth
              label="Email *"
              type="email"
              placeholder="Enter your email"
              {...register("email", {
                required: "Email is required",
                pattern: {
                  value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                  message: "Invalid email address",
                },
              })}
              error={!!errors.email}
              helperText={errors.email?.message}
              sx={{ mb: 2 }}
            />

            <TextField
              fullWidth
              label="Password *"
              type={showPassword ? "text" : "password"}
              placeholder="Enter your password"
              {...register("password", {
                required: "Password is required",
                minLength: {
                  value: 6,
                  message: "Password must be at least 6 characters",
                },
              })}
              error={!!errors.password}
              helperText={errors.password?.message}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword(!showPassword)}
                      edge="end"
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
              sx={{ mb: 2 }}
            />

            <SignInButton
              type="submit"
              fullWidth
              variant="contained"
              disabled={isLoading}
            >
              {isLoading ? "Signing In..." : "Sign In"}
            </SignInButton>

            <Box sx={{ mt: 2, textAlign: "center" }}>
              <Link
                href="/forgot-password"
                sx={{ color: "#6366f1", textDecoration: "none" }}
              >
                Forgot Password?
              </Link>
            </Box>

            <Box sx={{ mt: 2, textAlign: "center" }}>
              <Link
                href="/sign-up"
                sx={{ color: "#6366f1", textDecoration: "none" }}
              >
                Sign Up
              </Link>
            </Box>
          </form>
        </FormPaper>
      </RightSection>
    </SignInContainer>
  );
}















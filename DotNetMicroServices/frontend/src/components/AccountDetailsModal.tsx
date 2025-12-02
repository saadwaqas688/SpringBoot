"use client";

import React from "react";
import {
  Drawer,
  Box,
  Typography,
  Avatar,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import {
  Close as CloseIcon,
  Person as PersonIcon,
  Lock as LockIcon,
} from "@mui/icons-material";
import styled from "styled-components";

const DrawerContent = styled(Box)`
  width: 320px;
  height: 100%;
  display: flex;
  flex-direction: column;
  background: white;
`;

const Header = styled(Box)`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px 20px 16px;
  border-bottom: 1px solid #e5e7eb;
`;

const ProfileSection = styled(Box)`
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 32px 20px;
  border-bottom: 1px solid #e5e7eb;
`;

const UserName = styled(Typography)`
  font-size: 1.25rem;
  font-weight: 600;
  margin-top: 16px;
  color: #1f2937;
`;

const UserEmail = styled(Typography)`
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 4px;
`;

const MenuList = styled(List)`
  padding: 8px 0;
  flex: 1;
`;

interface AccountDetailsModalProps {
  open: boolean;
  onClose: () => void;
  userName?: string;
  userEmail?: string;
  userImage?: string;
  userInitial?: string;
  onMyProfileClick?: () => void;
  onChangePasswordClick?: () => void;
}

export default function AccountDetailsModal({
  open,
  onClose,
  userName = "User",
  userEmail = "",
  userImage,
  userInitial = "U",
  onMyProfileClick,
  onChangePasswordClick,
}: AccountDetailsModalProps) {
  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      PaperProps={{
        sx: {
          width: 320,
          boxShadow: "-2px 0 8px rgba(0, 0, 0, 0.1)",
        },
      }}
      ModalProps={{
        BackdropProps: {
          sx: {
            backgroundColor: "rgba(0, 0, 0, 0.5)",
          },
        },
      }}
    >
      <DrawerContent>
        <Header>
          <Typography
            variant="h6"
            sx={{ fontWeight: 600, fontSize: "1.125rem" }}
          >
            Account Details
          </Typography>
          <IconButton
            onClick={onClose}
            size="small"
            sx={{
              color: "#6b7280",
              "&:hover": {
                backgroundColor: "#f3f4f6",
              },
            }}
          >
            <CloseIcon />
          </IconButton>
        </Header>

        <ProfileSection>
          <Avatar
            src={userImage}
            sx={{
              width: 100,
              height: 100,
              bgcolor: "#10b981",
              border: "3px solid #3b82f6",
              fontSize: "2.5rem",
              fontWeight: 600,
            }}
          >
            {userInitial}
          </Avatar>
          <UserName>{userName}</UserName>
          {userEmail && <UserEmail>{userEmail}</UserEmail>}
        </ProfileSection>

        <MenuList>
          <ListItem disablePadding>
            <ListItemButton
              onClick={() => {
                if (onMyProfileClick) {
                  onMyProfileClick();
                }
                onClose();
              }}
              sx={{
                py: 1.5,
                px: 3,
                "&:hover": {
                  backgroundColor: "#f3f4f6",
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: 40 }}>
                <PersonIcon sx={{ color: "#6b7280" }} />
              </ListItemIcon>
              <ListItemText
                primary="My Profile"
                primaryTypographyProps={{
                  fontSize: "0.9375rem",
                  color: "#1f2937",
                }}
              />
            </ListItemButton>
          </ListItem>

          <ListItem disablePadding>
            <ListItemButton
              onClick={() => {
                if (onChangePasswordClick) {
                  onChangePasswordClick();
                }
                onClose();
              }}
              sx={{
                py: 1.5,
                px: 3,
                "&:hover": {
                  backgroundColor: "#f3f4f6",
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: 40 }}>
                <LockIcon sx={{ color: "#6b7280" }} />
              </ListItemIcon>
              <ListItemText
                primary="Change Password"
                primaryTypographyProps={{
                  fontSize: "0.9375rem",
                  color: "#1f2937",
                }}
              />
            </ListItemButton>
          </ListItem>
        </MenuList>
      </DrawerContent>
    </Drawer>
  );
}

"use client";

import {
  Box,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  AppBar,
  Toolbar,
  Typography,
  Avatar,
  IconButton,
  InputBase,
} from "@mui/material";
import { useState } from "react";
import {
  Dashboard as DashboardIcon,
  Folder as FolderIcon,
  Quiz as QuizIcon,
  People as PeopleIcon,
  Settings as SettingsIcon,
  Logout as LogoutIcon,
  Search as SearchIcon,
  Notifications as NotificationsIcon,
  Menu as MenuIcon,
  ChatBubble as ChatIcon,
} from "@mui/icons-material";
import { useRouter } from "next/navigation";
import { useAppDispatch, useAppSelector } from "@/redux-store";
import { clearCredentials } from "@/redux-store";
import styled from "styled-components";

const drawerWidth = 280;

const Sidebar = styled(Box)`
  background: linear-gradient(180deg, #6366f1 0%, #8b5cf6 100%);
  height: 100vh;
  color: white;
`;

const MainContent = styled(Box)`
  flex-grow: 1;
  background: #f5f5f5;
  min-height: 100vh;
`;

const Header = styled(AppBar)`
  background: white;
  color: #1f2937;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
`;

const SearchBox = styled(Box)`
  display: flex;
  align-items: center;
  background: #f3f4f6;
  border-radius: 8px;
  padding: 8px 16px;
  width: 300px;
`;

const menuItems = [
  { text: "Dashboard", icon: DashboardIcon, path: "/dashboard" },
  {
    text: "Content",
    icon: FolderIcon,
    path: "/courses",
    children: [
      { text: "Courses", path: "/courses" },
      { text: "Course Library", path: "/courses/library" },
      { text: "Rapid Refresh Quizzes", path: "/quizzes" },
      { text: "Paths", path: "/paths" },
      { text: "Meetings", path: "/meetings" },
    ],
  },
  { text: "Engage", icon: ChatIcon, path: "/engage" },
  { text: "Facilitate", icon: PeopleIcon, path: "/facilitate" },
  { text: "Insights", icon: QuizIcon, path: "/insights" },
  { text: "User Management", icon: PeopleIcon, path: "/user-management" },
  { text: "Settings", icon: SettingsIcon, path: "/settings" },
];

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [expandedItems, setExpandedItems] = useState<string[]>(["Content"]);
  const user = useAppSelector((state) => state.auth.user);

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const toggleExpand = (item: string) => {
    setExpandedItems((prev) =>
      prev.includes(item) ? prev.filter((i) => i !== item) : [...prev, item]
    );
  };

  const handleLogout = () => {
    dispatch(clearCredentials());
    router.push("/sign-in");
  };

  const drawer = (
    <Sidebar>
      <Box sx={{ p: 3, textAlign: "center" }}>
        <Typography variant="h6" sx={{ fontWeight: 700 }}>
          Compliance Sheet L&D
        </Typography>
      </Box>
      <List>
        {menuItems.map((item) => (
          <Box key={item.text}>
            <ListItem disablePadding>
              <ListItemButton
                onClick={() => {
                  if (item.children) {
                    toggleExpand(item.text);
                  } else {
                    router.push(item.path);
                  }
                }}
                sx={{
                  color: "white",
                  "&:hover": { backgroundColor: "rgba(255, 255, 255, 0.1)" },
                }}
              >
                <ListItemIcon sx={{ color: "white", minWidth: 40 }}>
                  <item.icon />
                </ListItemIcon>
                <ListItemText primary={item.text} />
                {item.children && (
                  <Typography>
                    {expandedItems.includes(item.text) ? "▲" : "▼"}
                  </Typography>
                )}
              </ListItemButton>
            </ListItem>
            {item.children && expandedItems.includes(item.text) && (
              <List sx={{ pl: 4 }}>
                {item.children.map((child) => (
                  <ListItem key={child.text} disablePadding>
                    <ListItemButton
                      onClick={() => router.push(child.path)}
                      sx={{
                        color: "rgba(255, 255, 255, 0.8)",
                        "&:hover": {
                          backgroundColor: "rgba(255, 255, 255, 0.1)",
                        },
                      }}
                    >
                      <ListItemText primary={`• ${child.text}`} />
                    </ListItemButton>
                  </ListItem>
                ))}
              </List>
            )}
          </Box>
        ))}
        <ListItem disablePadding sx={{ mt: 2 }}>
          <ListItemButton
            onClick={handleLogout}
            sx={{
              color: "white",
              "&:hover": { backgroundColor: "rgba(255, 255, 255, 0.1)" },
            }}
          >
            <ListItemIcon sx={{ color: "white", minWidth: 40 }}>
              <LogoutIcon />
            </ListItemIcon>
            <ListItemText primary="Logout" />
          </ListItemButton>
        </ListItem>
      </List>
      <Box
        sx={{
          position: "absolute",
          bottom: 20,
          left: 20,
          color: "rgba(255, 255, 255, 0.7)",
          fontSize: "0.75rem",
        }}
      >
        Copyright © Consultancy Outfit
      </Box>
    </Sidebar>
  );

  return (
    <Box sx={{ display: "flex" }}>
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: "block", sm: "none" },
            "& .MuiDrawer-paper": {
              boxSizing: "border-box",
              width: drawerWidth,
            },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: "none", sm: "block" },
            "& .MuiDrawer-paper": {
              boxSizing: "border-box",
              width: drawerWidth,
            },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      <MainContent>
        <Header position="static">
          <Toolbar>
            <IconButton
              color="inherit"
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ mr: 2, display: { sm: "none" } }}
            >
              <MenuIcon />
            </IconButton>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              Compliance Sheet L&D
            </Typography>
            <SearchBox>
              <SearchIcon sx={{ color: "#9ca3af", mr: 1 }} />
              <InputBase placeholder="Search..." sx={{ flex: 1 }} />
            </SearchBox>
            <IconButton color="inherit" sx={{ ml: 2 }}>
              <NotificationsIcon />
            </IconButton>
            <Typography sx={{ ml: 2, mr: 1 }}>Welcome!</Typography>
            <Avatar sx={{ bgcolor: "#10b981" }}>T</Avatar>
          </Toolbar>
        </Header>
        <Box sx={{ p: 3 }}>{children}</Box>
      </MainContent>
    </Box>
  );
}


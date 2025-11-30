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
import { useAppDispatch } from "@/redux-store";
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
  width: 100%;
  overflow-x: hidden;
  
  @media (max-width: 960px) {
    width: 100vw;
    overflow-x: hidden;
  }
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
  
  @media (max-width: 960px) {
    width: 200px;
  }
  
  @media (max-width: 600px) {
    display: none;
  }
`;

const allMenuItems = [
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

// Menu items for user role (only Dashboard and Courses)
const userMenuItems = [
  { text: "Dashboard", icon: DashboardIcon, path: "/dashboard" },
  { text: "Courses", icon: FolderIcon, path: "/courses" },
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
  
  // Get user role from localStorage instead of Redux state (persists on refresh)
  const getUserRoleFromStorage = () => {
    if (typeof window === "undefined") return "";
    try {
      const userInfo = localStorage.getItem("user_info");
      if (userInfo) {
        const user = JSON.parse(userInfo);
        return user?.role || user?.Role || "";
      }
    } catch (error) {
      console.error("Error reading user info from localStorage:", error);
    }
    return "";
  };
  
  // Filter menu items based on user role
  const userRole = getUserRoleFromStorage();
  const isUserRole = userRole.toLowerCase() === "user";
  const menuItems = isUserRole ? userMenuItems : allMenuItems;

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
                  if (item.children && !isUserRole) {
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
                {item.children && !isUserRole && (
                  <Typography>
                    {expandedItems.includes(item.text) ? "▲" : "▼"}
                  </Typography>
                )}
              </ListItemButton>
            </ListItem>
            {item.children && !isUserRole && expandedItems.includes(item.text) && (
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
          display: { xs: "none", sm: "block" },
        }}
      >
        Copyright © Consultancy Outfit
      </Box>
    </Sidebar>
  );

  return (
    <Box sx={{ display: "flex", width: "100%", overflowX: "hidden" }}>
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
          <Toolbar sx={{ flexWrap: { xs: "wrap", sm: "nowrap" }, gap: { xs: 1, sm: 0 } }}>
            <IconButton
              color="inherit"
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ mr: 2, display: { sm: "none" } }}
            >
              <MenuIcon />
            </IconButton>
            <Typography 
              variant="h6" 
              component="div" 
              sx={{ 
                flexGrow: 1,
                fontSize: { xs: "0.875rem", sm: "1.25rem" },
                display: { xs: "none", sm: "block" }
              }}
            >
              Compliance Sheet L&D
            </Typography>
            <SearchBox>
              <SearchIcon sx={{ color: "#9ca3af", mr: 1 }} />
              <InputBase placeholder="Search..." sx={{ flex: 1 }} />
            </SearchBox>
            <Box sx={{ display: "flex", alignItems: "center", gap: { xs: 0.5, sm: 1 } }}>
              <IconButton color="inherit" sx={{ display: { xs: "none", sm: "flex" } }}>
                <NotificationsIcon />
              </IconButton>
              <Typography sx={{ display: { xs: "none", md: "block" }, mr: 1 }}>
                Welcome!
              </Typography>
              <Avatar sx={{ bgcolor: "#10b981", width: { xs: 32, sm: 40 }, height: { xs: 32, sm: 40 } }}>
                T
              </Avatar>
            </Box>
          </Toolbar>
        </Header>
        <Box sx={{ p: { xs: 1, sm: 2, md: 3 } }}>{children}</Box>
      </MainContent>
    </Box>
  );
}


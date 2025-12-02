"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import {
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  Avatar,
  Card,
  CardContent,
  IconButton,
  CircularProgress,
  Link,
} from "@mui/material";
import {
  Send as SendIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
} from "@mui/icons-material";
import {
  useGetDiscussionByIdQuery,
  useGetPostsByDiscussionQuery,
  useGetCommentsByPostQuery,
  useCreatePostMutation,
  useUpdatePostMutation,
  useDeletePostMutation,
} from "@/services/discussions-api";
import { useSelector } from "react-redux";
import type { RootState } from "@/redux-store/store";

interface PostWithComments {
  id: string;
  userId: string;
  content: string;
  createdAt: string;
  comments?: any[];
}

export default function DiscussionDetailPage() {
  const params = useParams();
  const router = useRouter();
  const discussionId = params?.id as string;

  const { user, isAuthenticated, accessToken } = useSelector(
    (state: RootState) => state.auth
  );

  // Log user state on mount for debugging
  useEffect(() => {
    console.log("Discussion page - Auth state:", {
      user,
      isAuthenticated,
      hasAccessToken: !!accessToken,
      userKeys: user ? Object.keys(user) : null,
      userStringified: user ? JSON.stringify(user, null, 2) : null,
    });
  }, [user, isAuthenticated, accessToken]);

  // Helper to get user ID from various possible field names
  // Backend returns UserInfoDto with Id (capital I), but JSON serialization might convert to camelCase
  const getUserId = () => {
    // First try from Redux state
    if (user) {
      // Try all possible field name variations
      const userId =
        user.Id || user.id || user._id || user.userId || user.UserId;
      if (userId) {
        console.log("Found userId from Redux:", userId);
        return userId;
      }
      console.warn(
        "User object exists but no ID found. User keys:",
        Object.keys(user)
      );
    } else {
      console.warn("User object is null or undefined");
    }

    // Fallback to localStorage
    if (typeof window !== "undefined") {
      try {
        const storedUser = localStorage.getItem("user_info");
        if (storedUser) {
          const parsedUser = JSON.parse(storedUser);
          const userId =
            parsedUser?.Id ||
            parsedUser?.id ||
            parsedUser?._id ||
            parsedUser?.userId ||
            parsedUser?.UserId;
          if (userId) {
            console.log("Found userId from localStorage:", userId);
            return userId;
          }
        }
      } catch (error) {
        console.error("Error reading user from localStorage:", error);
      }
    }

    return null;
  };
  const [replyText, setReplyText] = useState("");
  const [editingPostId, setEditingPostId] = useState<string | null>(null);
  const [editingText, setEditingText] = useState("");
  const [expandedPosts, setExpandedPosts] = useState<Set<string>>(new Set());

  // Debug: Log user object structure on mount and when it changes
  useEffect(() => {
    console.log("=== User Object Debug ===");
    console.log("User object:", user);
    console.log("User type:", typeof user);
    console.log("User keys:", user ? Object.keys(user) : "null");
    console.log("User.Id:", user?.Id);
    console.log("User.id:", user?.id);
    console.log("isAuthenticated:", isAuthenticated);
    console.log("getUserId() result:", getUserId());
    console.log("=========================");
  }, [user, isAuthenticated]);

  const { data: discussionData, isLoading: discussionLoading } =
    useGetDiscussionByIdQuery(discussionId);
  const {
    data: postsData,
    isLoading: postsLoading,
    refetch: refetchPosts,
  } = useGetPostsByDiscussionQuery(discussionId);
  const [createPost, { isLoading: isCreating }] = useCreatePostMutation();
  const [updatePost] = useUpdatePostMutation();
  const [deletePost] = useDeletePostMutation();

  const discussion = discussionData?.data || (discussionData as any);
  const posts = postsData?.data || [];

  // Fetch comments for each post
  const { data: commentsData } = useGetCommentsByPostQuery(
    expandedPosts.size > 0 ? Array.from(expandedPosts)[0] : "",
    { skip: expandedPosts.size === 0 }
  );

  const handleSendReply = async () => {
    console.log("handleSendReply called", {
      replyText,
      user,
      userKeys: user ? Object.keys(user) : null,
      userStringified: user ? JSON.stringify(user, null, 2) : null,
      discussionId,
      discussion,
    });

    if (!replyText.trim()) {
      console.log("No reply text");
      return;
    }

    const userId = getUserId();
    console.log("getUserId() returned:", userId);

    if (!userId) {
      console.error("No user ID found", {
        user,
        userKeys: user ? Object.keys(user) : null,
        localStorage:
          typeof window !== "undefined"
            ? localStorage.getItem("user_info")
            : null,
      });
      alert("Please sign in to post a reply");
      return;
    }

    if (!discussionId) {
      console.error("No discussion ID found");
      alert("Discussion ID is missing");
      return;
    }

    try {
      const postData = {
        discussionId: discussionId,
        lessonId: discussion?.lessonId || "",
        userId: userId,
        content: replyText,
        parentPostId: null,
      };
      console.log("Creating post with data:", postData);

      const result = await createPost(postData).unwrap();
      console.log("Post created successfully:", result);
      setReplyText("");
      refetchPosts();
    } catch (error) {
      console.error("Failed to create post:", error);
      alert("Failed to create post. Please try again.");
    }
  };

  const handleReplyToPost = async (postId: string, replyContent: string) => {
    const userId = getUserId();
    if (!replyContent.trim() || !userId) return;

    try {
      const userId = getUserId();
      if (!userId) {
        alert("Please sign in to post a reply");
        return;
      }
      await createPost({
        discussionId: discussionId,
        lessonId: discussion?.lessonId || "",
        userId: userId,
        content: replyContent,
        parentPostId: postId,
      }).unwrap();
      refetchPosts();
      if (expandedPosts.has(postId)) {
        // Refresh comments
        refetchPosts();
      }
    } catch (error) {
      console.error("Failed to create reply:", error);
    }
  };

  const handleEditPost = async (postId: string) => {
    if (!editingText.trim()) return;

    try {
      await updatePost({ id: postId, content: editingText }).unwrap();
      setEditingPostId(null);
      setEditingText("");
      refetchPosts();
    } catch (error) {
      console.error("Failed to update post:", error);
    }
  };

  const handleDeletePost = async (postId: string) => {
    if (!confirm("Are you sure you want to delete this post?")) return;

    try {
      await deletePost(postId).unwrap();
      refetchPosts();
    } catch (error) {
      console.error("Failed to delete post:", error);
    }
  };

  const toggleComments = (postId: string) => {
    const newExpanded = new Set(expandedPosts);
    if (newExpanded.has(postId)) {
      newExpanded.delete(postId);
    } else {
      newExpanded.add(postId);
    }
    setExpandedPosts(newExpanded);
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      weekday: "long",
      day: "numeric",
      month: "long",
      hour: "numeric",
      minute: "2-digit",
      hour12: true,
    });
  };

  const getUserInitials = (userId: string, userName?: string) => {
    if (userName) {
      return userName.charAt(0).toUpperCase();
    }
    return userId?.charAt(0)?.toUpperCase() || "U";
  };

  if (discussionLoading) {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100vh",
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ display: "flex", gap: 3, p: 3 }}>
      {/* Main Content */}
      <Box sx={{ flex: 2 }}>
        {/* Breadcrumbs */}
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
          Facilitate {">"} Discussion {">"} Discussion
        </Typography>

        {/* Page Title */}
        <Typography
          variant="h4"
          component="h1"
          sx={{ fontWeight: 700, mb: 3, color: "#6366f1" }}
        >
          Discussion
        </Typography>

        {/* Discussion Description */}
        {discussion?.description && (
          <Typography variant="body1" sx={{ mb: 3, color: "text.secondary" }}>
            {discussion.description}
          </Typography>
        )}

        {/* Posts */}
        {postsLoading ? (
          <CircularProgress />
        ) : (
          <Box sx={{ mb: 3 }}>
            {/* Load previous comments link */}
            {posts.length > 3 && (
              <Link
                component="button"
                variant="body2"
                onClick={() => {}}
                sx={{ mb: 2, cursor: "pointer", color: "#6366f1" }}
              >
                Load previous comments ({posts.length - 3} to {posts.length})
              </Link>
            )}

            {/* Posts List */}
            {posts.map((post: any) => (
              <PostCard
                key={post.id || post._id}
                post={post}
                isOwn={post.userId === getUserId()}
                onReply={handleReplyToPost}
                onEdit={handleEditPost}
                onDelete={handleDeletePost}
                onToggleComments={toggleComments}
                isExpanded={expandedPosts.has(post.id || post._id)}
                formatDate={formatDate}
                getUserInitials={getUserInitials}
                editingPostId={editingPostId}
                editingText={editingText}
                setEditingPostId={setEditingPostId}
                setEditingText={setEditingText}
                discussionId={discussionId}
                lessonId={discussion?.lessonId || ""}
                currentUserId={getUserId() || ""}
              />
            ))}

            {/* Reply Input */}
            <Paper
              sx={{
                p: 2,
                mt: 3,
                display: "flex",
                gap: 2,
                alignItems: "flex-start",
              }}
            >
              <Avatar sx={{ bgcolor: "#6366f1", mt: 1 }}>
                {getUserInitials(getUserId() || "", user?.name)}
              </Avatar>
              <TextField
                fullWidth
                placeholder="Write a reply...."
                value={replyText}
                onChange={(e) => setReplyText(e.target.value)}
                multiline
                minRows={4}
                maxRows={10}
                variant="outlined"
                sx={{
                  flex: 1,
                  "& .MuiOutlinedInput-root": {
                    backgroundColor: "white",
                  },
                }}
                onKeyPress={(e) => {
                  if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
                    e.preventDefault();
                    handleSendReply();
                  }
                }}
              />
              <IconButton
                color="primary"
                onClick={handleSendReply}
                disabled={!replyText.trim() || isCreating}
                sx={{ mt: 1 }}
              >
                <SendIcon />
              </IconButton>
            </Paper>
          </Box>
        )}
      </Box>

      {/* Sidebar */}
      <Box sx={{ flex: 1, display: "flex", flexDirection: "column", gap: 2 }}>
        {/* Discussion Prompt */}
        <Paper sx={{ p: 2 }}>
          <Typography variant="h6" sx={{ mb: 1, fontWeight: 600 }}>
            Discussion Prompt
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {discussion?.description || "No prompt available"}
          </Typography>
        </Paper>

        {/* Course Info */}
        <Paper sx={{ p: 2 }}>
          <Typography variant="h6" sx={{ mb: 1, fontWeight: 600 }}>
            {discussion?.courseTitle?.toUpperCase() || "COURSE"}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
            {discussion?.discussionTitle || "Discussion"}
          </Typography>
          <Typography variant="body2" sx={{ mb: 2 }}>
            {discussion?.contributions ?? 0} Contributions
          </Typography>
          <Button
            variant="text"
            onClick={() => router.push("/discussions")}
            sx={{ color: "#6366f1", textTransform: "none" }}
          >
            View All Discussions
          </Button>
        </Paper>
      </Box>
    </Box>
  );
}

interface PostCardProps {
  post: any;
  isOwn: boolean;
  onReply: (postId: string, content: string) => void;
  onEdit: (postId: string) => void;
  onDelete: (postId: string) => void;
  onToggleComments: (postId: string) => void;
  isExpanded: boolean;
  formatDate: (date: string) => string;
  getUserInitials: (userId: string, userName?: string) => string;
  editingPostId: string | null;
  editingText: string;
  setEditingPostId: (id: string | null) => void;
  setEditingText: (text: string) => void;
  discussionId: string;
  lessonId: string;
  currentUserId: string;
}

function PostCard({
  post,
  isOwn,
  onReply,
  onEdit,
  onDelete,
  onToggleComments,
  isExpanded,
  formatDate,
  getUserInitials,
  editingPostId,
  editingText,
  setEditingPostId,
  setEditingText,
  discussionId,
  lessonId,
  currentUserId,
}: PostCardProps) {
  const [replyText, setReplyText] = useState("");
  const [createPost] = useCreatePostMutation();
  const { data: commentsData, refetch: refetchComments } =
    useGetCommentsByPostQuery(post.id || post._id, { skip: !isExpanded });

  const comments = commentsData?.data || [];

  const handleSendReply = async () => {
    if (!replyText.trim()) return;
    try {
      await createPost({
        discussionId,
        lessonId,
        userId: currentUserId,
        content: replyText,
        parentPostId: post.id || post._id,
      }).unwrap();
      setReplyText("");
      refetchComments();
      onReply(post.id || post._id, replyText);
    } catch (error) {
      console.error("Failed to create reply:", error);
    }
  };

  return (
    <Card
      sx={{
        mb: 2,
        backgroundColor: isOwn ? "#e0e7ff" : "#f3f4f6",
      }}
    >
      <CardContent>
        <Box sx={{ display: "flex", gap: 2, mb: 1 }}>
          <Avatar sx={{ bgcolor: "#6366f1" }}>
            {getUserInitials(post.userId || "", post.userName)}
          </Avatar>
          <Box sx={{ flex: 1 }}>
            <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
              {post.userName || `User ${post.userId?.substring(0, 8)}`}
            </Typography>
          </Box>
          <Typography variant="caption" color="text.secondary">
            {formatDate(post.createdAt)}
          </Typography>
        </Box>

        {editingPostId === (post.id || post._id) ? (
          <Box>
            <TextField
              fullWidth
              multiline
              rows={3}
              value={editingText}
              onChange={(e) => setEditingText(e.target.value)}
              sx={{ mb: 1 }}
            />
            <Box sx={{ display: "flex", gap: 1 }}>
              <Button size="small" onClick={() => onEdit(post.id || post._id)}>
                Save
              </Button>
              <Button size="small" onClick={() => setEditingPostId(null)}>
                Cancel
              </Button>
            </Box>
          </Box>
        ) : (
          <>
            <Typography variant="body1" sx={{ mb: 1 }}>
              {post.content}
            </Typography>
            <Box sx={{ display: "flex", gap: 2, mt: 2 }}>
              {isOwn && (
                <>
                  <Button
                    size="small"
                    color="error"
                    onClick={() => onDelete(post.id || post._id)}
                  >
                    Delete
                  </Button>
                  <Button
                    size="small"
                    onClick={() => {
                      setEditingPostId(post.id || post._id);
                      setEditingText(post.content);
                    }}
                  >
                    Edit
                  </Button>
                </>
              )}
              {!isOwn && (
                <Button
                  size="small"
                  onClick={() => onToggleComments(post.id || post._id)}
                >
                  Reply
                </Button>
              )}
            </Box>

            {/* Comments Section */}
            {isExpanded && (
              <Box sx={{ mt: 3, pl: 4, borderLeft: "2px solid #e5e7eb" }}>
                {comments.map((comment: any) => (
                  <Box key={comment.id || comment._id} sx={{ mb: 2 }}>
                    <Box sx={{ display: "flex", gap: 2, mb: 1 }}>
                      <Avatar
                        sx={{ bgcolor: "#6366f1", width: 32, height: 32 }}
                      >
                        {getUserInitials(
                          comment.userId || "",
                          comment.userName
                        )}
                      </Avatar>
                      <Box sx={{ flex: 1 }}>
                        <Typography
                          variant="subtitle2"
                          sx={{ fontWeight: 600 }}
                        >
                          {comment.userName ||
                            `User ${comment.userId?.substring(0, 8)}`}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {formatDate(comment.createdAt)}
                        </Typography>
                      </Box>
                    </Box>
                    <Typography variant="body2">{comment.content}</Typography>
                  </Box>
                ))}

                {/* Reply Input */}
                <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
                  <Avatar sx={{ bgcolor: "#6366f1", width: 32, height: 32 }}>
                    U
                  </Avatar>
                  <TextField
                    fullWidth
                    size="small"
                    placeholder="Write a reply..."
                    value={replyText}
                    onChange={(e) => setReplyText(e.target.value)}
                    onKeyPress={(e) => {
                      if (e.key === "Enter" && !e.shiftKey) {
                        e.preventDefault();
                        handleSendReply();
                      }
                    }}
                  />
                  <IconButton
                    color="primary"
                    onClick={handleSendReply}
                    disabled={!replyText.trim()}
                  >
                    <SendIcon fontSize="small" />
                  </IconButton>
                </Box>
              </Box>
            )}
          </>
        )}
      </CardContent>
    </Card>
  );
}

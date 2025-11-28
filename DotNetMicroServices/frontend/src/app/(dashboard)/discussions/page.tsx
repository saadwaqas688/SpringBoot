"use client";

import { useState } from "react";
import {
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  Avatar,
  Card,
  CardContent,
} from "@mui/material";
import { Send as SendIcon } from "@mui/icons-material";
import {
  useGetPostsByLessonQuery,
  useCreatePostMutation,
} from "@/services/discussions-api";
import styled from "styled-components";

const DiscussionContainer = styled(Box)`
  display: flex;
  gap: 2rem;
  height: calc(100vh - 200px);
`;

const MainContent = styled(Box)`
  flex: 2;
  display: flex;
  flex-direction: column;
`;

const Sidebar = styled(Box)`
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 1rem;
`;

const PostCard = styled(Card)`
  margin-bottom: 1rem;
  background: ${(props: { isOwn?: boolean }) =>
    props.isOwn ? "#e0e7ff" : "#f3f4f6"};
`;

const ReplyInput = styled(Box)`
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: white;
  border-radius: 8px;
  margin-top: auto;
`;

export default function DiscussionsPage() {
  const [replyText, setReplyText] = useState("");
  const lessonId = "sample-lesson-id"; // This should come from route params or context
  const { data: postsData, isLoading } = useGetPostsByLessonQuery(lessonId);
  const [createPost] = useCreatePostMutation();

  const posts = postsData?.data || [];

  const handleSendReply = async () => {
    if (!replyText.trim()) return;

    try {
      await createPost({
        lessonId,
        content: replyText,
        authorId: "current-user-id", // Get from auth state
      }).unwrap();
      setReplyText("");
    } catch (error) {
      console.error("Failed to create post:", error);
    }
  };

  return (
    <Box>
      <Box sx={{ mb: 2 }}>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
          Facilitate {">"} Discussion
        </Typography>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
          Discussion
        </Typography>
      </Box>

      <DiscussionContainer>
        <MainContent>
          <Typography variant="h5" sx={{ mb: 2, fontWeight: 600 }}>
            First discussion
          </Typography>

          {isLoading ? (
            <Typography>Loading posts...</Typography>
          ) : (
            <Box sx={{ flex: 1, overflowY: "auto", mb: 2 }}>
              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ mb: 2, cursor: "pointer" }}
              >
                Load previous comments (2 to 3)
              </Typography>

              {posts.map((post: any) => (
                <PostCard key={post.id} isOwn={post.isOwn}>
                  <CardContent>
                    <Box sx={{ display: "flex", gap: 2, mb: 1 }}>
                      <Avatar sx={{ bgcolor: "#6366f1" }}>
                        {post.authorName?.charAt(0) || "U"}
                      </Avatar>
                      <Box sx={{ flex: 1 }}>
                        <Typography
                          variant="subtitle2"
                          sx={{ fontWeight: 600 }}
                        >
                          {post.authorName || "Unknown User"}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {new Date(post.createdAt).toLocaleDateString()}
                        </Typography>
                      </Box>
                    </Box>
                    <Typography variant="body1" sx={{ mb: 1 }}>
                      {post.content}
                    </Typography>
                    <Box sx={{ display: "flex", gap: 2 }}>
                      <Button size="small" color="error">
                        Delete
                      </Button>
                      <Button size="small">Reply</Button>
                      <Button size="small">More Replies...</Button>
                    </Box>
                  </CardContent>
                </PostCard>
              ))}
            </Box>
          )}

          <ReplyInput>
            <Avatar sx={{ bgcolor: "#6366f1" }}>U</Avatar>
            <TextField
              fullWidth
              placeholder="write a reply..."
              value={replyText}
              onChange={(e) => setReplyText(e.target.value)}
              onKeyPress={(e) => {
                if (e.key === "Enter" && !e.shiftKey) {
                  e.preventDefault();
                  handleSendReply();
                }
              }}
              multiline
              maxRows={4}
            />
            <Button
              variant="contained"
              sx={{ bgcolor: "#6366f1", minWidth: 100 }}
              onClick={handleSendReply}
              startIcon={<SendIcon />}
            >
              Send
            </Button>
          </ReplyInput>
        </MainContent>

        <Sidebar>
          <Paper sx={{ p: 2, mb: 2 }}>
            <Typography variant="h6" sx={{ mb: 1, fontWeight: 600 }}>
              Discussion Prompt
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Discuss Different Business Scenarios
            </Typography>
          </Paper>

          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" sx={{ mb: 1, fontWeight: 600 }}>
              FIRST COURSE
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
              First Discussion
            </Typography>
            <Typography variant="body2" sx={{ mb: 2 }}>
              4 Contributions
            </Typography>
            <Button
              variant="text"
              sx={{ color: "#6366f1", textTransform: "none" }}
            >
              View All Discussion
            </Button>
          </Paper>
        </Sidebar>
      </DiscussionContainer>
    </Box>
  );
}


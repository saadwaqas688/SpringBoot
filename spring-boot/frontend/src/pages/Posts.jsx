import { useState, useEffect, useRef } from "react";
import { postsAPI, discussionsAPI } from "../services/api";
import { toast } from "react-toastify";
import ProtectedRoute from "../components/ProtectedRoute";
import { useAuth } from "../context/AuthContext";
import { useSearchParams, useNavigate } from "react-router-dom";

const Posts = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const messagesEndRef = useRef(null);
  const [discussion, setDiscussion] = useState(null);
  const [posts, setPosts] = useState([]);
  const [selectedDiscussion, setSelectedDiscussion] = useState("");
  const [loading, setLoading] = useState(true);
  const [editingPost, setEditingPost] = useState(null);
  const [formData, setFormData] = useState({
    discussionId: "",
    courseId: "",
    content: "",
  });

  useEffect(() => {
    const discussionId = searchParams.get("discussionId");
    if (discussionId) {
      setSelectedDiscussion(discussionId);
      fetchDiscussion(discussionId);
      fetchPosts(discussionId);
    } else {
      setLoading(false);
    }
  }, [searchParams]);

  useEffect(() => {
    // Auto-scroll to bottom when new messages arrive
    scrollToBottom();
  }, [posts]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  const fetchDiscussion = async (discussionId) => {
    if (!discussionId) return;
    try {
      const data = await discussionsAPI.getById(discussionId);
      setDiscussion(data);
      setFormData((prev) => ({
        ...prev,
        discussionId: data.id,
        courseId: data.courseId,
      }));
    } catch (error) {
      console.error("Error fetching discussion:", error);
      toast.error("Failed to fetch discussion");
    }
  };

  const fetchPosts = async (discussionId) => {
    if (!discussionId) return;
    try {
      setLoading(true);
      const data = await postsAPI.getByDiscussionId(discussionId);
      setPosts(data);
    } catch (error) {
      console.error("Error fetching posts:", error);
      toast.error("Failed to fetch posts");
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!formData.content.trim()) {
      toast.error("Please enter some content");
      return;
    }
    try {
      if (editingPost) {
        await postsAPI.update(editingPost.id, {
          content: formData.content,
        });
        toast.success("Post updated successfully!");
      } else {
        if (!formData.discussionId || !formData.courseId) {
          toast.error("Discussion ID or Course ID is missing");
          return;
        }
        await postsAPI.create({
          discussionId: formData.discussionId,
          courseId: formData.courseId,
          content: formData.content,
        });
        toast.success("Post created successfully!");
      }
      setEditingPost(null);
      setFormData({
        ...formData,
        content: "",
      });
      if (selectedDiscussion) {
        // Wait a bit for the database to update, then refresh
        setTimeout(() => {
          fetchPosts(selectedDiscussion);
        }, 500);
      }
    } catch (error) {
      const errorMessage =
        error.response?.data?.message ||
        error.message ||
        (editingPost ? "Failed to update post" : "Failed to create post");
      toast.error(errorMessage);
    }
  };

  const handleEdit = (post) => {
    setEditingPost(post);
    setFormData({
      ...formData,
      content: post.content,
    });
    setTimeout(() => {
      document.getElementById("message-input")?.focus();
    }, 100);
  };

  const cancelForm = () => {
    setEditingPost(null);
    setFormData({
      ...formData,
      content: "",
    });
  };

  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this post?")) {
      try {
        await postsAPI.delete(id);
        toast.success("Post deleted successfully!");
        if (selectedDiscussion) {
          fetchPosts(selectedDiscussion);
        }
      } catch (error) {
        const errorMessage =
          error.response?.data?.message ||
          error.message ||
          "Failed to delete post";
        toast.error(errorMessage);
      }
    }
  };

  const isOwnPost = (post) => {
    return post.userId === user?.id;
  };

  if (loading && !discussion) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!selectedDiscussion) {
    return (
      <ProtectedRoute>
        <div className="px-4 py-6">
          <div className="text-center py-12">
            <p className="text-gray-500 text-lg mb-4">
              No discussion selected. Please select a discussion from the
              Discussions page.
            </p>
            <button
              onClick={() => navigate("/discussions")}
              className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            >
              Go to Discussions
            </button>
          </div>
        </div>
      </ProtectedRoute>
    );
  }

  return (
    <ProtectedRoute>
      <div className="flex flex-col h-[calc(100vh-4rem)]">
        {/* Header */}
        <div className="bg-white border-b border-gray-200 px-4 py-3 flex-shrink-0">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <button
                onClick={() => navigate("/discussions")}
                className="text-gray-600 hover:text-gray-900 text-lg"
              >
                ←
              </button>
              <div>
                <h1 className="text-xl font-bold text-gray-900">
                  {discussion?.title || "Discussion Posts"}
                </h1>
                {discussion?.description && (
                  <p className="text-sm text-gray-500">
                    {discussion.description}
                  </p>
                )}
              </div>
            </div>
            <button
              onClick={() => fetchPosts(selectedDiscussion)}
              className="text-gray-600 hover:text-gray-900"
              title="Refresh"
            >
              ↻
            </button>
          </div>
        </div>

        {/* Posts Area */}
        <div className="flex-1 overflow-y-auto bg-gray-50 px-4 py-4">
          {loading ? (
            <div className="flex justify-center items-center h-full">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
          ) : posts.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-full text-center">
              <div className="text-6xl mb-4">💬</div>
              <p className="text-gray-500 text-lg mb-2">No posts yet</p>
              <p className="text-gray-400 text-sm">
                Start the conversation by posting a message below
              </p>
            </div>
          ) : (
            <div className="space-y-4">
              {posts.map((post) => (
                <div
                  key={post.id}
                  className="bg-white rounded-lg p-4 shadow-sm border border-gray-200"
                >
                  <div className="flex items-start justify-between mb-2">
                    <div className="flex-1">
                      <div className="text-sm font-bold text-gray-800 mb-2">
                        {post.userId || "Unknown User"}
                        {post.updatedAt !== post.createdAt && (
                          <span className="ml-2 text-xs italic text-gray-500 font-normal">
                            (edited)
                          </span>
                        )}
                      </div>
                      <div className="text-sm text-gray-800 whitespace-pre-wrap break-words mb-3">
                        {post.content}
                      </div>
                      <div className="flex items-center justify-between pt-2 border-t border-gray-200">
                        <div className="flex items-center space-x-3">
                          {isOwnPost(post) && (
                            <>
                              <button
                                onClick={() => handleDelete(post.id)}
                                className="text-xs text-red-600 hover:text-red-800 hover:underline"
                              >
                                Delete
                              </button>
                              <button
                                onClick={() => handleEdit(post)}
                                className="text-xs text-blue-600 hover:text-blue-800 hover:underline"
                              >
                                Edit
                              </button>
                            </>
                          )}
                        </div>
                        <div className="text-xs text-gray-500">
                          {new Date(post.createdAt).toLocaleDateString(
                            "en-US",
                            {
                              weekday: "long",
                              day: "numeric",
                              month: "long",
                              hour: "numeric",
                              minute: "2-digit",
                            }
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
              <div ref={messagesEndRef} />
            </div>
          )}
        </div>

        {/* Input Form */}
        <div className="bg-white border-t border-gray-200 px-4 py-3 flex-shrink-0">
          <form onSubmit={handleSubmit} className="flex space-x-2">
            <textarea
              id="message-input"
              name="content"
              required
              maxLength={5000}
              rows={2}
              className="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
              value={formData.content}
              onChange={handleChange}
              placeholder={
                editingPost ? "Edit your post..." : "Type your post..."
              }
              onKeyDown={(e) => {
                if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
                  handleSubmit(e);
                }
              }}
            />
            <div className="flex flex-col space-y-1">
              <button
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 text-white font-bold px-4 py-2 rounded-lg h-full"
                disabled={!formData.content.trim()}
              >
                {editingPost ? "Update" : "Post"}
              </button>
              {editingPost && (
                <button
                  type="button"
                  onClick={cancelForm}
                  className="bg-gray-300 hover:bg-gray-400 text-gray-700 text-xs px-2 py-1 rounded"
                >
                  Cancel
                </button>
              )}
            </div>
          </form>
          <p className="text-xs text-gray-400 mt-1">Press Ctrl+Enter to send</p>
        </div>
      </div>
    </ProtectedRoute>
  );
};

export default Posts;




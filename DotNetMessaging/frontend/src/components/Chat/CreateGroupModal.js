import React, { useState, useEffect } from "react";
import { FiX, FiCheck } from "react-icons/fi";
import api from "../../services/api";
import "./ContactsModal.css";

function CreateGroupModal({ isOpen, onClose, onGroupCreated }) {
  const [groupName, setGroupName] = useState("");
  const [description, setDescription] = useState("");
  const [users, setUsers] = useState([]); // Changed from contacts to users
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedMembers, setSelectedMembers] = useState(new Set());
  const [loading, setLoading] = useState(false);
  const [loadingUsers, setLoadingUsers] = useState(false);

  useEffect(() => {
    if (isOpen) {
      setGroupName("");
      setDescription("");
      setSelectedMembers(new Set());
      setSearchQuery("");
      setUsers([]);
      // Load all users when modal opens (or you can keep it empty until search)
      loadAllUsers();
    }
  }, [isOpen]);

  // Load all users (or first few users) instead of just contacts
  const loadAllUsers = async () => {
    setLoadingUsers(true);
    try {
      console.log("[CreateGroupModal] Loading all users for group creation...");
      // Search with empty query to get all users (or use a different endpoint)
      // For now, let's search for empty string which should return all users
      const response = await api.get("/users/search?query=");
      console.log("[CreateGroupModal] API response:", response);
      console.log("[CreateGroupModal] Users data:", response.data);
      console.log("[CreateGroupModal] Users count:", response.data?.length || 0);
      const usersList = response.data || [];
      setUsers(usersList);
      console.log("[CreateGroupModal] Users state set to:", usersList.length, "items");
    } catch (error) {
      console.error("[CreateGroupModal] Failed to load users:", error);
      console.error("[CreateGroupModal] Error response:", error.response);
      setUsers([]);
    } finally {
      setLoadingUsers(false);
      console.log("[CreateGroupModal] Loading users completed. Current users state:", users.length);
    }
  };

  // Search users as user types
  const searchUsers = async (query) => {
    if (!query.trim()) {
      loadAllUsers();
      return;
    }

    setLoadingUsers(true);
    try {
      console.log("[CreateGroupModal] Searching users with query:", query);
      const response = await api.get(`/users/search?query=${encodeURIComponent(query)}`);
      console.log("[CreateGroupModal] Search results:", response.data);
      setUsers(response.data || []);
    } catch (error) {
      console.error("[CreateGroupModal] Failed to search users:", error);
      setUsers([]);
    } finally {
      setLoadingUsers(false);
    }
  };

  const handleSearchChange = (e) => {
    const query = e.target.value;
    setSearchQuery(query);
    searchUsers(query);
  };

  const toggleMember = (userId) => {
    setSelectedMembers((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(userId)) {
        newSet.delete(userId);
      } else {
        newSet.add(userId);
      }
      return newSet;
    });
  };

  const handleCreate = async () => {
    if (!groupName.trim()) {
      alert("Please provide a group name");
      return;
    }

    setLoading(true);
    try {
      const requestData = {
        name: groupName,
        description: description || null,
        memberIds: Array.from(selectedMembers),
      };
      
      console.log("Creating group with data:", requestData);
      console.log("API endpoint: /groups");
      console.log("Full URL:", api.defaults.baseURL + "/groups");
      console.log("Selected members count:", selectedMembers.size);
      console.log("Selected members:", Array.from(selectedMembers));

      console.log("Sending POST request to /groups...");
      const response = await api.post("/groups", requestData);
      console.log("Response received:", response);
      console.log("Response status:", response.status);
      console.log("Response data:", response.data);

      if (response.data) {
        console.log("Group created successfully:", response.data);
        onGroupCreated(response.data);
        onClose();
        setGroupName("");
        setDescription("");
        setSelectedMembers(new Set());
      } else {
        throw new Error("No data in response");
      }
    } catch (error) {
      console.error("=== GROUP CREATION ERROR ===");
      console.error("Error object:", error);
      console.error("Error message:", error.message);
      console.error("Error response:", error.response);
      console.error("Error response data:", error.response?.data);
      console.error("Error response status:", error.response?.status);
      console.error("Error response headers:", error.response?.headers);
      console.error("Request config:", error.config);
      
      let errorMessage = "Failed to create group";
      
      if (error.response) {
        // Server responded with error status
        const data = error.response.data;
        if (typeof data === "string") {
          errorMessage = data;
        } else if (data?.message) {
          errorMessage = data.message;
        } else if (data?.title) {
          errorMessage = data.title;
        } else {
          errorMessage = `Server error (${error.response.status}): ${JSON.stringify(data)}`;
        }
      } else if (error.request) {
        // Request was made but no response received
        errorMessage = "No response from server. Please check if the backend is running.";
      } else {
        // Something else happened
        errorMessage = error.message || "An unexpected error occurred";
      }
      
      alert(`Failed to create group: ${errorMessage}`);
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div 
      className="modal-overlay" 
      style={{ zIndex: 1001 }} // Ensure this modal is above ContactsModal
      onClick={(e) => {
        // Only close if clicking directly on the overlay, not on child elements
        if (e.target === e.currentTarget) {
          onClose();
        }
      }}
    >
      <div className="modal-content" onClick={(e) => e.stopPropagation()} style={{ maxHeight: "90vh", display: "flex", flexDirection: "column" }}>
        <div className="modal-header" style={{ flexShrink: 0 }}>
          <h2>Create Group</h2>
          <button className="close-btn" onClick={onClose}>
            <FiX />
          </button>
        </div>

        <div className="modal-body" style={{ flex: 1, overflowY: "auto", minHeight: 0 }}>
          <div className="form-group" style={{ marginBottom: "1rem" }}>
            <label
              style={{
                display: "block",
                marginBottom: "0.5rem",
                fontWeight: 600,
                color: "#111b21",
              }}
            >
              Group Name *
            </label>
            <input
              type="text"
              placeholder="Enter group name"
              value={groupName}
              onChange={(e) => setGroupName(e.target.value)}
              className="form-input"
              style={{
                width: "100%",
                boxSizing: "border-box",
                padding: "0.75rem",
                border: "1px solid #e4e6eb",
                borderRadius: "8px",
                fontSize: "0.95rem",
                outline: "none",
              }}
            />
          </div>

          <div className="form-group" style={{ marginBottom: "1rem" }}>
            <label
              style={{
                display: "block",
                marginBottom: "0.5rem",
                fontWeight: 600,
                color: "#111b21",
              }}
            >
              Description (Optional)
            </label>
            <textarea
              placeholder="Enter group description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="form-textarea"
              rows="3"
              style={{
                width: "100%",
                boxSizing: "border-box",
                padding: "0.75rem",
                border: "1px solid #e4e6eb",
                borderRadius: "8px",
                fontSize: "0.95rem",
                outline: "none",
                resize: "vertical",
                fontFamily: "inherit",
              }}
            />
          </div>

          <div className="contacts-list" style={{ marginTop: "1rem" }}>
            <h3>Select Members *</h3>
            
            {/* Search input for finding users */}
            <div className="search-section" style={{ marginBottom: "0.75rem" }}>
              <input
                type="text"
                placeholder="Search users by name or email..."
                value={searchQuery}
                onChange={handleSearchChange}
                className="search-input"
                style={{
                  width: "100%",
                  padding: "0.75rem",
                  border: "1px solid #e4e6eb",
                  borderRadius: "20px",
                  fontSize: "0.95rem",
                  outline: "none",
                  boxSizing: "border-box",
                }}
              />
            </div>

            {/* Debug info - remove in production */}
            {process.env.NODE_ENV === 'development' && (
              <div style={{ fontSize: "0.75rem", color: "#999", marginBottom: "0.5rem" }}>
                Debug: loading={loadingUsers ? "true" : "false"}, users={users.length}
              </div>
            )}
            
            <div style={{ 
              minHeight: "200px",
              maxHeight: "300px", 
              overflowY: "auto", 
              border: "1px solid #e4e6eb", 
              borderRadius: "8px", 
              padding: "0.5rem",
              backgroundColor: "#fafafa"
            }}>
              {loadingUsers ? (
                <div className="empty-state" style={{ padding: "2rem", textAlign: "center", color: "#667781" }}>
                  {searchQuery ? "Searching users..." : "Loading users..."}
                </div>
              ) : users.length > 0 ? (
                users.map((user) => (
                  <div
                    key={user.id}
                    className="user-item"
                    onClick={(e) => {
                      e.stopPropagation();
                      e.preventDefault();
                      console.log("Toggling member:", user.id, "in CreateGroupModal");
                      toggleMember(user.id);
                    }}
                    onMouseDown={(e) => {
                      // Prevent any potential event bubbling
                      e.stopPropagation();
                    }}
                    style={{
                      backgroundColor: selectedMembers.has(user.id) ? "#e9f5e9" : "transparent",
                      marginBottom: "0.5rem",
                      cursor: "pointer",
                      pointerEvents: "auto",
                    }}
                  >
                    <div className="user-info" onClick={(e) => e.stopPropagation()}>
                      <div className="user-avatar">
                        {user.profilePictureUrl ? (
                          <img
                            src={user.profilePictureUrl}
                            alt={user.username}
                          />
                        ) : (
                          <div className="avatar-placeholder">
                            {user.username?.[0]?.toUpperCase() || "?"}
                          </div>
                        )}
                      </div>
                      <div className="user-details">
                        <div className="user-name">{user.username || "Unknown"}</div>
                        <div className="user-email">{user.email || ""}</div>
                      </div>
                    </div>
                    {selectedMembers.has(user.id) && (
                      <div style={{ color: "#25d366", fontSize: "1.2rem" }}>
                        <FiCheck />
                      </div>
                    )}
                  </div>
                ))
              ) : (
                <div className="empty-state" style={{ padding: "2rem", textAlign: "center", color: "#667781" }}>
                  <p>{searchQuery ? "No users found." : "Start typing to search for users."}</p>
                  <p style={{ fontSize: "0.9rem", marginTop: "0.5rem" }}>
                    {searchQuery ? "Try a different search term." : "Type a name or email to find users to add to the group."}
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>

        <div className="modal-footer" style={{ flexShrink: 0, padding: "1rem", borderTop: "1px solid #e4e6eb" }}>
          <button
            className="create-group-btn"
            onClick={handleCreate}
            disabled={
              loading || !groupName.trim()
            }
            style={{
              opacity: loading || !groupName.trim() ? 0.6 : 1,
              cursor: loading || !groupName.trim() ? "not-allowed" : "pointer",
            }}
          >
            {loading ? "Creating..." : "Create Group"}
          </button>
        </div>
      </div>
    </div>
  );
}

export default CreateGroupModal;

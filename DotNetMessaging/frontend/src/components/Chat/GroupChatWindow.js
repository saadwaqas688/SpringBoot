import React, { useState, useEffect, useRef } from "react";
import {
  FiSend,
  FiPaperclip,
  FiX,
  FiUsers,
  FiSettings,
  FiUserPlus,
  FiUserMinus,
} from "react-icons/fi";
import { formatDistanceToNow } from "date-fns";
import { useAuth } from "../../contexts/AuthContext";
import api from "../../services/api";
import signalRService from "../../services/signalRService";
import MessageBubble from "./MessageBubble";
import "./ChatWindow.css";

function GroupChatWindow({ group, onClose }) {
  const { user } = useAuth();
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [replyingTo, setReplyingTo] = useState(null);
  const [typingUsers, setTypingUsers] = useState(new Set());
  const [showGroupInfo, setShowGroupInfo] = useState(false);
  const [groupMembers, setGroupMembers] = useState(group.members || []);
  const [isAdmin, setIsAdmin] = useState(false);
  const [isTyping, setIsTyping] = useState(false);
  const messagesEndRef = useRef(null);
  const typingTimeoutRef = useRef(null);
  const currentGroupIdRef = useRef(group?.id);

  useEffect(() => {
    currentGroupIdRef.current = group?.id;
    setTypingUsers(new Set()); // Clear typing users when group changes
  }, [group?.id]);

  useEffect(() => {
    if (!group?.id) return;

    loadMessages();
    checkAdminStatus();

    // Register handlers FIRST, then join group
    const handleNewMessageWrapper = (message) => handleNewMessage(message);
    const handleUserTypingWrapper = (groupId, userId, userName, typing) => {
      console.log("🔵 [GroupChatWindow] UserTypingGroup event received:", {
        groupId,
        userId,
        userName,
        typing,
        currentGroupId: currentGroupIdRef.current,
        currentUserId: user?.id
      });

      // Compare as strings to avoid type mismatch
      const currentGroupId = String(currentGroupIdRef.current);
      const receivedGroupId = String(groupId);

      if (receivedGroupId === currentGroupId) {
        if (userId === user?.id) {
          console.log("⚠️ [GroupChatWindow] Ignoring typing from current user");
          return;
        }

        console.log(`✅ [GroupChatWindow] Updating typing indicator: ${userName} is ${typing ? "typing" : "not typing"}`);
        
        // Directly update state here instead of calling another function
        setTypingUsers((prev) => {
          const newSet = new Set(prev);
          const userKey = userName || userId;
          if (typing) {
            newSet.add(userKey);
            console.log("[GroupChatWindow] ✅ Added typing user:", userKey);
          } else {
            newSet.delete(userKey);
            console.log("[GroupChatWindow] ❌ Removed typing user:", userKey);
          }
          console.log("[GroupChatWindow] 📝 Typing users set:", Array.from(newSet), "Size:", newSet.size);
          return newSet;
        });
      } else {
        console.log(`⚠️ [GroupChatWindow] Ignoring typing for different group. Current: "${currentGroupId}", Received: "${receivedGroupId}"`);
      }
    };
    const handleReactionUpdateWrapper = (message) =>
      handleReactionUpdate(message);
    const handleMessageDeletedWrapper = (messageId) =>
      handleMessageDeleted(messageId);

    const registerHandlers = () => {
      if (signalRService.isConnected()) {
        console.log("🔌 [GroupChatWindow] Registering SignalR event handlers for group:", currentGroupIdRef.current);
        
        // Test if handler registration works by logging
        const testHandler = () => {
          console.log("🧪 [GroupChatWindow] TEST: UserTypingGroup handler is active and receiving events!");
        };
        
        signalRService.on("NewGroupMessage", handleNewMessageWrapper);
        signalRService.on("UserTypingGroup", handleUserTypingWrapper);
        signalRService.on("MessageReactionUpdated", handleReactionUpdateWrapper);
        signalRService.on("MessageDeleted", handleMessageDeletedWrapper);
        
        console.log("✅ [GroupChatWindow] All event handlers registered, including UserTypingGroup");
        
        // Verify the connection has the handler
        const connection = signalRService.getConnection();
        if (connection) {
          console.log("🔍 [GroupChatWindow] Verifying handler registration on connection...");
          console.log("📡 [GroupChatWindow] SignalR connection state:", connection.state);
        }
        
        return true;
      } else {
        console.warn("⚠️ [GroupChatWindow] Cannot register handlers: SignalR not connected");
        return false;
      }
    };

    const connection = signalRService.getConnection();
    if (connection) {
      connection.onreconnected(() => {
        console.log("[GroupChatWindow] SignalR reconnected, re-registering handlers");
        registerHandlers();
        joinGroup();
      });
    }

    // Ensure SignalR is connected before joining group and registering handlers
    const initializeGroup = async () => {
      if (!signalRService.isConnected()) {
        console.log("⏳ [GroupChatWindow] Waiting for SignalR connection...");
        await new Promise(resolve => setTimeout(resolve, 500));
      }

      if (signalRService.isConnected()) {
        console.log("✅ [GroupChatWindow] SignalR is connected");
        
        // Register handlers first
        registerHandlers();
        
        // Then join the group
        console.log("🔄 [GroupChatWindow] Joining group after handler registration...");
        await joinGroup();
      } else {
        console.error("❌ [GroupChatWindow] Cannot initialize: SignalR not connected");
      }
    };

    initializeGroup();

    const handlerTimeout = setTimeout(() => {
      if (signalRService.isConnected() && !registerHandlers()) {
        setTimeout(() => registerHandlers(), 1000);
      }
    }, 100);

    return () => {
      clearTimeout(handlerTimeout);
      leaveGroup();
      stopTyping(); // Stop typing when leaving group
      signalRService.off("NewGroupMessage", handleNewMessageWrapper);
      signalRService.off("UserTypingGroup", handleUserTypingWrapper);
      signalRService.off("MessageReactionUpdated", handleReactionUpdateWrapper);
      signalRService.off("MessageDeleted", handleMessageDeletedWrapper);
    };
  }, [group?.id, user?.id]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    console.log("[GroupChatWindow] Typing users changed:", Array.from(typingUsers), "Size:", typingUsers.size);
    if (typingUsers.size > 0) {
      scrollToBottom();
    }
  }, [typingUsers.size]);

  const checkAdminStatus = () => {
    const currentUserMember = groupMembers.find((m) => m.user.id === user?.id);
    setIsAdmin(currentUserMember?.role === "Admin");
  };

  const loadMessages = async () => {
    try {
      const response = await api.get(`/messages/group/${group.id}`);
      setMessages(response.data);
      // Refresh group list to update unread count
      window.dispatchEvent(new Event("refreshGroupList"));
    } catch (error) {
      console.error("Failed to load messages:", error);
    }
  };

  const joinGroup = async () => {
    if (currentGroupIdRef.current) {
      try {
        if (!signalRService.isConnected()) {
          console.warn("[GroupChatWindow] SignalR not connected, cannot join group");
          return;
        }
        console.log(`🔄 [GroupChatWindow] Joining group: ${currentGroupIdRef.current}`);
        await signalRService.invoke("JoinGroup", currentGroupIdRef.current);
        console.log(`✅ [GroupChatWindow] Successfully joined group: ${currentGroupIdRef.current}`);
        
        // Verify group membership (this is async, but we log it)
        const connection = signalRService.getConnection();
        if (connection) {
          console.log(`📋 [GroupChatWindow] Connection ID: ${connection.connectionId}`);
          console.log(`📋 [GroupChatWindow] Should be in group: Group_${currentGroupIdRef.current}`);
        }
      } catch (error) {
        console.error("❌ [GroupChatWindow] Failed to join group:", error);
      }
    }
  };

  const leaveGroup = async () => {
    if (group?.id) {
      await signalRService.invoke("LeaveGroup", group.id);
    }
  };

  const handleNewMessage = (message) => {
    if (message.groupId === group.id) {
      setMessages((prev) => {
        // Check if message already exists to prevent duplicates
        const messageExists = prev.some(m => m.id === message.id);
        if (messageExists) {
          console.log("[GroupChatWindow] Message already exists, skipping duplicate:", message.id);
          return prev;
        }
        console.log("[GroupChatWindow] Adding new message from SignalR:", message.id);
        return [...prev, message];
      });
    }
  };

  const handleUserTyping = (groupId, userId, userName, typing) => {
    console.log("[GroupChatWindow] handleUserTyping called:", { groupId, userId, userName, typing, currentGroupId: currentGroupIdRef.current });
    
    // Compare as strings to avoid type mismatch issues
    const currentGroupId = String(currentGroupIdRef.current);
    const receivedGroupId = String(groupId);
    
    if (receivedGroupId === currentGroupId) {
      console.log("[GroupChatWindow] Group IDs match, updating typing users");
      setTypingUsers((prev) => {
        const newSet = new Set(prev);
        const userKey = userName || userId;
        if (typing) {
          newSet.add(userKey);
          console.log("[GroupChatWindow] Added typing user:", userKey);
        } else {
          newSet.delete(userKey);
          console.log("[GroupChatWindow] Removed typing user:", userKey);
        }
        console.log("[GroupChatWindow] Typing users set:", Array.from(newSet), "Size:", newSet.size);
        return newSet;
      });
    } else {
      console.log(`[GroupChatWindow] Group IDs don't match. Current: "${currentGroupId}", Received: "${receivedGroupId}"`);
    }
  };

  const handleTyping = () => {
    const groupId = currentGroupIdRef.current;
    if (!groupId) {
      console.warn("[GroupChatWindow] Cannot send typing indicator: no group ID");
      return;
    }
    if (!signalRService.isConnected()) {
      console.warn("[GroupChatWindow] Cannot send typing indicator: SignalR not connected");
      return;
    }

    if (!isTyping) {
      setIsTyping(true);
      console.log(`[GroupChatWindow] Sending typing indicator: true for group ${groupId}`);
      signalRService.invoke("SendGroupTyping", groupId, true)
        .then(() => console.log("[GroupChatWindow] Typing indicator sent successfully"))
        .catch((error) => {
          console.error("[GroupChatWindow] Failed to send typing indicator:", error);
        });
    }

    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    typingTimeoutRef.current = setTimeout(() => {
      setIsTyping(false);
      console.log(`[GroupChatWindow] Sending typing indicator: false for group ${groupId}`);
      signalRService.invoke("SendGroupTyping", groupId, false)
        .then(() => console.log("[GroupChatWindow] Typing stop indicator sent successfully"))
        .catch((error) => {
          console.error("[GroupChatWindow] Failed to stop typing indicator:", error);
        });
    }, 3000);
  };

  const stopTyping = () => {
    const groupId = currentGroupIdRef.current;
    if (!groupId) return;

    if (isTyping) {
      setIsTyping(false);
      signalRService.invoke("SendGroupTyping", groupId, false).catch((error) => {
        console.error("[GroupChatWindow] Failed to stop typing indicator:", error);
      });
    }
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }
  };

  const handleReactionUpdate = (message) => {
    if (message.groupId === group.id) {
      setMessages((prev) =>
        prev.map((m) => (m.id === message.id ? message : m))
      );
    }
  };

  const handleMessageDeleted = (messageId) => {
    setMessages((prev) => prev.filter((m) => m.id !== messageId));
  };

  const sendMessage = async () => {
    if (!newMessage.trim() && !replyingTo) return;

    stopTyping(); // Stop typing when message is sent

    try {
      const response = await api.post("/messages", {
        groupId: group.id,
        content: newMessage,
        type: "Text",
        replyToMessageId: replyingTo?.id,
      });

      // Add message optimistically - SignalR will also broadcast it, but we check for duplicates
      setMessages((prev) => {
        const messageExists = prev.some(m => m.id === response.data.id);
        if (messageExists) {
          console.log("[GroupChatWindow] Message already exists, skipping:", response.data.id);
          return prev;
        }
        return [...prev, response.data];
      });
      
      setNewMessage("");
      setReplyingTo(null);
    } catch (error) {
      console.error("Failed to send message:", error);
    }
  };

  const handleFileUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("file", file);
    formData.append("groupId", group.id);

    try {
      const response = await api.post("/media/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      // Add message optimistically - SignalR will also broadcast it, but we check for duplicates
      setMessages((prev) => {
        const messageExists = prev.some(m => m.id === response.data.id);
        if (messageExists) {
          console.log("[GroupChatWindow] File message already exists, skipping:", response.data.id);
          return prev;
        }
        return [...prev, response.data];
      });
    } catch (error) {
      console.error("Failed to upload file:", error);
      alert("Failed to upload file. Please try again.");
    }
  };

  const handleAddMember = async (userId) => {
    try {
      await api.post(`/groups/${group.id}/members`, [userId]);
      // Reload group data
      const response = await api.get(`/groups`);
      const updatedGroup = response.data.find((g) => g.id === group.id);
      if (updatedGroup) {
        setGroupMembers(updatedGroup.members);
      }
      alert("Member added successfully");
    } catch (error) {
      console.error("Failed to add member:", error);
      alert("Failed to add member. You may not have permission.");
    }
  };

  const handleRemoveMember = async (memberId) => {
    try {
      await api.delete(`/groups/${group.id}/members/${memberId}`);
      // Reload group data
      const response = await api.get(`/groups`);
      const updatedGroup = response.data.find((g) => g.id === group.id);
      if (updatedGroup) {
        setGroupMembers(updatedGroup.members);
      }
      alert("Member removed successfully");
    } catch (error) {
      console.error("Failed to remove member:", error);
      alert("Failed to remove member. You may not have permission.");
    }
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  return (
    <div className="chat-window">
      <div className="chat-window-header">
        <div className="chat-header-info">
          <div className="chat-header-avatar">
            {group.profilePictureUrl ? (
              <img src={group.profilePictureUrl} alt={group.name} />
            ) : (
              <div className="avatar-placeholder group-avatar">
                <FiUsers />
              </div>
            )}
          </div>
          <div className="chat-header-details">
            <div className="chat-header-name">{group.name}</div>
            <div className="chat-header-status">
              {groupMembers.length} members
            </div>
          </div>
        </div>
        <div className="chat-header-actions">
          <button
            className="icon-btn"
            onClick={() => setShowGroupInfo(!showGroupInfo)}
            title="Group Info"
          >
            <FiSettings />
          </button>
          <button className="close-btn" onClick={onClose}>
            <FiX />
          </button>
        </div>
      </div>

      {showGroupInfo && (
        <div className="group-info-panel">
          <div className="group-info-header">
            <h3>Group Info</h3>
            <button onClick={() => setShowGroupInfo(false)}>
              <FiX />
            </button>
          </div>
          <div className="group-info-content">
            <div className="group-description">
              <strong>Description:</strong>
              <p>{group.description || "No description"}</p>
            </div>
            <div className="group-members-list">
              <strong>Members ({groupMembers.length}):</strong>
              {groupMembers.map((member) => (
                <div key={member.id} className="member-item">
                  <div className="member-info">
                    <div className="member-avatar">
                      {member.user.profilePictureUrl ? (
                        <img
                          src={member.user.profilePictureUrl}
                          alt={member.user.username}
                        />
                      ) : (
                        <div className="avatar-placeholder">
                          {member.user.username[0].toUpperCase()}
                        </div>
                      )}
                    </div>
                    <div className="member-details">
                      <div className="member-name">
                        {member.user.username}
                        {member.role === "Admin" && (
                          <span className="admin-badge">Admin</span>
                        )}
                      </div>
                      <div className="member-email">{member.user.email}</div>
                    </div>
                  </div>
                  {isAdmin && member.user.id !== user?.id && (
                    <button
                      className="remove-member-btn"
                      onClick={() => handleRemoveMember(member.user.id)}
                      title="Remove member"
                    >
                      <FiUserMinus />
                    </button>
                  )}
                </div>
              ))}
            </div>
            {isAdmin && (
              <div className="group-actions">
                <button
                  className="add-member-btn"
                  onClick={() => {
                    const userId = prompt("Enter user ID to add:");
                    if (userId) {
                      handleAddMember(userId);
                    }
                  }}
                >
                  <FiUserPlus /> Add Member
                </button>
              </div>
            )}
          </div>
        </div>
      )}

      <div className="chat-messages">
        {messages.map((message) => (
          <MessageBubble
            key={message.id}
            message={message}
            onReply={setReplyingTo}
            onReaction={async (emoji) => {
              try {
                await api.post(`/messages/${message.id}/reaction`, {
                  messageId: message.id,
                  emoji,
                });
              } catch (error) {
                console.error("Failed to add reaction:", error);
              }
            }}
          />
        ))}
        {typingUsers.size > 0 && (
          <div className="typing-indicator" style={{ padding: "0.5rem 1rem", fontStyle: "italic", color: "#667781", backgroundColor: "#f0f2f5", borderRadius: "8px", margin: "0.5rem 0" }}>
            {Array.from(typingUsers).length === 1 ? (
              <span>{Array.from(typingUsers)[0]} is typing...</span>
            ) : (
              <span>{Array.from(typingUsers).slice(0, -1).join(", ")} and {Array.from(typingUsers).slice(-1)[0]} are typing...</span>
            )}
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      {replyingTo && (
        <div className="reply-preview">
          <div className="reply-content">
            <div className="reply-label">
              Replying to {replyingTo.senderName}
            </div>
            <div className="reply-text">{replyingTo.content}</div>
          </div>
          <button className="reply-cancel" onClick={() => setReplyingTo(null)}>
            <FiX />
          </button>
        </div>
      )}

      <div className="chat-input">
        <label className="attach-btn">
          <FiPaperclip />
          <input
            type="file"
            onChange={handleFileUpload}
            style={{ display: "none" }}
            accept="image/*,video/*,audio/*,.pdf,.doc,.docx"
          />
        </label>
        <input
          type="text"
          placeholder="Type a message..."
          value={newMessage}
          onChange={(e) => {
            setNewMessage(e.target.value);
            handleTyping(); // Send typing indicator when user types
          }}
          onKeyPress={(e) => {
            if (e.key === "Enter" && !e.shiftKey) {
              e.preventDefault();
              sendMessage();
            }
          }}
          onBlur={stopTyping} // Stop typing when input loses focus
        />
        <button className="send-btn" onClick={sendMessage}>
          <FiSend />
        </button>
      </div>
    </div>
  );
}

export default GroupChatWindow;

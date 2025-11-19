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
  const messagesEndRef = useRef(null);

  useEffect(() => {
    loadMessages();
    joinGroup();
    checkAdminStatus();

    const handleNewMessageWrapper = (message) => handleNewMessage(message);
    const handleUserTypingWrapper = (groupId, userId, userName, typing) =>
      handleUserTyping(groupId, userId, userName, typing);
    const handleReactionUpdateWrapper = (message) =>
      handleReactionUpdate(message);
    const handleMessageDeletedWrapper = (messageId) =>
      handleMessageDeleted(messageId);

    signalRService.on("NewGroupMessage", handleNewMessageWrapper);
    signalRService.on("UserTypingGroup", handleUserTypingWrapper);
    signalRService.on("MessageReactionUpdated", handleReactionUpdateWrapper);
    signalRService.on("MessageDeleted", handleMessageDeletedWrapper);

    return () => {
      leaveGroup();
      signalRService.off("NewGroupMessage", handleNewMessageWrapper);
      signalRService.off("UserTypingGroup", handleUserTypingWrapper);
      signalRService.off("MessageReactionUpdated", handleReactionUpdateWrapper);
      signalRService.off("MessageDeleted", handleMessageDeletedWrapper);
    };
  }, [group?.id]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

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
    if (group?.id) {
      await signalRService.invoke("JoinGroup", group.id);
    }
  };

  const leaveGroup = async () => {
    if (group?.id) {
      await signalRService.invoke("LeaveGroup", group.id);
    }
  };

  const handleNewMessage = (message) => {
    if (message.groupId === group.id) {
      setMessages((prev) => [...prev, message]);
    }
  };

  const handleUserTyping = (groupId, userId, userName, typing) => {
    if (groupId === group.id) {
      setTypingUsers((prev) => {
        const newSet = new Set(prev);
        if (typing) {
          newSet.add(userName || userId);
        } else {
          newSet.delete(userName || userId);
        }
        return newSet;
      });
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

    try {
      const response = await api.post("/messages", {
        groupId: group.id,
        content: newMessage,
        type: "Text",
        replyToMessageId: replyingTo?.id,
      });

      setMessages((prev) => [...prev, response.data]);
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

      setMessages((prev) => [...prev, response.data]);
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
          <div className="typing-indicator">
            {Array.from(typingUsers).map((userName, index) => (
              <span key={index}>{userName} is typing...</span>
            ))}
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
          onChange={(e) => setNewMessage(e.target.value)}
          onKeyPress={(e) => {
            if (e.key === "Enter" && !e.shiftKey) {
              e.preventDefault();
              sendMessage();
            }
          }}
        />
        <button className="send-btn" onClick={sendMessage}>
          <FiSend />
        </button>
      </div>
    </div>
  );
}

export default GroupChatWindow;

import React, { useState, useEffect, useRef } from "react";
import { FiSend, FiSmile, FiPaperclip, FiX } from "react-icons/fi";
import { format, formatDistanceToNow } from "date-fns";
import api from "../../services/api";
import signalRService from "../../services/signalRService";
import MessageBubble from "./MessageBubble";
import "./ChatWindow.css";

function ChatWindow({ chat, onClose, onChatUpdate }) {
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [replyingTo, setReplyingTo] = useState(null);
  const [isTyping, setIsTyping] = useState(false);
  const [typingUsers, setTypingUsers] = useState(new Set());
  const [currentChat, setCurrentChat] = useState(chat);
  const messagesEndRef = useRef(null);
  const typingTimeoutRef = useRef(null);

  useEffect(() => {
    setCurrentChat(chat);
  }, [chat]);

  useEffect(() => {
    if (!currentChat?.id) return;

    loadMessages();
    joinChat();

    const handleNewMessageWrapper = (message) => {
      if (message.chatId === currentChat.id) {
        setMessages((prev) => [...prev, message]);
      }
    };

    const handleUserTypingWrapper = (chatId, userId, userName, typing) => {
      if (chatId === currentChat.id) {
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

    const handleReactionUpdateWrapper = (message) => {
      if (message.chatId === currentChat.id) {
        setMessages((prev) =>
          prev.map((m) => (m.id === message.id ? message : m))
        );
      }
    };

    const handleMessageDeletedWrapper = (messageId) => {
      setMessages((prev) => prev.filter((m) => m.id !== messageId));
    };

    const handleUserOnlineWrapper = (userId) => {
      if (currentChat.otherUser?.id === userId) {
        setCurrentChat((prev) => ({
          ...prev,
          otherUser: { ...prev.otherUser, isOnline: true },
        }));
        if (onChatUpdate) {
          onChatUpdate({
            ...currentChat,
            otherUser: { ...currentChat.otherUser, isOnline: true },
          });
        }
      }
    };

    const handleUserOfflineWrapper = (userId) => {
      if (currentChat.otherUser?.id === userId) {
        setCurrentChat((prev) => ({
          ...prev,
          otherUser: {
            ...prev.otherUser,
            isOnline: false,
            lastSeen: new Date().toISOString(),
          },
        }));
        if (onChatUpdate) {
          onChatUpdate({
            ...currentChat,
            otherUser: {
              ...currentChat.otherUser,
              isOnline: false,
              lastSeen: new Date().toISOString(),
            },
          });
        }
      }
    };

    signalRService.on("NewMessage", handleNewMessageWrapper);
    signalRService.on("UserTyping", handleUserTypingWrapper);
    signalRService.on("MessageReactionUpdated", handleReactionUpdateWrapper);
    signalRService.on("MessageDeleted", handleMessageDeletedWrapper);
    signalRService.on("UserOnline", handleUserOnlineWrapper);
    signalRService.on("UserOffline", handleUserOfflineWrapper);

    return () => {
      leaveChat();
      signalRService.off("NewMessage", handleNewMessageWrapper);
      signalRService.off("UserTyping", handleUserTypingWrapper);
      signalRService.off("MessageReactionUpdated", handleReactionUpdateWrapper);
      signalRService.off("MessageDeleted", handleMessageDeletedWrapper);
      signalRService.off("UserOnline", handleUserOnlineWrapper);
      signalRService.off("UserOffline", handleUserOfflineWrapper);
    };
  }, [currentChat?.id, onChatUpdate]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const loadMessages = async () => {
    try {
      const response = await api.get(`/messages/chat/${currentChat?.id}`);
      setMessages(response.data);
      // Refresh chat list to update unread count
      window.dispatchEvent(new Event("refreshChatList"));
    } catch (error) {
      console.error("Failed to load messages:", error);
    }
  };

  const joinChat = async () => {
    if (currentChat?.id) {
      await signalRService.invoke("JoinChat", currentChat.id);
    }
  };

  const leaveChat = async () => {
    if (currentChat?.id) {
      await signalRService.invoke("LeaveChat", currentChat.id);
    }
  };

  const sendMessage = async () => {
    if (!newMessage.trim() && !replyingTo) return;

    try {
      const response = await api.post("/messages", {
        chatId: currentChat?.id,
        content: newMessage,
        type: "Text",
        replyToMessageId: replyingTo?.id,
      });

      setMessages((prev) => [...prev, response.data]);
      setNewMessage("");
      setReplyingTo(null);
      stopTyping();
    } catch (error) {
      console.error("Failed to send message:", error);
    }
  };

  const handleTyping = () => {
    if (!isTyping) {
      setIsTyping(true);
      signalRService.invoke("SendTyping", currentChat?.id, true);
    }

    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    typingTimeoutRef.current = setTimeout(() => {
      setIsTyping(false);
      signalRService.invoke("SendTyping", currentChat?.id, false);
    }, 1000);
  };

  const stopTyping = () => {
    if (isTyping) {
      setIsTyping(false);
      signalRService.invoke("SendTyping", currentChat?.id, false);
    }
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }
  };

  const handleFileUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("file", file);
    formData.append("chatId", currentChat?.id);

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

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  return (
    <div className="chat-window">
      <div className="chat-window-header">
        <div className="chat-header-info">
          <div className="chat-header-avatar">
            {currentChat?.otherUser?.profilePictureUrl ? (
              <img
                src={currentChat.otherUser.profilePictureUrl}
                alt={currentChat.otherUser.username}
              />
            ) : (
              <div className="avatar-placeholder">
                {currentChat?.otherUser?.username?.[0]?.toUpperCase()}
              </div>
            )}
            {currentChat?.otherUser?.isOnline && (
              <div className="online-indicator" />
            )}
          </div>
          <div className="chat-header-details">
            <div className="chat-header-name">
              {currentChat?.otherUser?.username}
            </div>
            <div className="chat-header-status">
              {currentChat?.otherUser?.isOnline
                ? "Online"
                : currentChat?.otherUser?.lastSeen
                ? `Last seen ${formatDistanceToNow(
                    new Date(currentChat.otherUser.lastSeen),
                    { addSuffix: true }
                  )}`
                : "Offline"}
            </div>
          </div>
        </div>
        <button className="close-btn" onClick={onClose}>
          <FiX />
        </button>
      </div>

      <div className="chat-messages">
        {messages.map((message, index) => (
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
          onChange={(e) => {
            setNewMessage(e.target.value);
            handleTyping();
          }}
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

export default ChatWindow;

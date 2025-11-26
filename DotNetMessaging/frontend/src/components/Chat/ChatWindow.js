import React, { useState, useEffect, useRef } from "react";
import { FiSend, FiSmile, FiPaperclip, FiX } from "react-icons/fi";
import { format, formatDistanceToNow } from "date-fns";
import * as signalR from "@microsoft/signalr";
import { useAuth } from "../../contexts/AuthContext";
import api from "../../services/api";
import signalRService from "../../services/signalRService";
import MessageBubble from "./MessageBubble";
import "./ChatWindow.css";

function ChatWindow({ chat, onClose, onChatUpdate }) {
  const { user: currentUser } = useAuth();
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [replyingTo, setReplyingTo] = useState(null);
  const [isTyping, setIsTyping] = useState(false);
  const [typingUsers, setTypingUsers] = useState(new Set());
  const [currentChat, setCurrentChat] = useState(chat);
  const messagesEndRef = useRef(null);
  const typingTimeoutRef = useRef(null);
  const onChatUpdateRef = useRef(onChatUpdate);
  const currentChatIdRef = useRef(currentChat?.id);

  // Keep onChatUpdate ref up to date
  useEffect(() => {
    onChatUpdateRef.current = onChatUpdate;
  }, [onChatUpdate]);

  console.log("currentChat", currentChat);
  useEffect(() => {
    setCurrentChat(chat);
    currentChatIdRef.current = chat?.id;
  }, [chat]);

  useEffect(() => {
    if (!currentChat?.id) return;

    // Clear typing users when chat changes
    setTypingUsers(new Set());

    loadMessages();

    // Ensure SignalR is connected before joining chat
    const initializeChat = async () => {
      if (!signalRService.isConnected()) {
        console.log("Waiting for SignalR connection...");
        // Wait a bit for connection if not ready
        await new Promise((resolve) => setTimeout(resolve, 500));
      }

      if (signalRService.isConnected()) {
        await joinChat();
      } else {
        console.error("Cannot join chat: SignalR not connected");
      }
    };

    initializeChat();

    const handleNewMessageWrapper = (message) => {
      if (message.chatId === currentChat.id) {
        setMessages((prev) => {
          // Check if message already exists to prevent duplicates
          const messageExists = prev.some((m) => m.id === message.id);
          if (messageExists) {
            console.log(
              "[ChatWindow] Message already exists, skipping duplicate:",
              message.id
            );
            return prev;
          }
          return [...prev, message];
        });
      }
    };

    const handleUserTypingWrapper = (chatId, userId, userName, typing) => {
      console.log("🔵 UserTyping event received:", {
        chatId,
        userId,
        userName,
        typing,
        currentChatId: currentChatIdRef.current,
        currentUserId: currentUser?.id,
      });

      // Use ref to get current chat ID to avoid closure issues
      if (chatId === currentChatIdRef.current) {
        // Filter out current user - don't show "you are typing" to yourself
        if (userId === currentUser?.id) {
          console.log("⚠️ Ignoring typing from current user");
          return;
        }

        console.log(
          `✅ Updating typing indicator: ${userName} is ${
            typing ? "typing" : "not typing"
          }`
        );
        setTypingUsers((prev) => {
          const newSet = new Set(prev);
          const userKey = userName || userId;
          if (typing) {
            newSet.add(userKey);
          } else {
            newSet.delete(userKey);
          }
          console.log("📝 Typing users set:", Array.from(newSet));
          return newSet;
        });
      } else {
        console.log(
          `⚠️ Ignoring typing for different chat. Current: ${currentChatIdRef.current}, Received: ${chatId}`
        );
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
        if (onChatUpdateRef.current) {
          onChatUpdateRef.current({
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
        if (onChatUpdateRef.current) {
          onChatUpdateRef.current({
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

    // Register event handlers AFTER ensuring connection is ready
    const registerHandlers = () => {
      if (signalRService.isConnected()) {
        console.log(
          "🔌 Registering SignalR event handlers for chat:",
          currentChatIdRef.current
        );
        signalRService.on("NewMessage", handleNewMessageWrapper);
        signalRService.on("UserTyping", handleUserTypingWrapper);
        signalRService.on(
          "MessageReactionUpdated",
          handleReactionUpdateWrapper
        );
        signalRService.on("MessageDeleted", handleMessageDeletedWrapper);
        signalRService.on("UserOnline", handleUserOnlineWrapper);
        signalRService.on("UserOffline", handleUserOfflineWrapper);
        console.log("✅ All event handlers registered");
        return true;
      } else {
        console.warn("⚠️ Cannot register handlers: SignalR not connected");
        return false;
      }
    };

    // Listen for connection state changes and re-register handlers on reconnect
    const connection = signalRService.getConnection();
    const handleConnectionStateChange = () => {
      if (connection?.state === signalR.HubConnectionState.Connected) {
        console.log(
          "🔌 Connection state changed to Connected, re-registering handlers"
        );
        registerHandlers();
      }
    };

    if (connection) {
      connection.onclose(() => {
        console.log("❌ SignalR connection closed");
      });
      connection.onreconnecting(() => {
        console.log("🔄 SignalR reconnecting...");
      });
      connection.onreconnected(() => {
        console.log("✅ SignalR reconnected, re-registering handlers");
        registerHandlers();
        // Rejoin chat after reconnection
        joinChat();
      });
    }

    // Register handlers after a short delay to ensure connection is ready
    const handlerTimeout = setTimeout(() => {
      if (!registerHandlers()) {
        // If not connected, try again after a longer delay
        setTimeout(() => registerHandlers(), 1000);
      }
    }, 100);

    // Also register immediately if already connected
    if (signalRService.isConnected()) {
      registerHandlers();
    }

    return () => {
      clearTimeout(handlerTimeout);
      leaveChat();
      signalRService.off("NewMessage", handleNewMessageWrapper);
      signalRService.off("UserTyping", handleUserTypingWrapper);
      signalRService.off("MessageReactionUpdated", handleReactionUpdateWrapper);
      signalRService.off("MessageDeleted", handleMessageDeletedWrapper);
      signalRService.off("UserOnline", handleUserOnlineWrapper);
      signalRService.off("UserOffline", handleUserOfflineWrapper);
    };
  }, [currentChat?.id, currentUser?.id]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  // Scroll when typing indicator appears/disappears
  useEffect(() => {
    if (typingUsers.size > 0) {
      scrollToBottom();
    }
  }, [typingUsers.size]);

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
      try {
        // Wait for SignalR connection to be ready
        if (!signalRService.isConnected()) {
          console.warn("SignalR not connected, cannot join chat");
          return;
        }

        await signalRService.invoke("JoinChat", currentChat.id);
        console.log(`✅ Successfully joined chat group: ${currentChat.id}`);
      } catch (error) {
        console.error("❌ Failed to join chat:", error);
      }
    }
  };

  const leaveChat = async () => {
    if (currentChat?.id && signalRService.isConnected()) {
      try {
        await signalRService.invoke("LeaveChat", currentChat.id);
      } catch (error) {
        // Silently fail during cleanup/logout
        console.warn("Failed to leave chat during cleanup:", error);
      }
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

      // Add message optimistically - SignalR will also broadcast it, but we check for duplicates
      setMessages((prev) => {
        const messageExists = prev.some((m) => m.id === response.data.id);
        if (messageExists) {
          console.log(
            "[ChatWindow] Message already exists, skipping:",
            response.data.id
          );
          return prev;
        }
        return [...prev, response.data];
      });

      setNewMessage("");
      setReplyingTo(null);
      stopTyping();
    } catch (error) {
      console.error("Failed to send message:", error);
    }
  };

  const handleTyping = () => {
    const chatId = currentChat?.id;
    if (!chatId) {
      console.warn("Cannot send typing indicator: no chat ID");
      return;
    }

    // Check if SignalR connection is ready
    if (!signalRService.isConnected()) {
      console.warn("Cannot send typing indicator: SignalR not connected");
      return;
    }

    if (!isTyping) {
      setIsTyping(true);
      console.log(`Sending typing indicator: true for chat ${chatId}`);
      signalRService
        .invoke("SendTyping", chatId, true)
        .then(() => console.log("Typing indicator sent successfully"))
        .catch((error) => {
          console.error("Failed to send typing indicator:", error);
        });
    }

    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    // Increase timeout to 3 seconds for better UX
    typingTimeoutRef.current = setTimeout(() => {
      setIsTyping(false);
      console.log(`Sending typing indicator: false for chat ${chatId}`);
      signalRService
        .invoke("SendTyping", chatId, false)
        .then(() => console.log("Typing stop indicator sent successfully"))
        .catch((error) => {
          console.error("Failed to stop typing indicator:", error);
        });
    }, 3000);
  };

  const stopTyping = () => {
    const chatId = currentChat?.id;
    if (!chatId) return;

    if (isTyping) {
      setIsTyping(false);
      signalRService.invoke("SendTyping", chatId, false).catch((error) => {
        console.error("Failed to stop typing indicator:", error);
      });
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

      // Add message optimistically - SignalR will also broadcast it, but we check for duplicates
      setMessages((prev) => {
        const messageExists = prev.some((m) => m.id === response.data.id);
        if (messageExists) {
          console.log(
            "[ChatWindow] File message already exists, skipping:",
            response.data.id
          );
          return prev;
        }
        return [...prev, response.data];
      });
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
              <span key={index}>{userName || "Someone"} is typing...</span>
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

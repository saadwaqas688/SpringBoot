import React, { useState, useEffect, useCallback } from "react";
import { useAuth } from "../contexts/AuthContext";
import signalRService from "../services/signalRService";
import api from "../services/api";
import ChatList from "./Chat/ChatList";
import ChatWindow from "./Chat/ChatWindow";
import GroupChatWindow from "./Chat/GroupChatWindow";
import ContactsModal from "./Chat/ContactsModal";
import CreateGroupModal from "./Chat/CreateGroupModal";
import "./ChatApp.css";

function ChatApp() {
  const { user, logout, loading } = useAuth();
  const [selectedChat, setSelectedChat] = useState(null);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [chats, setChats] = useState([]);
  const [groups, setGroups] = useState([]);
  const [showContacts, setShowContacts] = useState(false);
  const [showCreateGroup, setShowCreateGroup] = useState(false);

  // Memoize the onChatUpdate callback to prevent unnecessary re-renders
  const handleChatUpdate = useCallback((updatedChat) => {
    setSelectedChat(updatedChat);
  }, []);

  useEffect(() => {
    // Only start SignalR connection after auth is ready and user is authenticated
    if (!loading && user) {
      const token = localStorage.getItem("token");
      if (token) {
        signalRService.startConnection(token).catch(console.error);
      }
    }

    return () => {
      signalRService.stopConnection();
    };
  }, [loading, user]);

  useEffect(() => {
    // Only set up API refresh handlers after auth is ready
    if (loading || !user) return;

    const refreshChatList = async () => {
      try {
        const chatsResponse = await api.get("/chats");
        setChats(chatsResponse.data);
      } catch (error) {
        console.error("Failed to refresh chats:", error);
      }
    };

    const refreshGroupList = async () => {
      try {
        const groupsResponse = await api.get("/groups");
        setGroups(groupsResponse.data);
      } catch (error) {
        console.error("Failed to refresh groups:", error);
      }
    };

    window.addEventListener("refreshChatList", refreshChatList);
    window.addEventListener("refreshGroupList", refreshGroupList);

    // Listen for new messages
    signalRService.on("NewMessage", (message) => {
      setChats((prev) =>
        prev.map((chat) => {
          if (chat.id === message.chatId) {
            return {
              ...chat,
              lastMessage: message,
              unreadCount: chat.unreadCount + 1,
            };
          }
          return chat;
        })
      );
    });

    signalRService.on("NewGroupMessage", (message) => {
      setGroups((prev) =>
        prev.map((group) => {
          if (group.id === message.groupId) {
            return { ...group, lastMessage: message };
          }
          return group;
        })
      );
    });

    signalRService.on("UserOnline", (userId) => {
      setChats((prev) =>
        prev.map((chat) => {
          if (chat.otherUser?.id === userId) {
            return {
              ...chat,
              otherUser: { ...chat.otherUser, isOnline: true },
            };
          }
          return chat;
        })
      );
      // Also update selected chat if it's the same user
      setSelectedChat((prev) => {
        if (prev && prev.otherUser?.id === userId) {
          return {
            ...prev,
            otherUser: { ...prev.otherUser, isOnline: true },
          };
        }
        return prev;
      });
    });

    signalRService.on("UserOffline", (userId) => {
      setChats((prev) =>
        prev.map((chat) => {
          if (chat.otherUser?.id === userId) {
            return {
              ...chat,
              otherUser: {
                ...chat.otherUser,
                isOnline: false,
                lastSeen: new Date().toISOString(),
              },
            };
          }
          return chat;
        })
      );
      // Also update selected chat if it's the same user
      setSelectedChat((prev) => {
        if (prev && prev.otherUser?.id === userId) {
          return {
            ...prev,
            otherUser: {
              ...prev.otherUser,
              isOnline: false,
              lastSeen: new Date().toISOString(),
            },
          };
        }
        return prev;
      });
    });

    return () => {
      window.removeEventListener("refreshChatList", refreshChatList);
      window.removeEventListener("refreshGroupList", refreshGroupList);
      signalRService.off("NewMessage");
      signalRService.off("NewGroupMessage");
      signalRService.off("UserOnline");
      signalRService.off("UserOffline");
    };
  }, [loading, user]);

  return (
    <div className="chat-app">
      <div className="chat-sidebar">
        <div className="chat-header">
          <div className="user-info">
            <div className="user-avatar">
              {user?.username?.[0]?.toUpperCase()}
            </div>
            <div className="user-details">
              <div className="user-name">{user?.username}</div>
              <div className="user-status">{user?.status || "Available"}</div>
            </div>
          </div>
          <button className="logout-btn" onClick={logout}>
            Logout
          </button>
        </div>
        <ChatList
          chats={chats}
          groups={groups}
          setChats={setChats}
          setGroups={setGroups}
          selectedChat={selectedChat}
          selectedGroup={selectedGroup}
          onSelectChat={setSelectedChat}
          onSelectGroup={setSelectedGroup}
          onShowContacts={() => setShowContacts(true)}
        />
      </div>
      <div className="chat-main">
        {selectedChat ? (
          <ChatWindow
            chat={selectedChat}
            onClose={() => setSelectedChat(null)}
            onChatUpdate={handleChatUpdate}
          />
        ) : selectedGroup ? (
          <GroupChatWindow
            group={selectedGroup}
            onClose={() => setSelectedGroup(null)}
          />
        ) : (
          <div className="no-chat-selected">
            <h2>Select a chat to start messaging</h2>
          </div>
        )}
      </div>

      <ContactsModal
        isOpen={showContacts}
        onClose={() => setShowContacts(false)}
        onStartChat={(chat) => {
          setSelectedChat(chat);
          setChats((prev) => {
            const exists = prev.find((c) => c.id === chat.id);
            if (exists) return prev;
            return [chat, ...prev];
          });
        }}
        onCreateGroup={() => {
          setShowContacts(false);
          setShowCreateGroup(true);
        }}
      />

      <CreateGroupModal
        isOpen={showCreateGroup}
        onClose={() => setShowCreateGroup(false)}
        onGroupCreated={(group) => {
          setSelectedGroup(group);
          setGroups((prev) => [group, ...prev]);
        }}
      />
    </div>
  );
}

export default ChatApp;

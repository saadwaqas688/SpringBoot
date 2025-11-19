import React, { useEffect, useState } from "react";
import { FiSearch, FiPlus, FiUsers } from "react-icons/fi";
import api from "../../services/api";
import "./ChatList.css";

function ChatList({
  chats,
  groups,
  setChats,
  setGroups,
  selectedChat,
  selectedGroup,
  onSelectChat,
  onSelectGroup,
  onShowContacts,
}) {
  const [searchQuery, setSearchQuery] = useState("");
  const [activeTab, setActiveTab] = useState("chats");

  useEffect(() => {
    loadChats();
    loadGroups();
  }, []);

  useEffect(() => {
    const handleRefresh = () => {
      loadChats();
      loadGroups();
    };

    window.addEventListener("refreshChatList", handleRefresh);
    window.addEventListener("refreshGroupList", handleRefresh);

    return () => {
      window.removeEventListener("refreshChatList", handleRefresh);
      window.removeEventListener("refreshGroupList", handleRefresh);
    };
  }, []);

  const loadChats = async () => {
    try {
      const response = await api.get("/chats");
      setChats(response.data);
    } catch (error) {
      console.error("Failed to load chats:", error);
    }
  };

  const loadGroups = async () => {
    try {
      const response = await api.get("/groups");
      setGroups(response.data);
    } catch (error) {
      console.error("Failed to load groups:", error);
    }
  };

  const filteredChats = chats.filter((chat) =>
    chat.otherUser.username.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const filteredGroups = groups.filter((group) =>
    group.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const formatTime = (date) => {
    if (!date) return "";
    const d = new Date(date);
    const now = new Date();
    const diff = now - d;
    const minutes = Math.floor(diff / 60000);

    if (minutes < 1) return "Just now";
    if (minutes < 60) return `${minutes}m ago`;
    if (minutes < 1440) return `${Math.floor(minutes / 60)}h ago`;
    return d.toLocaleDateString();
  };

  return (
    <div className="chat-list">
      <div className="chat-list-header">
        <div className="search-box">
          <FiSearch className="search-icon" />
          <input
            type="text"
            placeholder="Search..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
        <div className="chat-list-actions">
          <button
            className="icon-btn"
            onClick={onShowContacts}
            title="New Chat"
          >
            <FiPlus />
          </button>
        </div>
      </div>

      <div className="chat-tabs">
        <button
          className={`tab ${activeTab === "chats" ? "active" : ""}`}
          onClick={() => setActiveTab("chats")}
        >
          Chats
        </button>
        <button
          className={`tab ${activeTab === "groups" ? "active" : ""}`}
          onClick={() => setActiveTab("groups")}
        >
          <FiUsers /> Groups
        </button>
      </div>

      <div className="chat-items">
        {activeTab === "chats" ? (
          filteredChats.length > 0 ? (
            filteredChats.map((chat) => (
              <div
                key={chat.id}
                className={`chat-item ${
                  selectedChat?.id === chat.id ? "active" : ""
                }`}
                onClick={() => onSelectChat(chat)}
              >
                <div className="chat-item-avatar">
                  {chat.otherUser.profilePictureUrl ? (
                    <img
                      src={chat.otherUser.profilePictureUrl}
                      alt={chat.otherUser.username}
                    />
                  ) : (
                    <div className="avatar-placeholder">
                      {chat.otherUser.username[0].toUpperCase()}
                    </div>
                  )}
                  {chat.otherUser.isOnline && (
                    <div className="online-indicator" />
                  )}
                </div>
                <div className="chat-item-content">
                  <div className="chat-item-header">
                    <div className="chat-item-name">
                      {chat.otherUser.username}
                    </div>
                    {chat.lastMessage && (
                      <div className="chat-item-time">
                        {formatTime(chat.lastMessage.createdAt)}
                      </div>
                    )}
                  </div>
                  <div className="chat-item-preview">
                    {chat.lastMessage ? (
                      <>
                        {chat.lastMessage.type !== "Text" && (
                          <span className="media-indicator">
                            {chat.lastMessage.type === "Image" && "📷"}
                            {chat.lastMessage.type === "Video" && "🎥"}
                            {chat.lastMessage.type === "Audio" && "🎵"}
                            {chat.lastMessage.type === "Document" && "📄"}{" "}
                          </span>
                        )}
                        {chat.lastMessage.content}
                      </>
                    ) : (
                      "No messages yet"
                    )}
                  </div>
                </div>
                {chat.unreadCount > 0 && (
                  <div className="unread-badge">{chat.unreadCount}</div>
                )}
              </div>
            ))
          ) : (
            <div className="empty-state">No chats found</div>
          )
        ) : filteredGroups.length > 0 ? (
          filteredGroups.map((group) => (
            <div
              key={group.id}
              className={`chat-item ${
                selectedGroup?.id === group.id ? "active" : ""
              }`}
              onClick={() => onSelectGroup(group)}
            >
              <div className="chat-item-avatar">
                {group.profilePictureUrl ? (
                  <img src={group.profilePictureUrl} alt={group.name} />
                ) : (
                  <div className="avatar-placeholder group-avatar">
                    <FiUsers />
                  </div>
                )}
              </div>
              <div className="chat-item-content">
                <div className="chat-item-header">
                  <div className="chat-item-name">{group.name}</div>
                  {group.lastMessage && (
                    <div className="chat-item-time">
                      {formatTime(group.lastMessage.createdAt)}
                    </div>
                  )}
                </div>
                <div className="chat-item-preview">
                  {group.lastMessage ? (
                    <>
                      {group.lastMessage.senderName}:{" "}
                      {group.lastMessage.content}
                    </>
                  ) : (
                    "No messages yet"
                  )}
                </div>
              </div>
              {group.unreadCount > 0 && (
                <div className="unread-badge">{group.unreadCount}</div>
              )}
            </div>
          ))
        ) : (
          <div className="empty-state">No groups found</div>
        )}
      </div>
    </div>
  );
}

export default ChatList;

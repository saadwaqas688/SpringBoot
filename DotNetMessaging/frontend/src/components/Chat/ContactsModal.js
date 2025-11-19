import React, { useState, useEffect } from "react";
import { FiX, FiUserPlus } from "react-icons/fi";
import api from "../../services/api";
import "./ContactsModal.css";

function ContactsModal({ isOpen, onClose, onStartChat, onCreateGroup }) {
  const [searchQuery, setSearchQuery] = useState("");
  const [users, setUsers] = useState([]);
  const [contacts, setContacts] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isOpen) {
      loadContacts();
    }
  }, [isOpen]);

  const loadContacts = async () => {
    try {
      const response = await api.get("/users/contacts");
      setContacts(response.data);
    } catch (error) {
      console.error("Failed to load contacts:", error);
    }
  };

  const searchUsers = async (query) => {
    if (!query.trim()) {
      setUsers([]);
      return;
    }

    setLoading(true);
    try {
      const response = await api.get(
        `/users/search?query=${encodeURIComponent(query)}`
      );
      setUsers(response.data);
    } catch (error) {
      console.error("Failed to search users:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchChange = (e) => {
    const query = e.target.value;
    setSearchQuery(query);
    searchUsers(query);
  };

  const handleAddContact = async (userId) => {
    try {
      await api.post(`/users/contacts/${userId}`);
      loadContacts();
    } catch (error) {
      console.error("Failed to add contact:", error);
    }
  };

  const handleStartChat = async (userId) => {
    try {
      const response = await api.post(`/chats/with/${userId}`);
      onStartChat(response.data);
      onClose();
    } catch (error) {
      console.error("Failed to start chat:", error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>New Chat</h2>
          <button className="close-btn" onClick={onClose}>
            <FiX />
          </button>
        </div>

        <div className="modal-body">
          <div className="search-section">
            <input
              type="text"
              placeholder="Search users..."
              value={searchQuery}
              onChange={handleSearchChange}
              className="search-input"
            />
          </div>

          {searchQuery ? (
            <div className="users-list">
              <h3>Search Results</h3>
              {loading ? (
                <div className="loading">Loading...</div>
              ) : users.length > 0 ? (
                users.map((user) => (
                  <div key={user.id} className="user-item">
                    <div className="user-info">
                      <div className="user-avatar">
                        {user.profilePictureUrl ? (
                          <img
                            src={user.profilePictureUrl}
                            alt={user.username}
                          />
                        ) : (
                          <div className="avatar-placeholder">
                            {user.username[0].toUpperCase()}
                          </div>
                        )}
                        {user.isOnline && <div className="online-indicator" />}
                      </div>
                      <div className="user-details">
                        <div className="user-name">{user.username}</div>
                        <div className="user-email">{user.email}</div>
                      </div>
                    </div>
                    <div className="user-actions">
                      <button
                        className="action-btn"
                        onClick={() => handleAddContact(user.id)}
                        title="Add to contacts"
                      >
                        <FiUserPlus />
                      </button>
                      <button
                        className="chat-btn"
                        onClick={() => handleStartChat(user.id)}
                      >
                        Chat
                      </button>
                    </div>
                  </div>
                ))
              ) : (
                <div className="empty-state">No users found</div>
              )}
            </div>
          ) : (
            <div className="contacts-list">
              <h3>Contacts</h3>
              {contacts.length > 0 ? (
                contacts.map((contact) => (
                  <div
                    key={contact.id}
                    className="user-item"
                    onClick={() => handleStartChat(contact.id)}
                  >
                    <div className="user-info">
                      <div className="user-avatar">
                        {contact.profilePictureUrl ? (
                          <img
                            src={contact.profilePictureUrl}
                            alt={contact.username}
                          />
                        ) : (
                          <div className="avatar-placeholder">
                            {contact.username[0].toUpperCase()}
                          </div>
                        )}
                        {contact.isOnline && (
                          <div className="online-indicator" />
                        )}
                      </div>
                      <div className="user-details">
                        <div className="user-name">{contact.username}</div>
                        <div className="user-email">{contact.email}</div>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className="empty-state">
                  No contacts yet. Search for users to add them.
                </div>
              )}
            </div>
          )}

          <div className="modal-footer">
            <button className="create-group-btn" onClick={onCreateGroup}>
              Create Group
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ContactsModal;


import React, { useState, useEffect } from "react";
import { FiX, FiCheck } from "react-icons/fi";
import api from "../../services/api";
import "./ContactsModal.css";

function CreateGroupModal({ isOpen, onClose, onGroupCreated }) {
  const [groupName, setGroupName] = useState("");
  const [description, setDescription] = useState("");
  const [contacts, setContacts] = useState([]);
  const [selectedMembers, setSelectedMembers] = useState(new Set());
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
    if (!groupName.trim() || selectedMembers.size === 0) {
      alert("Please provide a group name and select at least one member");
      return;
    }

    setLoading(true);
    try {
      const response = await api.post("/groups", {
        name: groupName,
        description,
        memberIds: Array.from(selectedMembers),
      });

      onGroupCreated(response.data);
      onClose();
      setGroupName("");
      setDescription("");
      setSelectedMembers(new Set());
    } catch (error) {
      console.error("Failed to create group:", error);
      alert("Failed to create group");
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Create Group</h2>
          <button className="close-btn" onClick={onClose}>
            <FiX />
          </button>
        </div>

        <div className="modal-body">
          <div className="form-group" style={{ marginBottom: "1rem" }}>
            <label
              style={{
                display: "block",
                marginBottom: "0.5rem",
                fontWeight: 600,
                color: "#111b21",
              }}
            >
              Group Name
            </label>
            <input
              type="text"
              placeholder="Enter group name"
              value={groupName}
              onChange={(e) => setGroupName(e.target.value)}
              className="form-input"
              style={{
                width: "100%",
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
                padding: "0.75rem",
                border: "1px solid #e4e6eb",
                borderRadius: "8px",
                fontSize: "0.95rem",
                outline: "none",
                resize: "vertical",
              }}
            />
          </div>

          <div className="contacts-list">
            <h3>Select Members</h3>
            {contacts.length > 0 ? (
              contacts.map((contact) => (
                <div
                  key={contact.id}
                  className="user-item"
                  onClick={() => toggleMember(contact.id)}
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
                    </div>
                    <div className="user-details">
                      <div className="user-name">{contact.username}</div>
                      <div className="user-email">{contact.email}</div>
                    </div>
                  </div>
                  {selectedMembers.has(contact.id) && (
                    <div style={{ color: "#25d366", fontSize: "1.2rem" }}>
                      <FiCheck />
                    </div>
                  )}
                </div>
              ))
            ) : (
              <div className="empty-state">
                No contacts available. Add contacts first.
              </div>
            )}
          </div>

          <div className="modal-footer">
            <button
              className="create-group-btn"
              onClick={handleCreate}
              disabled={
                loading || !groupName.trim() || selectedMembers.size === 0
              }
            >
              {loading ? "Creating..." : "Create Group"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default CreateGroupModal;

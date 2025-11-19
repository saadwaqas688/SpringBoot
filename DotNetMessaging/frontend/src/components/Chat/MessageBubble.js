import React, { useState } from "react";
import { format } from "date-fns";
import { FiCornerUpRight, FiSmile } from "react-icons/fi";
import { useAuth } from "../../contexts/AuthContext";
import "./MessageBubble.css";

const EMOJI_OPTIONS = ["👍", "❤️", "😂", "😮", "😢", "🙏"];

function MessageBubble({ message, onReply, onReaction }) {
  const { user } = useAuth();
  const [showReactions, setShowReactions] = useState(false);
  const isOwn = message.senderId === user?.id;

  const handleReactionClick = (emoji) => {
    onReaction(emoji);
    setShowReactions(false);
  };

  const renderMedia = () => {
    if (!message.mediaUrl) return null;

    switch (message.type) {
      case "Image":
        return (
          <img
            src={`http://localhost:5000${message.mediaUrl}`}
            alt={message.content}
            className="message-media"
            onClick={() =>
              window.open(`http://localhost:5000${message.mediaUrl}`, "_blank")
            }
          />
        );
      case "Video":
        return (
          <video
            src={`http://localhost:5000${message.mediaUrl}`}
            controls
            className="message-media"
          />
        );
      case "Audio":
        return (
          <audio
            src={`http://localhost:5000${message.mediaUrl}`}
            controls
            className="message-audio"
          />
        );
      case "Document":
        return (
          <a
            href={`http://localhost:5000${message.mediaUrl}`}
            target="_blank"
            rel="noopener noreferrer"
            className="message-document"
          >
            📄 {message.mediaFileName || message.content}
          </a>
        );
      default:
        return null;
    }
  };

  // Group reactions by emoji
  const groupedReactions = message.reactions?.reduce((acc, reaction) => {
    if (!acc[reaction.emoji]) {
      acc[reaction.emoji] = [];
    }
    acc[reaction.emoji].push(reaction);
    return acc;
  }, {});

  return (
    <div className={`message-bubble ${isOwn ? "own" : "other"}`}>
      {!isOwn && <div className="message-sender">{message.senderName}</div>}

      {message.replyToMessage && (
        <div className="message-reply">
          <div className="reply-sender">
            {message.replyToMessage.senderName}
          </div>
          <div className="reply-content">{message.replyToMessage.content}</div>
        </div>
      )}

      <div className="message-content-wrapper">
        {renderMedia()}
        {message.type === "Text" && (
          <div className="message-content">{message.content}</div>
        )}

        <div className="message-footer">
          <div className="message-time">
            {format(new Date(message.createdAt), "HH:mm")}
          </div>
          {isOwn && <div className="message-status">✓✓</div>}
        </div>
      </div>

      {groupedReactions && Object.keys(groupedReactions).length > 0 && (
        <div className="message-reactions">
          {Object.entries(groupedReactions).map(([emoji, reactions]) => (
            <span
              key={emoji}
              className="reaction-badge"
              onClick={() => handleReactionClick(emoji)}
              title={reactions.map((r) => r.userName).join(", ")}
            >
              {emoji} {reactions.length}
            </span>
          ))}
        </div>
      )}

      <div className="message-actions">
        <button
          className="action-btn"
          onClick={() => setShowReactions(!showReactions)}
          title="Add reaction"
        >
          <FiSmile />
        </button>
        <button
          className="action-btn"
          onClick={() => onReply(message)}
          title="Reply"
        >
          <FiCornerUpRight />
        </button>
      </div>

      {showReactions && (
        <div className="reaction-picker">
          {EMOJI_OPTIONS.map((emoji) => (
            <button
              key={emoji}
              className="reaction-option"
              onClick={() => handleReactionClick(emoji)}
            >
              {emoji}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

export default MessageBubble;

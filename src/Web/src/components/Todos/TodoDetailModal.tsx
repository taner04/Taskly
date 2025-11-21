import { useState, useEffect } from "react";
import type { Todo } from "../../types/todo";
import "./TodoDetailModal.css";

// Validation constants
const MIN_TITLE_LENGTH = 3;
const MAX_TITLE_LENGTH = 100;
const MIN_DESCRIPTION_LENGTH = 3;
const MAX_DESCRIPTION_LENGTH = 512;

interface TodoDetailModalProps {
  todo: Todo | null;
  isOpen: boolean;
  onClose: () => void;
  onSave?: (todo: Todo) => void;
}

const TodoDetailModal = ({
  todo,
  isOpen,
  onClose,
  onSave,
}: TodoDetailModalProps) => {
  const [editedTodo, setEditedTodo] = useState<Todo | null>(null);
  const [saveErrors, setSaveErrors] = useState<{ [key: string]: string }>({});

  useEffect(() => {
    if (todo) {
      setEditedTodo(todo);
      setSaveErrors({});
    }
  }, [todo]);

  if (!isOpen || !editedTodo) return null;

  const validateFields = () => {
    const newErrors: { [key: string]: string } = {};

    // Validate title
    if (
      !editedTodo.title ||
      editedTodo.title.trim().length < MIN_TITLE_LENGTH
    ) {
      newErrors.title = `Title must be at least ${MIN_TITLE_LENGTH} characters`;
    } else if (editedTodo.title.length > MAX_TITLE_LENGTH) {
      newErrors.title = `Title must not exceed ${MAX_TITLE_LENGTH} characters`;
    }

    // Validate description
    if (
      editedTodo.description &&
      editedTodo.description.trim().length > 0 &&
      editedTodo.description.trim().length < MIN_DESCRIPTION_LENGTH
    ) {
      newErrors.description = `Description must be at least ${MIN_DESCRIPTION_LENGTH} characters`;
    } else if (
      editedTodo.description &&
      editedTodo.description.length > MAX_DESCRIPTION_LENGTH
    ) {
      newErrors.description = `Description must not exceed ${MAX_DESCRIPTION_LENGTH} characters`;
    }

    return newErrors;
  };

  const getFieldError = (field: string) => {
    const currentErrors = validateFields();
    return currentErrors[field] || "";
  };

  const isTitleValid = () => {
    return (
      editedTodo.title &&
      editedTodo.title.trim().length >= MIN_TITLE_LENGTH &&
      editedTodo.title.length <= MAX_TITLE_LENGTH
    );
  };

  const isDescriptionValid = () => {
    if (!editedTodo.description || editedTodo.description.trim().length === 0) {
      return true; // Optional field
    }
    return (
      editedTodo.description.trim().length >= MIN_DESCRIPTION_LENGTH &&
      editedTodo.description.length <= MAX_DESCRIPTION_LENGTH
    );
  };

  const handleSave = () => {
    const errors = validateFields();
    if (Object.keys(errors).length === 0) {
      setSaveErrors({});
      onSave?.(editedTodo);
      onClose();
    } else {
      setSaveErrors(errors);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2 className="modal-title">Edit Todo</h2>
          <button
            className="modal-close-button"
            onClick={onClose}
            aria-label="Close modal"
          >
            <svg
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <line x1="18" y1="6" x2="6" y2="18" />
              <line x1="6" y1="6" x2="18" y2="18" />
            </svg>
          </button>
        </div>
        <div className="modal-body">
          <div className="modal-field">
            <label className="modal-label">Title</label>
            <input
              type="text"
              className={`modal-input ${
                !isTitleValid() && editedTodo.title ? "error" : ""
              }`}
              value={editedTodo.title}
              onChange={(e) =>
                setEditedTodo({ ...editedTodo, title: e.target.value })
              }
            />
            <div className="modal-field-info">
              <span
                className={`char-count ${
                  !isTitleValid() && editedTodo.title
                    ? "error"
                    : isTitleValid()
                    ? "valid"
                    : ""
                }`}
              >
                {editedTodo.title.length}/{MAX_TITLE_LENGTH}
                {isTitleValid() && <span className="status-icon">✓</span>}
              </span>
              {!isTitleValid() && editedTodo.title && (
                <span className="error-message">{getFieldError("title")}</span>
              )}
            </div>
          </div>

          <div className="modal-field">
            <label className="modal-label">Description</label>
            <textarea
              className={`modal-textarea ${
                !isDescriptionValid() && editedTodo.description ? "error" : ""
              }`}
              value={editedTodo.description || ""}
              onChange={(e) =>
                setEditedTodo({ ...editedTodo, description: e.target.value })
              }
              placeholder="Add a description..."
              rows={4}
            />
            <div className="modal-field-info">
              <span
                className={`char-count ${
                  !isDescriptionValid() && editedTodo.description
                    ? "error"
                    : isDescriptionValid() && editedTodo.description
                    ? "valid"
                    : ""
                }`}
              >
                {(editedTodo.description || "").length}/{MAX_DESCRIPTION_LENGTH}
                {isDescriptionValid() && editedTodo.description && (
                  <span className="status-icon">✓</span>
                )}
              </span>
              {!isDescriptionValid() && editedTodo.description && (
                <span className="error-message">
                  {getFieldError("description")}
                </span>
              )}
            </div>
          </div>

          <div className="modal-field inline">
            <div className="modal-field">
              <label className="modal-label">Priority</label>
              <select
                className="modal-select"
                value={editedTodo.priority}
                onChange={(e) =>
                  setEditedTodo({
                    ...editedTodo,
                    priority: parseInt(e.target.value) as 0 | 1 | 2,
                  })
                }
              >
                <option value={0}>Low</option>
                <option value={1}>Normal</option>
                <option value={2}>High</option>
              </select>
            </div>

            <div className="modal-field">
              <label className="modal-label">Status</label>
              <select
                className="modal-select"
                value={editedTodo.isCompleted ? "completed" : "pending"}
                onChange={(e) =>
                  setEditedTodo({
                    ...editedTodo,
                    isCompleted: e.target.value === "completed",
                  })
                }
              >
                <option value="pending">Pending</option>
                <option value="completed">Completed</option>
              </select>
            </div>
          </div>
        </div>
        <div className="modal-footer">
          <button className="modal-button secondary" onClick={onClose}>
            Cancel
          </button>
          <button className="modal-button primary" onClick={handleSave}>
            Save Changes
          </button>
        </div>
      </div>

      {Object.keys(saveErrors).length > 0 && (
        <div className="error-modal-overlay">
          <div
            className="error-modal-content"
            onClick={(e) => e.stopPropagation()}
          >
            <h3 className="error-modal-title">Cannot Save</h3>
            <div className="error-modal-errors">
              {Object.entries(saveErrors).map(([field, message]) => (
                <div key={field} className="error-item">
                  <span className="error-field-name">
                    {field.charAt(0).toUpperCase() + field.slice(1)}
                  </span>
                  <span className="error-field-message">{message}</span>
                </div>
              ))}
            </div>
            <button
              className="error-modal-button"
              onClick={() => setSaveErrors({})}
            >
              Okay
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default TodoDetailModal;

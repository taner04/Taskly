import type { Todo } from "../../types/todo";
import "./TodoDetailModal.css";

interface TodoDetailModalProps {
  todo: Todo | null;
  isOpen: boolean;
  onClose: () => void;
}

const TodoDetailModal = ({ todo, isOpen, onClose }: TodoDetailModalProps) => {
  if (!isOpen || !todo) return null;

  const getPriorityColor = (priority: number) => {
    switch (priority) {
      case 2:
        return "priority-high";
      case 1:
        return "priority-normal";
      case 0:
        return "priority-low";
      default:
        return "priority-normal";
    }
  };

  const getPriorityLabel = (priority: number) => {
    switch (priority) {
      case 2:
        return "High";
      case 1:
        return "Normal";
      case 0:
        return "Low";
      default:
        return "Normal";
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2 className="modal-title">Todo Details</h2>
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
            <div
              className={`modal-value ${todo.isCompleted ? "completed" : ""}`}
            >
              {todo.title}
            </div>
          </div>

          <div className="modal-field">
            <label className="modal-label">Description</label>
            <div className="modal-value">
              {todo.description || (
                <span className="modal-empty">No description provided</span>
              )}
            </div>
          </div>

          <div className="modal-field">
            <label className="modal-label">Priority</label>
            <div className="modal-value">
              <span
                className={`priority-badge ${getPriorityColor(todo.priority)}`}
              >
                {getPriorityLabel(todo.priority)}
              </span>
            </div>
          </div>

          <div className="modal-field">
            <label className="modal-label">Status</label>
            <div className="modal-value">
              {todo.isCompleted ? (
                <span className="completed-badge">Completed</span>
              ) : (
                <span className="pending-badge">Pending</span>
              )}
            </div>
          </div>
        </div>
        <div className="modal-footer">
          <button className="modal-button secondary" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

export default TodoDetailModal;

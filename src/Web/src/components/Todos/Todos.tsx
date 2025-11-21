import { useEffect, useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import type { Todo } from "../../types/todo";
import { TodoPriority } from "../../types/todo";
import TodoDetailModal from "./TodoDetailModal";
import "./Todos.css";

// Dummy data
const dummyTodos: Todo[] = [
  {
    id: "1",
    title: "Complete project documentation",
    description:
      "Write comprehensive documentation for the Taskly project including API endpoints and frontend components",
    priority: TodoPriority.High,
    isCompleted: false,
    userId: "user-1",
  },
  {
    id: "2",
    title: "Review code changes",
    description: "Review all recent pull requests and provide feedback",
    priority: TodoPriority.Normal,
    isCompleted: false,
    userId: "user-1",
  },
  {
    id: "3",
    title: "Update dependencies",
    description:
      "Update npm packages to latest versions and fix any breaking changes",
    priority: TodoPriority.Low,
    isCompleted: true,
    userId: "user-1",
  },
  {
    id: "4",
    title: "Design new feature",
    description: "Create mockups and wireframes for the new dashboard feature",
    priority: TodoPriority.High,
    isCompleted: false,
    userId: "user-1",
  },
  {
    id: "5",
    title: "Write unit tests",
    description: "Add unit tests for the Todo component and API service",
    priority: TodoPriority.Normal,
    isCompleted: false,
    userId: "user-1",
  },
];

type ViewMode = "grid" | "list";
type PriorityFilter = "all" | 0 | 1 | 2;
type StatusFilter = "all" | "active" | "completed";

const Todos = () => {
  const { isLoading: authLoading } = useAuth0();
  const [todos, setTodos] = useState<Todo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [viewMode, setViewMode] = useState<ViewMode>("grid");
  const [selectedTodo, setSelectedTodo] = useState<Todo | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [priorityFilter, setPriorityFilter] = useState<PriorityFilter>("all");
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("all");
  const [contextMenu, setContextMenu] = useState<{
    todoId: string;
    x: number;
    y: number;
    alignLeft?: boolean;
  } | null>(null);

  useEffect(() => {
    // Load todos regardless of auth status (for dummy data demo)
    if (!authLoading) {
      // Simulate loading delay
      setTimeout(() => {
        setTodos(dummyTodos);
        setIsLoading(false);
      }, 500);
    }
  }, [authLoading]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      const target = event.target as HTMLElement;
      // Check if click is outside context menu
      if (!target.closest(".context-menu")) {
        closeContextMenu();
      }
    };

    if (contextMenu) {
      // Use capture phase to catch clicks immediately
      document.addEventListener("click", handleClickOutside, true);
      return () => {
        document.removeEventListener("click", handleClickOutside, true);
      };
    }
  }, [contextMenu]);

  const loadTodos = () => {
    setIsLoading(true);
    // Simulate loading delay
    setTimeout(() => {
      setTodos(dummyTodos);
      setIsLoading(false);
    }, 500);
  };

  const getPriorityColor = (priority: number) => {
    switch (priority) {
      case 2: // High
        return "priority-high";
      case 1: // Normal
        return "priority-normal";
      case 0: // Low
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

  const getFilteredTodos = () => {
    return todos.filter((todo) => {
      // Filter by search query (title)
      const matchesSearch = todo.title
        .toLowerCase()
        .includes(searchQuery.toLowerCase());

      // Filter by priority
      const matchesPriority =
        priorityFilter === "all" || todo.priority === priorityFilter;

      // Filter by status
      const matchesStatus =
        statusFilter === "all" ||
        (statusFilter === "completed" && todo.isCompleted) ||
        (statusFilter === "active" && !todo.isCompleted);

      return matchesSearch && matchesPriority && matchesStatus;
    });
  };

  const handleTodoDoubleClick = (todo: Todo) => {
    setSelectedTodo(todo);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedTodo(null);
  };

  const handleCompleteTodo = (e: React.MouseEvent, todoId: string) => {
    e.stopPropagation();
    setTodos(
      todos.map((todo) =>
        todo.id === todoId ? { ...todo, isCompleted: !todo.isCompleted } : todo
      )
    );
  };

  const handleDeleteTodo = (e: React.MouseEvent, todoId: string) => {
    e.stopPropagation();
    setTodos(todos.filter((todo) => todo.id !== todoId));
    setContextMenu(null);
  };

  const handleContextMenu = (e: React.MouseEvent, todoId: string) => {
    e.preventDefault();
    e.stopPropagation();

    // Check if menu would go off-screen to the right (use 200px as approximate menu width)
    const shouldAlignLeft = e.clientX + 200 > window.innerWidth;

    setContextMenu({
      todoId,
      x: e.clientX,
      y: e.clientY,
      alignLeft: shouldAlignLeft,
    });
  };

  const closeContextMenu = () => {
    setContextMenu(null);
  };

  if (authLoading || isLoading) {
    return (
      <div className="todos-container">
        <div className="todos-loading">
          <div className="loading-spinner"></div>
          <p>Loading todos...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="todos-container">
      <div className="todos-header">
        <h1 className="todos-title">My Todos</h1>
        <div className="todos-header-actions">
          <div className="view-toggle">
            <button
              onClick={() => setViewMode("grid")}
              className={`view-toggle-button ${
                viewMode === "grid" ? "active" : ""
              }`}
              aria-label="Grid view"
              title="Grid view"
            >
              <svg
                width="20"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
              >
                <rect x="3" y="3" width="7" height="7" />
                <rect x="14" y="3" width="7" height="7" />
                <rect x="3" y="14" width="7" height="7" />
                <rect x="14" y="14" width="7" height="7" />
              </svg>
            </button>
            <button
              onClick={() => setViewMode("list")}
              className={`view-toggle-button ${
                viewMode === "list" ? "active" : ""
              }`}
              aria-label="List view"
              title="List view"
            >
              <svg
                width="20"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
              >
                <line x1="8" y1="6" x2="21" y2="6" />
                <line x1="8" y1="12" x2="21" y2="12" />
                <line x1="8" y1="18" x2="21" y2="18" />
                <line x1="3" y1="6" x2="3.01" y2="6" />
                <line x1="3" y1="12" x2="3.01" y2="12" />
                <line x1="3" y1="18" x2="3.01" y2="18" />
              </svg>
            </button>
          </div>
          <button
            onClick={loadTodos}
            className="button refresh-button"
            aria-label="Refresh todos"
          >
            <svg
              width="20"
              height="20"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <path d="M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8" />
              <path d="M21 3v5h-5" />
              <path d="M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16" />
              <path d="M3 21v-5h5" />
            </svg>
          </button>
        </div>
      </div>

      <div className="todos-search-and-filters">
        <div className="search-and-dropdowns">
          <input
            type="text"
            placeholder="Search todos by title..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="todos-search-input"
          />

          <select
            value={priorityFilter}
            onChange={(e) => {
              const value = e.target.value;
              if (value === "all") {
                setPriorityFilter("all");
              } else {
                setPriorityFilter(parseInt(value) as 0 | 1 | 2);
              }
            }}
            className="filter-dropdown"
          >
            <option value="all">All Priorities</option>
            <option value={2}>High Priority</option>
            <option value={1}>Normal Priority</option>
            <option value={0}>Low Priority</option>
          </select>

          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value as StatusFilter)}
            className="filter-dropdown"
          >
            <option value="all">All Status</option>
            <option value="active">Active</option>
            <option value="completed">Completed</option>
          </select>
        </div>
      </div>

      {getFilteredTodos().length === 0 ? (
        <div className="todos-empty-state">
          <svg
            width="64"
            height="64"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.5"
          >
            <path d="M9 11l3 3L22 4" />
            <path d="M21 12v7a2 2 0 01-2 2H5a2 2 0 01-2-2V5a2 2 0 012-2h11" />
          </svg>
          <h2>
            {searchQuery || priorityFilter !== "all" || statusFilter !== "all"
              ? "No todos match your filters"
              : "No todos yet"}
          </h2>
          <p>
            {searchQuery || priorityFilter !== "all" || statusFilter !== "all"
              ? "Try adjusting your search or filters"
              : "Create your first todo to get started!"}
          </p>
        </div>
      ) : (
        <div
          className={`todos-list ${
            viewMode === "list" ? "list-view" : "grid-view"
          }`}
        >
          {getFilteredTodos().map((todo) => (
            <div
              key={todo.id}
              className={`todo-card ${todo.isCompleted ? "completed" : ""}`}
              onDoubleClick={() => handleTodoDoubleClick(todo)}
              style={{ cursor: "pointer" }}
            >
              <div className="todo-content">
                <div className="todo-header">
                  <h3 className="todo-title">{todo.title}</h3>
                  <div className="todo-badges">
                    <span
                      className={`priority-badge ${getPriorityColor(
                        todo.priority
                      )}`}
                    >
                      {getPriorityLabel(todo.priority)}
                    </span>
                    {todo.isCompleted && (
                      <span className="completed-badge">Completed</span>
                    )}
                  </div>
                </div>
                {todo.description && (
                  <p className="todo-description">{todo.description}</p>
                )}
              </div>
              <button
                className="todo-context-menu-button"
                onClick={(e) => handleContextMenu(e, todo.id)}
                aria-label="Actions"
                title="More actions"
              >
                <svg
                  width="20"
                  height="20"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                >
                  <circle cx="12" cy="5" r="2" />
                  <circle cx="12" cy="12" r="2" />
                  <circle cx="12" cy="19" r="2" />
                </svg>
              </button>
            </div>
          ))}
        </div>
      )}
      {contextMenu && (
        <div
          className={`context-menu ${
            contextMenu.alignLeft ? "align-left" : ""
          }`}
          style={{
            top: `${contextMenu.y}px`,
            left: contextMenu.alignLeft ? "auto" : `${contextMenu.x}px`,
            right: contextMenu.alignLeft
              ? `${window.innerWidth - contextMenu.x}px`
              : "auto",
          }}
          onMouseLeave={closeContextMenu}
        >
          <button
            className="context-menu-item"
            onClick={(e) => {
              e.stopPropagation();
              const todo = todos.find((t) => t.id === contextMenu.todoId);
              if (todo) {
                handleCompleteTodo(e, todo.id);
              }
            }}
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <polyline points="20 6 9 17 4 12" />
            </svg>
            <span>
              {todos.find((t) => t.id === contextMenu.todoId)?.isCompleted
                ? "Mark as Incomplete"
                : "Mark as Complete"}
            </span>
          </button>
          <button
            className="context-menu-item delete"
            onClick={(e) => {
              e.stopPropagation();
              handleDeleteTodo(e, contextMenu.todoId);
            }}
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <polyline points="3 6 5 6 21 6" />
              <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
              <line x1="10" y1="11" x2="10" y2="17" />
              <line x1="14" y1="11" x2="14" y2="17" />
            </svg>
            <span>Delete</span>
          </button>
        </div>
      )}
      <TodoDetailModal
        todo={selectedTodo}
        isOpen={isModalOpen}
        onClose={handleCloseModal}
        onSave={(updatedTodo) => {
          setTodos(
            todos.map((todo) =>
              todo.id === updatedTodo.id ? updatedTodo : todo
            )
          );
        }}
      />
    </div>
  );
};

export default Todos;

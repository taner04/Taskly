import { useState, useEffect } from "react";
import { useUser } from "../../contexts/UserContext";
import "./Sidebar.css";

type View = "home" | "todos";
type Theme = "light" | "dark";

interface SidebarProps {
  isAuthenticated: boolean;
  currentView?: View;
  onViewChange?: (view: View) => void;
  onShowProfileModal?: () => void;
}

const Sidebar = ({
  isAuthenticated,
  currentView = "home",
  onViewChange,
  onShowProfileModal,
}: SidebarProps) => {
  const { user, logout, loginWithRedirect } = useUser();
  const [showUserMenu, setShowUserMenu] = useState(false);
  const [theme, setTheme] = useState<Theme>(() => {
    const saved = localStorage.getItem("theme");
    return (saved as Theme) || "light";
  });

  // Apply theme to document
  useEffect(() => {
    localStorage.setItem("theme", theme);
    document.documentElement.setAttribute("data-theme", theme);
  }, [theme]);

  // Apply accent color on mount
  useEffect(() => {
    const savedColor = localStorage.getItem("accentColor");
    if (savedColor) {
      document.documentElement.style.setProperty(
        "--accent-primary",
        savedColor
      );
    }
  }, []);

  const handleNavClick = (
    e: React.MouseEvent<HTMLAnchorElement>,
    view: View
  ) => {
    e.preventDefault();
    onViewChange?.(view);
  };

  return (
    <aside className="sidebar expanded">
      <div className="sidebar-header">
        <h2 className="sidebar-title">Taskly</h2>
      </div>

      <nav className="sidebar-nav">
        <a
          href="#"
          className={`nav-item ${currentView === "home" ? "active" : ""}`}
          onClick={(e) => handleNavClick(e, "home")}
        >
          <svg
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
            <polyline points="9 22 9 12 15 12 15 22" />
          </svg>
          <span>Home</span>
        </a>
        <a
          href="#"
          className={`nav-item ${currentView === "todos" ? "active" : ""}`}
          onClick={(e) => handleNavClick(e, "todos")}
        >
          <svg
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <path d="M9 11l3 3L22 4" />
            <path d="M21 12v7a2 2 0 01-2 2H5a2 2 0 01-2-2V5a2 2 0 012-2h11" />
          </svg>
          <span>Todo</span>
        </a>
      </nav>

      <div className="sidebar-footer">
        <div className="footer-buttons-row">
          <div className="account-button-container">
            <button
              className="account-button"
              onClick={() => {
                if (isAuthenticated) {
                  onShowProfileModal?.();
                } else {
                  loginWithRedirect();
                }
              }}
              aria-label={isAuthenticated ? "Profile menu" : "Login"}
            >
              <svg
                width="20"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
              >
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                <circle cx="12" cy="7" r="4" />
              </svg>
              <span>{isAuthenticated ? "Account" : "Login"}</span>
            </button>
            {showUserMenu && isAuthenticated && (
              <div
                className="user-menu-popup"
                onMouseLeave={() => setShowUserMenu(false)}
              >
                <div className="user-info">
                  <div className="user-avatar">
                    {user?.picture && (
                      <img src={user.picture} alt={user.name || "User"} />
                    )}
                  </div>
                  <div className="user-details">
                    <div className="user-name">{user?.name}</div>
                    <div className="user-email">{user?.email}</div>
                  </div>
                </div>
                <div className="user-menu-divider"></div>
                <button
                  className="user-menu-item logout-item"
                  onClick={() => {
                    logout({
                      logoutParams: { returnTo: window.location.origin },
                    });
                    setShowUserMenu(false);
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
                    <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                    <polyline points="16 17 21 12 16 7" />
                    <line x1="21" y1="12" x2="9" y2="12" />
                  </svg>
                  Logout
                </button>
              </div>
            )}
          </div>

          <button
            className="theme-toggle-button"
            onClick={() => setTheme(theme === "light" ? "dark" : "light")}
            aria-label={`Switch to ${
              theme === "light" ? "dark" : "light"
            } theme`}
            title={`Switch to ${theme === "light" ? "dark" : "light"} theme`}
          >
            {theme === "light" ? (
              <svg
                width="20"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
              >
                <circle cx="12" cy="12" r="5" />
                <line x1="12" y1="1" x2="12" y2="3" />
                <line x1="12" y1="21" x2="12" y2="23" />
                <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
                <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
                <line x1="1" y1="12" x2="3" y2="12" />
                <line x1="21" y1="12" x2="23" y2="12" />
                <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
                <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
              </svg>
            ) : (
              <svg
                width="20"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
              >
                <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
              </svg>
            )}
          </button>
        </div>
      </div>
    </aside>
  );
};

export default Sidebar;

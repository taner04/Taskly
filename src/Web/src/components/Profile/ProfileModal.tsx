import { useRef, useState } from "react";
import { useUser } from "../../contexts/UserContext";
import packageJson from "../../../package.json";
import "./ProfileModal.css";

interface ProfileModalProps {
  onClose: () => void;
}

const ProfileModal = ({ onClose }: ProfileModalProps) => {
  const { user, logout } = useUser();
  const modalRef = useRef<HTMLDivElement>(null);
  const [activeTab, setActiveTab] = useState<"profile" | "settings" | "info">(
    "profile"
  );
  const [theme, setTheme] = useState<"light" | "dark">(() => {
    const saved = localStorage.getItem("theme");
    return (saved as "light" | "dark") || "light";
  });
  const [accentColor, setAccentColor] = useState<string>(() => {
    const saved = localStorage.getItem("accentColor");
    return saved || "#0071e3";
  });

  const accentColors = [
    { name: "blue", hex: "#0071e3" },
    { name: "purple", hex: "#9333ea" },
    { name: "green", hex: "#34c759" },
    { name: "red", hex: "#ff3b30" },
    { name: "orange", hex: "#ff9500" },
  ];

  const handleLogout = () => {
    logout({ logoutParams: { returnTo: window.location.origin } });
  };

  const handleThemeChange = (newTheme: "light" | "dark") => {
    setTheme(newTheme);
    localStorage.setItem("theme", newTheme);
    document.documentElement.setAttribute("data-theme", newTheme);
  };

  const handleAccentColorChange = (hex: string) => {
    setAccentColor(hex);
    localStorage.setItem("accentColor", hex);
    document.documentElement.style.setProperty("--accent-primary", hex);
  };

  const handleOverlayMouseMove = (e: React.MouseEvent) => {
    if (
      modalRef.current &&
      !modalRef.current.contains(e.currentTarget as Node)
    ) {
      return;
    }
  };

  const handleOverlayMouseLeave = () => {
    // No longer closes on mouse leave
  };

  return (
    <div
      className="modal-overlay"
      onClick={(e) => {
        if (e.target === e.currentTarget) {
          onClose();
        }
      }}
      onMouseMove={handleOverlayMouseMove}
      onMouseLeave={handleOverlayMouseLeave}
    >
      <div
        ref={modalRef}
        className="modal-content profile-modal"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-tabs">
          <button
            className={`tab-button ${activeTab === "profile" ? "active" : ""}`}
            onClick={() => setActiveTab("profile")}
          >
            Profile
          </button>
          <button
            className={`tab-button ${activeTab === "settings" ? "active" : ""}`}
            onClick={() => setActiveTab("settings")}
          >
            Settings
          </button>
          <button
            className={`tab-button ${activeTab === "info" ? "active" : ""}`}
            onClick={() => setActiveTab("info")}
          >
            Info
          </button>
        </div>

        {activeTab === "profile" && (
          <div className="modal-content-container">
            <div className="profile-header">
              {user?.picture && (
                <img
                  src={user.picture}
                  alt="Profile"
                  className="profile-picture"
                />
              )}
              <div className="profile-name-section">
                <h3 className="profile-name">
                  <span className="greeting-text">
                    {theme === "light" ? "Hey" : "Welcome"},&nbsp;
                  </span>
                  <span className="profile-name-accent">
                    {user?.name || "N/A"}
                  </span>
                  <span className="greeting-text">!</span>
                </h3>
                <p className="tagline-text">Stay organized, stay focused.</p>
              </div>
            </div>

            <div className="profile-details">
              <div className="detail-row">
                <span className="detail-label">Email</span>
                <span className="detail-value">{user?.email || "N/A"}</span>
              </div>
              {user?.email_verified !== undefined && (
                <div className="detail-row">
                  <span className="detail-label">Email Verified</span>
                  <span className="detail-value">
                    {user.email_verified ? "✓ Yes" : "✗ No"}
                  </span>
                </div>
              )}
              {user?.sub && (
                <div className="detail-row">
                  <span className="detail-label">User ID</span>
                  <span className="detail-value user-id">{user.sub}</span>
                </div>
              )}
            </div>

            <button
              className="modal-logout-button logout-button-profile"
              onClick={handleLogout}
              title="Logout"
              aria-label="Logout"
            >
              <svg
                width="20"
                height="20"
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

        {activeTab === "settings" && (
          <div className="modal-content-container">
            <div className="settings-section">
              <label className="settings-label">Theme</label>
              <div className="theme-options">
                <button
                  className={`theme-option ${
                    theme === "light" ? "active" : ""
                  }`}
                  onClick={() => handleThemeChange("light")}
                  title="Light Theme"
                  aria-label="Light Theme"
                >
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
                </button>
                <button
                  className={`theme-option ${theme === "dark" ? "active" : ""}`}
                  onClick={() => handleThemeChange("dark")}
                  title="Dark Theme"
                  aria-label="Dark Theme"
                >
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
                </button>
              </div>
            </div>

            <div className="settings-section">
              <label className="settings-label">Accent Color</label>
              <div className="color-options">
                {accentColors.map((color) => (
                  <button
                    key={color.name}
                    className={`color-option ${
                      accentColor === color.hex ? "active" : ""
                    }`}
                    style={{ backgroundColor: color.hex }}
                    onClick={() => handleAccentColorChange(color.hex)}
                    title={color.name}
                    aria-label={`${color.name} accent color`}
                  />
                ))}
              </div>
            </div>
          </div>
        )}

        {activeTab === "info" && (
          <div className="modal-content-container">
            <div className="info-section">
              <div className="info-item">
                <div className="info-header">
                  <svg
                    width="16"
                    height="16"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                  >
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                    <circle cx="12" cy="7" r="4" />
                  </svg>
                  <span className="info-label">Founder</span>
                </div>
                <span className="info-text">Taner Gadomski</span>
              </div>
              <div className="info-item">
                <div className="info-header">
                  <svg
                    width="16"
                    height="16"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                  >
                    <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                    <line x1="16" y1="2" x2="16" y2="6" />
                    <line x1="8" y1="2" x2="8" y2="6" />
                    <line x1="3" y1="10" x2="21" y2="10" />
                  </svg>
                  <span className="info-label">Founded</span>
                </div>
                <span className="info-text">September 2024</span>
              </div>
              <div className="info-item">
                <div className="info-header">
                  <svg
                    width="16"
                    height="16"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                  >
                    <circle cx="12" cy="12" r="10" />
                    <path d="M12 6v6l4 2" />
                  </svg>
                  <span className="info-label">Version</span>
                </div>
                <span className="info-text">{packageJson.version}</span>
              </div>
              <div className="info-item">
                <div className="info-header">
                  <svg
                    width="16"
                    height="16"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                  >
                    <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
                  </svg>
                  <span className="info-label">License</span>
                </div>
                <span className="info-text">Open Source</span>
              </div>
            </div>

            <a
              href="https://github.com/taner04/Taskly"
              target="_blank"
              rel="noopener noreferrer"
              className="github-link"
            >
              View on GitHub
            </a>
          </div>
        )}
      </div>
    </div>
  );
};

export default ProfileModal;

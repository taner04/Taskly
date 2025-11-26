import { useRef, useState } from "react";
import { useUser } from "../../contexts/UserContext";
import "./AccountModal.css";

interface AccountModalProps {
  onClose: () => void;
}

const AccountModal = ({ onClose }: AccountModalProps) => {
  const { user, logout } = useUser();
  const modalRef = useRef<HTMLDivElement>(null);
  const [activeTab, setActiveTab] = useState<"account" | "settings">("account");
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
        className="modal-content account-modal"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-header">
          <h2>Account</h2>
          <button
            className="modal-logout-button"
            onClick={handleLogout}
            title="Logout"
            aria-label="Logout"
          >
            <svg
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
              <polyline points="16 17 21 12 16 7" />
              <line x1="21" y1="12" x2="9" y2="12" />
            </svg>
          </button>
        </div>

        <div className="modal-tabs">
          <button
            className={`tab-button ${activeTab === "account" ? "active" : ""}`}
            onClick={() => setActiveTab("account")}
          >
            Account
          </button>
          <button
            className={`tab-button ${activeTab === "settings" ? "active" : ""}`}
            onClick={() => setActiveTab("settings")}
          >
            Settings
          </button>
        </div>

        {activeTab === "account" && (
          <div className="account-info">
            {user?.picture && (
              <img
                src={user.picture}
                alt="Profile"
                className="profile-picture"
              />
            )}
            <div className="info-section">
              <label>Name</label>
              <p>{user?.name || "N/A"}</p>
            </div>
            <div className="info-section">
              <label>Email</label>
              <p>{user?.email || "N/A"}</p>
            </div>
            {user?.email_verified !== undefined && (
              <div className="info-section">
                <label>Email Verified</label>
                <p>{user.email_verified ? "✓ Yes" : "✗ No"}</p>
              </div>
            )}
            {user?.sub && (
              <div className="info-section">
                <label>User ID</label>
                <p className="user-id">{user.sub}</p>
              </div>
            )}
          </div>
        )}

        {activeTab === "settings" && (
          <div className="settings-content">
            <div className="settings-section">
              <label className="settings-label">Theme</label>
              <div className="theme-options">
                <button
                  className={`theme-option ${
                    theme === "light" ? "active" : ""
                  }`}
                  onClick={() => handleThemeChange("light")}
                  title="Light Theme"
                >
                  ☀️
                </button>
                <button
                  className={`theme-option ${theme === "dark" ? "active" : ""}`}
                  onClick={() => handleThemeChange("dark")}
                  title="Dark Theme"
                >
                  🌙
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
      </div>
    </div>
  );
};

export default AccountModal;

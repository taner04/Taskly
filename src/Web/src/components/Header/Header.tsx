import "./Header.css";

interface HeaderProps {
  onToggleSidebar?: () => void;
  isExpanded?: boolean;
  title?: string;
}

const Header = ({
  onToggleSidebar,
  isExpanded = true,
  title = "Taskly",
}: HeaderProps) => {
  return (
    <header className="app-header">
      <div className="header-content">
        <div className="header-left">
          <button
            className="sidebar-toggle"
            onClick={onToggleSidebar}
            aria-label={isExpanded ? "Collapse sidebar" : "Expand sidebar"}
          >
            <svg
              width="20"
              height="20"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              {isExpanded ? (
                <path d="M15 18l-6-6 6-6" />
              ) : (
                <path d="M9 18l6-6-6-6" />
              )}
            </svg>
          </button>
          {isExpanded && <h2 className="header-title">{title}</h2>}
        </div>
        <div className="header-spacer"></div>
      </div>
    </header>
  );
};

export default Header;

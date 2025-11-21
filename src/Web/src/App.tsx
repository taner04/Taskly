import { useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import Sidebar from "./components/Sidebar";
import Home from "./components/Home";
import Todos from "./components/Todos";

type View = "home" | "todos";

function App() {
  const { isAuthenticated, isLoading, error } = useAuth0();
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState<boolean>(() => {
    const saved = localStorage.getItem("sidebarExpanded");
    return saved !== null ? !JSON.parse(saved) : false;
  });
  const [currentView, setCurrentView] = useState<View>("home");

  if (isLoading) {
    return (
      <div className="app-container">
        <div className="loading-state">
          <div className="loading-text">Loading...</div>
        </div>
      </div>
    );
  }

  if (error) {
    // For development with dummy data, we'll allow the app to continue
    // even if Auth0 has errors, but log the error
    console.warn("Auth0 error (continuing with limited functionality):", error);

    // Still show the app but with a warning banner
    return (
      <div className="app-with-error">
        <div className="error-banner">
          <p>⚠️ Authentication warning: {error.message}. Using demo mode.</p>
        </div>
        <div
          className={`app-layout ${
            isSidebarCollapsed ? "sidebar-collapsed" : ""
          }`}
        >
          <div className="sidebar-and-header">
            <Sidebar
              isAuthenticated={false}
              onToggle={(collapsed) => setIsSidebarCollapsed(collapsed)}
              currentView={currentView}
              onViewChange={setCurrentView}
            />
          </div>
          <div className="app-main">
            <main className="main-content">
              <div className="content-wrapper">
                {currentView === "todos" ? (
                  <Todos />
                ) : (
                  <Home onViewChange={setCurrentView} />
                )}
              </div>
            </main>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div
      className={`app-layout ${isSidebarCollapsed ? "sidebar-collapsed" : ""}`}
    >
      <div className="sidebar-and-header">
        <Sidebar
          isAuthenticated={isAuthenticated}
          onToggle={(collapsed) => setIsSidebarCollapsed(collapsed)}
          currentView={currentView}
          onViewChange={setCurrentView}
        />
      </div>
      <div className="app-main">
        <main className="main-content">
          {currentView === "todos" ? <Todos /> : <Home />}
        </main>
      </div>
    </div>
  );
}

export default App;

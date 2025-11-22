import { useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import Sidebar from "./components/Sidebar";
import Home from "./components/Home";
import Todos from "./components/Todos";
import { ProfileModal } from "./components/Profile";

type View = "home" | "todos";

function App() {
  const { isAuthenticated, isLoading, error, loginWithRedirect } = useAuth0();
  const [currentView, setCurrentView] = useState<View>("home");
  const [showProfileModal, setShowProfileModal] = useState(false);

  const handleViewChange = (view: View) => {
    if (view === "todos" && !isAuthenticated) {
      loginWithRedirect();
      return;
    }
    setCurrentView(view);
  };

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
      <>
        <div className="app-with-error">
          <div className="error-banner">
            <p>⚠️ Authentication warning: {error.message}. Using demo mode.</p>
          </div>
        </div>
        <div className="app-layout">
          <div className="sidebar-and-header">
            <Sidebar
              isAuthenticated={false}
              currentView={currentView}
              onViewChange={handleViewChange}
              onShowProfileModal={() => setShowProfileModal(true)}
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
        {showProfileModal && (
          <ProfileModal onClose={() => setShowProfileModal(false)} />
        )}
      </>
    );
  }

  return (
    <div className="app-layout">
      <div className="sidebar-and-header">
        <Sidebar
          isAuthenticated={isAuthenticated}
          currentView={currentView}
          onViewChange={handleViewChange}
          onShowProfileModal={() => setShowProfileModal(true)}
        />
      </div>
      <div className="app-main">
        <main className="main-content">
          {currentView === "todos" ? <Todos /> : <Home />}
        </main>
      </div>
      {showProfileModal && (
        <ProfileModal onClose={() => setShowProfileModal(false)} />
      )}
    </div>
  );
}

export default App;

import React from "react";
import ReactDOM from "react-dom/client";
import { Auth0Provider } from "@auth0/auth0-react";
import App from "./App.tsx";
import { UserProvider } from "./contexts/UserContext.tsx";
import "./styles/index.css";

// Initialize theme and accent color from localStorage
const initializeThemeAndAccent = () => {
  const savedTheme = localStorage.getItem("theme") || "system";
  const savedAccent = localStorage.getItem("accentColor") || "blue";

  const root = document.documentElement;

  // Set theme
  if (savedTheme === "system") {
    const isDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
    root.setAttribute("data-theme", isDark ? "dark" : "light");
  } else {
    root.setAttribute("data-theme", savedTheme);
  }

  // Set accent color
  root.setAttribute("data-accent", savedAccent);
};

initializeThemeAndAccent();

const domain = import.meta.env.VITE_AUTH0_DOMAIN;
const clientId = import.meta.env.VITE_AUTH0_CLIENT_ID;

if (!domain || !clientId) {
  console.error("Auth0 configuration missing. Please check your .env file.");
  console.error("Required: VITE_AUTH0_DOMAIN and VITE_AUTH0_CLIENT_ID");
}

const rootElement = document.getElementById("root");
if (!rootElement) {
  throw new Error("Root element not found");
}

ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    {domain && clientId ? (
      <Auth0Provider
        domain={domain}
        clientId={clientId}
        authorizationParams={{
          redirect_uri: window.location.origin,
        }}
      >
        <UserProvider>
          <App />
        </UserProvider>
      </Auth0Provider>
    ) : (
      <UserProvider>
        <App />
      </UserProvider>
    )}
  </React.StrictMode>
);

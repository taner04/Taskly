import React from "react";
import ReactDOM from "react-dom/client";
import { Auth0Provider } from "@auth0/auth0-react";
import App from "./App.tsx";
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

// Validate Auth0 configuration
if (!domain || !clientId) {
  console.warn("Auth0 configuration missing. Running in demo mode.");
}

const rootElement = document.getElementById("root");
if (!rootElement) {
  throw new Error("Root element not found");
}

// Only render Auth0Provider if we have valid credentials
const authConfig =
  domain && clientId
    ? {
        domain,
        clientId,
        authorizationParams: {
          redirect_uri: window.location.origin,
          audience: import.meta.env.VITE_AUTH0_AUDIENCE,
        },
        onRedirectCallback: (appState: any) => {
          // Handle redirect after login
          if (appState?.returnTo) {
            window.location.href = appState.returnTo;
          }
        },
        useRefreshTokens: true,
        cacheLocation: "localstorage" as const,
      }
    : null;

ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    {authConfig ? (
      <Auth0Provider {...authConfig}>
        <App />
      </Auth0Provider>
    ) : (
      <App />
    )}
  </React.StrictMode>
);

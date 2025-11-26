import { useState } from "react";
import { useUser } from "../../contexts/UserContext";
import { ProfileModal } from "../Profile";

const LoginButton = () => {
  const { isAuthenticated, loginWithRedirect } = useUser();
  const [showProfileModal, setShowProfileModal] = useState(false);

  if (!isAuthenticated) {
    return (
      <button onClick={() => loginWithRedirect()} className="button login">
        Log In
      </button>
    );
  }

  return (
    <>
      <button
        onClick={() => setShowProfileModal(true)}
        className="button profile"
      >
        Profile
      </button>
      {showProfileModal && (
        <ProfileModal onClose={() => setShowProfileModal(false)} />
      )}
    </>
  );
};

export default LoginButton;

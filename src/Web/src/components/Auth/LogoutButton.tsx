import { useUser } from "../../contexts/UserContext";

const LogoutButton = () => {
  const { logout } = useUser();
  return (
    <button
      onClick={() =>
        logout({ logoutParams: { returnTo: window.location.origin } })
      }
      className="button logout"
    >
      Log Out
    </button>
  );
};

export default LogoutButton;

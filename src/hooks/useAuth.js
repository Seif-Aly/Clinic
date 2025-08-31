import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

export default function useAuth() {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      setUser(null);
      return;
    }

    try {
      const decoded = jwtDecode(token);

      // roles can be in different places depending on token mapping
      const rawRoles =
        decoded.role ??
        decoded.roles ??
        decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      const roles = Array.isArray(rawRoles)
        ? rawRoles
        : rawRoles
        ? [rawRoles]
        : [];

      setUser({
        email: decoded.email || decoded.unique_name || decoded.name || "",
        roles,
      });
    } catch (e) {
      console.error("Invalid token", e);
      setUser(null);
    }
  }, []);

  return user;
}

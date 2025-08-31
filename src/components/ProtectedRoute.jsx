<<<<<<< HEAD
import React from "react";
import { Navigate } from "react-router-dom";
import { jwtDecode } from 'jwt-decode';


export default function ProtectedRoute({ children, role }) {
    const token = localStorage.getItem("token");
    if (!token) return <Navigate to="/login" replace />;

    if (role) {
        try {
            const decoded = jwtDecode(token);
            const roles = (decoded.role) ? (Array.isArray(decoded.role) ? decoded.role : [decoded.role]) : (decoded.roles || []);
            // support 'role' claim or 'roles'
            if (!roles.includes(role)) return <Navigate to="/dashboard" replace />;
        } catch (e) {
            console.error("Invalid token", e);
            return <Navigate to="/login" replace />;
        }
    }

    return children;
}
=======
import React from "react";
import { Navigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

export default function ProtectedRoute({ children, role }) {
  const token = localStorage.getItem("token");
  if (!token) return <Navigate to="/login" replace />;

  if (role) {
    try {
      const decoded = jwtDecode(token);

      let roles = [];
      if (decoded.role)
        roles = Array.isArray(decoded.role) ? decoded.role : [decoded.role];
      else if (decoded.roles)
        roles = Array.isArray(decoded.roles) ? decoded.roles : [decoded.roles];
      else if (
        decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
      ) {
        const claim =
          decoded[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ];
        roles = Array.isArray(claim) ? claim : [claim];
      }
      if (!roles.includes(role)) return <Navigate to="/dashboard" replace />;
    } catch (e) {
      return <Navigate to="/login" replace />;
    }
  }

  return children;
}
>>>>>>> main

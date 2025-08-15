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
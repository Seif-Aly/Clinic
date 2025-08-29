import React from "react";
import { Link, Outlet } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import ChatbotWidget from "./ChatbotWidget";

export default function DashboardLayout() {
  const logout = () => {
    localStorage.removeItem("token");
    window.location.href = "/login";
  };

  let isAdmin = false;

  try {
    const token = localStorage.getItem("token");
    if (token) {
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

      isAdmin = roles.includes("Admin");
    }
  } catch {}

  return (
    <div className="d-flex">
      <aside
        style={{ width: 250 }}
        className="bg-primary text-white vh-100 d-flex flex-column p-3"
      >
        <h5 className="text-center mb-3">Control panel</h5>
        <nav className="flex-grow-1">
          <Link className="sidebar-link" to="/dashboard">
            Home
          </Link>
          <Link className="sidebar-link" to="/doctors">
            Doctors
          </Link>
          <Link className="sidebar-link" to="/patients">
            patients
          </Link>
          <Link className="sidebar-link" to="/appointments">
            Appointments
          </Link>
          <Link className="sidebar-link" to="/hospitals">
            Hospitals
          </Link>
          <Link className="sidebar-link" to="/clinics">
            Clinics
          </Link>
          <Link className="sidebar-link" to="/prescriptions">
            prescriptions
          </Link>

          {isAdmin && (
            <>
              <hr style={{ opacity: 0.2 }} />
              <Link className="sidebar-link" to="/admins/new">
                Add admin
              </Link>
            </>
          )}
        </nav>

        <div className="mt-auto">
          <button className="btn btn-outline-light w-100" onClick={logout}>
            Logout
          </button>
        </div>
      </aside>

      <main className="flex-fill p-4 position-relative">
        <Outlet />
        <ChatbotWidget />
      </main>
    </div>
  );
}

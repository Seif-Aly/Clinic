import React from "react";
import useAuth from "../hooks/useAuth";

export default function PublicHeader() {
  const user = useAuth();

  const logout = () => {
    localStorage.removeItem("token");
    window.location.href = "/";
  };

  return (
    <div className="d-flex justify-content-between align-items-center p-2 border-bottom">
      <a href="/" className="d-flex align-items-center text-decoration-none">
        <img
          src="/Logo.png"
          alt="Logo"
          height={40}
          className="me-2"
          style={{ objectFit: "contain" }}
        />
        <span className="fw-bold text-dark fs-5">Clinic</span>
      </a>

      <div className="d-flex gap-2">
        {!user && (
          <>
            <a className="btn btn-outline-secondary" href="/patient-login">
              Patient Login
            </a>
            <a className="btn btn-outline-success" href="/register">
              Register
            </a>
            <a className="btn btn-primary" href="/login">
              Admin/Doctor
            </a>
          </>
        )}

        {user && user.roles.includes("Patient") && (
          <>
            <span className="me-2">Welcome, {user.email}</span>
            <a className="btn btn-outline-info" href="/my-appointments">
              My Appointments
            </a>
            <button className="btn btn-outline-danger" onClick={logout}>
              Logout
            </button>
          </>
        )}

        {user &&
          (user.roles.includes("Admin") || user.roles.includes("Doctor")) && (
            <>
              <span className="me-2">Welcome, {user.email}</span>
              <a className="btn btn-primary" href="/dashboard">
                Dashboard
              </a>
              <button className="btn btn-outline-danger" onClick={logout}>
                Logout
              </button>
            </>
          )}
      </div>
    </div>
  );
}

import React from "react";
import { Link, Outlet } from "react-router-dom";

export default function DashboardLayout() {
    const logout = () => {
        localStorage.removeItem("token");
        window.location.href = "/login";
    };

    return (
        <div className="d-flex">
            <aside style={{ width: 250 }} className="bg-primary text-white vh-100 d-flex flex-column p-3">
                <h5 className="text-center mb-3">Control panel</h5>
                <nav className="flex-grow-1">
                    <Link className="sidebar-link" to="/dashboard">Home</Link>
                    <Link className="sidebar-link" to="/doctors">Doctors</Link>
                    <Link className="sidebar-link" to="/patients">patients</Link>
                    <Link className="sidebar-link" to="/appointments">Appointments</Link>
                    <Link className="sidebar-link" to="/hospitals">Hospitals</Link>
                    <Link className="sidebar-link" to="/prescriptions">prescriptions</Link>
                </nav>

                <div className="mt-auto">
                    <button className="btn btn-outline-light w-100" onClick={logout}>Login</button>
                </div>
            </aside>

            <main className="flex-fill p-4">
                <Outlet />
            </main>
        </div>
    );
}
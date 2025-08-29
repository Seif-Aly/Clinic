import React, { useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";
import { jwtDecode } from "jwt-decode";
import { Link } from "react-router-dom";

export default function PatientLogin() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setLoading(true);
    try {
      const res = await api.post("/Auth/login", { email, password });
      const token = res.data?.token ?? res.data?.Token ?? res.data;
      if (!token) throw ["Login did not return a token."];

      // check roles
      const decoded = jwtDecode(token);
      const rawRoles =
        decoded.role ??
        decoded.roles ??
        decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      const roles = Array.isArray(rawRoles)
        ? rawRoles
        : rawRoles
        ? [rawRoles]
        : [];

      if (!roles.includes("Patient")) {
        throw ["This login page is for patients only."];
      }

      localStorage.setItem("token", token);
      window.location.href = "/"; // back to home
    } catch (err) {
      console.error("Login failed", err);
      setErrors(extractErrorMessages(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: 420 }}>
      <h3 className="mb-3">Patient Login</h3>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <form onSubmit={submit} className="card p-3">
        <input
          className="form-control mb-2"
          type="email"
          placeholder="Email"
          autoComplete="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          className="form-control mb-3"
          type="password"
          placeholder="Password"
          autoComplete="current-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button
          className="btn btn-primary w-100"
          type="submit"
          disabled={loading}
        >
          {loading ? "Logging in..." : "Login"}
        </button>
      </form>

      <div className="text-center mt-3">
        <small>
          Don’t have an account? <Link to="/register">Register</Link>
        </small>
      </div>
    </div>
  );
}

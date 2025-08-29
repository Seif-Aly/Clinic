import React, { useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";

export default function AdminLogin() {
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
      const token =
        res.data.token ?? res.data.accessToken ?? res.data?.Token ?? res.data;
      if (!token) throw new Error("Login succeeded but no token was returned.");
      localStorage.setItem("token", token);
      window.location.href = "/dashboard";
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <form onSubmit={submit} className="card p-4" style={{ width: 360 }}>
        <h4 className="text-center mb-3">Admin Login</h4>

        <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

        <input
          className="form-control mb-2"
          placeholder=" Email "
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          autoComplete="username"
          required
        />
        <input
          className="form-control mb-3"
          placeholder="Password "
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="current-password"
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
    </div>
  );
}

import React, { useState } from "react";
import api from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";
import SuccessAlert from "../components/SuccessAlert";

export default function AddAdmin() {
  const [email, setEmail] = useState("");
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [busy, setBusy] = useState(false);

  const [errors, setErrors] = useState([]);
  const [success, setSuccess] = useState("");

  async function submit(e) {
    e.preventDefault();
    setErrors([]);
    setSuccess("");
    if (password !== confirm) {
      setErrors(["Passwords do not match."]);
      return;
    }

    setBusy(true);
    try {
      const payload = {
        username: userName || email,
        password,
        role: "Admin",
        email: email.trim().toLowerCase(),
      };
      const res = await api.post("/Auth/register-admin", payload);

      const token = res?.data?.token;
      if (token) localStorage.setItem("token", token);

      setSuccess("Admin user created successfully.");
      setEmail("");
      setUserName("");
      setPassword("");
      setConfirm("");
    } catch (err) {
      const msg =
        err?.response?.data?.message ||
        err?.response?.data?.errors?.[0]?.description ||
        "Failed to create admin.";
      setErrors([msg]);
    } finally {
      setBusy(false);
    }
  }

  return (
    <div>
      <h4>Add Admin</h4>
      <SuccessAlert message={success} onClose={() => setSuccess("")} />
      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />
      <form onSubmit={submit} style={{ maxWidth: 480 }}>
        <input
          className="form-control mb-2"
          placeholder="Email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          className="form-control mb-2"
          placeholder="Username (optional)"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
        />
        <input
          className="form-control mb-2"
          placeholder="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <input
          className="form-control mb-3"
          placeholder="Confirm password"
          type="password"
          value={confirm}
          onChange={(e) => setConfirm(e.target.value)}
          required
        />
        <button className="btn btn-primary" type="submit" disabled={busy}>
          {busy ? "Saving..." : "Create Admin"}
        </button>
      </form>
    </div>
  );
}

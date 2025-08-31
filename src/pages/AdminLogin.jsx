<<<<<<< HEAD
import React, { useState } from "react";
import api from "../services/api";

export default function AdminLogin() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);

    const submit = async (e) => {
        e.preventDefault();
        setLoading(true);
        try {
            const res = await api.post("/Auth/login", { email, password }); 
            const token = res.data.token ?? res.data.accessToken ?? res.data;
            if (!token) throw new Error("No token");
            localStorage.setItem("token", token);
            window.location.href = "/dashboard";
        } catch (err) {
            console.error(err);
            alert(err.response?.data?.message || "Invalid email or password");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center vh-100">
            <form onSubmit={submit} className="card p-4" style={{ width: 360 }}>
                <h4 className="text-center mb-3">Admin Login</h4>
                <input className="form-control mb-2" placeholder=" Email " type="email" value={email} onChange={e => setEmail(e.target.value)} required />
                <input className="form-control mb-3" placeholder="Password " type="password" value={password} onChange={e => setPassword(e.target.value)} required />
                <button className="btn btn-primary w-100" type="submit" disabled={loading}>
                    {loading ? "Logging in..." : "Login"}
                </button>
            </form>
        </div>
    );
}
=======
import React, { useState } from "react";
import api from "../services/api";
import { jwtDecode } from "jwt-decode";
import ErrorsAlert from "../components/ErrorsAlert";

export default function AdminLogin() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState([]);

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setLoading(true);
    try {
      const res = await api.post("/Auth/login", { email, password });
      const token = res.data?.token ?? res.data?.accessToken ?? res.data;
      if (!token) throw ["No token"];

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

      if (!roles.includes("Admin") && !roles.includes("Doctor")) {
        throw ["This login page is for Admin/Doctor only."];
      }

      localStorage.setItem("token", token);
      window.location.href = "/dashboard";
    } catch (err) {
      console.error(err);
      const msg = err?.response?.data?.message || "Invalid email or password";
      setErrors(Array.isArray(msg) ? msg : [msg]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <form onSubmit={submit} className="card p-4" style={{ width: 360 }}>
        <h4 className="text-center mb-3">Admin/Doctor Login</h4>

        <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

        <input
          className="form-control mb-2"
          placeholder="Email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          className="form-control mb-3"
          placeholder="Password"
          type="password"
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
    </div>
  );
}
>>>>>>> main

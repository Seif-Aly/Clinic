<<<<<<< HEAD
import React, { useState } from "react";

function AdminLogin() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch("https://localhost:7005/api/Auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    email: email,
                    password: password,
                }),
            });

            if (!response.ok) {
                alert("Invalid email or password");
                return;
            }

            const data = await response.json();
            console.log(data); // ›ÌÂ «·‹ JWT Token

            // ‰Œ“‰ «· Êﬂ‰ ›Ì Local Storage
            localStorage.setItem("token", data.token);

            alert("Login successful!");
            // Â‰« „„ﬂ‰  ⁄„· Redirect ··‹ Dashboard
        } catch (error) {
            console.error("Error:", error);
            alert("Something went wrong!");
        }
    };

    return (
        <div className="container mt-5" style={{ maxWidth: "400px" }}>
            <h3 className="text-center mb-4">Admin Login</h3>
            <form onSubmit={handleLogin}>
                <div className="mb-3">
                    <label>Email</label>
                    <input
                        type="email"
                        className="form-control"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div className="mb-3">
                    <label>Password</label>
                    <input
                        type="password"
                        className="form-control"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit" className="btn btn-primary w-100">
                    Login
                </button>
            </form>
        </div>
    );
}

export default AdminLogin;
=======
import React, { useState } from "react";
import ErrorsAlert from "../components/ErrorsAlert";
import api, { extractErrorMessages } from "../services/api";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(false);

  const handleLogin = async (e) => {
    e.preventDefault();
    setErrors([]);
    setLoading(true);
    try {
      const response = await api.post("/Auth/login", { email, password });
      const token =
        response.data.token ??
        response.data.accessToken ??
        response.data?.Token ??
        response.data;
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
    <div className="container mt-5" style={{ maxWidth: "400px" }}>
      <h3 className="text-center mb-4">Admin Login</h3>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <form onSubmit={handleLogin}>
        <div className="mb-3">
          <label>Email</label>
          <input
            type="email"
            className="form-control"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label>Password</label>
          <input
            type="password"
            className="form-control"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button
          type="submit"
          className="btn btn-primary w-100"
          disabled={loading}
        >
          {loading ? "Logging in..." : "Login"}
        </button>
      </form>
    </div>
  );
}
>>>>>>> main

import React, { useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";
import SuccessAlert from "../components/SuccessAlert";

export default function RegisterPatient() {
  const [errors, setErrors] = useState([]);
  const [saving, setSaving] = useState(false);
  const [success, setSuccess] = useState("");

  const [account, setAccount] = useState({
    email: "",
    password: "",
  });

  const [profile, setProfile] = useState({
    fullName: "",
    nationalId: "",
    phone: "",
    gender: "",
    dateOfBirth: "",
  });

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setSaving(true);
    setSuccess("");
    try {
      const regRes = await api.post("/Auth/register-patient", {
        email: account.email,
        password: account.password,
      });
      const token = regRes.data?.token ?? regRes.data?.Token ?? regRes.data;
      if (!token) throw ["Registration did not return a token."];
      localStorage.setItem("token", token);

      const pRes = await api.post("/patients", {
        fullName: profile.fullName,
        nationalId: profile.nationalId,
        email: account.email,
        phone: profile.phone,
        gender: profile.gender,
        dateOfBirth:
          profile.dateOfBirth || new Date().toISOString().split("T")[0],
      });
      const pid = pRes.data?.id || pRes.data?.patientId || pRes.data?.PatientId;
      if (pid) localStorage.setItem("patientId", pid);

      setSuccess("Registration successful! Redirecting to home...");
      setTimeout(() => (window.location.href = "/"), 2500);
      window.location.href = "/";
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: 720 }}>
      <SuccessAlert message={success} onClose={() => setSuccess("")} />
      <h3 className="mb-3">Register as Patient</h3>
      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <form onSubmit={submit} className="card p-3">
        <h6 className="mb-2">Account</h6>
        <div className="row g-2">
          <div className="col-md-6">
            <input
              className="form-control"
              type="email"
              placeholder="Email"
              autoComplete="email"
              value={account.email}
              onChange={(e) =>
                setAccount({ ...account, email: e.target.value })
              }
              required
            />
          </div>
          <div className="col-md-6">
            <input
              className="form-control"
              type="password"
              placeholder="Password"
              autoComplete="new-password"
              value={account.password}
              onChange={(e) =>
                setAccount({ ...account, password: e.target.value })
              }
              required
            />
          </div>
        </div>

        <hr className="my-3" />
        <h6 className="mb-2">Patient profile</h6>
        <div className="row g-2">
          <div className="col-md-6">
            <input
              className="form-control"
              placeholder="Full name"
              value={profile.fullName}
              onChange={(e) =>
                setProfile({ ...profile, fullName: e.target.value })
              }
            />
          </div>
          <div className="col-md-6">
            <input
              className="form-control"
              placeholder="National ID"
              value={profile.nationalId}
              onChange={(e) =>
                setProfile({ ...profile, nationalId: e.target.value })
              }
            />
          </div>
          <div className="col-md-6">
            <input
              className="form-control"
              placeholder="Phone"
              value={profile.phone}
              onChange={(e) =>
                setProfile({ ...profile, phone: e.target.value })
              }
            />
          </div>
          <div className="col-md-3">
            <select
              className="form-select"
              value={profile.gender}
              onChange={(e) =>
                setProfile({ ...profile, gender: e.target.value })
              }
            >
              <option value="">Gender</option>
              <option>Male</option>
              <option>Female</option>
            </select>
          </div>
          <div className="col-md-3">
            <input
              type="date"
              className="form-control"
              value={profile.dateOfBirth}
              onChange={(e) =>
                setProfile({ ...profile, dateOfBirth: e.target.value })
              }
            />
          </div>
        </div>

        <button
          className="btn btn-success mt-3"
          type="submit"
          disabled={saving}
        >
          {saving ? "Creating account..." : "Register"}
        </button>
      </form>
    </div>
  );
}

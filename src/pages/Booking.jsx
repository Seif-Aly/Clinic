import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";
import PublicHeader from "../components/PublicHeader";

export default function Booking() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [errors, setErrors] = useState([]);
  const [saving, setSaving] = useState(false);
  const [doctor, setDoctor] = useState(null);

  const [patientModel, setPatientModel] = useState({
    fullName: "",
    nationalId: "",
    email: "",
    phone: "",
    gender: "",
    dateOfBirth: "",
  });
  const [patientId, setPatientId] = useState(
    localStorage.getItem("patientId") || ""
  );

  const [date, setDate] = useState("");
  const [time, setTime] = useState("");

  const token = localStorage.getItem("token");
  const decoded = jwtDecode(token);
  const rawRoles =
    decoded.role ??
    decoded.roles ??
    decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  const roles = Array.isArray(rawRoles) ? rawRoles : rawRoles ? [rawRoles] : [];

  const isLoggedIn = Boolean(token);
  const isPatient = roles.includes("Patient");

  useEffect(() => {
    const load = async () => {
      setErrors([]);
      try {
        const res = await api.get(`/doctors/${id}`);
        setDoctor(res.data);
      } catch (err) {
        setErrors(extractErrorMessages(err));
      }
    };
    load();
  }, [id]);

  const createPatientIfNeeded = async () => {
    if (patientId) return patientId;

    const payload = {
      fullName: patientModel.fullName,
      nationalId: patientModel.nationalId,
      email: patientModel.email,
      phone: patientModel.phone,
      gender: patientModel.gender,
      dateOfBirth: patientModel.dateOfBirth || new Date().toISOString(),
    };
    const r = await api.post("/patients", payload);
    const created = r.data;
    const newId = created?.id || created?.patientId || created?.PatientId;
    if (!newId) throw ["Could not determine patient id after creation."];
    localStorage.setItem("patientId", newId);
    setPatientId(newId);
    return newId;
  };

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    if (!isLoggedIn) {
      setErrors(["Please log in to book an appointment."]);
      return;
    }
    if (!isPatient) {
      setErrors(["Only patients can book appointments."]);
      return;
    }
    if (!date || !time) {
      setErrors(["Please choose date and time."]);
      return;
    }
    setSaving(true);
    try {
      const pid = await createPatientIfNeeded();
      console.log(pid);
      const iso = new Date(`${date}T${time}:00`).toISOString();

      const payload = {
        appointmentDateTime: iso,
        status: "Pending",
        doctorId: Number(id),
        patientId: Number(pid),
      };
      await api.post("/appointments", payload, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      navigate("/my-appointments");
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <>
      <PublicHeader />
      <div className="container py-4">
        <h3>Book appointment</h3>
        <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

        {!doctor && <div className="text-muted">Loading doctor...</div>}
        {doctor && (
          <div className="card mb-3">
            <div className="card-body">
              <div className="fw-bold">{doctor.fullName}</div>
              <div className="text-muted small">{doctor.specialization}</div>
              <div className="small">{doctor.phone}</div>
            </div>
          </div>
        )}

        {!isLoggedIn && (
          <div className="alert alert-warning">
            You must be logged in as a patient to book.{" "}
            <Link to="/login">Login</Link> or{" "}
            <Link to="/register">Register</Link>.
          </div>
        )}

        {isLoggedIn && !patientId && (
          <div className="card p-3 mb-3">
            <h6 className="mb-2">Patient information</h6>
            <div className="row g-2">
              <div className="col-md-6">
                <input
                  className="form-control"
                  placeholder="Full name"
                  value={patientModel.fullName}
                  onChange={(e) =>
                    setPatientModel({
                      ...patientModel,
                      fullName: e.target.value,
                    })
                  }
                />
              </div>
              <div className="col-md-6">
                <input
                  className="form-control"
                  placeholder="National ID"
                  value={patientModel.nationalId}
                  onChange={(e) =>
                    setPatientModel({
                      ...patientModel,
                      nationalId: e.target.value,
                    })
                  }
                />
              </div>
              <div className="col-md-6">
                <input
                  className="form-control"
                  placeholder="Email"
                  value={patientModel.email}
                  onChange={(e) =>
                    setPatientModel({ ...patientModel, email: e.target.value })
                  }
                />
              </div>
              <div className="col-md-6">
                <input
                  className="form-control"
                  placeholder="Phone"
                  value={patientModel.phone}
                  onChange={(e) =>
                    setPatientModel({ ...patientModel, phone: e.target.value })
                  }
                />
              </div>
              <div className="col-md-4">
                <select
                  className="form-select"
                  value={patientModel.gender}
                  onChange={(e) =>
                    setPatientModel({ ...patientModel, gender: e.target.value })
                  }
                >
                  <option value="">Gender</option>
                  <option>Male</option>
                  <option>Female</option>
                </select>
              </div>
              <div className="col-md-4">
                <input
                  type="date"
                  className="form-control"
                  value={patientModel.dateOfBirth}
                  onChange={(e) =>
                    setPatientModel({
                      ...patientModel,
                      dateOfBirth: e.target.value,
                    })
                  }
                />
              </div>
            </div>
            <div className="small text-muted mt-2">
              We’ll create your patient profile before booking.
            </div>
          </div>
        )}

        <form onSubmit={submit} className="card p-3">
          <div className="row g-2">
            <div className="col-md-4">
              <label className="form-label">Date</label>
              <input
                type="date"
                className="form-control"
                value={date}
                onChange={(e) => setDate(e.target.value)}
              />
            </div>
            <div className="col-md-4">
              <label className="form-label">Time</label>
              <input
                type="time"
                className="form-control"
                value={time}
                onChange={(e) => setTime(e.target.value)}
              />
            </div>
          </div>

          <button
            className="btn btn-success mt-3"
            type="submit"
            disabled={saving || !isLoggedIn}
          >
            {saving ? "Booking..." : "Confirm booking"}
          </button>
        </form>
      </div>
    </>
  );
}

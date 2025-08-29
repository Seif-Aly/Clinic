import React, { useEffect, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";
import useAuth from "../hooks/useAuth";
import PublicHeader from "../components/PublicHeader";

export default function PatientAppointments() {
  const user = useAuth();
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState([]);

  const loadAppointments = async () => {
    if (!user) return;
    setErrors([]);
    setLoading(true);
    try {
      const res = await api.get("/appointments/GetAppointments", {
        params: { patientId: localStorage.getItem("patientId") },
      });

      const data =
        res.data?.Returne?.appointmant ||
        res.data?.appointments ||
        (Array.isArray(res.data) ? res.data : []);

      setAppointments(data);
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAppointments();
    // eslint-disable-next-line
  }, []);

  const handleCancel = async (id) => {
    if (!window.confirm("Cancel this appointment?")) return;
    try {
      await api.delete(`/appointments/${id}`);
      setAppointments((prev) => prev.filter((a) => a.id !== id));
    } catch (err) {
      setErrors(extractErrorMessages(err));
    }
  };

  const getStatusBadge = (status) => {
    switch (status?.toLowerCase()) {
      case "pending":
        return <span className="badge bg-warning">Pending</span>;
      case "confirmed":
        return <span className="badge bg-success">Confirmed</span>;
      case "canceled":
        return <span className="badge bg-danger">Canceled</span>;
      default:
        return (
          <span className="badge bg-secondary">{status || "Unknown"}</span>
        );
    }
  };

  return (
    <>
      <PublicHeader />
      <div className="container py-4">
        <h3 className="mb-3">My Appointments</h3>

        <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

        {loading && (
          <div className="alert alert-info">Loading appointments...</div>
        )}

        {!loading && appointments.length === 0 && (
          <div className="alert alert-info">No appointments found.</div>
        )}

        <div className="row g-3">
          {appointments.map((a) => (
            <div className="col-md-6 col-lg-4" key={a.id}>
              <div
                className="card shadow-sm border-0 h-100"
                style={{ borderRadius: 16 }}
              >
                <div className="card-body d-flex flex-column">
                  <h5 className="card-title mb-1">
                    {a.doctor?.fullName || a.doctorName || "Unknown Doctor"}
                  </h5>
                  <p className="text-muted mb-2">
                    {a.doctor?.specialization || a.specialization || "N/A"}{" "}
                    <br />
                    {a.doctor?.clinic?.name && (
                      <>
                        Clinic: {a.doctor.clinic.name} <br />
                        Hospital: {a.doctor.clinic.hospital?.name}
                      </>
                    )}
                  </p>

                  <div className="mt-auto">
                    <div className="mb-2">
                      <strong>Date:</strong>{" "}
                      {new Date(
                        a.appointmentDateTime || a.date
                      ).toLocaleString()}
                    </div>
                    <div>{getStatusBadge(a.status)}</div>

                    {a.status?.toLowerCase() === "pending" && (
                      <button
                        className="btn btn-sm btn-outline-danger mt-2"
                        onClick={() => handleCancel(a.id)}
                      >
                        Cancel
                      </button>
                    )}
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </>
  );
}

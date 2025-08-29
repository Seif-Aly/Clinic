import React, { useEffect, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import { Link } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";

export default function PrescriptionsList() {
  const [prescriptions, setPrescriptions] = useState([]);
  const [errors, setErrors] = useState([]);

  useEffect(() => {
    const load = async () => {
      setErrors([]);
      try {
        const r = await api.get("/prescriptions");
        setPrescriptions(r.data || []);
      } catch (err) {
        setErrors(extractErrorMessages(err));
      }
    };
    load();
  }, []);

  const handleDelete = async (id) => {
    if (!window.confirm("Confirm deletion of the prescription")) return;
    setErrors([]);
    try {
      await api.delete(`/prescriptions/${id}`);
      setPrescriptions((prev) => prev.filter((p) => p.id !== id));
    } catch (err) {
      setErrors(extractErrorMessages(err));
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between mb-3">
        <h4>prescriptions</h4>
        <Link className="btn btn-primary" to="/dashboard/prescriptions/new">
          Add a prescription
        </Link>
      </div>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <table className="table">
        <thead>
          <tr>
            <th>#</th>
            <th>patient</th>
            <th>Doctor</th>
            <th>Date</th>
            <th>Operations</th>
          </tr>
        </thead>
        <tbody>
          {prescriptions.map((p) => (
            <tr key={p.id}>
              <td>{p.id}</td>
              <td>{p.patient?.fullName || p.patientName}</td>
              <td>{p.doctor?.fullName || p.doctorName}</td>
              <td>
                {p.dateIssued
                  ? new Date(p.dateIssued).toLocaleDateString()
                  : ""}
              </td>
              <td>
                <Link
                  className="btn btn-sm btn-info me-1"
                  to={`/dashboard/prescriptions/${p.id}`}
                >
                  View
                </Link>
                <Link
                  className="btn btn-sm btn-warning me-1"
                  to={`/dashboard/prescriptions/edit/${p.id}`}
                >
                  Edite
                </Link>
                <button
                  className="btn btn-sm btn-danger"
                  onClick={() => handleDelete(p.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {prescriptions.length === 0 && (
            <tr>
              <td colSpan="5" className="text-center">
                No prescriptions
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

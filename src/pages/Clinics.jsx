import React, { useEffect, useState } from "react";
import { getClinics, deleteClinic } from "../services/clinicApi";
import { Link } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";

export default function Clinics() {
  const [clinics, setClinics] = useState([]);
  const [errors, setErrors] = useState([]);

  useEffect(() => {
    loadClinics();
  }, []);

  const loadClinics = async () => {
    setErrors([]);
    try {
      const data = await getClinics();
      setClinics(data?.returns?.clinic || []);
    } catch (errMsgs) {
      setErrors(Array.isArray(errMsgs) ? errMsgs : [String(errMsgs)]);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete?")) return;
    setErrors([]);
    try {
      await deleteClinic(id);
      setClinics((prev) => prev.filter((c) => c.id !== id));
    } catch (errMsgs) {
      setErrors(Array.isArray(errMsgs) ? errMsgs : [String(errMsgs)]);
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4>Clinics</h4>
        <Link to="/dashboard/clinics/new" className="btn btn-primary">
          Add Clinic
        </Link>
      </div>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Name</th>
            <th>Specialization</th>
            <th>Hospital</th>
            <th>Operations</th>
          </tr>
        </thead>
        <tbody>
          {clinics.map((c) => (
            <tr key={c.id}>
              <td>{c.name}</td>
              <td>{c.specialization}</td>
              <td>{c.hospitalName}</td>
              <td>
                <Link
                  to={`/dashboard/clinics/${c.id}`}
                  className="btn btn-sm btn-warning me-1"
                >
                  Edit
                </Link>
                <button
                  className="btn btn-sm btn-danger"
                  onClick={() => handleDelete(c.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {clinics.length === 0 && (
            <tr>
              <td colSpan="4" className="text-center">
                No clinics found
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

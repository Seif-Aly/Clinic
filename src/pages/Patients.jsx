import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";
import {
  getPatients,
  deletePatient,
  createPatient,
} from "../services/patientApi";

export default function Patients() {
  const [patients, setPatients] = useState([]);
  const [errors, setErrors] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const load = async () => {
      setErrors([]);
      try {
        const data = await getPatients();
        setPatients(data);
      } catch (err) {
        setErrors(["Failed to load patients."]);
      }
    };
    load();
  }, []);

  const handleDelete = async (id) => {
    if (!window.confirm("Confirm deletion")) return;
    try {
      await deletePatient(id);
      setPatients((prev) => prev.filter((p) => p.id !== id));
    } catch (err) {
      setErrors(["Error deleting patient."]);
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4>Patients</h4>
        <button
          className="btn btn-primary"
          onClick={() => navigate("/patient/new")}
        >
          Add Patient
        </button>
      </div>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Name</th>
            <th>National ID</th>
            <th>Phone</th>
            <th>Operations</th>
          </tr>
        </thead>
        <tbody>
          {patients.map((p) => (
            <tr key={p.id}>
              <td>{p.fullName || p.name}</td>
              <td>{p.nationalId}</td>
              <td>{p.phone}</td>
              <td>
                <button
                  className="btn btn-sm btn-danger"
                  onClick={() => handleDelete(p.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {patients.length === 0 && (
            <tr>
              <td colSpan="4" className="text-center">
                No patients
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

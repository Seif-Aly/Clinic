<<<<<<< HEAD
import React, { useEffect, useState } from "react";
import api from "../services/api";

export default function Hospitals() {
    const [hospitals, setHospitals] = useState([]);

    useEffect(() => {
        api.get("/hospitals")
            .then(res => setHospitals(res.data || []))
            .catch(err => console.error(err));
    }, []);

    const handleDelete = async (id) => {
        if (!window.confirm("Confirm deletion?")) return;
        try {
            await api.delete(`/hospitals/${ id }`);
            setHospitals(prev => prev.filter(h => h.id !== id));
        } catch (err) {
            console.error(err);
            alert("error occurred while deleting");
        }
    };

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h4>Hospitals</h4>
            </div>

            <table className="table table-striped">
                <thead><tr><th>Name</th><th>Address</th><th>Phone</th><th>Operations</th></tr></thead>
                <tbody>
                    {hospitals.map(h => (
                        <tr key={h.id}>
                            <td>{h.name}</td>
                            <td>{h.address}</td>
                            <td>{h.phone}</td>
                            <td>
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(h.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
=======
import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";
import { getHospitals, deleteHospital } from "../services/hospitalApi";

export default function Hospitals() {
  const [hospitals, setHospitals] = useState([]);
  const [errors, setErrors] = useState([]);

  useEffect(() => {
    loadHospitals();
  }, []);

  const loadHospitals = async () => {
    setErrors([]);
    try {
      const data = await getHospitals();
      setHospitals(data.returns?.hospitals || []);
    } catch (errMsgs) {
      setErrors(Array.isArray(errMsgs) ? errMsgs : [String(errMsgs)]);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this hospital?"))
      return;
    setErrors([]);
    try {
      await deleteHospital(id);
      setHospitals((prev) => prev.filter((h) => h.id !== id));
    } catch (errMsgs) {
      setErrors(Array.isArray(errMsgs) ? errMsgs : [String(errMsgs)]);
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4>Hospitals</h4>
        <Link to="/dashboard/hospitals/new" className="btn btn-primary">
          Add Hospital
        </Link>
      </div>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Name</th>
            <th>Address</th>
            <th>Phone</th>
            <th>Operations</th>
          </tr>
        </thead>
        <tbody>
          {hospitals.map((h) => (
            <tr key={h.id}>
              <td>{h.name}</td>
              <td>{h.address}</td>
              <td>{h.phone}</td>
              <td>
                <Link
                  to={`/dashboard/hospitals/${h.id}`}
                  className="btn btn-sm btn-warning me-1"
                >
                  Edit
                </Link>
                <button
                  className="btn btn-sm btn-danger"
                  onClick={() => handleDelete(h.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {hospitals.length === 0 && (
            <tr>
              <td colSpan="4" className="text-center">
                No hospitals found
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
>>>>>>> main

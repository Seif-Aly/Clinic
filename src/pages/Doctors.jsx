<<<<<<< HEAD
﻿import React, { useEffect, useState } from "react";
import { getDoctors, deleteDoctor } from "../services/doctorApi";
import { Link } from "react-router-dom";

export default function Doctors() {
    const [doctors, setDoctors] = useState([]);

    useEffect(() => {
        loadDoctors();
    }, []);

    const loadDoctors = async () => {
        try {
            const data = await getDoctors();
            // لاحظ إن البيانات بترجع جوة Return.doctor
            setDoctors(data?.Return?.doctor || []);
        } catch (err) {
            console.error(err);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete?")) return;
        try {
            await deleteDoctor(id);
            setDoctors(prev => prev.filter(d => d.id !== id));
        } catch (err) {
            console.error(err);
            alert("Error occurred while deleting");
        }
    };

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h4>Doctors</h4>
                <Link to="/doctors/new" className="btn btn-primary">Add Doctor</Link>
            </div>

            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Specialization</th>
                        <th>Phone</th>
                        <th>Operations</th>
                    </tr>
                </thead>
                <tbody>
                    {doctors.map(d => (
                        <tr key={d.id}>
                            <td>{d.fullName}</td>
                            <td>{d.specialization}</td>
                            <td>{d.phone}</td>
                            <td>
                                <Link to={`/doctors/${d.id}`} className="btn btn-sm btn-warning me-1">Edit</Link>
                            <button className="btn btn-sm btn-danger" onClick={() => handleDelete(d.id)}>Delete</button>
                        </td>
                        </tr>
                    ))}
            </tbody>
        </table>
        </div >
    );
}
=======
﻿import React, { useEffect, useState } from "react";
import { getDoctors, deleteDoctor } from "../services/doctorApi";
import { Link } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";

export default function Doctors() {
  const [doctors, setDoctors] = useState([]);
  const [errors, setErrors] = useState([]);

  useEffect(() => {
    loadDoctors();
  }, []);

  const loadDoctors = async () => {
    setErrors([]);
    try {
      const data = await getDoctors();
      setDoctors(data?.doctors || []);
    } catch (errMsgs) {
      setErrors(Array.isArray(errMsgs) ? errMsgs : [String(errMsgs)]);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete?")) return;
    setErrors([]);
    try {
      await deleteDoctor(id);
      setDoctors((prev) => prev.filter((d) => d.id !== id));
    } catch (errMsgs) {
      setErrors(Array.isArray(errMsgs) ? errMsgs : [String(errMsgs)]);
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4>Doctors</h4>
        <Link to="/dashboard/doctors/new" className="btn btn-primary">
          Add Doctor
        </Link>
      </div>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Name</th>
            <th>Specialization</th>
            <th>Phone</th>
            <th>Operations</th>
          </tr>
        </thead>
        <tbody>
          {doctors.map((d) => (
            <tr key={d.id}>
              <td>{d.fullName}</td>
              <td>{d.specialization}</td>
              <td>{d.phone}</td>
              <td>
                <Link
                  to={`/dashboard/doctors/${d.id}`}
                  className="btn btn-sm btn-warning me-1"
                >
                  Edit
                </Link>
                <button
                  className="btn btn-sm btn-danger"
                  onClick={() => handleDelete(d.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {doctors.length === 0 && (
            <tr>
              <td colSpan="4" className="text-center">
                No doctors found
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
>>>>>>> main

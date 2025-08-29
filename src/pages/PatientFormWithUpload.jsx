import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";
import {
  getPatients,
  deletePatient,
  createPatient,
  getPatientById,
  updatePatient,
} from "../services/patientApi";
import { extractErrorMessages } from "../services/api";

export default function PatientFormWithUpload() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [saving, setSaving] = useState(false);
  const [errors, setErrors] = useState([]);

  const [model, setModel] = useState({
    fullName: "",
    nationalId: "",
    email: "",
    phone: "",
    gender: "",
    dateOfBirth: "",
  });

  useEffect(() => {
    if (id) {
      getPatientById(id)
        .then((data) => {
          setModel({
            ...data,
            dateOfBirth: data.dateOfBirth?.slice(0, 10), // format to yyyy-mm-dd
          });
        })
        .catch((err) => {
          setErrors(extractErrorMessages(err));
        });
    }
  }, [id]);

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setSaving(true);
    try {
      const payload = {
        ...model,
        id: id ? parseInt(id) : 0,
        dateOfBirth: new Date(model.dateOfBirth).toISOString(),
        appointments: [],
      };

      if (id) await updatePatient(id, payload);
      else await createPatient(payload);

      navigate("/dashboard/patients");
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h4>{id ? "Edit Patient" : "Add Patient"}</h4>
      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />
      <form onSubmit={submit}>
        <input
          className="form-control mb-2"
          placeholder="Full Name"
          value={model.fullName}
          onChange={(e) => setModel({ ...model, fullName: e.target.value })}
          required
        />
        <input
          className="form-control mb-2"
          placeholder="National ID"
          value={model.nationalId}
          onChange={(e) => setModel({ ...model, nationalId: e.target.value })}
        />
        <input
          className="form-control mb-2"
          placeholder="Email"
          type="email"
          value={model.email}
          onChange={(e) => setModel({ ...model, email: e.target.value })}
        />
        <input
          className="form-control mb-2"
          placeholder="Phone"
          value={model.phone}
          onChange={(e) => setModel({ ...model, phone: e.target.value })}
        />
        <select
          className="form-control mb-2"
          value={model.gender}
          onChange={(e) => setModel({ ...model, gender: e.target.value })}
        >
          <option value="">Select Gender</option>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </select>
        <input
          className="form-control mb-3"
          type="date"
          value={model.dateOfBirth}
          onChange={(e) => setModel({ ...model, dateOfBirth: e.target.value })}
        />

        <button className="btn btn-primary" type="submit" disabled={saving}>
          {saving ? "Saving..." : "Save"}
        </button>
      </form>
    </div>
  );
}

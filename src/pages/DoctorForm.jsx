import React, { useEffect, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import { useParams, useNavigate } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";
import { updateDoctor, createDoctor } from "../services/doctorApi";

export default function DoctorForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [errors, setErrors] = useState([]);
  const [saving, setSaving] = useState(false);
  const [model, setModel] = useState({
    fullName: "",
    email: "",
    phone: "",
    specialization: "",
    clinicId: "",
  });
  const [clinics, setClinics] = useState([]);

  useEffect(() => {
    api
      .get("/Clinics/GetClinics")
      .then((r) => setClinics(r.data || []))
      .catch(() => {});
    if (id) {
      api
        .get(`/doctors/${id}`)
        .then((res) => setModel(res.data))
        .catch(() => {});
    }
  }, [id]);

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setSaving(true);
    try {
      if (id) await updateDoctor(id, model);
      else await createDoctor(model);
      navigate("/doctors");
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h4>{id ? "Edit doctor" : "Add doctor"}</h4>
      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <form onSubmit={submit}>
        <input
          className="form-control mb-2"
          placeholder="Name"
          value={model.fullName}
          onChange={(e) => setModel({ ...model, fullName: e.target.value })}
          required
        />
        <input
          className="form-control mb-2"
          placeholder="Email"
          value={model.email}
          onChange={(e) => setModel({ ...model, email: e.target.value })}
        />
        <input
          className="form-control mb-2"
          placeholder="Phone"
          value={model.phone}
          onChange={(e) => setModel({ ...model, phone: e.target.value })}
        />
        <input
          className="form-control mb-2"
          placeholder="Specialization"
          value={model.specialization}
          onChange={(e) =>
            setModel({ ...model, specialization: e.target.value })
          }
        />
        {/* <select
          className="form-control mb-3"
          value={model.clinicId || ""}
          onChange={(e) => setModel({ ...model, clinicId: e.target.value })}
        >
          <option value="">Choose an appointment</option>
          {clinics.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select> */}

        <button className="btn btn-success" type="submit" disabled={saving}>
          {saving ? "Saving..." : "Save"}
        </button>
      </form>
    </div>
  );
}

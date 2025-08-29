import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";
import { updateDoctor, createDoctor } from "../services/doctorApi";

export default function DoctorFormWithUpload() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [model, setModel] = useState({
    fullName: "",
    email: "",
    phone: "",
    specialization: "",
    clinicId: "",
  });

  const [photoFile, setPhotoFile] = useState(null);
  const [clinics, setClinics] = useState([]);
  const [errors, setErrors] = useState([]);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    api.get("/Clinics/GetClinics").then((res) => {
      const allClinics = res.data?.returns?.clinic || [];
      setClinics(allClinics);
    });

    if (id) {
      api
        .get(`/doctors/${id}`)
        .then((r) => setModel(r.data))
        .catch(() => {});
    }
  }, [id]);

  const handleFileChange = (e) => {
    setPhotoFile(e.target.files[0] || null);
  };

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setSaving(true);

    try {
      const fd = new FormData();
      fd.append("Id", id || 0);
      fd.append("FullName", model.fullName);
      fd.append("Email", model.email);
      fd.append("Phone", model.phone);
      fd.append("Specialization", model.specialization);
      fd.append("ClinicId", model.clinicId);
      if (photoFile) fd.append("Image", photoFile);

      if (id) await updateDoctor(id, fd);
      else await createDoctor(fd);

      navigate("/doctors");
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h4>{id ? "Edit Doctor" : "Add Doctor"}</h4>
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

        <select
          className="form-control mb-2"
          value={model.clinicId || ""}
          onChange={(e) => setModel({ ...model, clinicId: e.target.value })}
          required
        >
          <option value="">Choose a Clinic</option>
          {clinics.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>

        <div className="mb-3">
          <label>Personal Photo</label>
          <input
            type="file"
            accept="image/*"
            className="form-control"
            onChange={handleFileChange}
          />
        </div>

        <button className="btn btn-success" type="submit" disabled={saving}>
          {saving ? "Saving..." : "Save"}
        </button>
      </form>
    </div>
  );
}

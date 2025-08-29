import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";

export default function ClinicFormWithUpload() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [model, setModel] = useState({
    name: "",
    specialization: "",
    hospitalId: "",
  });

  const [hospitals, setHospitals] = useState([]);
  const [photoFile, setPhotoFile] = useState(null);
  const [errors, setErrors] = useState([]);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    // Load hospitals for dropdown
    api
      .get("/hospitals/getHospitals")
      .then((r) => {
        const data = r.data?.returns?.hospitals || [];
        setHospitals(data);
      })
      .catch(() => {});

    // Load clinic if editing
    if (id) {
      api
        .get(`/clinics/${id}`)
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
      fd.append("name", model.name);
      fd.append("specialization", model.specialization);
      fd.append("hospitalId", model.hospitalId);
      if (photoFile) fd.append("image", photoFile);

      if (id) {
        await api.put(`/Clinics/PutClinic/${id}`, fd);
      } else {
        await api.post("/Clinics/PostClinic", fd);
      }

      navigate("/dashboard/clinics");
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h4>{id ? "Edit Clinic" : "Add Clinic"}</h4>
      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <form onSubmit={submit}>
        <input
          className="form-control mb-2"
          placeholder="Clinic Name"
          value={model.name}
          onChange={(e) => setModel({ ...model, name: e.target.value })}
          required
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
          className="form-control mb-3"
          value={model.hospitalId || ""}
          onChange={(e) => setModel({ ...model, hospitalId: e.target.value })}
          required
        >
          <option value="">Select Hospital</option>
          {hospitals.map((h) => (
            <option key={h.id} value={h.id}>
              {h.name}
            </option>
          ))}
        </select>

        <div className="mb-3">
          <label>Upload Image</label>
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

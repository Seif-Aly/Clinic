import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";

export default function HospitalFormWithUpload() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [model, setModel] = useState({
    name: "",
    address: "",
    phone: "",
  });
  const [imageFile, setImageFile] = useState(null);
  const [errors, setErrors] = useState([]);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (id) {
      api
        .get(`/Hospitals/GetHospital/${id}`)
        .then((r) => setModel(r.data))
        .catch(() => {});
    }
  }, [id]);

  const handleFileChange = (e) => {
    setImageFile(e.target.files[0] || null);
  };

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setSaving(true);

    try {
      const formData = new FormData();
      formData.append("name", model.name);
      formData.append("address", model.address);
      formData.append("phone", model.phone);
      if (imageFile) formData.append("image", imageFile);

      if (id) {
        await api.put(`/Hospitals/PutHospital/${id}`, formData);
      } else {
        await api.post("/Hospitals/PostHospital", formData);
      }

      navigate("/dashboard/hospitals");
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h4>{id ? "Edit Hospital" : "Add Hospital"}</h4>
      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <form onSubmit={submit}>
        <input
          className="form-control mb-2"
          placeholder="Hospital Name"
          value={model.name}
          onChange={(e) => setModel({ ...model, name: e.target.value })}
          required
        />
        <input
          className="form-control mb-2"
          placeholder="Address"
          value={model.address}
          onChange={(e) => setModel({ ...model, address: e.target.value })}
          required
        />
        <input
          className="form-control mb-2"
          placeholder="Phone"
          value={model.phone}
          onChange={(e) => setModel({ ...model, phone: e.target.value })}
          required
        />

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

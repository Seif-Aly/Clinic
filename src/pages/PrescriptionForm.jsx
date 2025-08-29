import React, { useEffect, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import { useNavigate, useParams } from "react-router-dom";
import ErrorsAlert from "../components/ErrorsAlert";

export default function PrescriptionForm() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [errors, setErrors] = useState([]);
  const [saving, setSaving] = useState(false);

  const [model, setModel] = useState({
    diagnosis: "",
    notes: "",
    dateIssued: new Date().toISOString().slice(0, 10),
    appointmentId: "",
    doctorId: "",
    patientId: "",
    items: [{ medicationName: "", dosage: "", instructions: "" }],
  });

  useEffect(() => {
    if (id) {
      api
        .get(`/prescriptions/${id}`)
        .then((r) => {
          const data = r.data;
          setModel({
            diagnosis: data.diagnosis || "",
            notes: data.notes || "",
            dateIssued: data.dateIssued
              ? data.dateIssued.slice(0, 10)
              : new Date().toISOString().slice(0, 10),
            appointmentId: data.appointmentId || "",
            doctorId: data.doctorId || "",
            patientId: data.patientId || "",
            items:
              data.prescriptionItems && data.prescriptionItems.length
                ? data.prescriptionItems.map((it) => ({
                    medicationName: it.medicineName || it.medicationName,
                    dosage: it.dosage,
                    instructions: it.instructions,
                  }))
                : [{ medicationName: "", dosage: "", instructions: "" }],
          });
        })
        .catch(() => {});
    }
  }, [id]);

  const handleItemChange = (idx, field, value) => {
    const next = [...model.items];
    next[idx][field] = value;
    setModel({ ...model, items: next });
  };

  const addItem = () =>
    setModel({
      ...model,
      items: [
        ...model.items,
        { medicationName: "", dosage: "", instructions: "" },
      ],
    });
  const removeItem = (idx) =>
    setModel({ ...model, items: model.items.filter((_, i) => i !== idx) });

  const submit = async (e) => {
    e.preventDefault();
    setErrors([]);
    setSaving(true);
    try {
      const payload = {
        Diagnosis: model.diagnosis,
        Notes: model.notes,
        DateIssued: model.dateIssued,
        AppointmentId: model.appointmentId,
        DoctorId: model.doctorId,
        PatientId: model.patientId,
        Items: model.items.map((i) => ({
          MedicationName: i.medicationName,
          Dosage: i.dosage,
          Instructions: i.instructions,
        })),
      };

      if (id) {
        await api.put(`/prescriptions/${id}`, payload);
      } else {
        await api.post("/prescriptions/with-items", payload);
      }

      navigate("/prescriptions");
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h4>{id ? "Edit prescription" : "Add prescription"}</h4>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <form onSubmit={submit}>
        <input
          className="form-control mb-2"
          placeholder="Diagnosis"
          value={model.diagnosis}
          onChange={(e) => setModel({ ...model, diagnosis: e.target.value })}
          required
        />
        <textarea
          className="form-control mb-2"
          placeholder="comments"
          value={model.notes}
          onChange={(e) => setModel({ ...model, notes: e.target.value })}
        />
        <input
          type="date"
          className="form-control mb-2"
          value={model.dateIssued}
          onChange={(e) => setModel({ ...model, dateIssued: e.target.value })}
        />

        <input
          className="form-control mb-2"
          placeholder="AppointmentId"
          value={model.appointmentId}
          onChange={(e) =>
            setModel({ ...model, appointmentId: e.target.value })
          }
        />
        <input
          className="form-control mb-2"
          placeholder="DoctorId"
          value={model.doctorId}
          onChange={(e) => setModel({ ...model, doctorId: e.target.value })}
        />
        <input
          className="form-control mb-2"
          placeholder="PatientId"
          value={model.patientId}
          onChange={(e) => setModel({ ...model, patientId: e.target.value })}
        />

        <hr />
        <h5>pharmaceutical</h5>
        {model.items.map((it, idx) => (
          <div key={idx} className="mb-2 border p-2 rounded">
            <div className="d-flex gap-2">
              <input
                className="form-control"
                placeholder="Name medicine"
                value={it.medicationName}
                onChange={(e) =>
                  handleItemChange(idx, "medicationName", e.target.value)
                }
                required
              />
              <input
                className="form-control"
                placeholder="Dosage"
                value={it.dosage}
                onChange={(e) =>
                  handleItemChange(idx, "dosage", e.target.value)
                }
              />
              <button
                type="button"
                className="btn btn-danger"
                onClick={() => removeItem(idx)}
              >
                Delete
              </button>
            </div>
            <textarea
              className="form-control mt-2"
              placeholder="directions"
              value={it.instructions}
              onChange={(e) =>
                handleItemChange(idx, "instructions", e.target.value)
              }
            />
          </div>
        ))}

        <button
          type="button"
          className="btn btn-sm btn-outline-primary mb-2"
          onClick={addItem}
        >
          Add medicine
        </button>
        <br />
        <button className="btn btn-success" type="submit" disabled={saving}>
          {saving ? "Saving..." : "Save the prescription"}
        </button>
      </form>
    </div>
  );
}

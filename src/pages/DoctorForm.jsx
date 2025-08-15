import React, { useEffect, useState } from "react";
import api from "../services/api";
import { useParams, useNavigate } from "react-router-dom";

export default function DoctorForm() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [model, setModel] = useState({ fullName: "", email: "", phone: "", specialization: "", clinicId: "" });
    const [clinics, setClinics] = useState([]);

    useEffect(() => {
        api.get("/clinics").then(r => setClinics(r.data || [])).catch(() => { });
        if (id) {
            api.get(`/doctors/${id}`).then(res => setModel(res.data)).catch(() => { });

        }
    }, [id]);

    const submit = async (e) => {
        e.preventDefault();
        try {
            if (id) await api.put(`/doctors/${ id }, model`);
            else await api.post("/doctors", model);
            navigate("/doctors");
        } catch (err) {
            console.error(err);
            alert("error occurred while saving");
        }
    };

    return (
        <div>
            <h4>{id ? "Edit doctor" : "Add doctor"}</h4>
            <form onSubmit={submit}>
                <input className="form-control mb-2" placeholder="Name" value={model.fullName} onChange={e => setModel({ ...model, fullName: e.target.value })} required />
                <input className="form-control mb-2" placeholder="Email" value={model.email} onChange={e => setModel({ ...model, email: e.target.value })} />
                <input className="form-control mb-2" placeholder="Phone" value={model.phone} onChange={e => setModel({ ...model, phone: e.target.value })} />
                <input className="form-control mb-2" placeholder="Specialization" value={model.specialization} onChange={e => setModel({ ...model, specialization: e.target.value })} />
                <select className="form-control mb-3" value={model.clinicId || ""} onChange={e => setModel({ ...model, clinicId: e.target.value })}>
                    <option value="">Choose an appointment</option>
                    {clinics.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>

                <button className="btn btn-success" type="submit">save</button>
            </form>
        </div>
    );
}
import React, { useEffect, useState } from "react";
import api from "../services/api";
import { useParams, Link } from "react-router-dom";

export default function PrescriptionDetails() {
    const { id } = useParams();
    const [pres, setPres] = useState(null);

    useEffect(() => {
        api.get(`/prescriptions/${ id }`)
            .then(r => setPres(r.data))
            .catch(err => console.error(err));
    }, [id]);

    if (!pres) return <div>Loading...</div>;

    return (
        <div>
            <h4>—Ê‘ … #{pres.id}</h4>
            <p><strong>Diagnosis:</strong> {pres.diagnosis}</p>
            <p><strong>patient:</strong> {pres.patient?.fullName || pres.patientName}</p>
            <p><strong>Doctor:</strong> {pres.doctor?.fullName || pres.doctorName}</p>
            <p><strong>Date:</strong> {pres.dateIssued ? new Date(pres.dateIssued).toLocaleString() : ''}</p>

            <h5 className="mt-3">pharmaceutical</h5>
            <ul className="list-group">
                {pres.prescriptionItems && pres.prescriptionItems.map(it => (
                    <li className="list-group-item" key={it.id || it.medicationName}>
                        <strong>{it.medicineName || it.medicationName}</strong> ó {it.dosage} <br />
                        <small>{it.instructions}</small>
                    </li>
                ))}
            </ul>

            <div className="mt-3">
                <Link to="/prescriptions" className="btn btn-secondary me-2">Back</Link>
                <Link to={`/prescriptions/edit / ${pres.id}`} className="btn btn-warning">Edit</Link>
        </div>
    </div >
  );
}
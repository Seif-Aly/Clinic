import React, { useEffect, useState } from "react";
import api from "../services/api";

export default function Patients() {
    const [patients, setPatients] = useState([]);

    useEffect(() => {
        api.get("/patients")
            .then(res => setPatients(res.data || []))
            .catch(err => console.error(err));
    }, []);

    const handleDelete = async (id) => {
        if (!window.confirm("Confirm deletion")) return;
        try {
            await api.delete(`/patients/${ id }`);
            setPatients(prev => prev.filter(p => p.id !== id));
        } catch (err) {
            console.error(err);
            alert("error occurred while saving the doctor");
        }
    };

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h4>patient</h4>
            </div>

            <table className="table table-striped">
                <thead><tr><th>Name</th><th>National ID</th><th>Phone</th><th>Operations</th></tr></thead>
                <tbody>
                    {patients.map(p => (
                        <tr key={p.id}>
                            <td>{p.fullName || p.name}</td>
                            <td>{p.nationalId}</td>
                            <td>{p.phone}</td>
                            <td>
                                
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(p.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
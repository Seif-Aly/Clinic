import React, { useEffect, useState } from "react";
import api from "../services/api";

export default function Hospitals() {
    const [hospitals, setHospitals] = useState([]);

    useEffect(() => {
        api.get("/hospitals")
            .then(res => setHospitals(res.data || []))
            .catch(err => console.error(err));
    }, []);

    const handleDelete = async (id) => {
        if (!window.confirm("Confirm deletion?")) return;
        try {
            await api.delete(`/hospitals/${ id }`);
            setHospitals(prev => prev.filter(h => h.id !== id));
        } catch (err) {
            console.error(err);
            alert("error occurred while deleting");
        }
    };

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h4>Hospitals</h4>
            </div>

            <table className="table table-striped">
                <thead><tr><th>Name</th><th>Address</th><th>Phone</th><th>Operations</th></tr></thead>
                <tbody>
                    {hospitals.map(h => (
                        <tr key={h.id}>
                            <td>{h.name}</td>
                            <td>{h.address}</td>
                            <td>{h.phone}</td>
                            <td>
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(h.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
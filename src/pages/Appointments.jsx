import React, { useEffect, useState } from "react";
import api from "../services/api";

export default function Appointments() {
    const [appointments, setAppointments] = useState([]);

    useEffect(() => {
        api.get("/appointments")
            .then(res => setAppointments(res.data || []))
            .catch(err => console.error(err));
    }, []);

    const handleDelete = async (id) => {
        if (!window.confirm("Confirm appointment cancellation ?")) return;
        try {
            await api.delete`(/appointments/${id})`;


            setAppointments(prev => prev.filter(a => a.id !== id));
        } catch (err) {
            console.error(err);
            alert("Error while deleting");
        }
    };

    return (
        <div>
            <h4>Appointments</h4>
            <table className="table table-striped">
                <thead><tr><th>the patient</th><th>The doctor</th><th>the date</th><th>the condition</th><th>Operations</th></tr></thead>
                <tbody>
                    {appointments.map(a => (
                        <tr key={a.id}>
                            <td>{a.patient?.fullName || a.patientName}</td>
                            <td>{a.doctor?.fullName || a.doctorName}</td>
                            <td>{a.date ? new Date(a.date).toLocaleString() : a.dateTime}</td>
                            <td>{a.status}</td>
                            <td>
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(a.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
import React, { useEffect, useState } from "react";
import api from "../services/api";

export default function Dashboard() {
    const [stats, setStats] = useState({ doctors: 0, patients: 0, appointments: 0, hospitals: 0 });
    const [latestAppointments, setLatestAppointments] = useState([]);
    const [latestPatients, setLatestPatients] = useState([]);

    useEffect(() => {
        const load = async () => {
            try {
               
                const [dRes, pRes, aRes, hRes] = await Promise.all([
                    api.get("/doctors"),
                    api.get("/patients"),
                    api.get("/appointments"),
                    api.get("/hospitals")
                ]);

                setStats({
                    doctors: Array.isArray(dRes.data) ? dRes.data.length : (dRes.data.count ?? 0),
                    patients: Array.isArray(pRes.data) ? pRes.data.length : (pRes.data.count ?? 0),
                    appointments: Array.isArray(aRes.data) ? aRes.data.length : (aRes.data.count ?? 0),
                    hospitals: Array.isArray(hRes.data) ? hRes.data.length : (hRes.data.count ?? 0),
                });

                
                const apps = Array.isArray(aRes.data) ? aRes.data.slice(-5).reverse() : [];
                const pats = Array.isArray(pRes.data) ? pRes.data.slice(-5).reverse() : [];

                setLatestAppointments(apps);
                setLatestPatients(pats);
            } catch (err) {
                console.error("Dashboard load failed", err);
            }
        };

        load();
    }, []);

    return (
        <div>
            <h2>Control panel</h2>
            <div className="row my-4">
                <div className="col-md-3">
                    <div className="card p-3 text-center card-small bg-primary text-white">
                        <div>doctors</div>
                        <h3>{stats.doctors}</h3>
                    </div>
                </div>
                <div className="col-md-3">
                    <div className="card p-3 text-center card-small bg-success text-white">
                        <div>patients</div>
                        <h3>{stats.patients}</h3>
                    </div>
                </div>
                <div className="col-md-3">
                    <div className="card p-3 text-center card-small bg-warning text-white">
                        <div>Appointments</div>
                        <h3>{stats.appointments}</h3>
                    </div>
                </div>
                <div className="col-md-3">
                    <div className="card p-3 text-center card-small bg-danger text-white">
                        <div>hospitals</div>
                        <h3>{stats.hospitals}</h3>
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-md-6">
                    <h5> Last appointments </h5>
                    <table className="table">
                        <thead><tr><th>patient</th><th>doctor</th><th>date</th></tr></thead>
                        <tbody>
                            {latestAppointments.map(a => (
                                <tr key={a.id || a.appointmentId}>
                                    <td>{a.patient?.fullName || a.patientName || a.patient}</td>
                                    <td>{a.doctor?.fullName || a.doctorName || a.doctor}</td>
                                    <td>{a.date ? new Date(a.date).toLocaleString() : a.dateTime}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                <div className="col-md-6">
                    <h5>  last patients </h5>
                    <table className="table">
                        <thead><tr><th> Name</th><th>Phone</th></tr></thead>
                        <tbody>
                            {latestPatients.map(p => (
                                <tr key={p.id}><td>{p.fullName || p.name}</td><td>{p.phone}</td></tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}
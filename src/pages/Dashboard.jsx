import React, { useEffect, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";

export default function Dashboard() {
  const [stats, setStats] = useState({
    doctors: 0,
    patients: 0,
    appointments: 0,
    hospitals: 0,
  });
  const [latestAppointments, setLatestAppointments] = useState([]);
  const [latestPatients, setLatestPatients] = useState([]);
  const [errors, setErrors] = useState([]);

  useEffect(() => {
    const load = async () => {
      setErrors([]);
      try {
        const [dRes, pRes, hRes] = await Promise.all([
          api.get("/doctors/Getdoctors"),
          api.get("/patients/GetPatients"),
          // api.get("/appointments/GetAppointments"),
          api.get("/hospitals/GetHospitals"),
        ]);

        const getCount = (data) => {
          if (!data) return 0;
          if (Array.isArray(data)) return data.length;

          if (typeof data === "object") {
            if ("count" in data) return data.count;
            if ("totalCount" in data) return data.totalCount;

            if ("doctors" in data) return getCount(data.doctors);
            if ("patients" in data) return getCount(data.patients);
            if ("appointments" in data) return getCount(data.appointments);
            if ("hospitals" in data) return getCount(data.hospitals);

            if ("returns" in data) return getCount(data.returns);
          }

          return 0;
        };

        setStats({
          doctors: getCount(dRes.data),
          patients: getCount(pRes.data),
          // appointments: getCount(aRes.data),
          hospitals: getCount(hRes.data),
        });

        console.log("Doctors data:", dRes.data);
        console.log("Patients data:", pRes.data);
        // console.log("Appointments data:", aRes.data);
        console.log("Hospitals data:", hRes.data);

        // const apps = Array.isArray(aRes.data)
        //   ? aRes.data.slice(-5).reverse()
        //   : [];
        const pats = Array.isArray(pRes.data)
          ? pRes.data.slice(-5).reverse()
          : [];

        // setLatestAppointments(apps);
        setLatestPatients(pats);
      } catch (err) {
        setErrors(extractErrorMessages(err));
      }
    };
    load();
  }, []);

  return (
    <div>
      <h2>Control panel</h2>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

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
            <h3>0</h3>
            {/* <h3>{stats.appointments}</h3> */}
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
            <thead>
              <tr>
                <th>patient</th>
                <th>doctor</th>
                <th>date</th>
              </tr>
            </thead>
            <tbody>
              {latestAppointments.map((a) => (
                <tr key={a.id || a.appointmentId}>
                  <td>{a.patient?.fullName || a.patientName || a.patient}</td>
                  <td>{a.doctor?.fullName || a.doctorName || a.doctor}</td>
                  <td>
                    {a.date ? new Date(a.date).toLocaleString() : a.dateTime}
                  </td>
                </tr>
              ))}
              {latestAppointments.length === 0 && (
                <tr>
                  <td colSpan="3">No recent appointments</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="col-md-6">
          <h5> last patients </h5>
          <table className="table">
            <thead>
              <tr>
                <th> Name</th>
                <th>Phone</th>
              </tr>
            </thead>
            <tbody>
              {latestPatients.map((p) => (
                <tr key={p.id}>
                  <td>{p.fullName || p.name}</td>
                  <td>{p.phone}</td>
                </tr>
              ))}
              {latestPatients.length === 0 && (
                <tr>
                  <td colSpan="2">No recent patients</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

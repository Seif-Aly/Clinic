import React, { useEffect, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";

export default function Appointments() {
  const [appointments, setAppointments] = useState([]);
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      setErrors([]);
      try {
        const res = await api.get("/appointments");
        setAppointments(res.data || []);
      } catch (err) {
        setErrors(extractErrorMessages(err));
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const handleDelete = async (id) => {
    if (!window.confirm("Confirm appointment cancellation ?")) return;
    setErrors([]);
    try {
      await api.delete(`/appointments/${id}`);
      setAppointments((prev) => prev.filter((a) => a.id !== id));
    } catch (err) {
      setErrors(extractErrorMessages(err));
    }
  };

  return (
    <div>
      <h4>Appointments</h4>

      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      {loading ? (
        <div>Loading...</div>
      ) : (
        <table className="table table-striped">
          <thead>
            <tr>
              <th>the patient</th>
              <th>The doctor</th>
              <th>the date</th>
              <th>the condition</th>
              <th>Operations</th>
            </tr>
          </thead>
          <tbody>
            {appointments.map((a) => (
              <tr key={a.id || a.appointmentId}>
                <td>{a.patient?.fullName || a.patientName}</td>
                <td>{a.doctor?.fullName || a.doctorName}</td>
                <td>
                  {new Date(
                    a.appointmentDateTime || a.dateTime || a.date
                  ).toLocaleString()}
                </td>
                <td>{a.status}</td>
                <td>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(a.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
            {appointments.length === 0 && (
              <tr>
                <td colSpan="5" className="text-center">
                  There are no appointments
                </td>
              </tr>
            )}
          </tbody>
        </table>
      )}
    </div>
  );
}

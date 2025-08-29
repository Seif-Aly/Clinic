import React, { useEffect, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";

export default function AppointmentsAdvanced() {
  const [appointments, setAppointments] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [patients, setPatients] = useState([]);
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(true);

  const [filters, setFilters] = useState({
    doctorId: "",
    patientId: "",
    status: "",
    fromDate: "",
    toDate: "",
  });

  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const totalPages = Math.ceil(total / pageSize) || 1;

  useEffect(() => {
    // load doctors & patients for filters
    api
      .get("/doctors")
      .then((r) => setDoctors(r.data || []))
      .catch(() => {});
    api
      .get("/patients")
      .then((r) => setPatients(r.data || []))
      .catch(() => {});
  }, []);

  useEffect(() => {
    const load = async () => {
      setErrors([]);
      setLoading(true);
      try {
        const params = {
          page,
          pageSize,
          doctorId: filters.doctorId || undefined,
          patientId: filters.patientId || undefined,
          status: filters.status || undefined,
          fromDate: filters.fromDate || undefined,
          toDate: filters.toDate || undefined,
        };
        const res = await api.get("/appointments", { params });
        if (res.data?.items) {
          setAppointments(res.data.items);
          setTotal(
            res.data.total ?? res.data.count ?? res.data.totalCount ?? 0
          );
        } else if (Array.isArray(res.data)) {
          const all = res.data;
          setTotal(all.length);
          setAppointments(all.slice((page - 1) * pageSize, page * pageSize));
        } else {
          setAppointments([]);
          setTotal(0);
        }
      } catch (err) {
        setErrors(extractErrorMessages(err));
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [page, filters, pageSize]);

  const onFilterChange = (k, v) => {
    setPage(1);
    setFilters((prev) => ({ ...prev, [k]: v }));
  };

  const clearFilters = () => {
    setFilters({
      doctorId: "",
      patientId: "",
      status: "",
      fromDate: "",
      toDate: "",
    });
    setPage(1);
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Confirm appointment cancellation ? ")) return;
    setErrors([]);
    try {
      await api.delete(`/appointments/${id}`);
      // reload
      setPage(1); // optional
    } catch (err) {
      setErrors(extractErrorMessages(err));
    }
  };

  return (
    <div>
      <h3>Appointments</h3>
      <ErrorsAlert errors={errors} onClose={() => setErrors([])} />

      <div className="card p-3 mb-3">
        <div className="row g-2">
          <div className="col-md-3">
            <select
              className="form-select"
              value={filters.doctorId}
              onChange={(e) => onFilterChange("doctorId", e.target.value)}
            >
              <option value="">All doctors</option>
              {doctors.map((d) => (
                <option key={d.id} value={d.id}>
                  {d.fullName || d.name}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-3">
            <select
              className="form-select"
              value={filters.patientId}
              onChange={(e) => onFilterChange("patientId", e.target.value)}
            >
              <option value="">All patients</option>
              {patients.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.fullName || p.name}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-2">
            <select
              className="form-select"
              value={filters.status}
              onChange={(e) => onFilterChange("status", e.target.value)}
            >
              <option value="">All cases</option>
              <option value="Pending">Pending</option>
              <option value="Confirmed">Confirmed</option>
              <option value="Cancelled">Cancelled</option>
              <option value="Done">Done</option>
            </select>
          </div>

          <div className="col-md-2">
            <input
              className="form-control"
              type="date"
              value={filters.fromDate}
              onChange={(e) => onFilterChange("fromDate", e.target.value)}
            />
          </div>
          <div className="col-md-2">
            <input
              className="form-control"
              type="date"
              value={filters.toDate}
              onChange={(e) => onFilterChange("toDate", e.target.value)}
            />
          </div>
        </div>

        <div className="mt-2">
          <button
            className="btn btn-sm btn-secondary me-2"
            onClick={clearFilters}
          >
            Clear filters{" "}
          </button>
        </div>
      </div>

      {loading ? (
        <div>Loading...</div>
      ) : (
        <>
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
                <tr key={a.id}>
                  <td>{a.patient?.fullName || a.patientName}</td>
                  <td>{a.doctor?.fullName || a.doctorName}</td>
                  <td>
                    {a.date ? new Date(a.date).toLocaleString() : a.dateTime}
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

          <div className="d-flex justify-content-between align-items-center">
            <div>Total results: {total}</div>
            <div>
              <button
                className="btn btn-sm btn-outline-primary me-1"
                disabled={page <= 1}
                onClick={() => setPage((p) => p - 1)}
              >
                previous
              </button>
              <span>
                page {page} / {totalPages}
              </span>
              <button
                className="btn btn-sm btn-outline-primary ms-1"
                disabled={page >= totalPages}
                onClick={() => setPage((p) => p + 1)}
              >
                next
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

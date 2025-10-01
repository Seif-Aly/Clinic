import React, { useEffect, useMemo, useState } from "react";
import api, { extractErrorMessages } from "../services/api";
import ErrorsAlert from "../components/ErrorsAlert";
import DoctorCard from "../components/DoctorCard";
import PublicHeader from "../components/PublicHeader";

export default function Home() {
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(false);

  const [doctors, setDoctors] = useState([]);
  const [hospitals, setHospitals] = useState([]);
  const [clinics, setClinics] = useState([]);

  const [filters, setFilters] = useState({
    q: "",
    specialization: "",
    hospitalId: "",
    clinicId: "",
  });

  const clinicsForHospital = useMemo(() => {
    if (!filters.hospitalId) return clinics;
    return clinics.filter(
      (c) => String(c.hospitalId) === String(filters.hospitalId)
    );
  }, [clinics, filters.hospitalId]);

  const stats = useMemo(
    () => ({
      doctors: doctors.length,
      hospitals: hospitals.length,
      clinics: clinics.length,
    }),
    [doctors, hospitals, clinics]
  );

  const mapDoctors = (d) =>
    d?.doctors ||
    d?.Returns?.doctor ||
    d?.Return?.doctor ||
    (Array.isArray(d) ? d : []) ||
    [];

  const mapHospitals = (d) =>
    d?.returns?.hospitals ||
    d?.Returns?.hospitals ||
    (Array.isArray(d) ? d : []) ||
    [];

  const mapClinics = (d) =>
    d?.returns?.clinic ||
    d?.Returns?.clinic ||
    (Array.isArray(d) ? d : []) ||
    [];

  const loadAll = async () => {
    setErrors([]);
    setLoading(true);
    try {
      const dRes = await api.get("/doctors/GetDoctors", {
        params: {
          NameDoctor: filters.q || undefined,
          Specialization: filters.specialization || undefined,
          HospitalId: filters.hospitalId || undefined,
          ClinicId: filters.clinicId || undefined,
        },
      });
      setDoctors(mapDoctors(dRes.data));

      const hRes = await api.get("/Hospitals/GetHospitals");
      setHospitals(mapHospitals(hRes.data));

      const cRes = await api.get("/Clinics/GetClinics");
      setClinics(mapClinics(cRes.data));
    } catch (err) {
      setErrors(extractErrorMessages(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAll();
  }, []);

  useEffect(() => {
    if (
      filters.clinicId &&
      clinicsForHospital.every((c) => String(c.id) !== String(filters.clinicId))
    ) {
      setFilters((f) => ({ ...f, clinicId: "" }));
    }
  }, [filters.hospitalId, clinicsForHospital.length]);

  const applyFilters = async (e) => {
    e.preventDefault();
    await loadAll();
  };

  const clearFilters = async () => {
    setFilters({ q: "", specialization: "", hospitalId: "", clinicId: "" });
    await loadAll();
  };

  const SkeletonCard = () => (
    <div className="card shadow-sm border-0" style={{ borderRadius: 16 }}>
      <div className="placeholder-glow p-3">
        <div
          className="placeholder col-4 rounded-circle"
          style={{ height: 64 }}
        ></div>
      </div>
      <div className="card-body">
        <h5 className="card-title placeholder-glow">
          <span className="placeholder col-6"></span>
        </h5>
        <p className="card-text placeholder-glow">
          <span className="placeholder col-8"></span>
          <span className="placeholder col-5 ms-2"></span>
        </p>
        <div className="placeholder-glow">
          <span className="placeholder col-4"></span>
        </div>
      </div>
    </div>
  );

  return (
    <>
      {/* Top bar */}
      <PublicHeader />

      {/* Hero */}
      <section
        className="w-100"
        style={{
          background:
            "linear-gradient(135deg, rgba(25,118,210,0.12) 0%, rgba(16,185,129,0.12) 100%)",
          borderBottom: "1px solid rgba(0,0,0,0.06)",
        }}
      >
        <div className="container py-5">
          <div className="row align-items-center g-3">
            <div className="col-lg-7">
              <h1 className="fw-bold mb-2" style={{ letterSpacing: "-0.02em" }}>
                Find & Book the right{" "}
                <span className="text-primary">doctor</span>
              </h1>
              <p className="text-muted mb-4">
                Browse doctors across hospitals & clinics. Filter by
                specialization and doctor name.
              </p>
              <div className="d-flex gap-3 flex-wrap">
                <div className="badge bg-primary-subtle text-primary px-3 py-2">
                  Doctors: <strong className="ms-1">{stats.doctors}</strong>
                </div>
                <div className="badge bg-success-subtle text-success px-3 py-2">
                  Hospitals: <strong className="ms-1">{stats.hospitals}</strong>
                </div>
                <div className="badge bg-warning-subtle text-warning px-3 py-2">
                  Clinics: <strong className="ms-1">{stats.clinics}</strong>
                </div>
              </div>
            </div>
            <div className="col-lg-5">
              {/* “Glass” filter card */}
              <form
                onSubmit={applyFilters}
                className="card border-0 shadow-lg p-3"
                style={{
                  backdropFilter: "blur(8px)",
                  borderRadius: 16,
                }}
              >
                <div className="mb-2">
                  <input
                    className="form-control form-control-lg"
                    placeholder="Search doctor name"
                    value={filters.q}
                    onChange={(e) =>
                      setFilters({ ...filters, q: e.target.value })
                    }
                  />
                </div>
                <div className="mb-2">
                  <input
                    className="form-control"
                    placeholder="Specialization (e.g. Cardiology)"
                    value={filters.specialization}
                    onChange={(e) =>
                      setFilters({ ...filters, specialization: e.target.value })
                    }
                  />
                </div>
                <div className="row g-2">
                  <div className="col-md-6">
                    <select
                      className="form-select"
                      value={filters.hospitalId}
                      onChange={(e) =>
                        setFilters({
                          ...filters,
                          hospitalId: e.target.value,
                          clinicId: "",
                        })
                      }
                    >
                      <option value="">All hospitals</option>
                      {hospitals.map((h) => (
                        <option key={h.id} value={h.id}>
                          {h.name}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="col-md-6">
                    <select
                      className="form-select"
                      value={filters.clinicId}
                      onChange={(e) =>
                        setFilters({ ...filters, clinicId: e.target.value })
                      }
                      disabled={
                        filters.hospitalId && clinicsForHospital.length === 0
                      }
                    >
                      <option value="">
                        {filters.hospitalId
                          ? "Clinics in hospital"
                          : "All clinics"}
                      </option>
                      {(filters.hospitalId ? clinicsForHospital : clinics).map(
                        (c) => (
                          <option key={c.id} value={c.id}>
                            {c.name}
                          </option>
                        )
                      )}
                    </select>
                  </div>
                </div>

                <div className="d-flex gap-2 mt-3">
                  <button
                    className="btn btn-primary w-100"
                    type="submit"
                    disabled={loading}
                  >
                    {loading ? "Searching..." : "Search"}
                  </button>
                  <button
                    className="btn btn-outline-secondary"
                    type="button"
                    onClick={clearFilters}
                    disabled={loading}
                  >
                    Clear
                  </button>
                </div>
              </form>
            </div>
          </div>

          {/* Error banner */}
          <div className="mt-3">
            <ErrorsAlert errors={errors} onClose={() => setErrors([])} />
          </div>
        </div>
      </section>

      {/* Results */}
      <div className="container py-4">
        <div className="d-flex align-items-center justify-content-between mb-3">
          <h5 className="mb-0">
            {loading ? "Searching doctors..." : `Doctors (${doctors.length})`}
          </h5>
          {filters.q ||
          filters.specialization ||
          filters.hospitalId ||
          filters.clinicId ? (
            <small className="text-muted">Active filters applied</small>
          ) : null}
        </div>

        {/* Skeletons */}
        {loading && (
          <div className="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-3">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="col">
                <SkeletonCard />
              </div>
            ))}
          </div>
        )}

        {/* Empty state */}
        {!loading && doctors.length === 0 && (
          <div className="alert alert-info border-0 shadow-sm" role="alert">
            No doctors found. Try changing filters.
          </div>
        )}

        {/* Grid */}
        {!loading && doctors.length > 0 && (
          <div className="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-3">
            {doctors.map((d) => (
              <div className="col" key={d.id}>
                <DoctorCard d={d} clinics={clinics} />
              </div>
            ))}
          </div>
        )}
      </div>
    </>
  );
}

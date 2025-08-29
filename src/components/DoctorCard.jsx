import React, { useMemo } from "react";
import api from "../services/api";

export default function DoctorCard({ d, clinics = [] }) {
  const fileName = d?.image || d?.images || null;

  const base = (api?.defaults?.baseURL || "").replace(/\/+$/, "");
  const primary = fileName ? `${base}/images/Doctor/${fileName}` : null;
  const fallback = fileName ? `${base}/images/Doctors/${fileName}` : null;

  const [imgSrc, setImgSrc] = React.useState(primary);

  React.useEffect(() => {
    setImgSrc(primary);
  }, [primary]);

  const onImgError = () => {
    if (imgSrc === primary && fallback) setImgSrc(fallback);
  };

  const clinicName = d?.clinicName || d?.clinic?.name || "";
  const hospitalName = useMemo(() => {
    if (!clinicName) return d?.hospitalName || "";
    const match = clinics.find(
      (c) => String(c?.name || "").toLowerCase() === clinicName.toLowerCase()
    );
    return match?.hospitalName || d?.hospitalName || "";
  }, [clinics, clinicName, d?.hospitalName]);

  const initials = (d?.fullName || "?")
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((p) => p[0]?.toUpperCase())
    .join("");

  return (
    <div className="card shadow-sm border-0 h-100" style={{ borderRadius: 16 }}>
      <div className="d-flex align-items-center gap-3 p-3">
        {fileName ? (
          <img
            src={imgSrc}
            onError={onImgError}
            alt={d?.fullName || "Doctor"}
            width={64}
            height={64}
            style={{ borderRadius: "50%", objectFit: "cover" }}
          />
        ) : (
          <div
            className="d-flex align-items-center justify-content-center"
            style={{
              width: 64,
              height: 64,
              borderRadius: "50%",
              background:
                "linear-gradient(135deg, rgba(25,118,210,.2), rgba(16,185,129,.2))",
              color: "#1f2937",
              fontWeight: 700,
            }}
            aria-label="No photo"
          >
            {initials}
          </div>
        )}

        <div className="flex-grow-1">
          <h5 className="mb-1">{d?.fullName || "Unknown Doctor"}</h5>
          <div className="text-muted small">
            {d?.specialization || "— specialization —"}
          </div>
        </div>
      </div>

      <div className="px-3 pb-3">
        <div className="d-flex flex-wrap gap-2">
          {clinicName && (
            <span className="badge bg-primary-subtle text-primary">
              {clinicName}
            </span>
          )}
          {hospitalName && (
            <span className="badge bg-success-subtle text-success">
              {hospitalName}
            </span>
          )}
          {d?.email && (
            <span className="badge bg-secondary-subtle text-secondary">
              {d.email}
            </span>
          )}
        </div>
      </div>

      <div className="px-3 pb-3">
        <a href={`/book/${d?.id}`} className="btn btn-outline-primary w-100">
          Book Appointment
        </a>
      </div>
    </div>
  );
}

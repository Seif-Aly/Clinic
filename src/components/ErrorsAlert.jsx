import React from "react";

export default function ErrorsAlert({ errors = [], onClose }) {
  if (!errors || errors.length === 0) return null;

  return (
    <div
      className="alert alert-danger alert-dismissible fade show"
      role="alert"
    >
      <strong>There were some problems:</strong>
      <ul className="mb-0">
        {errors.map((e, i) => (
          <li key={i}>{e}</li>
        ))}
      </ul>
      {onClose && (
        <button
          type="button"
          className="btn-close"
          onClick={onClose}
          aria-label="Close"
        ></button>
      )}
    </div>
  );
}

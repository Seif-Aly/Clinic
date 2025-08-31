import React from "react";

export default function SuccessAlert({ message, onClose }) {
  if (!message) return null;

  return (
    <div className="alert alert-success" role="alert">
      <div className="d-flex justify-content-between align-items-center">
        <strong>{message}</strong>
        <button type="button" className="btn-close" onClick={onClose} />
      </div>
    </div>
  );
}

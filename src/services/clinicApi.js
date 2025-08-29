import api, { extractErrorMessages } from "./api";

export async function getClinics() {
  try {
    const response = await api.get("/Clinics/GetClinics");
    return response.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function getClinic(id) {
  try {
    const response = await api.get(`/Clinics/${id}`);
    return response.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function createClinic(formData) {
  try {
    const response = await api.post("/Clinics/PostClinic", formData);
    return response.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function updateClinic(id, formData) {
  try {
    const response = await api.put(`/Clinics/PutClinic/${id}`, formData);
    return response.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function deleteClinic(id) {
  try {
    const response = await api.delete(`/Clinics/DeleteClinic/${id}`);
    return response.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

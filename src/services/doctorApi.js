import api, { extractErrorMessages } from "./api";

export async function getDoctors() {
  try {
    const res = await api.get("/Doctors/GetDoctors");
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function deleteDoctor(id) {
  try {
    await api.delete(`/doctors/DeleteDoctor/${id}`);
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function getDoctor(id) {
  try {
    const res = await api.get(`/doctors/${id}`);
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function createDoctor(payload) {
  try {
    const res = await api.post("/doctors/PostDoctor", payload);
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function updateDoctor(id, payload) {
  try {
    const res = await api.put(`/doctors/PutDoctor/${id}`, payload);
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

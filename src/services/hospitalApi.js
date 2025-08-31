import api, { extractErrorMessages } from "./api";

export async function getHospitals() {
  try {
    const res = await api.get("/hospitals/GetHospitals");
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function getHospital(id) {
  try {
    const res = await api.get(`/hospitals/GetHospital/${id}`);
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function createHospital(payload) {
  try {
    const res = await api.post("/hospitals/PostHospital", payload);
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function updateHospital(id, payload) {
  try {
    const res = await api.put(`/hospitals/PutHospital/${id}`, payload);
    return res.data;
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

export async function deleteHospital(id) {
  try {
    await api.delete(`/hospitals/${id}`);
  } catch (err) {
    throw extractErrorMessages(err);
  }
}

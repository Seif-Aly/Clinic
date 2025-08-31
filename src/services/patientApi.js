import api from "./api";

export const getPatients = async () => {
  const res = await api.get("/Patients/GetPatients");
  return res.data?.returns?.patients || [];
};

export const getPatientById = async (id) => {
  const res = await api.get(`/Patients/${id}`);
  return res.data;
};

export const createPatient = async (formData) => {
  const res = await api.post("/Patients", formData);
  return res.data;
};

export const updatePatient = async (id, formData) => {
  const res = await api.put(`/Patients/${id}`, formData);
  return res.data;
};

export const deletePatient = async (id) => {
  await api.delete(`/Patients/${id}`);
};

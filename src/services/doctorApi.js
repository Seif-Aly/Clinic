<<<<<<< HEAD
import axios from "axios";

const API_URL = "http://localhost:7005/api/Doctors"; // ÚÏá ÇáÈæÑÊ áæ ãÎÊáİ

// ÌáÈ ŞÇÆãÉ ÇáÃØÈÇÁ ãÚ ÇáİáÊÑÉ æÇáÕİÍÇÊ
export const getDoctors = async (filters = {}, page = 1) => {
    try {
        const response = await axios.get(`${API_URL} / GetDoctors)`, {
            params: { ...filters, page },
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching doctors:", error);
        throw error;
    }
};

// ÌáÈ ØÈíÈ æÇÍÏ
export const getDoctorById = async (id) => {
    try {
        const response = await axios.get(`${ API_URL } / ${ id }`);
        return response.data;
    } catch (error) {
        console.error("Error fetching doctor:", error);
        throw error;
    }
};

// ÅÖÇİÉ ØÈíÈ ÌÏíÏ (ãÚ ÕæÑÉ)
export const createDoctor = async (doctorData) => {
    try {
        const formData = new FormData();
        Object.keys(doctorData).forEach((key) => {
            formData.append(key, doctorData[key]);
        });

        const response = await axios.post(`${ API_URL }/ PostDoctor)`, formData, {
            headers: { "Content-Type": "multipart/form-data" },
        });

        return response.data;
    } catch (error) {
        console.error("Error creating doctor:", error);
        throw error;
    }
};

// ÊÚÏíá ØÈíÈ
export const updateDoctor = async (id, doctorData) => {
    try {
        const formData = new FormData();
        Object.keys(doctorData).forEach((key) => {
            formData.append(key, doctorData[key]);
        });

        const response = await axios.put(`${ API_URL }/ PutDoctor)` / `${ id }`, formData, {
            headers: { "Content-Type": "multipart/form-data" },
        });

        return response.data;
    } catch (error) {
        console.error("Error updating doctor:", error);
        throw error;
    }
};

// ÍĞİ ØÈíÈ
export const deleteDoctor = async (id) => {
    try {
        const response = await axios.delete(`${ API_URL }  DeleteDoctor / ${ id }`);
        return response.data;
    } catch (error) {
        console.error("Error deleting doctor:", error);
        throw error;
    }
};
=======
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
>>>>>>> main

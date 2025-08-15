import axios from "axios";

const API_URL = "http://localhost:7005/api/Doctors"; // ÚÏá ÇáÈæÑÊ áæ ãÎÊáÝ

// ÌáÈ ÞÇÆãÉ ÇáÃØÈÇÁ ãÚ ÇáÝáÊÑÉ æÇáÕÝÍÇÊ
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

// ÅÖÇÝÉ ØÈíÈ ÌÏíÏ (ãÚ ÕæÑÉ)
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

// ÍÐÝ ØÈíÈ
export const deleteDoctor = async (id) => {
    try {
        const response = await axios.delete(`${ API_URL }  DeleteDoctor / ${ id }`);
        return response.data;
    } catch (error) {
        console.error("Error deleting doctor:", error);
        throw error;
    }
};
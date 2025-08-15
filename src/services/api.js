import axios from "axios";

const API_BASE = "https://localhost:7005/api"; 

const api = axios.create({
    baseURL: API_BASE,
    headers: {
        "Content-Type": "application/json",
    },
});

api.interceptors.request.use(config => {
    const token = localStorage.getItem("token");
    if (token) config.headers.Authorization = `Bearer ${ token }`;
    return config;
}, err => Promise.reject(err));

export default api;
import axios from "axios";

const base = (
  process.env.REACT_APP_API_BASE || "http://localhost:5055/api"
).replace(/\/$/, "");

const api = axios.create({
  baseURL: base,
  timeout: 20000,
});

api.interceptors.request.use((config) => {
  const t = localStorage.getItem("token");
  if (t) config.headers.Authorization = `Bearer ${t}`;
  return config;
});

export function extractErrorMessages(error) {
  const msgs = [];

  if (!error) return ["Unknown error"];

  if (error.response) {
    const { status, data } = error.response;

    if (typeof data === "string") msgs.push(data);

    if (data?.message) msgs.push(data.message);

    if (Array.isArray(data?.errors)) {
      for (const e of data.errors) {
        if (typeof e === "string") msgs.push(e);
        else if (e?.description) msgs.push(e.description);
      }
    }

    const bag =
      data?.errors && typeof data.errors === "object"
        ? data.errors
        : typeof data === "object"
        ? data
        : null;
    if (bag && typeof bag === "object") {
      for (const v of Object.values(bag)) {
        if (Array.isArray(v))
          v.forEach((m) => typeof m === "string" && msgs.push(m));
        else if (typeof v === "string") msgs.push(v);
      }
    }

    if (status === 401)
      msgs.push("Your session has expired or you are not logged in.");
    if (status === 403)
      msgs.push("You do not have permission to perform this action.");
    if (status >= 500) msgs.push("Server error. Please try again later.");
  }
  // No response (network)
  else if (error.request) {
    msgs.push("Network error: unable to reach the server.");
  }
  // Request setup error
  else if (error.message) {
    msgs.push(error.message);
  }

  return Array.from(
    new Set(msgs.map((m) => (m || "").toString().trim()).filter(Boolean))
  );
}

export default api;

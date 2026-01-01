import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5267/gateway", // API Gateway only
});

api.interceptors.request.use((config) => {
  const token = sessionStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;

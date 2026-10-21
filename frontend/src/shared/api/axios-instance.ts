import axios from "axios";

export const apiClient = axios.create({
  baseURL: "http://localhost:5047/api",
  headers: { "Content-Type": "application/json; charset=utf-8" },
});

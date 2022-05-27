import axios from "axios";

const API_URL = "admin";

const login = async (username: string, password: string) => {
  return axios
    .post(API_URL + "/login", {
      username,
      password,
    })
};

const logout = () => {
  localStorage.removeItem("username");
  localStorage.removeItem("token");
};

const getCurrentUser = () => {
  const usernameJson = localStorage.getItem("username");
  if (usernameJson == null) {
    return null;
  }
  return JSON.parse(usernameJson);
};

const getCurrentToken = () => {
  const tokenJson = localStorage.getItem("token");
  if (tokenJson == null) {
    return null;
  }
  return JSON.parse(tokenJson);
};

export const authService = {
  login,
  logout,
  getCurrentUser,
  getCurrentToken
};

export default authService;
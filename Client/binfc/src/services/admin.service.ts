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
  localStorage.removeItem("selectedItem")
  localStorage.removeItem("selectedRouted")
};

const changePassword = async (data: any) => {
  return await axios
    .post(API_URL + "/changepassword", data)
    .then((response) => {
      return response
    })
};

const updateLanguage = async (username: string | null, language: string) => {
  return await axios
    .put(
      API_URL + `/updatelanguage`, null,
      {
        params: {
          username, language
        }
      })
};

const getLanguage = async (username: string) => {
  return await axios
    .get(API_URL + "/getlanguage", {
      params: { username: username }
    })
    .then((response) => {
      return response
    })
};

export const adminService = {
  login,
  logout,
  changePassword,
  updateLanguage,
  getLanguage
};

export default adminService;
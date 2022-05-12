import axios from "axios";

const API_URL = "/admin";

const login = (username: string, password: string) => {
  return axios
    .post(API_URL + "/Login", {
      username,
      password,
    })
    .then((response) => {
      if (response.data.token) {
        const username = JSON.stringify(response.data.username)
        const token = JSON.stringify(response.data.token)
        localStorage.setItem("username", username);
        localStorage.setItem("token", token);

        axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;
      }

      return response.data;
    });
};

const logout = () => {
  localStorage.removeItem("username");
  localStorage.removeItem("token");
};

const getCurrentUser = () => {
  const usernameJson = localStorage.getItem("username");
  if (usernameJson == null){
    return null;
  }
  return JSON.parse(usernameJson);
};

const getCurrentToken = () => {
  const tokenJson = localStorage.getItem("token");
  if (tokenJson == null){
    return null;
  }
  return JSON.parse(tokenJson);
};

const authService = {
  login,
  logout,
  getCurrentUser,
  getCurrentToken
};

export default authService;
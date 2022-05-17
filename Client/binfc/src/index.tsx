import axios from "axios";
import { ToastContainer, toast } from 'material-react-toastify';
import 'material-react-toastify/dist/ReactToastify.css';
import React from "react";
import ReactDOM from "react-dom";
import { HashRouter, Route, Routes, Navigate } from "react-router-dom";
import AuthLayout from "./layouts/auth";
import AdminLayout from "./layouts/admin";

axios.defaults.baseURL = process.env.REACT_APP_BASE_URL;
axios.defaults.headers.post['Content-Type'] = 'application/json';

axios.interceptors.request.use(function (config) {
  if (config.headers != null){
    config.headers.Authorization = 'Bearer ' + localStorage.getItem('token');
  }
  return config;
}, function (error) {
  return Promise.reject(error);
});

axios.interceptors.response.use(function (response) {
  return response;
}, function (error) {
  toast.error(error.response.data);
  return Promise.reject(error);
});

ReactDOM.render(
    <React.StrictMode>
      <HashRouter>
        <Routes>
          <Route path={`/auth`} element={<AuthLayout/>} />
          <Route path={`/admin`} element={<AdminLayout/>} />
          <Route path="/" element={<Navigate replace to="/admin" />} />
        </Routes>
      </HashRouter>
    </React.StrictMode>,
  document.getElementById("root")
);

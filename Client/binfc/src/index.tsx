import axios from "axios";
import 'material-react-toastify/dist/ReactToastify.css';
import React from "react";
import ReactDOM from 'react-dom/client';
import { HashRouter, Route, Routes, Navigate } from "react-router-dom";
import LoginPage from "./components/Login";
import "antd/dist/antd.variable.min.css";
import { Provider } from "react-redux";
import { store } from "./store/index";
import { I18nextProvider } from "react-i18next";
import i18n from "./i18n"
import DashBoard from "./components/DashBoard";
import PaymentsSettings from "./components/PaymentsSettings";
axios.defaults.baseURL = process.env.REACT_APP_BASE_URL;
axios.defaults.headers.post['Content-Type'] = 'application/json';
axios.interceptors.request.use(function (config) {
  if (config.headers != null) {
    config.headers.Authorization = 'Bearer ' + localStorage.getItem('token');
  }
  return config;
}, function (error) {
  return Promise.reject(error);
});

axios.interceptors.response.use(function (response) {
  return response;
}, function (error) {
  return Promise.reject(error);
});

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);


root.render(
  <React.StrictMode>
    <Provider store={store}>
        <HashRouter>
          <I18nextProvider i18n={i18n}>
            <Routes>
              <Route path="/" element={<Navigate replace to="/login" />} />
              <Route path={`/dashboard/*`} element={<DashBoard/>} />
              <Route path="/login" element={<LoginPage />} />
            </Routes>
          </I18nextProvider>
        </HashRouter>
    </Provider>
  </React.StrictMode>
);

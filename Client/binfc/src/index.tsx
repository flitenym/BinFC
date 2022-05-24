import axios from "axios";
// import 'material-react-toastify/dist/ReactToastify.css';
import React from "react";
import { ThemeSwitcherProvider } from "react-css-theme-switcher";
import ReactDOM from 'react-dom/client';
import { HashRouter } from "react-router-dom";
import "antd/dist/antd.variable.min.css";
import { Provider } from "react-redux";
import { store } from "./store/index";
import { I18nextProvider } from "react-i18next";
import i18n from "./i18n"
import App from "./App";
import "./themes/dark-theme.less"
import "./themes/dark-theme.less"
import "./styles.scss"
const lightThemeIsSelected = localStorage.getItem("theme")
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

const themes = {
  dark: `${process.env.PUBLIC_URL}/dark-theme.css`,
  light: `${process.env.PUBLIC_URL}/light-theme.css`,
};

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <React.StrictMode>
    <Provider store={store}>
      <HashRouter>
        <I18nextProvider i18n={i18n}>
          <ThemeSwitcherProvider themeMap={themes} defaultTheme={lightThemeIsSelected ? lightThemeIsSelected : "dark"}>
            <App />
          </ThemeSwitcherProvider>
        </I18nextProvider>
      </HashRouter>
    </Provider>
  </React.StrictMode>
);

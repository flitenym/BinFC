import { FunctionComponent, useEffect } from 'react';
import { Form, Input, Button, Layout, } from "antd";
import { useDispatch, } from "react-redux";
import "./LoginStyles.scss";
import adminService from '../../services/admin.service';
import { useNavigate } from "react-router-dom";
import { logInSuccess } from '../../store/actions';
import axios from "axios";
import { useTranslation } from "react-i18next";
import i18n from '../../i18n';

interface IProps { }

const LoginPage: FunctionComponent<IProps> = () => {
  const { Content } = Layout;
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { t } = useTranslation("authentication, common");
  const token = localStorage.getItem("token");
  const ruIsSelected = localStorage.getItem("i18nextLng") === "ru"

  useEffect(() => {
    i18n.changeLanguage(ruIsSelected ? "ru" : 'en')
  }, [ruIsSelected])

  const onFinish = (values: any): any => {
    adminService.login(values?.username, values?.password)
      .then((response) => {
        if (response.status !== 200) {
          return
        }
        if (response.data) {
          const password = values?.password;
          const { token, username } = response?.data
          dispatch(logInSuccess({ username, token }));
          localStorage.setItem("username", username);
          localStorage.setItem("selectedItem", "1");
          localStorage.setItem("selectedRouted", "/dashboard/Settings");
          localStorage.setItem("token", token);
          axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;
          navigate("/dashboard");
        }
        return response.data;
      });
  };

  useEffect(() => {
    if (token) {
      navigate("/dashboard");
    }
  }, [navigate, token])

  return (
    <Content
      style={{ minHeight: "100vh", display: "flex" }}
    >
      <div className="login-form-wrapper">
        <Form
          labelCol={{ span: 10 }}
          wrapperCol={{ span: 26 }}
          autoComplete="off"
          name="normal_login"
          onFinish={onFinish}
        >
          <span className="form-description">{t("authentication:login")}</span>
          <Form.Item
            name="username"
            rules={[
              {
                required: true,
                message:`${t("common:ThisFieldRequired")}`,
              },
            ]}
          >
            <Input />
          </Form.Item>
          <span className="form-description">{t("authentication:password")}</span>
          <Form.Item
            name="password"
            rules={[
              {
                required: true,
                message: `${t("common:ThisFieldRequired")}`,
                whitespace: true,
              },
            ]}
          >
            <Input.Password type="password" />
          </Form.Item>
          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              className="login-form-button"
            >
              Войти
            </Button>
          </Form.Item>
        </Form>
      </div>
    </Content>
  );
};

export default LoginPage;



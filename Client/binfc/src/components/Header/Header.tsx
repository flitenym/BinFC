import {
    Button,
    Dropdown,
    Form,
    Input,
    Menu,
    Modal,
    PageHeader,
    Space,
    Typography
} from 'antd';
import { languageChange, modeChange } from '../../store/actions';
import { FunctionComponent, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector, } from "react-redux";
import "./Header.scss"
import adminService from '../../services/admin.service';
import { useThemeSwitcher } from "react-css-theme-switcher";
import IconSun from './IconSun';
import IconMoon from './IconMoon';
import { Link, useNavigate } from 'react-router-dom';
import React from 'react';
import 'material-react-toastify/dist/ReactToastify.css';
import { t } from 'i18next';
import { useForm } from 'antd/lib/form/Form';
import { toast } from 'material-react-toastify';

const Header: FunctionComponent = () => {
    const navigate = useNavigate();
    const [language, setLanguage] = useState<string | null>();
    const [mode, setMode] = useState<string | null>();
    const { i18n } = useTranslation("authentication");
    const { switcher, themes } = useThemeSwitcher();
    const [modalVisible, setModalVisible] = useState<boolean>(false);
    const username = useSelector((state: any) => state?.authState?.username)
    const ruIsSelected = localStorage.getItem("i18nextLng") === "ru"
    const lightThemeIsSelected = localStorage.getItem("theme") === "Light"
    const user = localStorage.getItem("username")
    const dispatch = useDispatch();

    const [form] = useForm();

    useEffect(() => {
        adminService.getLanguage(user ?? "").then((response) => {
            let chooseLanguage = localStorage.getItem("i18nextLng") ?? "ru"
            if (response.status === 200) {
                if (chooseLanguage !== response.data) {
                    chooseLanguage = response.data
                    setLanguageFunc(chooseLanguage, true)
                } else {
                    setLanguageFunc(chooseLanguage, false)
                }
            }
        })
    }, [])

    const changeMode = (mode: any) => {
        if (mode === 'Light' || lightThemeIsSelected) {
            setMode("Dark")
            dispatch(modeChange({ mode: "Dark" }))
            switcher({ theme: themes.dark });
            localStorage.setItem("theme", "Dark")
        } else {
            setMode("Light")
            dispatch(modeChange({ mode: "Light" }))
            switcher({ theme: themes.light });
            localStorage.setItem("theme", "Light")
        }
    }

    const changeLanguage = (lang: any) => {
        if (lang === language) {
            return
        }
        if (lang === 'ru') {
            setLanguageFunc("ru", true, lang)
        } else {
            setLanguageFunc("en", true, lang)
        }
    }

    const setLanguageFunc = (localeToStorage: string, flag: boolean, language?: any) => {
        if (flag) {
            adminService.updateLanguage(
                localStorage.getItem("username"),
                localeToStorage
            )
        }
        localStorage.setItem("i18nextLng", localeToStorage)
        setLanguage(localeToStorage);
        dispatch(languageChange({ locale: language ? language : localeToStorage }))
        i18n.changeLanguage(localeToStorage)
    }

    const menu = (
        <Menu
            items={[
                {
                    label: (
                        <Link key={21} onClick={() => {
                            adminService?.logout()
                        }} to={"/login"}>
                            {`${t("common:Logout")}`}
                        </Link>
                    ),
                    key: 20
                },
                {
                    label: (
                        <Typography.Text key={22} onClick={() => setModalVisible(true)}>
                            {`${t("common:ChangePassword")}`}
                        </Typography.Text>
                    ),
                    key: 23
                },
            ]}
        />
    );

    const languageMenu = (
        <Menu
            items={[
                {
                    label: (
                        <Typography.Text key={48} onClick={() =>
                            changeLanguage('ru')}>
                            {`${t("common:Russian")}`}
                        </Typography.Text>
                    ),
                    key: 22
                },
                {
                    label: (
                        <Typography.Text key={49} onClick={() =>
                            changeLanguage('en')}>
                            {`${t("common:English")}`}
                        </Typography.Text>
                    ),
                    key: 21
                }
            ]}
        />
    );

    const changePass = (values: any) => {
        if (values.newPassword !== values.repeatPassword) {
            toast.error(`${t("common:ErrorNewConfirmPass")}`);
            return
        } else {
            adminService.changePassword({
                username: localStorage.getItem("username"),
                newpassword: values.newPassword,
                oldpassword: values.password
            }).then((response) => {
                if (response.status !== 200) {
                    return
                } else {
                    toast.success(`${t("common:SuccessChangePass")}`);
                    setModalVisible(false)
                    setTimeout(() => {
                        adminService.logout()
                        navigate("/login");
                    }, 2000);
                }
            })
        }
    }

    return (
        <PageHeader
            key={25}
            ghost={false}
            title={
                <Typography.Text key={16}>
                    {<>
                        <b style={{ fontSize: "24px" }}>
                            FAT
                        </b>
                        <span
                            style={{
                                marginLeft: "6px",
                                fontSize: "24px"
                            }}>
                            CAMEL
                        </span>
                    </>}
                </Typography.Text>}
            extra={[
                <React.Fragment key={27}>
                    <Dropdown
                        key={10}
                        placement="bottomLeft"
                        arrow
                        overlay={languageMenu}>
                        <a style={{ fontSize: "16px" }} key={47} onClick={e => e.preventDefault()}>
                            {language === "ru" ? `${t("common:Russian")}` : `${t("common:English")}`}
                        </a>
                    </Dropdown>
                    <Button
                        className='btn'
                        key={11}
                        type='link'
                        style={{
                            border: "none",
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center"
                        }} onClick={() =>
                            changeMode(mode)
                        } icon={lightThemeIsSelected
                            ? <IconSun key={12} />
                            : <IconMoon key={13} />}>
                    </Button>
                    <Dropdown
                        key={14}
                        placement="bottom"
                        arrow
                        overlay={menu}>
                        <a style={{ fontSize: "16px" }} key={15} onClick={e => e.preventDefault()}>
                            {username}
                        </a>
                    </Dropdown>
                    <Modal
                        title={`${t("common:ChangePassword")}`}
                        centered
                        forceRender
                        visible={modalVisible}
                        footer={null}
                        onCancel={() => setModalVisible(false)}
                    >
                        <Form
                            form={form}
                            onFinish={(data) => changePass(data)}
                        >
                            <span key={9} className="form-description">{`${t("common:OldPassword")}`}</span>
                            <Form.Item
                                name="password"
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                            >
                                <Input.Password
                                    type="password"
                                />
                            </Form.Item>
                            <span key={23} className="form-description">{`${t("common:NewPassword")}`}</span>
                            <Form.Item
                                name="newPassword"
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                            >
                                <Input.Password
                                    type="password"
                                />
                            </Form.Item>
                            <span key={25} className="form-description">{`${t("common:ConfirmPassword")}`}</span>
                            <Form.Item
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                name="repeatPassword"
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                            >
                                <Input.Password
                                    type="password"
                                />
                            </Form.Item>
                            <Space style={{ marginTop: "16px" }}>
                                <Button
                                    type="primary"
                                    htmlType="submit"
                                >
                                    {`${t("common:ButtonSave")}`}
                                </Button>
                                <Button
                                    type="default"
                                    onClick={() => setModalVisible(false)}
                                >
                                    {`${t("common:ButtonClose")}`}
                                </Button>
                            </Space>
                        </Form>
                    </Modal>
                </React.Fragment>
            ]}
        />
    )
}

export default Header

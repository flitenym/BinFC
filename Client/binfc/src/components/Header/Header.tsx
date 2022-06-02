import {
    Button,
    Dropdown,
    Menu,
    PageHeader,
    Typography
} from 'antd';
import { languageChange, modeChange } from '../../store/actions';
import { FunctionComponent, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector, } from "react-redux";
import "./Header.scss"
import authService from '../../services/auth.service';
import { useThemeSwitcher } from "react-css-theme-switcher";
import IconSun from './IconSun';
import IconMoon from './IconMoon';
import { Link } from 'react-router-dom';
import React from 'react';
import { t } from 'i18next';

const Header: FunctionComponent = () => {
    const [language, setLanguage] = useState<string | null>();
    const [mode, setMode] = useState<string | null>();
    const { i18n } = useTranslation("authentication");
    const { switcher, themes } = useThemeSwitcher();
    const username = useSelector((state: any) => state?.authState?.username)
    const ruIsSelected = localStorage.getItem("i18nextLng") === "ru"
    const lightThemeIsSelected = localStorage.getItem("theme") === "Light"
    const dispatch = useDispatch();

    useEffect(() => {
        const chooseLanguage = localStorage.getItem("i18nextLng")
        if (chooseLanguage) {
            setLanguageFunc(chooseLanguage, false)
        } else {
            setLanguageFunc("ru", true)
        }
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

    const changeLanguage = (language: any) => {
        if (language === 'ru') {
            setLanguageFunc("en", true, language)
        } else {
            setLanguageFunc("ru", true, language)
        }
    }

    const setLanguageFunc = (localeToStorage: string, flag: boolean, language?: any) => {
        if (flag) {
            localStorage.setItem("i18nextLng", localeToStorage)
        }
        setLanguage(localeToStorage);
        dispatch(languageChange({ locale: language ? language : localeToStorage }))
        i18n.changeLanguage(localeToStorage)
    }

    const menu = (
        <Menu
            items={[
                {
                    label: (
                        <Link key={21} onClick={() => authService?.logout()} to={"/login"}>
                            Logout
                        </Link>
                    ),
                    key: 20
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
                            changeLanguage('en')}>
                            {`${t("common:Russian")}`}
                        </Typography.Text>
                    ),
                    key: 22
                },
                {
                    label: (
                        <Typography.Text key={49} onClick={() =>
                            changeLanguage('ru')}>
                            {`${t("common:English")}`}
                        </Typography.Text>
                    ),
                    key: 21
                }
            ]}
        />
    );

    return (
        <PageHeader
            key={25}
            ghost={false}
            title={
                <Typography.Text key={16}>
                    {<>
                        <b style={{ fontSize: "24px"}}>
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
                        <a style={{ fontSize: "16px"}} key={47} onClick={e => e.preventDefault()}>
                            {ruIsSelected ? `${t("common:Russian")}` : `${t("common:English")}`}
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
                        placement="bottomLeft"
                        arrow
                        overlay={menu}>
                        <a style={{ fontSize: "16px"}} key={15} onClick={e => e.preventDefault()}>
                            {username}
                        </a>
                    </Dropdown>
                </React.Fragment>
            ]}
        />
    )
}

export default Header

import { Button, Dropdown, Menu, PageHeader, Space, Switch } from 'antd';
import { languageChange } from '../../store/actions';
import { FunctionComponent, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector, } from "react-redux";
import { DownOutlined } from '@ant-design/icons';
import "./Header.scss"
import authService from '../../services/auth.service';

const Header: FunctionComponent = () => {
    const [language, setLanguage] = useState<string | null>();
    const { i18n } = useTranslation("authentication");
    const username = useSelector((state: any) => state?.authState?.username)
    const ru = localStorage.getItem("i18nextLng") === "ru"
    const dispatch = useDispatch();

    useEffect(() => {
        const chooseLanguage = localStorage.getItem("i18nextLng")
        if (chooseLanguage) {
            setLanguageFunc(chooseLanguage, false)
        } else {
            setLanguageFunc("ru", true)
        }
    }, [])

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
                        <a onClick={() => authService?.logout()} href="/login">
                            Logout
                        </a>
                    ),
                    key: '0',
                },
            ]}
        />
    );

    return (
        <PageHeader
            ghost={false}
            style={{
                background: "#001529"
            }}
            className="site-page-header"
            title="FAT CAMEL"
            extra={[
                <Switch
                    onClick={() =>
                        changeLanguage(language)
                    }
                    checkedChildren="RU"
                    unCheckedChildren="EN"
                    defaultChecked={ru ? true : false}
                    key="1"
                />,
                <Dropdown key="2" overlay={menu}>
                    <a onClick={e => e.preventDefault()}>
                        <Space>
                            {username}
                            <DownOutlined />
                        </Space>
                    </a>
                </Dropdown>
            ]}
        />
    )
}

export default Header

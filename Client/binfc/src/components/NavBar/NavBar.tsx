import { FunctionComponent, useEffect, useState } from "react"
import { useNavigate } from "react-router-dom";
import type { MenuProps } from 'antd';
import { Menu, Button } from 'antd';
import {
    UsergroupAddOutlined,
    SettingOutlined,
    DollarCircleOutlined,
    HistoryOutlined,
    RadiusSettingOutlined
} from '@ant-design/icons';
import { useTranslation } from "react-i18next";
import "./Navbar.scss"
import PaymentsSettings from "../PaymentsSettings";
import { Content } from "antd/lib/layout/layout";
import Users from "../Users";
import Payout from "../Payout";
import PaymentHistory from "../PaymentHistory";
import BinanceSettings from "../BinanceSettings";

type MenuItem = Required<MenuProps>['items'][number];

const NavBar: FunctionComponent = ({ children }: any) => {
    const { t } = useTranslation("common");
    const [routeSelected, setRouteSelected] = useState<string>('/dashboard/PaymentsSettings')
    const navigate = useNavigate();
    const selectedItem = localStorage.getItem("selectedItem")
    useEffect(() => {
        { returnComponent(routeSelected) }
    }, [])

    function getItem(
        route?: string,
        label?: React.ReactNode,
        key?: React.Key,
        icon?: React.ReactNode,
        children?: MenuItem[],
        type?: 'group',
    ): MenuItem {
        return {
            route,
            label,
            key,
            icon,
            children,
            type,
        } as MenuItem;
    }

    const items: MenuItem[] = [
        getItem('/dashboard/PaymentsSettings', t("common:PayoutSettings"), '1', <SettingOutlined />),
        getItem('/dashboard/Users', t("common:Users"), '2', <UsergroupAddOutlined />),
        getItem('/dashboard/Payout', t("common:Payout"), '3', <DollarCircleOutlined />),
        getItem('/dashboard/PaymentHistory', t("common:PaymentHistory"), '4', <HistoryOutlined />),
        getItem('/dashboard/BinanceSettings', t("common:BinanceSettings"), '5', <RadiusSettingOutlined />),
    ];

    const onClick = (e: any) => {
        items.map((item: any) => {
            if (item?.key === e.key) {
                setRouteSelected(item?.route)
                localStorage.setItem("selectedItem", item?.route)
                return navigate(item?.route)
            }
        });
    };

    const returnComponent = (route: string) => {
        switch (route) {
            case "/dashboard/PaymentsSettings":
                return <PaymentsSettings />
            case "/dashboard/Users":
                return <Users />
            case "/dashboard/Payout":
                return <Payout />
            case "/dashboard/PaymentHistory":
                return <PaymentHistory />
            case "/dashboard/BinanceSettings":
                return <BinanceSettings />
            default:
                return <PaymentsSettings />
        }
    }


    return (
        <div style={{ display: "flex" }}>
            <Menu
                mode="inline"
                theme="dark"
                style={{ width: "256px" }}
                items={items}
                onClick={(e) => onClick(e)}
            >
            </Menu>
            <Content
                style={{
                    padding: 24,
                    margin: 0,
                }}
            >
                {returnComponent(routeSelected)}
            </Content>
        </div>
    );

}

export default NavBar
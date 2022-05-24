import { FunctionComponent, useEffect } from "react"
import { Outlet, useNavigate } from "react-router-dom";
import type { MenuProps } from 'antd';
import { Menu } from 'antd';
import {
    UsergroupAddOutlined,
    SettingOutlined,
    DollarCircleOutlined,
    HistoryOutlined,
    RadiusSettingOutlined
} from '@ant-design/icons';
import { useTranslation } from "react-i18next";
import "./Navbar.scss"
import { Content } from "antd/lib/layout/layout";

type MenuItem = Required<MenuProps>['items'][number];

const NavBar: FunctionComponent = () => {
    const { t } = useTranslation("common");
    const navigate = useNavigate();
    const selectedItem = localStorage.getItem("selectedItem")
    const selectedRouted = localStorage.getItem("selectedRouted")

    useEffect(() => {
        navigate(selectedRouted ? selectedRouted : "/dashboard/PaymentsSettings")
    }, [navigate, selectedRouted])

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
                localStorage.setItem("selectedItem", item?.key)
                localStorage.setItem("selectedRouted", item?.route)
                return navigate(item?.route)
            }
            return navigate(item?.route)
        });
    };

    return (
        <div style={{ display: "flex" }}>
            <Menu
                defaultActiveFirst
                defaultSelectedKeys={[selectedItem ? selectedItem : "1"]}
                mode="inline"
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
                <div style={{ maxWidth: "1600px" }}>
                    <Outlet />
                </div>
            </Content>
        </div>
    );

}

export default NavBar
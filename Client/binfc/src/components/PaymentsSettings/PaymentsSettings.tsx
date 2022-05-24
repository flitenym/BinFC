import { Button, Checkbox, Input, Radio, Space, Typography } from "antd";
import { Content } from "antd/lib/layout/layout";
import { FunctionComponent, useEffect, useState } from "react"
import { useTranslation } from "react-i18next";
import settingsService from "../../services/settings.servise";

interface IData {
    id?: number;
    key: string;
    value: string | null;
}

interface ICronExpression {
    key: string;
    value: string | null;
}

interface ISellCurrency {
    key: string;
    value: string | null;
}

interface IsNotification {
    key: string;
    value: boolean;
}

const PaymentsSettings: FunctionComponent = () => {
    const { t } = useTranslation("common");
    const [inputValue, setInputValue] = useState('');
    const [cronExpression, setCronExpression] = useState<ICronExpression>({
        key: '',
        value: ''
    });
    const [sellCurrency, setSellCurrency] = useState<ISellCurrency>({
        key: '',
        value: ''
    });
    const [isNotification, setIsNotification] = useState<IsNotification>({
        key: '',
        value: false
    });

    useEffect(() => {
        settingsService.getSettings().then((data) => {
            data.filter((item: IData) => {
                if (item?.key === "CronExpression") {
                    return setCronExpression({
                        key: item?.key,
                        value: item?.value,
                    })
                }
            })
            data.filter((item: IData) => {
                if (item?.key === "SellCurrency") {
                    return setSellCurrency({
                        key: item?.key,
                        value: item?.value,
                    })
                }
            })
            data.filter((item: IData) => {
                if (item?.key === "IsNotification") {
                    return setIsNotification({
                        key: item?.key,
                        value: item?.value === "True",
                    })
                }
            })
        })
    }, [])

    const onChangeValue = (value: string) => {
        setInputValue(value);
    }

    const submitValue = (value?: string) => {

    }

    const saveSettings = () => {
        let formData = new FormData();
        formData.append('settings[0].key', 'CronExpression');
        formData.append('settings[0].value', inputValue);
        formData.append('settings[1].key', 'SellCurrency');
        formData.append('settings[1].value', `${sellCurrency?.value}`);
        formData.append('settings[2].key', 'IsNotification');
        formData.append('settings[2].value', `${isNotification?.value ? "True" : "False"}`);
        return settingsService.saveSettings(formData)
    };

    return (
        <Content style={{ display: "flex", flexDirection: "column" }}>
            <Typography.Text>{t("common:PaymentDateSetting")}</Typography.Text>
            <div style={{ marginTop: "25px" }}>
                <Input
                    defaultValue={cronExpression?.value ?? ''}
                    key={cronExpression?.key}
                    onChange={(e) => onChangeValue(e?.target?.value)}
                    style={{ maxWidth: "225px" }}
                />
                <Button
                    type="primary"
                    onClick={() => submitValue}
                    style={{ textAlign: "left", marginLeft: "30px" }}>
                    {t("common:CheckDates")}
                </Button>
            </div>
            <div style={{
                marginTop: "32px",
                display: "flex",
                flexDirection: "column"
            }}>
                {t("common:CommissionConversion")}
                <Radio.Group
                    key={sellCurrency.key}
                    onChange={(event) => {
                        setSellCurrency(
                            {
                                value: event.target.value,
                                key: event.target.value,
                            }
                        )
                    }}
                    defaultValue={sellCurrency.value}
                    style={{ marginTop: "16px" }}>
                    <Space direction="vertical">
                        <Radio value={"USDT"}>USDT</Radio>
                        <Radio value={'BUSD'}>BUSD</Radio>
                    </Space>
                </Radio.Group>
            </div>
            <div style={{
                marginTop: "32px",
                display: "flex",
                flexDirection: "column"
            }}>
                {t("common:Notifications")}
                <Checkbox
                    key={isNotification.key}
                    defaultChecked={isNotification.value}
                    style={{ marginTop: "16px" }}
                    onChange={(event) => {
                        setIsNotification(
                            {
                                value: event.target.checked,
                                key: event.target.value,
                            }
                        )
                    }}>
                    {t("common:EnableNofications")}
                </Checkbox>
            </div>
            <div style={{ marginTop: "25px" }}>
                <Button
                    type="primary"
                    onClick={() => saveSettings()}
                    style={{ textAlign: "left" }}>
                    {t("common:SaveSettings")}
                </Button>
            </div>
        </Content >
    );

}

export default PaymentsSettings;
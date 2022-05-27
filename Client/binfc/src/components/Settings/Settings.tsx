import { Button, Checkbox, Input, List, Modal, Radio, Space, Typography } from "antd";
import { Content } from "antd/lib/layout/layout";
import { FunctionComponent, useEffect, useState } from "react"
import { useTranslation } from "react-i18next";
import cronService from "../../services/cronjob.service";
import settingsService from "../../services/settings.servise";

interface IData {
    id?: number;
    key: string;
    value: string | null;
}

interface ICronExpression {
    key: string;
    value: string | null;
    isChanged: boolean;
}

interface ISellCurrency {
    key: string;
    value: string | null;
    isChanged: boolean;
}

interface IsNotification {
    key: string;
    value: boolean;
    isChanged: boolean;
}

const Settings: FunctionComponent = () => {
    const { t } = useTranslation("common");
    const [inputValue, setInputValue] = useState('');
    const [modalDatesVisible, setModalDatesVisible] = useState(false);
    const [modalData, setModalData] = useState<any>([]);
    const [cronExpression, setCronExpression] = useState<ICronExpression>({
        key: '',
        value: '',
        isChanged: false,
    });
    const [sellCurrency, setSellCurrency] = useState<ISellCurrency>({
        key: '',
        value: '',
        isChanged: false,
    });
    const [isNotification, setIsNotification] = useState<IsNotification>({
        key: '',
        value: false,
        isChanged: false,
    });

    useEffect(() => {
        settingsService.getSettings().then((data) => {
            data.length && data?.filter((item: IData) => {
                if (item?.key === "CronExpression") {
                    setInputValue(item?.value ?? '')
                    return setCronExpression({
                        key: item?.key,
                        value: item?.value,
                        isChanged: false,
                    })
                }
            })
            data.length && data?.filter((item: IData) => {
                if (item?.key === "SellCurrency") {
                    return setSellCurrency({
                        key: item?.key,
                        value: item?.value,
                        isChanged: false,
                    })
                }
            })
            data.length && data?.filter((item: IData) => {
                if (item?.key === "IsNotification") {
                    return setIsNotification({
                        key: item?.key,
                        value: item?.value === "True",
                        isChanged: false,
                    })
                }
            })
        })
    }, [])

    const onChangeValue = (value: string) => {
        setInputValue(value);
        cronExpression.isChanged = true;
        setCronExpression(cronExpression)
    }

    const checkDates = (cronExpression?: string) => {
        cronService.next(cronExpression ?? '').then((response) => {
            if (response.status !== 200) {
                return Promise.reject(response);
            } else {
                setModalData(response.data)
                setModalDatesVisible(true)
            }
        })
    }

    const saveSettings = () => {
        cronService.check(inputValue).then((response) => {
            if (response !== 200) {

            } else {
                let formData = new FormData();
                let i = 0;
                if (cronExpression.isChanged) {
                    formData.append(`settings[${i}].key`, 'CronExpression');
                    formData.append(`settings[${i}].value`, inputValue);
                    i++;
                }
                if (sellCurrency.isChanged) {
                    formData.append(`settings[${i}].key`, 'SellCurrency');
                    formData.append(`settings[${i}].value`, `${sellCurrency?.value}`);
                    i++;
                }
                if (isNotification.isChanged) {
                    formData.append(`settings[${i}].key`, 'IsNotification');
                    formData.append(`settings[${i}].value`, `${isNotification?.value ? "True" : "False"}`);
                    i++;
                }
                if (cronExpression.isChanged || sellCurrency.isChanged || isNotification.isChanged) {
                    cronExpression.isChanged = sellCurrency.isChanged = isNotification.isChanged = false;
                    return settingsService.saveSettings(formData)
                }
            }
        })

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
                    onClick={() => checkDates(inputValue)}
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
                                isChanged: true,
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
                                isChanged: true,
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
            <Modal
                title={t("common:Dates")}
                centered
                visible={modalDatesVisible}
                footer={null}
                onCancel={() => setModalDatesVisible(false)}
            >
                <List
                    size="small"
                    bordered
                    split
                    dataSource={modalData}
                    renderItem={(item: any, index: number) =>
                        <List.Item key={index + 1} style={{ padding: "6px 16px" }}>
                            {(index + 1) + "." + " " + item}
                        </List.Item>}
                />
            </Modal>
        </Content >
    );

}

export default Settings;
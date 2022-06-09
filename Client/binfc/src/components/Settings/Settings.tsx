import { Button, Checkbox, Input, List, Modal, Radio, Space, Typography } from "antd";
import { Content } from "antd/lib/layout/layout";
import moment from "moment";
import { FunctionComponent, MutableRefObject, useCallback, useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next";
import binancesellService from "../../services/binancesell.service";
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

interface IApiKey {
    key: string;
    value: string | null;
    isChanged: boolean;
}

interface IApiSecret {
    key: string;
    value: string | null;
    isChanged: boolean;
}

const Settings: FunctionComponent = () => {
    const { t } = useTranslation("common");
    const [inputValue, setInputValue] = useState('');
    const [spotValue, setSpotValue] = useState('');
    const [futureValue, setFutureValue] = useState('');
    const [apiInputValue, setApiInputValue] = useState('');
    const [apiSecretInputValue, setSecretInputValue] = useState('');
    const [notificationNamesInputValue, setNotificationNamesInputValue] = useState('');
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
    const [apiKey, setApiKey] = useState<IApiKey>({
        key: '',
        value: '',
        isChanged: false,
    });
    const [apiSecret, setApiSecret] = useState<IApiSecret>({
        key: '',
        value: '',
        isChanged: false,
    });
    const [notificationNames, setNotificationNames] = useState<IApiSecret>({
        key: '',
        value: '',
        isChanged: false,
    });
    const [spotPercent, setSpotPercent] = useState<IApiKey>({
        key: '',
        value: '',
        isChanged: false,
    });
    const [futurePercent, setFuturePercent] = useState<IApiKey>({
        key: '',
        value: '',
        isChanged: false,
    });
    const [disableButtons, setDisableButtons] = useState("False")
    const [buttonsFlag, setButtonsFlag] = useState(true)
    const ws = useRef<any>(null);

    useEffect(() => {
        settingsService.getSettings().then((data) => {
            data.length && data?.filter((item: IData) => {
                if (item?.key === "NotificationNames") {
                    setNotificationNamesInputValue(item?.value ?? '')
                    return setNotificationNames({
                        key: item?.key,
                        value: item?.value,
                        isChanged: false,
                    })
                }
            })
            data.length && data?.filter((item: IData) => {
                if (item?.key === "ApiSecret") {
                    setSecretInputValue(item?.value ?? '')
                    return setApiSecret({
                        key: item?.key,
                        value: item?.value,
                        isChanged: false,
                    })
                }
            })
            data.length && data?.filter((item: IData) => {
                if (item?.key === "ApiKey") {
                    setApiInputValue(item?.value ?? '')
                    return setApiKey({
                        key: item?.key,
                        value: item?.value,
                        isChanged: false,
                    })
                }
            })
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
            data.length && data?.filter((item: IData) => {
                if (item?.key === "SpotPercent") {
                    return setSpotPercent({
                        key: item?.key,
                        value: item?.value,
                        isChanged: false,
                    })
                }
            })
            data.length && data?.filter((item: IData) => {
                if (item?.key === "FuturesPercent") {
                    return setFuturePercent({
                        key: item?.key,
                        value: item?.value,
                        isChanged: false,
                    })
                }
            })
        })
        ws.current = new WebSocket("ws://185.179.190.122/binfc_server/ws"); // создаем ws соединение

        const interval = setInterval(() => {
            ws.current.send('');
            ws.current.onmessage = (e: any) => {
                setDisableButtons(e.data)
                setButtonsFlag(false)
            };
        }, 5000);

        return () => {
            clearInterval(interval)
        };
    }, [])

    const onChangeValue = (value: string) => {
        setInputValue(value);
        cronExpression.isChanged = true;
        setCronExpression(cronExpression)
    }

    const onChangeNameNofitication = (value: string) => {
        setNotificationNamesInputValue(value);
        notificationNames.isChanged = true;
        setNotificationNames(notificationNames)
    }

    const onChangeApiSecret = (value: string) => {
        setSecretInputValue(value);
        apiSecret.isChanged = true;
        setApiSecret(apiSecret)
    }

    const onChangeApiKey = (value: string) => {
        setApiInputValue(value);
        apiKey.isChanged = true;
        setApiKey(apiKey)
    }

    const onChangeSpotKey = (value: string) => {
        setSpotValue(value);
        spotPercent.isChanged = true;
        setSpotPercent(apiKey)
    }

    const onChangeFutureKey = (value: string) => {
        setFutureValue(value);
        futurePercent.isChanged = true;
        setFuturePercent(apiKey)
    }

    const checkDates = (cronExpression?: string) => {
        let localeDate: any[] = []
        cronService.next(cronExpression ?? '').then((response) => {
            if (response.status !== 200) {
                return Promise.reject(response);
            } else {
                response.data.map((item: any) => {
                    localeDate.push(moment.utc(item).local().format('YYYY-MM-DD HH:mm:ss'))                 
                });
                setModalData(localeDate)
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
                if (futurePercent.isChanged) {
                    formData.append(`settings[${i}].key`, 'FuturesPercent');
                    formData.append(`settings[${i}].value`, futureValue);
                    i++;
                }
                if (spotPercent.isChanged) {
                    formData.append(`settings[${i}].key`, 'SpotPercent');
                    formData.append(`settings[${i}].value`, spotValue);
                    i++;
                }
                if (notificationNames.isChanged) {
                    formData.append(`settings[${i}].key`, 'NotificationNames');
                    formData.append(`settings[${i}].value`, notificationNamesInputValue);
                    i++;
                }
                if (apiSecret.isChanged) {
                    formData.append(`settings[${i}].key`, 'ApiSecret');
                    formData.append(`settings[${i}].value`, apiSecretInputValue);
                    i++;
                }
                if (apiKey.isChanged) {
                    formData.append(`settings[${i}].key`, 'ApiKey');
                    formData.append(`settings[${i}].value`, apiInputValue);
                    i++;
                }
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
                if (cronExpression.isChanged
                    || notificationNames.isChanged
                    || spotPercent.isChanged
                    || futurePercent.isChanged
                    || apiSecret.isChanged
                    || sellCurrency.isChanged
                    || isNotification.isChanged
                    || apiKey.isChanged) {
                    cronExpression.isChanged
                        = notificationNames.isChanged
                        = apiSecret.isChanged
                        = futurePercent.isChanged
                        = spotPercent.isChanged
                        = sellCurrency.isChanged
                        = apiKey.isChanged
                        = isNotification.isChanged
                        = false;
                    return settingsService.saveSettings(formData)
                }
            }
        })

    };

    const binanceStart = () => {
        binancesellService.start()
        setDisableButtons("True")
    }

    const binanceRestart = () => {
        binancesellService.restart()
    }

    const binanceStop = () => {
        binancesellService.stop()
        setDisableButtons("False")
    }

    return (
        <Content style={{ display: "flex", flexDirection: "column" }}>
            <Typography.Text>{t("common:PaymentDateSetting")}</Typography.Text>
            <div style={{ marginTop: "20px" }}>
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
            <div style={{ marginTop: "20px", display: "flex", flexDirection: "column" }}>
                <Typography.Text>{t("common:NicknamesNotification")}</Typography.Text>
                <Input
                    defaultValue={notificationNames?.value ?? ''}
                    key={notificationNames?.key}
                    onChange={(e) => onChangeNameNofitication(e?.target?.value)}
                    style={{ maxWidth: "350px", marginTop: "8px" }}
                />
            </div>
            <div style={{ marginTop: "20px", display: "flex", flexDirection: "column" }}>
                <Typography.Text>{"Api Secret"}</Typography.Text>
                <Input
                    defaultValue={apiSecret?.value ?? ''}
                    key={apiSecret?.key}
                    onChange={(e) => onChangeApiSecret(e?.target?.value)}
                    style={{ maxWidth: "350px", marginTop: "8px" }}
                />
            </div>
            <div style={{ marginTop: "20px", display: "flex", flexDirection: "column" }}>
                <Typography.Text>{"Api Key"}</Typography.Text>
                <Input
                    defaultValue={apiKey?.value ?? ''}
                    key={apiKey?.key}
                    onChange={(e) => onChangeApiKey(e?.target?.value)}
                    style={{ maxWidth: "350px", marginTop: "8px" }}
                />
            </div>
            <div style={{ marginTop: "20px", display: "flex", flexDirection: "column" }}>
                <Typography.Text>{t("common:SpotPercent")}</Typography.Text>
                <Input
                    min={0}
                    max={100}
                    type="number"
                    defaultValue={spotPercent?.value ?? ''}
                    key={spotPercent?.key}
                    onChange={(e) => onChangeSpotKey(e?.target?.value)}
                    style={{ maxWidth: "350px", marginTop: "8px" }}
                />
            </div>
            <div style={{ marginTop: "20px", display: "flex", flexDirection: "column" }}>
                <Typography.Text>{t("common:FuturePercent")}</Typography.Text>
                <Input
                    min={0}
                    max={100}
                    type="number"
                    defaultValue={futurePercent?.value ?? ''}
                    key={futurePercent?.key}
                    onChange={(e) => onChangeFutureKey(e?.target?.value)}
                    style={{ maxWidth: "350px", marginTop: "8px" }}
                />
            </div>

            <div style={{ marginTop: "20px" }}>
                <Space>
                    <Button
                        type="primary"
                        disabled={disableButtons === "False"  && !buttonsFlag ? false : true}
                        onClick={() => binanceStart()}
                        style={{ textAlign: "center" }}>
                        {t("common:Start")}
                    </Button>
                    <Button
                        type="primary"
                        disabled={!buttonsFlag ? false : true}
                        onClick={() => binanceRestart()}
                        style={{ textAlign: "center" }}>
                        {t("common:Restart")}
                    </Button>
                    <Button
                        type="primary"
                        disabled={disableButtons === "True"  && !buttonsFlag ? false : true}
                        onClick={() => binanceStop()}
                        style={{ textAlign: "center" }}>
                        {t("common:Stop")}
                    </Button>
                </Space>
            </div>
            <div style={{ marginTop: "20px" }}>
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
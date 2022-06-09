import { Button, Modal, Table, Typography } from "antd";
import { t } from "i18next";
import { FunctionComponent, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { paymentService } from "../../services/payment.service";
import { Spin, Space } from 'antd';
import { sortedString } from "../../helpers/sortedHelper";
import { useSelector } from "react-redux/es/hooks/useSelector";

const { Title } = Typography;

const Payout: FunctionComponent = () => {
    const { i18n } = useTranslation("common");
    const language = useSelector((state: any) => state?.languageState.locale)
    const [paymentsData, setPaymentsData] = useState<any>([]);
    const [modalPaymentVisible, setIsModalPaymentVisible] = useState<boolean>(false);
    const [modalData, setModalData] = useState<any>([])
    const [paymentData, setPaymentData] = useState<any>([])
    const [isLoading, setIsLoading] = useState(false)
    const [cost, setCost] = useState<number>(0)
    const [isLoadingModal, seTisLoadingModal] = useState(false)
    const [forceReload, setForceReload] = useState(false)
    const [balanceData, setBalanceData] = useState<any>([]);
    const [btnDisabled, setBtnDisabled] = useState(false)
    const [selectedPaymentsId, setSelectedPaymentsId] = useState({
        selectedPaymentskey: [],
        loading: false
    });

    useEffect(() => {
        const resultPayments: any[] = [];
        setIsLoading(true)
        paymentService.getPaymentData().then((response) => {
            if (response.status !== 200) {
                setIsLoading(false)
                return
            }
            response.data.length && response.data?.map((item: any) => {
                resultPayments.push(
                    {
                        id: item?.userId,
                        key: item?.userId,
                        userName: item.userName === null ? t("common:noData") : item.userName,
                        bepAddress: item ? item?.bepAddress : t("common:noData"),
                        trcAddress: item ? item?.trcAddress : t("common:noData"),
                        usdt: item ? item?.usdt : t("common:noData"),
                    }
                )
                return setPaymentsData(resultPayments)
            })
            setIsLoading(false)
        })
        paymentService.getBalanceData().then((response) => {
            if (response.status === 200) {               
                setBalanceData(response.data);
            }
            else {
                setBalanceData(t("common:BalanceUncertain"));
            }
        })
        setForceReload(false)
    }, [forceReload, language])

    const { selectedPaymentskey } = selectedPaymentsId;
    const rowSelectionPaymentsId = {
        selectedPaymentskey,
        onChange: (selectedPaymentskey: any) => {
            setSelectedPaymentsId({
                ...selectedPaymentsId,
                selectedPaymentskey: selectedPaymentskey
            });
        }
    };

    const getRowData = () => {
        const payment: any[] = []
        const modal: any[] = []
        let cost = 0;
        paymentsData?.map((item: any) => {
            selectedPaymentsId?.selectedPaymentskey.map((i: any) => {
                if (item?.id === i) {
                    payment.push({
                        usdt: item.usdt,
                        userId: item.id,
                        userName: item.userName === t("common:noData") ? null : item.userName,
                        trcAddress: item.trcAddress === t("common:noData") ? null : item.trcAddress,
                        bepAddress: item.bepAddress === t("common:noData") ? null : item.bepAddress,
                    })
                    modal.push({
                        key: item.id,
                        usdt: item.usdt,
                        id: item.id
                    })
                    cost += item.usdt;
                }
                return setCost(cost)
            })
            return setPaymentData(payment);
        })
        setModalData(modal)
    }

    const columnPayments = [
        {
            title: t("common:TableId"),
            dataIndex: "id",
            key: "id",
            sorter: (a: { id: number; }, b: { id: number; }) => a.id - b.id,
        },
        {
            title: t("common:TableName"),
            dataIndex: "userName",
            key: "userName",
            sorter: ((a: any, b: any) => sortedString(a.userName, b.userName)),
        },
        {
            title: t("common:TableTrc"),
            dataIndex: "trcAddress",
            key: "trcAddress",
            sorter: ((a: any, b: any) => sortedString(a.trcAddress, b.trcAddress)),

        },
        {
            title: t("common:TableBep"),
            dataIndex: "bepAddress",
            key: "bepAddress",
            sorter: ((a: any, b: any) => sortedString(a.bepAddress, b.bepAddress)),
        },
        {
            title: t("common:Payout"),
            dataIndex: "usdt",
            key: "usdt",
            sorter: (a: any, b: any) => a.usdt - b.usdt,
        },
    ]

    const columnModalPayments = [
        { title: t("common:TableId"), dataIndex: "id", key: "id", },
        { title: t("common:Payout"), dataIndex: "usdt", key: "usdt", },
    ]

    return (
        <>
            <Table
                key={20}
                locale={{
                    triggerDesc: t("common:TriggerDesc"),
                    triggerAsc: t("common:TriggerAsc"),
                    cancelSort: t("common:CancelSort")
                }}
                rowSelection={rowSelectionPaymentsId}
                columns={columnPayments}
                dataSource={paymentsData}
                loading={{ indicator: <Spin size="large" />, spinning: isLoading }}
            />
            <Space style={{ marginTop: "16px" }}>
                <Button
                    key={6}
                    disabled={btnDisabled}
                    type="primary"
                    onClick={() => {
                        getRowData();
                        setIsModalPaymentVisible(true)
                    }}
                >
                    {`${t("common:Pay")}`}
                </Button>
                <p style={{ display: "flex", flexDirection: "column", width: "100%" }}>
                    {balanceData}
                </p>
            </Space>
            <Modal
                key={21}
                forceRender
                footer={null}
                onCancel={() => setIsModalPaymentVisible(false)}
                title={`${t("common:Payout")}`}
                visible={modalPaymentVisible}

            > <Table
                    key={20}
                    columns={columnModalPayments}
                    footer={undefined}
                    pagination={false}
                    dataSource={modalData}
                    loading={{ indicator: <Spin size="large" />, spinning: isLoadingModal }}
                />
                <Space style={{
                    marginTop: "16px",
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "normal"
                }}>
                    <Button
                        key={24}
                        type="primary"
                        disabled={btnDisabled}
                        onClick={() => {
                            seTisLoadingModal(true)
                            setBtnDisabled(true)
                            paymentService.postPaymentData(paymentData).then((response) => {
                                setTimeout(() => {
                                    setIsModalPaymentVisible(false)
                                    seTisLoadingModal(false)
                                    setBtnDisabled(false)
                                    setForceReload(true)
                                    rowSelectionPaymentsId.onChange([]);
                                }, 2000)
                            })
                        }}
                        htmlType="submit"
                        className="login-form-button"
                    >
                        {`${t("common:Pay")}`}
                    </Button>
                    <Title level={4}>{Math.floor(cost * 100) / 100}</Title>
                </Space>
            </Modal>
        </>
    )

}

export default Payout;
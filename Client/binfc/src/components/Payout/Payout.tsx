import { Button, Descriptions, Table } from "antd";
import { t } from "i18next";
import { FunctionComponent, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { paymentService } from "../../services/payment.service";
import { Spin, Space } from 'antd';



const Payout: FunctionComponent = () => {
    const { i18n } = useTranslation("common");
    const [forceUpdate, setForceUpdate] = useState(false)
    const [paymentsData, setPaymentsData] = useState<any>([]);
    const [modalPaymentVisible, setIsModalPaymentVisible] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState(false)
    const [balanceData, setBalanceData] = useState<any>([]);
    const [selectedPaymentsId, setSelectedPaymentsId] = useState({
        selectedPaymentskey: [],
        loading: false
    });

    useEffect(() => {
        const resultPayments: any[] = [];
        setIsLoading(true)
        paymentService.getPaymentData().then((data) => {
            data.length && data?.map((item: any) => {
                resultPayments.push(
                    {
                        id: item?.userId,
                        key: item?.userId,
                        userName: item ? item?.userName : t("common:noData"),
                        bepAddress: item ? item?.bepAddress : t("common:noData"),
                        trcAddress: item ? item?.trcAddress : t("common:noData"),
                        usdt: item ? item?.usdt : t("common:noData"),
                    }
                )
            })
            setPaymentsData(resultPayments)
            setIsLoading(false)
        })
        paymentService.getBalanceData().then((data) => {
            setBalanceData(data);
        })
    }, [forceUpdate])

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

    const fetchPayment = () => {
        const result: any[] = []
        paymentsData?.map((item: any) => {
            selectedPaymentsId?.selectedPaymentskey.map((i: any) => {
                if (item?.id === i) {
                    result.push(item)
                }
            })
        })
        console.log(result);
        paymentService.postPaymentData(result)
    }

    const columnPayments = [
        { title: t("common:TableId"), dataIndex: "id", key: "id", },
        { title: t("common:TableName"), dataIndex: "userName", key: "userName", },
        { title: t("common:TableTrc"), dataIndex: "trcAddress", key: "trcAddress", },
        { title: t("common:TableBep"), dataIndex: "bepAddress", key: "bepAddress", },
        { title: t("common:Payout"), dataIndex: "usdt", key: "usdt", },
    ]

    return (
        <>
            <Table
                key={20}
                rowSelection={rowSelectionPaymentsId}
                columns={columnPayments}
                dataSource={paymentsData}
                loading={{ indicator: <Spin size="large" />, spinning: isLoading }}
            />
            <Space style={{ marginTop: "16px" }}>
                <Button
                    key={6}
                    type="primary"
                    onClick={() => {
                        fetchPayment();
                        setIsModalPaymentVisible(true)
                    }}
                >
                    {`${t("common:Pay")}`}
                </Button>
                <p style={{ display: "flex", flexDirection: "column", width: "50%" }}>
                    {balanceData}
                </p>
            </Space>

        </>
    )

}

export default Payout;
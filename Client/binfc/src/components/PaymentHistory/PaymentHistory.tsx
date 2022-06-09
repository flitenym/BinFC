import { faSearch } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Input, Space, Button, InputRef, Spin } from "antd";
import { ColumnType } from "antd/lib/table";
import { Table } from "ant-table-extensions";
import { FilterConfirmProps, TablePaginationConfig } from "antd/lib/table/interface";
import React, { FunctionComponent, useEffect, useRef, useState } from "react"
import Highlighter from "react-highlight-words";
import { useTranslation } from "react-i18next";
import { payHistoryService } from "../../services/payhistory.service";
import SearchOutlined from '@ant-design/icons';
import moment from "moment";
import { exportPayHistoryService } from "../../services/export.service";
import { sortedNumbers, sortedString } from "../../helpers/sortedHelper";

const PaymentHistory: FunctionComponent = () => {
    const { t } = useTranslation("common");
    const [paymentData, setPaymentData] = useState([]);
    const [paymentDataTable, setPaymentDataTable] = useState<any>([]);
    const [exportData, setExportData] = useState<any>(null);
    const searchInput = useRef<InputRef>(null);
    const [searchText, setSearchText] = useState('');
    const [isLoading, setIsLoading] = useState(false)
    const [searchedColumn, setSearchedColumn] = useState('');
    const [selectedPaymentsId, setSelectedPaymentsId] = useState<any>({
        selectedRowKeys: [],
        loading: false
    });
    const [pagination, setPagination] = useState<TablePaginationConfig>({
        current: 1,
        pageSize: 10,
        hideOnSinglePage: false,
        showSizeChanger: true,
        pageSizeOptions: ["10", "20", "50", "100", "300"],
        locale: { items_per_page: "" },
    });
    useEffect(() => {
        const arraySelectedElements = paymentDataTable.reduce((acc: any[], item: { id: any; }) => {
            if (selectedPaymentsId?.selectedRowKeys?.includes(item.id)) {
                acc.push(item.id)
            };
            return acc;
        }, []);
        setExportData(arraySelectedElements)
    }, [selectedPaymentsId])

    useEffect(() => {
        setIsLoading(true)
        const resultDataTable: any[] = [];
        payHistoryService.getPayHistory().then((data) => {
            setPaymentData(data)
            data.map((item: any) => {
                resultDataTable.push({
                    id: item?.id ? item?.id : t("common:noData"),
                    key: item?.id,
                    userName: item.user.userName ? item?.user.userName : t("common:noData"),
                    tableUserID: item.user.userId ? item?.user.userId : t("common:noData"),
                    sendedSum: item.sendedSum ? item.sendedSum : t("common:noData"),
                    sendedTime: item?.sendedTime ? `${moment.utc(item?.sendedTime).local().format('YYYY-MM-DD HH:mm:ss')}` : t("common:noData"),
                    numberPay: item?.numberPay ? item?.numberPay : t("common:noData"),
                    userId: item.userId ? item.userId : t("common:noData"),
                })
            })
            setPaymentDataTable(resultDataTable);
            setIsLoading(false)
        })
    }, [])

    const handleSearch = (
        selectedKeys: string[],
        confirm: (param?: FilterConfirmProps) => void,
        dataIndex: any,
    ) => {
        confirm();
        setSearchText(selectedKeys[0]);
        setSearchedColumn(dataIndex);
    };

    const handleReset = (clearFilters: () => void) => {
        clearFilters();
        setSearchText('');
    };

    const getColumnSearchProps = (dataIndex: any): ColumnType<any> => ({
        filterDropdown: ({ setSelectedKeys, selectedKeys, confirm, clearFilters }) => (
            <div style={{ padding: 8 }}>
                <Input
                    ref={searchInput}
                    placeholder={`Search ${dataIndex}`}
                    value={selectedKeys[0]}
                    onChange={e => setSelectedKeys(e.target.value ? [e.target.value] : [])}
                    onPressEnter={() => handleSearch(selectedKeys as string[], confirm, dataIndex)}
                    style={{ marginBottom: 8, display: 'block' }}
                />
                <Space>
                    <Button
                        type="primary"
                        onClick={() => handleSearch(selectedKeys as string[], confirm, dataIndex)}
                        icon={<SearchOutlined color="#fff" />}
                        size="small"
                        style={{ width: 90 }}
                    >
                        Search
                    </Button>
                    <Button
                        onClick={() => {
                            clearFilters && handleReset(clearFilters)
                            handleSearch([''], confirm, dataIndex)
                        }}
                        size="small"
                        style={{ width: 90 }}
                    >
                        Reset
                    </Button>
                </Space>
            </div>
        ),
        filterIcon: () => (
            <FontAwesomeIcon icon={faSearch} />
        ),
        onFilter: (value, record) =>
            record[dataIndex]
                .toString()
                .toLowerCase()
                .includes((value as string).toLowerCase()),
        onFilterDropdownVisibleChange: visible => {
            if (visible) {
                setTimeout(() => searchInput.current?.select(), 100);
            }
        },
        render: text =>
            searchedColumn === dataIndex ? (
                <Highlighter
                    highlightStyle={{ backgroundColor: '#ffc069', padding: 0 }}
                    searchWords={[searchText]}
                    autoEscape
                    textToHighlight={text ? text.toString() : ''}
                />
            ) : (
                text
            ),
    });

    const columnPaymenyHistory = [
        {
            title: t("common:UserID"),
            dataIndex: "tableUserID",
            key: "tableUserID",
            ...getColumnSearchProps('tableUserID'),
            sorter: (a: { tableUserID: number; }, b: { tableUserID: number; }) => a.tableUserID - b.tableUserID,

        },
        {
            title: t("common:UserFIO"),
            dataIndex: "userName",
            key: "userName",
            ...getColumnSearchProps('userName'),
            sorter: ((a: { userName: any; }, b: { userName: any; }) => sortedString(a.userName, b.userName)),
        },
        {
            title: t("common:TableUSDT"),
            dataIndex: "sendedSum",
            key: "sendedSum",
            ...getColumnSearchProps('sendedSum'),
            sorter: ((a: { sendedSum: any; }, b: { sendedSum: any; }) => sortedNumbers(a.sendedSum, b.sendedSum)),
        },
        {
            title: t("common:DatePayment"),
            dataIndex: "sendedTime",
            key: "sendedTime",
            ...getColumnSearchProps('sendedTime'),
            sorter: (a: { sendedTime: moment.MomentInput; }, b: { sendedTime: moment.MomentInput; }) => moment(a.sendedTime).unix() - moment(b.sendedTime).unix()

        },
        {
            title: t("common:NumberPayment"),
            dataIndex: "numberPay",
            key: "numberPay",
            ...getColumnSearchProps('numberPay'),
            sorter: (a: { numberPay: number; }, b: { numberPay: number; }) => a.numberPay - b.numberPay,

        },
    ]

    const { selectedRowKeys } = selectedPaymentsId;

    const rowSelection = {
        selectedRowKeys,
        onChange: (selectedRowKeys: any) => {
            setSelectedPaymentsId({
                ...selectedPaymentsId,
                selectedRowKeys: selectedRowKeys
            });
        }
    };

    const handleChangePaymentHistoryTable = (pagination: any, filters: any, sorter: any, extra: { currentDataSource: Array<any>[] }) => {
        setPagination({
            total: extra?.currentDataSource?.length,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
    }

    const exportPaymentsId = (ids: number[]) => {
        exportPayHistoryService.exportPayHistory(ids)
    }

    return (
        <React.Fragment>
            <Table
                locale={{
                    triggerDesc: t("common:TriggerDesc"),
                    triggerAsc: t("common:TriggerAsc"),
                    cancelSort: t("common:CancelSort")
                }}
                key={1}
                rowSelection={rowSelection}
                columns={columnPaymenyHistory}
                dataSource={paymentDataTable}
                loading={{ indicator: <Spin size="large" />, spinning: isLoading }}
                onChange={handleChangePaymentHistoryTable}
                pagination={pagination}
                scroll={{ y: 600 }}
                style={{ marginBottom: "16px" }}
            />
            <Button
                type="primary"
                onClick={() => exportPaymentsId(exportData)}
            >
                {t("common:Export")}
            </Button>
        </React.Fragment>
    );

}

export default PaymentHistory;
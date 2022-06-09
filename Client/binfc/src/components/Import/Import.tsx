import { faSearch } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button, Input, InputRef, Modal, Space, Spin, Table, Tabs, Upload } from "antd";
import { ColumnType, FilterConfirmProps, TablePaginationConfig } from "antd/lib/table/interface";
import { t } from "i18next";
import { FunctionComponent, useEffect, useRef, useState } from "react"
import Highlighter from "react-highlight-words";
import dataService from "../../services/data.service";
import SearchOutlined from '@ant-design/icons';
import { UploadOutlined } from '@ant-design/icons';
import { importService } from "../../services/import.service";
import { useTranslation } from "react-i18next";
import moment from "moment";
import { sortedNumbers, sortedString } from "../../helpers/sortedHelper";

const { TabPane } = Tabs;

const Import: FunctionComponent = () => {
    const [isModalSpotVisible, setIsModalSpotVisible] = useState(false);
    const [isModalFutureVisible, setIsModalFutureVisible] = useState(false);
    const [forceReload, setForceReload] = useState<boolean>(false);
    const { i18n } = useTranslation("common");
    const [spotFile, setSpotFile] = useState<any>(null);
    const [futuresFile, setFuturesFile] = useState<any>(null);
    const [spotData, setSpotData] = useState<any>([])
    const [futuresData, setFuturesData] = useState<any>([])
    const [searchText, setSearchText] = useState('');
    const [searchedColumn, setSearchedColumn] = useState('');
    const searchInput = useRef<InputRef>(null);
    const [isLoading, setIsLoading] = useState(false)
    const [selectedSpotsId, setSelectedSpotsId] = useState({
        selectedRowKeys: [],
        loading: false
    });
    const [selectedFuturesId, setSelectedFuturesId] = useState({
        selectedRowKeysFutures: [],
        loading: false
    });
    const [paginationSpotTable, setPaginationSpotTable] = useState<TablePaginationConfig>({
        current: 1,
        defaultPageSize: 10,
        hideOnSinglePage: false,
        showSizeChanger: true,
        pageSizeOptions: ["10", "20", "50", "100", "300"],
        locale: { items_per_page: "" },
    });
    const [paginationFutureTable, setPaginationFutureTable] = useState<TablePaginationConfig>({
        current: 1,
        defaultPageSize: 10,
        hideOnSinglePage: false,
        showSizeChanger: true,
        pageSizeOptions: ["10", "20", "50", "100", "300"],
        locale: { items_per_page: "" },
    });


    useEffect(() => {
        setIsLoading(true)
        const resultSpot: any[] = [];
        const resultFutures: any[] = [];
        dataService.getSpotData().then((data) => {
            data.length && data?.map((item: any) => {
                resultSpot.push(
                    {
                        id: item?.id,
                        agentEarnUsdt: item?.agentEarnUsdt ? item?.agentEarnUsdt : t("common:noData"),
                        isPaid: item?.isPaid ? "Да" : "Нет",
                        loadingDate: item?.loadingDate ? `${moment.utc(item?.loadingDate).local().format('YYYY-MM-DD HH:mm:ss')}` : t("common:noData"),
                        user: item?.user ? item?.user : t("common:noData"),
                        userId: item?.user?.userId ? item?.user?.userId : t("common:noData"),
                        userName: item?.userName ? item?.userName : t("common:noData"),
                        key: item?.id,
                    }
                )
            })
            setPaginationSpotTable({
                total: resultSpot?.length,
                hideOnSinglePage: false,
                showSizeChanger: true,
                pageSizeOptions: ["10", "20", "50", "100", "300"],
                locale: { items_per_page: "" },
            });
            setSpotData(resultSpot)
        })
        dataService.getFuturesData().then((data) => {
            data.map((item: any) => {
                resultFutures.push(
                    {
                        id: item?.id,
                        agentEarnUsdt: item?.agentEarnUsdt ? item?.agentEarnUsdt : t("common:noData"),
                        isPaid: item?.isPaid ? "Да" : "Нет",
                        loadingDate: item?.loadingDate ? `${moment.utc(item?.loadingDate).local().format('YYYY-MM-DD HH:mm:ss')}` : t("common:noData"),
                        user: item?.user ? item?.user : t("common:noData"),
                        userId: item?.user?.userId ? item?.user?.userId : t("common:noData"),
                        userName: item?.userName ? item?.userName : t("common:noData"),
                        key: item?.id,
                    }
                )
            })
            setPaginationFutureTable({
                total: resultFutures?.length,
                hideOnSinglePage: false,
                showSizeChanger: true,
                pageSizeOptions: ["10", "20", "50", "100", "300"],
                locale: { items_per_page: "" },
            });
            setFuturesData(resultFutures)
            setForceReload(false)
            setIsLoading(false)
        })
    }, [forceReload])

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

    const getColumnSearchProps = (dataIndex: any,): ColumnType<any> => ({
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
                setTimeout(() => searchInput.current?.select());
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

    const columnSpot = [
        {
            title: "ID",
            dataIndex: "userId",
            key: "userId",
            ...getColumnSearchProps('userId'),
            sorter: (a: any, b: any) => a.userId - b.userId,
        },
        {
            title: t("common:TableName"),
            dataIndex: "userName",
            key: "userName",
            ...getColumnSearchProps('userName'),
            sorter: ((a: { userName: any; }, b: { userName: any; }) => sortedString(a.userName, b.userName)),
        },
        {
            title: t("common:TableUSDT"),
            dataIndex: "agentEarnUsdt",
            key: "agentEarnUsdt",
            ...getColumnSearchProps('agentEarnUsdt'),
            sorter: ((a: { agentEarnUsdt: any; }, b: { agentEarnUsdt: any; }) => sortedNumbers(a.agentEarnUsdt, b.agentEarnUsdt)),
        },
        {
            title: t("common:LoadingTime"),
            dataIndex: "loadingDate",
            key: "loadingDate",
            ...getColumnSearchProps('loadingDate'),
            sorter: (a: { loadingDate: moment.MomentInput; }, b: { loadingDate: moment.MomentInput; }) => moment(a.loadingDate).unix() - moment(b.loadingDate).unix()
        },
        {
            title: t("common:Paid"),
            dataIndex: "isPaid",
            key: "isPaid",
            ...getColumnSearchProps('isPaid'),
            sorter: (a: { isPaid: string; }, b: { isPaid: string; }) => a.isPaid.length - b.isPaid.length,
        },
    ]

    const columnFutures = [
        {
            title: "ID",
            dataIndex: "userId",
            key: "userId",
            ...getColumnSearchProps('userId'),
            sorter: (a: any, b: any) => a.userId - b.userId,
        },
        {
            title: t("common:TableName"),
            dataIndex: "userName",
            key: "userName",
            ...getColumnSearchProps('userName'),
            sorter: ((a: { userName: any; }, b: { userName: any; }) => sortedString(a.userName, b.userName)),
        },
        {
            title: t("common:TableUSDT"),
            dataIndex: "agentEarnUsdt",
            key: "agentEarnUsdt",
            ...getColumnSearchProps('agentEarnUsdt'),
            sorter: ((a: { agentEarnUsdt: any; }, b: { agentEarnUsdt: any; }) => sortedNumbers(a.agentEarnUsdt, b.agentEarnUsdt)),
        },
        {
            title: t("common:LoadingTime"),
            dataIndex: "loadingDate",
            key: "loadingDate",
            ...getColumnSearchProps('loadingDate',),
            sorter: (a: { loadingDate: moment.MomentInput; }, b: { loadingDate: moment.MomentInput; }) => moment(a.loadingDate).unix() - moment(b.loadingDate).unix()
        },
        {
            title: t("common:Paid"),
            dataIndex: "isPaid",
            key: "isPaid",
            ...getColumnSearchProps('isPaid'),
            sorter: (a: { isPaid: string; }, b: { isPaid: string; }) => a.isPaid.length - b.isPaid.length,
        },
    ]

    const { selectedRowKeys } = selectedSpotsId;
    const { selectedRowKeysFutures } = selectedFuturesId;

    const rowSelection = {
        selectedRowKeys,
        onChange: (selectedRowKeys: any) => {
            setSelectedSpotsId({
                ...selectedSpotsId,
                selectedRowKeys: selectedRowKeys
            });
        }
    };

    const rowSelectionFutures = {
        selectedRowKeysFutures,
        onChange: (selectedRowKeysFutures: any) => {
            setSelectedFuturesId({
                ...selectedFuturesId,
                selectedRowKeysFutures: selectedRowKeysFutures
            });
        }
    };

    const removeSpots = (array: any) => {
        const arrayWithoutDeleteElements = spotData.reduce((acc: any[], item: { id: any; }) => {
            if (!array.includes(item.id)) {
                acc.push(item)
            };
            return acc;
        }, []);
        setPaginationSpotTable({
            total: arrayWithoutDeleteElements?.length,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
        setSpotData(arrayWithoutDeleteElements)
        dataService.deleteSpotData(array)
    }

    const removeAllSpots = () => {
        setSpotData([])
        setPaginationSpotTable({
            total: 0,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
        dataService.deleteAllSpotData()
    }

    const removeFutures = (array: any) => {
        const arrayWithoutDeleteElements = futuresData.reduce((acc: any[], item: { id: any; }) => {
            if (!array.includes(item.id)) {
                acc.push(item)
            };
            return acc;
        }, []);
        setPaginationFutureTable({
            total: arrayWithoutDeleteElements?.length,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
        setFuturesData(arrayWithoutDeleteElements)
        dataService.deleteFuturesData(array)
    }

    const removeAllFutures = () => {
        setFuturesData([])
        setPaginationFutureTable({
            total: 0,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
        dataService.deleteAllFuturesData()
    }

    const handleChangeSpotTable = (pagination: any, filters: any, sorter: any, extra: { currentDataSource: Array<any>[] }) => {
        setPaginationSpotTable({
            total: extra?.currentDataSource?.length,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
    }

    const handleChangeFutureTable = (pagination: any, filters: any, sorter: any, extra: { currentDataSource: Array<any>[] }) => {
        setPaginationFutureTable({
            total: extra?.currentDataSource?.length,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
    }

    const importSpotData = (data: any) => {
        const formData = new FormData();
        formData.append("file", data)
        formData.append("importType", "0")
        importService.importData(formData)
        setIsModalSpotVisible(false)
        setTimeout(() => {
            setForceReload(true)
        }, 1000)
    }

    const importFuturesData = (data: any) => {
        const formData = new FormData();
        formData.append("file", data)
        formData.append("importType", "1")
        importService.importData(formData)
        setIsModalFutureVisible(false)
        setTimeout(() => {
            setForceReload(true)
        }, 1000)
    }

    return (
        <Tabs defaultActiveKey="1" type="card" >
            <TabPane tab="Spot" key="1">
                <Table
                    locale={{
                        triggerDesc: t("common:TriggerDesc"),
                        triggerAsc: t("common:TriggerAsc"),
                        cancelSort: t("common:CancelSort")
                    }}
                    key={3}
                    rowSelection={rowSelection}
                    columns={columnSpot}
                    dataSource={spotData}
                    loading={{ indicator: <Spin size="large" />, spinning: isLoading }}
                    onChange={handleChangeSpotTable}
                    pagination={paginationSpotTable}
                    scroll={{ y: 600 }}
                />
                <Space key={4}>
                    <Button
                        key={5}
                        style={{ marginTop: "16px" }}
                        type="primary"
                        onClick={() => removeSpots(selectedSpotsId?.selectedRowKeys)}
                    >{`${t("common:ButtonDelete")}`}</Button>
                    <Button
                        key={228}
                        style={{ marginTop: "16px" }}
                        type="dashed"
                        onClick={() => removeAllSpots()}
                    >
                        {`${t("common:ButtonDeleteAll")}`}
                    </Button>
                    <Upload key={6} showUploadList={false}
                        beforeUpload={file => {
                            setIsModalSpotVisible(true)
                            setSpotFile(file);
                            return false;
                        }} accept={".xls, .xlsx, .csv"}
                    >
                        <Button
                            key={7}
                            style={{ marginTop: "16px" }}
                        >
                            <UploadOutlined />
                            {`${t("common:ImportFile")}`}
                        </Button>
                    </Upload>
                    <Modal
                        key={32}
                        title={`${t("common:ImportSpotFile")}`}
                        visible={isModalSpotVisible}
                        onOk={() => importSpotData(spotFile)}
                        okText={`${t("common:Yes")}`}
                        cancelText={`${t("common:No")}`}
                        onCancel={
                            () => {
                                setSpotFile(null)
                                setIsModalSpotVisible(false)
                            }
                        }>
                        {`${t("common:ConfirmImoportFile")} "${spotFile?.name}" ? `}
                    </Modal>
                </Space>
            </TabPane>
            <TabPane tab="Futures" key="2">
                <Table
                    locale={{
                        triggerDesc: t("common:TriggerDesc"),
                        triggerAsc: t("common:TriggerAsc"),
                        cancelSort: t("common:CancelSort")
                    }}
                    key={8}
                    rowSelection={rowSelectionFutures}
                    columns={columnFutures}
                    dataSource={futuresData}
                    pagination={paginationFutureTable}
                    onChange={handleChangeFutureTable}
                    loading={{ indicator: <Spin size="large" />, spinning: isLoading }}
                    scroll={{ y: 600 }}
                />
                <Space key={4}>
                    <Button
                        key={5}
                        style={{ marginTop: "16px" }}
                        type="primary"
                        onClick={() => removeFutures(selectedFuturesId?.selectedRowKeysFutures)}
                    >
                        {`${t("common:ButtonDelete")}`}
                    </Button>
                    <Button
                        key={322}
                        style={{ marginTop: "16px" }}
                        type="dashed"
                        onClick={() => removeAllFutures()}
                    >
                        {`${t("common:ButtonDeleteAll")}`}
                    </Button>
                    <Upload key={6} showUploadList={false}
                        beforeUpload={file => {
                            setIsModalFutureVisible(true)
                            setFuturesFile(file);
                            return false;
                        }} accept={".xls, .xlsx, .csv"}
                    >
                        <Button
                            key={7}
                            style={{ marginTop: "16px" }}
                        >
                            <UploadOutlined />
                            {`${t("common:ImportFile")}`}
                        </Button>
                    </Upload>
                    <Modal
                        key={33}
                        title={`${t("common:ImportFutureFile")}`}
                        visible={isModalFutureVisible}
                        onOk={() => importFuturesData(futuresFile)}
                        okText={`${t("common:Yes")}`}
                        cancelText={`${t("common:No")}`}
                        onCancel={
                            () => {
                                setFuturesFile(null)
                                setIsModalFutureVisible(false)
                            }
                        }>
                        {`${t("common:ConfirmImoportFile")} "${futuresFile?.name}" ? `}
                    </Modal>
                </Space>
            </TabPane>
        </Tabs >
    );

}

export default Import;
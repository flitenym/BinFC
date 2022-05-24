import { Table, TablePaginationConfig, Typography, Modal, TableProps, InputRef, Input, Space, Button } from "antd";
import { ColumnType, FilterConfirmProps, FilterValue } from "antd/lib/table/interface";
import { FunctionComponent, useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next";
import SearchOutlined from '@ant-design/icons';
import Highlighter from 'react-highlight-words';
import usersService from "../../services/users.service";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSearch } from '@fortawesome/free-solid-svg-icons'

const Users: FunctionComponent = () => {
    const { t } = useTranslation("common");
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [usersSettings, setUsersSettings] = useState<any[]>([]);
    const [pagination, setPagination] = useState<TablePaginationConfig>({
        current: 1,
        pageSize: 10,
    });
    const [searchText, setSearchText] = useState('');
    const [searchedColumn, setSearchedColumn] = useState('');
    const searchInput = useRef<InputRef>(null);

    useEffect(() => {
        usersService.getUsers().then((response) => {
            getSettings(response)
            setPagination({
                total: response?.length,
            });
        })
    }, [])

    const getSettings = (data: any) => {
        const result: any[] = [];
        data.map((item: any, index: number) => {
            result.push(
                {
                    key: index ? index : t("common:noData"),
                    id: item?.userId ? item?.userId : t("common:noData"),
                    name: item?.userName ? item?.userName : t("common:noData"),
                    email: item?.userEmail ? item?.userEmail : t("common:noData"),
                    trc: item?.trcAddress ? item?.trcAddress : t("common:noData"),
                    bep: item?.bepAddress ? item?.bepAddress : t("common:noData"),
                    unique: item?.unique.name ? item?.unique.name : t("common:noData"),
                    uniqueId: item?.unique.id ? item?.unique.id : t("common:noData"),
                    notUserId: item?.id ? item?.id : t("common:noData"),
                    chatId: item?.chatId ? item?.chatId : t("common:noData"),
                    isAdmin: item?.isAdmin ? item?.isAdmin : t("common:noData"),
                }
            )
            return result
        })
        setUsersSettings(result)
        return result
    }

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
                .includes((value as string)),
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

    const column = [
        {
            title: t("common:TableId"),
            dataIndex: "id",
            key: "id",
            ...getColumnSearchProps('id'),
        },
        { title: t("common:TableName"), dataIndex: "name", key: "name", ...getColumnSearchProps('name'), },
        { title: t("common:TableEmail"), dataIndex: "email", key: "email", ...getColumnSearchProps('email'), },
        { title: t("common:TableTrc"), dataIndex: "trc", key: "trc", ...getColumnSearchProps('trc'), },
        { title: t("common:TableBep"), dataIndex: "bep", key: "bep", ...getColumnSearchProps('bep'), },
        { title: t("common:TableUnique"), dataIndex: "unique", key: "unique", ...getColumnSearchProps('unique'), },

    ]

    const onRowClick = (data: any) => {
        console.log(data);
    }

    const handleChange = (pagination: any, filters: any, sorter: any, extra: { currentDataSource: Array<any>[] }) => {
        setPagination({
            total: extra?.currentDataSource?.length
        });
    }

    return (
        <>
            <Typography.Text>
                {t("common:TableTitle")}
            </Typography.Text>
            <Table
                onRow={(record) => {
                    return {
                        onClick: (event) => {
                            onRowClick(record);
                        },
                        onDoubleClick: event => { }, // double click row
                        onContextMenu: event => { }, // right button click row
                        onMouseEnter: event => { }, // mouse enter row
                        onMouseLeave: event => { }, // mouse leave row
                        style: {
                            backGround: "#FFFFFF",
                            boxShadow: "inset 0px -1px 0px #E6E6E6",
                        }
                    };
                }}
                columns={column}
                dataSource={usersSettings}
                pagination={pagination}
                style={{
                    background: "#F5F5F5",
                    marginTop: "25px",
                }}
                onChange={handleChange}
                scroll={{ y: 600 }}
            />
            <Modal title="Basic Modal" visible={isModalVisible} /* onOk={handleOk} onCancel={handleCancel} */>
                <p>Some contents...</p>
                <p>Some contents...</p>
                <p>Some contents...</p>
            </Modal>
        </>
    );

}

export default Users;
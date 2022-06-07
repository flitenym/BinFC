import { Table, TablePaginationConfig, Typography, Modal, InputRef, Input, Space, Button, Form, Select, Tag, Spin } from "antd";
import { ColumnType, FilterConfirmProps } from "antd/lib/table/interface";
import React, { FunctionComponent, useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next";
import SearchOutlined from '@ant-design/icons';
import Highlighter from 'react-highlight-words';
import usersService from "../../services/users.service";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSearch } from '@fortawesome/free-solid-svg-icons'
import { useForm } from "antd/lib/form/Form";
import uniqueService from "../../services/unique.servise";
import "./UsersStyles.scss"
import { sortedString } from "../../helpers/sortedHelper";
const { Option } = Select;

const Users: FunctionComponent = () => {
    const { t } = useTranslation("common");
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [uniqueData, setUniqueData] = useState<any>([]);
    const [usersSettings, setUsersSettings] = useState<any[]>([]);
    const [searchText, setSearchText] = useState('');
    const [searchedColumn, setSearchedColumn] = useState('');
    const searchInput = useRef<InputRef>(null);
    const [forceUpdate, setForceUpdate] = useState(false)
    const [selectedRowData, setSelectedRowData] = useState<any>()
    const [isLoading, setIsLoading] = useState(false)
    const [pagination, setPagination] = useState<TablePaginationConfig>({
        current: 1,
        pageSize: 10,
        hideOnSinglePage: false,
        showSizeChanger: true,
        pageSizeOptions: ["10", "20", "50", "100", "300"],
        locale: { items_per_page: "" },
    });
    const [selectedUsersId, setSelectedUsersId] = useState<any>({
        selectedRowKeys: [],
        loading: false
    });

    const { selectedRowKeys } = selectedUsersId;

    const rowSelection = {
        selectedRowKeys,
        onChange: (selectedRowKeys: any) => {
            setSelectedUsersId({
                ...selectedUsersId,
                selectedRowKeys: selectedRowKeys
            });
        }
    };

    const [usersForm] = useForm();

    useEffect(() => {
        setIsLoading(true)
        usersService.getUsers().then((response) => {
            getSettings(response)
            setPagination({
                total: response?.length,
                hideOnSinglePage: false,
                showSizeChanger: true,
                pageSizeOptions: ["10", "20", "50", "100", "300"],
                locale: { items_per_page: "" },
            });
            setIsLoading(false)
        })
        uniqueService.getUnique().then((response) => {
            setUniqueData(response)
        })
        setForceUpdate(false)
    }, [forceUpdate])

    useEffect(() => {
        usersForm.resetFields()
        usersForm.setFieldsValue(selectedRowData)
    }, [selectedRowData, usersForm])

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

    const getSettings = (data: any) => {
        const result: any[] = [];
        data?.length && data?.map((item: any) => {
            result.push(
                {
                    key: item?.id,
                    id: item?.id ? item?.id : t("common:noData"),
                    userName: item?.userName ? item?.userName : t("common:noData"),
                    userEmail: item?.userEmail ? item?.userEmail : t("common:noData"),
                    trcAddress: item?.trcAddress ? item?.trcAddress : t("common:noData"),
                    bepAddress: item?.bepAddress ? item?.bepAddress : t("common:noData"),
                    uniqueName: item?.unique.name ? item?.unique.name : t("common:noData"),
                    uniqueId: item?.unique.id ? item?.unique.id : t("common:noData"),
                    userId: item?.userId ? item?.userId : t("common:noData"),
                    chatId: item?.chatId ? item?.chatId : t("common:noData"),
                    isAdmin: item?.isAdmin ? item?.isAdmin : t("common:noData"),
                    isApproved: item?.isApproved ? [true] : [false],
                    status: item?.isApproved ? t("common:Approved") : t("common:NotApproved"),
                }
            )
            return setUsersSettings(result)
        })
    }

    const column = [
        {
            title: t("common:TableId"),
            dataIndex: "userId",
            key: "userId",
            ...getColumnSearchProps('userId'),
            sorter: (a: { userId: number; }, b: { userId: number; }) => a.userId - b.userId,
        },
        {
            title: t("common:TableIsApproved"),
            dataIndex: "isApproved",
            key: "isApproved",
            sorter: ((a: { status: any; }, b: { status: any; }) => sortedString(a.status, b.status)),
            ...getColumnSearchProps('status'), render: (usersSettings: any, { isApproved }: any) => (
                <>
                    {usersSettings?.map((isApproved: boolean, index: number) => {
                        let color = isApproved ? 'green' : 'rgb(108,11,15)'
                        let title = isApproved ? t("common:Approved") : t("common:NotApproved")
                        return (
                            <Tag color={color} key={index}>
                                {title}
                            </Tag>
                        );
                    })}
                </>)
        },
        {
            title: t("common:TableName"),
            dataIndex: "userName",
            key: "userName",
            ...getColumnSearchProps('userName'),
            sorter: ((a: { userName: any; }, b: { userName: any; }) => sortedString(a.userName, b.userName))
        },
        {
            title: t("common:TableEmail"),
            dataIndex: "userEmail",
            key: "userEmail",
            ...getColumnSearchProps('userEmail'),
            sorter: ((a: { userEmail: any; }, b: { userEmail: any; }) => sortedString(a.userEmail, b.userEmail))
        },
        {
            title: t("common:TableTrc"),
            dataIndex: "trcAddress",
            key: "trcAddress",
            ...getColumnSearchProps('trcAddress'),
            sorter: ((a: { trcAddress: any; }, b: { trcAddress: any; }) => sortedString(a.trcAddress, b.trcAddress))

        },
        {
            title: t("common:TableBep"),
            dataIndex: "bepAddress",
            key: "bepAddress",
            ...getColumnSearchProps('bepAddress'),
            sorter: ((a: { bepAddress: any; }, b: { bepAddress: any; }) => sortedString(a.bepAddress, b.bepAddress))
        },
        {
            title: t("common:TableUnique"),
            dataIndex: "uniqueName",
            key: "uniqueName",
            ...getColumnSearchProps('uniqueName'),
            sorter: ((a: { uniqueName: any; }, b: { uniqueName: any; }) => sortedString(a.uniqueName, b.uniqueName))
        },

    ]

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

    const handleChange = (pagination: any, filters: any, sorter: any, extra: { currentDataSource: Array<any>[] }) => {
        setPagination({
            total: extra?.currentDataSource?.length,
            hideOnSinglePage: false,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50", "100", "300"],
            locale: { items_per_page: "" },
        });
    }

    const onRowClick = (record: any) => {
        setSelectedRowData(record)
        setIsModalVisible(true)
    }

    const onCloseModal = () => {
        setIsModalVisible(false)
    }

    const modifiedModalData = (data: any) => {
        const uniqueId = uniqueData?.find((item: any) => {
            if (item.name === data.uniqueName) {
                return item.id
            }
        })
        setUsersSettings(usersSettings.map((item) => {
            if (item.id === selectedRowData.id) {
                item.key = selectedRowData.key;
                item.bepAddress = data.bepAddress;
                item.userEmail = data.userEmail;
                item.trcAddress = data.trcAddress;
                item.uniqueName = data.uniqueName;
                item.uniqueId = uniqueId.id;
                item.userName = data.userName;
                usersService.upadeteUser({
                    id: item.id,
                    userName: item.userName === t("common:noData") ? null : item.userName,
                    userEmail: item.userEmail === t("common:noData") ? null : item.userEmail,
                    trcAddress: item.trcAddress === t("common:noData") ? null : item.trcAddress,
                    bepAddress: item.bepAddress === t("common:noData") ? null : item.bepAddress,
                    uniqueId: item.uniqueId
                })
                return item
            } else {
                return item
            }
        }))

    }

    const approveUsers = (usersId: number[]) => {
        usersService.approveUsers(usersId)
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
    }

    const approveAllUsers = () => {
        usersService.approveAllUsers()
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
    }

    const notApproveAllUsers = () => {
        usersService.notApproveAllUsers()
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
    }

    const notApproveUsers = (usersId: number[]) => {
        usersService.notApproveUsers(usersId)
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
    }

    return (
        <React.Fragment key={1}>
            <Typography.Text key={2}>
                {t("common:TableTitle")}
            </Typography.Text>
            <Table
                locale={{
                    triggerDesc: t("common:TriggerDesc"),
                    triggerAsc: t("common:TriggerAsc"),
                    cancelSort: t("common:CancelSort")
                }}
                key={3}
                rowSelection={rowSelection}
                loading={{ indicator: <Spin size="large" />, spinning: isLoading }}
                onRow={(record) => {
                    return {
                        onDoubleClick: (event) => onRowClick(record),
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
            <Modal
                key={4}
                forceRender
                footer={null}
                onCancel={onCloseModal}
                title={`${selectedRowData?.userId}`}
                visible={isModalVisible}
            >
                <Form
                    key={5}
                    form={usersForm}
                    onFinish={(data) => modifiedModalData(data)}
                    initialValues={{
                        selectedRowData
                    }}
                >
                    <span key={6} className="form-description">{t("common:TableEmail")}</span>
                    <Form.Item
                        key={7}
                        style={{
                            marginTop: "4px",
                            marginBottom: "16px",
                        }}
                        name="userEmail"
                    >
                        <Input key={8} />
                    </Form.Item>
                    <span key={9} className="form-description">{t("common:UserName")}</span>
                    <Form.Item
                        key={10}
                        style={{
                            marginTop: "4px",
                            marginBottom: "16px",
                        }}
                        name="userName"
                    >
                        <Input key={11} />
                    </Form.Item>
                    <span key={12} className="form-description">{t("common:TableTrc")}</span>
                    <Form.Item
                        key={13}
                        style={{
                            marginTop: "4px",
                            marginBottom: "16px",
                        }}
                        name="trcAddress"
                    >
                        <Input key={14} />
                    </Form.Item>
                    <span key={15} className="form-description">{t("common:TableBep")}</span>
                    <Form.Item
                        key={16}
                        style={{
                            marginTop: "4px",
                            marginBottom: "16px",
                        }}
                        name="bepAddress"
                    >
                        <Input key={17} />
                    </Form.Item>
                    <span key={18} className="form-description">{t("common:TableUnique")}</span>
                    <Form.Item key={19} style={{
                        marginTop: "4px",
                        marginBottom: "16px",
                    }} name="uniqueName">
                        <Select
                            key={20}
                        >
                            {uniqueData && uniqueData.length && uniqueData?.map((item: any) => {
                                return (
                                    <Option
                                        value={item.name}
                                        key={item.id}
                                    >
                                        {item.name}
                                    </Option>)
                            })}
                        </Select>
                    </Form.Item>
                    <Form.Item key={21} >
                        <Space key={22}>
                            <Button
                                key={23}
                                type="primary"
                                onClick={onCloseModal}
                                htmlType="submit"
                                className="login-form-button"
                            >
                                {t("common:ButtonSave")}
                            </Button>
                            <Button key={24}
                                onClick={onCloseModal}
                                type="default"
                                className="login-form-button"
                            >
                                {t("common:ButtonClose")}
                            </Button>
                        </Space>
                    </Form.Item>
                </Form>
            </Modal>
            <Space style={{ marginTop: "16px" }}>
                <Button
                    type="primary"
                    onClick={() => approveUsers(selectedUsersId.selectedRowKeys)}
                >
                    {t("common:Confirm")}
                </Button>
                <Button
                    type="primary"
                    onClick={() => approveAllUsers()}
                >
                    {t("common:ConfirmAll")}
                </Button>
                <Button
                    type="default"
                    onClick={() => notApproveUsers(selectedUsersId.selectedRowKeys)}
                >
                    {t("common:NotConfirm")}
                </Button>
                <Button
                    type="default"
                    onClick={() => notApproveAllUsers()}
                >
                    {t("common:NotConfirmAll")}
                </Button>
            </Space>
        </React.Fragment>
    );

}

export default Users;
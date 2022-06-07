import { Button, Form, Input, Modal, Select, Space, Spin, Table, Tabs } from "antd";
import { useForm } from "antd/lib/form/Form";
import { t } from "i18next";
import { FunctionComponent, useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next";
import { sortedString } from "../../helpers/sortedHelper";
import scaleService from "../../services/scale.service";
import uniqueScaleService from "../../services/uniquescale.service";

const { TabPane } = Tabs;
const { Option } = Select;

const Scales: FunctionComponent = () => {
    const { i18n } = useTranslation("common");
    const [forceUpdate, setForceUpdate] = useState(false)
    const [spotData, setSpotData] = useState<any>([])
    const [futuresData, setFuturesData] = useState<any>([])
    const [uniquesData, setUniquesData] = useState<any>([])
    const [selectedUniquesId, setSelecteddUniquesId] = useState({
        selectedRowKeysUniques: [],
        loading: false
    });
    const [selectedSpotsId, setSelectedSpotsId] = useState({
        selectedRowKeysSpots: [],
        loading: false
    });
    const [selectedFuturesId, setSelectedFuturesId] = useState({
        selectedRowKeysFutures: [],
        loading: false
    });
    const [IsLoading, setIsLoading] = useState(false)
    const [isModalSpotsVisible, setIsModalSpotsVisible] = useState(false);
    const [isModalUniqueVisible, setIsModalUniqueVisible] = useState(false);
    const [isModalFutureVisible, setIsModalFutureVisible] = useState(false);
    const [selectedRowData, setSelectedRowData] = useState<any>()
    const [selectedRowFuturesData, setSelectedRowFuturesData] = useState<any>()
    const [selectedRowUniqueData, setSelectedRowUniqueData] = useState<any>()
    const isNewSpotsData = useRef<boolean>();
    const isNewFutureData = useRef<boolean>();
    const isNewUniqueData = useRef<boolean>();

    const [spotsForm] = useForm();
    const [futuresForm] = useForm();
    const [uniqueForm] = useForm();

    useEffect(() => {
        setIsLoading(true)
        const resultSpot: any[] = [];
        const resultFutures: any[] = [];
        const resultUnique: any[] = [];
        scaleService.getScaleSpotData().then((data) => {
            data.length && data?.map((item: any) => {
                resultSpot.push(
                    {
                        id: item?.id,
                        key: item?.id,
                        fromValue: item ? item?.fromValue : t("common:noData"),
                        percent: item ? item?.percent : t("common:noData"),
                        uniqueId: item?.uniqueId ? item?.unique.name : t("common:noData"),
                    }
                )
                return setSpotData(resultSpot)
            })
        })
        scaleService.getScaleFuturesData().then((data) => {
            data.length && data?.map((item: any) => {
                resultFutures.push(
                    {
                        id: item?.id,
                        key: item?.id,
                        fromValue: item ? item?.fromValue : t("common:noData"),
                        percent: item ? item?.percent : t("common:noData"),
                        uniqueId: item?.uniqueId ? item?.unique.name : t("common:noData"),
                    }
                )
                return setFuturesData(resultFutures)
            })
        })
        uniqueScaleService.getUniqueData().then((data) => {
            data.length && data?.map((item: any) => {
                resultUnique.push(
                    {
                        id: item?.id,
                        key: item?.id,
                        name: item?.name ? item?.name : t("common:noData"),
                    }
                )
                return setUniquesData(resultUnique)
            })
        })
        setIsLoading(false)
        setForceUpdate(false)
    }, [forceUpdate])

    const columnSpots = [
        {
            title: t("common:From"),
            dataIndex: "fromValue",
            key: "fromValue",
            sorter: (a: any, b: any) => a.fromValue - b.fromValue,
        },
        {
            title: t("common:Percent"),
            dataIndex: "percent",
            key: "percent",
            sorter: (a: any, b: any) => a.percent - b.percent,
        },
        {
            title: t("common:TableUnique"),
            dataIndex: "uniqueId",
            key: "uniqueId",
            sorter: ((a: { uniqueId: any; }, b: { uniqueId: any; }) => sortedString(a.uniqueId, b.uniqueId)),
        },
    ]

    const columnFutures = [
        {
            title: t("common:From"),
            dataIndex: "fromValue",
            key: "fromValue",
            sorter: (a: any, b: any) => a.fromValue - b.fromValue,
        },
        {
            title: t("common:Percent"),
            dataIndex: "percent",
            key: "percent",
            sorter: (a: any, b: any) => a.percent - b.percent,
        },
        {
            title: t("common:TableUnique"),
            dataIndex: "uniqueId",
            key: "uniqueId",
            sorter: ((a: { uniqueId: any; }, b: { uniqueId: any; }) => sortedString(a.uniqueId, b.uniqueId)),
        },
    ]

    const columnUniques = [
        {
            title: t("common:TableName"),
            dataIndex: "name",
            key: "name",
            sorter: ((a: { name: any; }, b: { name: any; }) => sortedString(a.name, b.name)),
        },
    ]
    const { selectedRowKeysUniques } = selectedUniquesId;
    const rowSelectionUniquessId = {
        selectedRowKeysUniques,
        onChange: (selectedRowKeysUniques: any) => {
            setSelecteddUniquesId({
                ...selectedUniquesId,
                selectedRowKeysUniques: selectedRowKeysUniques
            });
        }
    };
    const { selectedRowKeysSpots } = selectedSpotsId;
    const rowSelectionSpotsId = {
        selectedRowKeysSpots,
        onChange: (selectedRowKeysSpots: any) => {
            setSelectedSpotsId({
                ...selectedSpotsId,
                selectedRowKeysSpots: selectedRowKeysSpots
            });
        }
    };
    const { selectedRowKeysFutures } = selectedFuturesId;
    const rowSelectionFuturesId = {
        selectedRowKeysFutures,
        onChange: (selectedRowKeysFutures: any) => {
            setSelectedFuturesId({
                ...selectedFuturesId,
                selectedRowKeysFutures: selectedRowKeysFutures
            });
        }
    };

    useEffect(() => {
        spotsForm.resetFields()
        spotsForm.setFieldsValue(selectedRowData)
    }, [selectedRowData, spotsForm])

    useEffect(() => {
        uniqueForm.resetFields()
        uniqueForm.setFieldsValue(selectedRowUniqueData)
    }, [selectedRowUniqueData, uniqueForm])

    useEffect(() => {
        futuresForm.resetFields()
        futuresForm.setFieldsValue(selectedRowFuturesData)
    }, [selectedRowFuturesData, futuresForm])

    const onCloseSpotsModal = () => {
        setIsModalSpotsVisible(false)
    }

    const onCloseFutureModal = () => {
        setIsModalFutureVisible(false)
    }

    const onCloseUniqueModal = () => {
        setIsModalUniqueVisible(false)
    }

    const createSpotsData = (data: any) => {
        const uniqueId = uniquesData?.find((item: any) => {
            if (item.name === data.uniqueId) {
                return item.id
            }
        })
        if (!isNewSpotsData.current) {
            scaleService.updateScalePostsData({
                id: selectedRowData.id,
                fromValue: +data.fromValue,
                uniqueId: uniqueId.id ?? data.uniqueId,
                percent: +data.percent
            })
            setSpotData(spotData.map((item: any) => {
                if (item.id === selectedRowData.id) {
                    item.key = item.key - 0.5 + Math.random() * (selectedRowData.key - selectedRowData.key + 1)
                    item.fromValue = +data.fromValue;
                    item.uniqueId = data.uniqueId;
                    item.percent = +data.percent;
                    return item;
                } else {
                    return item;
                }
            }))
        } else {
            scaleService.createScalePostData({
                fromValue: data.fromValue,
                uniqueId: uniqueId.id ?? data.uniqueId,
                percent: data.percent
            })
        }
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
        setIsModalSpotsVisible(false)
        spotsForm.resetFields()
    }

    const createFutureData = (data: any) => {
        const uniqueId = uniquesData?.find((item: any) => {
            if (item.name === data.uniqueId) {
                return item.id
            }
        })
        if (!isNewFutureData.current) {
            scaleService.updateScaleFutureData({
                id: selectedRowFuturesData.id,
                fromValue: data.fromValue,
                uniqueId: uniqueId.id ?? data.uniqueId,
                percent: data.percent
            })
            setFuturesData(futuresData.map((item: any) => {
                if (item.id === selectedRowFuturesData.id) {
                    item.key = item.key - 0.5 + Math.random() * (selectedRowFuturesData.key - selectedRowFuturesData.key + 1)
                    item.fromValue = data.fromValue;
                    item.uniqueId = data.uniqueId;
                    item.percent = data.percent;
                    return item;
                } else {
                    return item;
                }
            }))
        } else {
            scaleService.createScaleFutureData({
                fromValue: data.fromValue,
                uniqueId: uniqueId.id,
                percent: data.percent
            })
        }
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
        setIsModalFutureVisible(false)
        futuresForm.resetFields()
    }

    const createUniqueData = (data: any) => {
        if (!isNewUniqueData.current) {
            uniqueScaleService.updateScaleUniqueData({
                id: selectedRowUniqueData.id,
                name: data.name
            })
            setUniquesData(uniquesData.map((item: any) => {
                if (item.id === selectedRowUniqueData.id) {
                    item.key = item.key - 0.5 + Math.random() * (selectedRowUniqueData.key - selectedRowUniqueData.key + 1)
                    item.name = data.name;
                    return item;
                } else {
                    return item;
                }
            }))
        } else {
            uniqueScaleService.createScaleUniqueData({
                name: data.name,
            })
        }
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
        setIsModalUniqueVisible(false)
        uniqueForm.resetFields()
    }

    const removeSpots = (array: any) => {
        const arrayWithoutDeleteElements = spotData.reduce((acc: any[], item: { id: any; }) => {
            if (!array.includes(item.id)) {
                acc.push(item)
            };
            return acc;
        }, []);
        setSpotData(arrayWithoutDeleteElements)
        scaleService.deleteScaleSpotData(array)
    }

    const removeFutures = (array: any) => {
        const arrayWithoutDeleteElements = futuresData.reduce((acc: any[], item: { id: any; }) => {
            if (!array.includes(item.id)) {
                acc.push(item)
            };
            return acc;
        }, []);
        setFuturesData(arrayWithoutDeleteElements)
        scaleService.deleteScaleFutureData(array)
    }

    const removeUniques = (array: any) => {
        const arrayWithoutDeleteElements = uniquesData.reduce((acc: any[], item: { id: any; }) => {
            if (!array.includes(item.id)) {
                acc.push(item)
            };
            return acc;
        }, []);
        setUniquesData(arrayWithoutDeleteElements)
        uniqueScaleService.deleteScaleUniqueData(array)
        setTimeout(() => {
            setForceUpdate(true)
        }, 2000)
    }

    const onRowClick = (record: any) => {
        setSelectedRowData(record)
        isNewSpotsData.current = false;
        setIsModalSpotsVisible(true)
    }

    const onRowFuturesClick = (record: any) => {
        setSelectedRowFuturesData(record)
        isNewFutureData.current = false;
        setIsModalFutureVisible(true)
    }

    const onRowUniqueClick = (record: any) => {
        setSelectedRowUniqueData(record)
        isNewUniqueData.current = false;
        setIsModalUniqueVisible(true)
    }

    return (
        <>
            <Tabs defaultActiveKey="1" type="card" >
                <TabPane tab="Spot" key="1">
                    <Table
                        locale={{
                            triggerDesc: t("common:TriggerDesc"),
                            triggerAsc: t("common:TriggerAsc"),
                            cancelSort: t("common:CancelSort")
                        }}
                        key={3}
                        loading={{ indicator: <Spin size="large" />, spinning: IsLoading }}
                        rowSelection={rowSelectionSpotsId}
                        columns={columnSpots}
                        dataSource={spotData}
                        scroll={{ y: 230, x: 400 }}
                        onRow={(record) => {
                            return {
                                onDoubleClick: (event) => onRowClick(record),
                                style: {
                                    backGround: "#FFFFFF",
                                    boxShadow: "inset 0px -1px 0px #E6E6E6",
                                }
                            };
                        }}
                    />
                    <Modal
                        key={7}
                        // forceRender={true}
                        footer={null}
                        onCancel={onCloseSpotsModal}
                        title={`${"Шкала"}`}
                        visible={isModalSpotsVisible}
                    >
                        <Form
                            key={8}
                            form={spotsForm}
                            onFinish={(data) => createSpotsData(data)}
                            initialValues={{
                                selectedRowData
                            }}
                        >
                            <span key={9} className="form-description">{`${t("common:From")}`}</span>
                            <Form.Item
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                key={7}
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                                name="fromValue"
                            >
                                <Input type="number" key={10} />
                            </Form.Item>
                            <span key={11} className="form-description">{`${t("common:Percent")}`}</span>
                            <Form.Item
                                key={12}
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                                name="percent"
                            >
                                <Input type="number" key={13} />
                            </Form.Item>
                            <span key={14} className="form-description">{`${t("common:TableUnique")}`}</span>
                            <Form.Item
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                key={15}
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                                name="uniqueId"
                            >
                                <Select
                                    key={10}
                                >
                                    {uniquesData.length && uniquesData?.map((item: any) => {
                                        return (
                                            <Option
                                                value={item.name}
                                                key={item.key}
                                            >
                                                {item.name}
                                            </Option>)
                                    })}
                                </Select>
                            </Form.Item>
                            <Space style={{ marginTop: "16px" }}>
                                <Button
                                    key={6}
                                    type="primary"
                                    htmlType="submit"
                                    className="login-form-button"
                                >
                                    {`${t("common:ButtonSave")}`}
                                </Button>
                                <Button key={7}
                                    type="default"
                                    className="login-form-button"
                                    onClick={onCloseSpotsModal}
                                >
                                    {`${t("common:ButtonClose")}`}
                                </Button>
                            </Space>
                        </Form>
                    </Modal>
                    <Space style={{ marginTop: "16px" }}>
                        <Button
                            key={6}
                            type="primary"
                            onClick={() => {
                                setIsModalSpotsVisible(true)
                                isNewSpotsData.current = true;
                            }}
                            className="login-form-button"
                        >
                            {`${t("common:ButtonCreate")}`}
                        </Button>
                        <Button key={7}
                            type="default"
                            className="login-form-button"
                            onClick={() => removeSpots(selectedSpotsId?.selectedRowKeysSpots)}
                        >
                            {`${t("common:ButtonDelete")}`}
                        </Button>
                    </Space>
                </TabPane>
                <TabPane tab="Futures" key="2">
                    <Table
                        locale={{
                            triggerDesc: t("common:TriggerDesc"),
                            triggerAsc: t("common:TriggerAsc"),
                            cancelSort: t("common:CancelSort")
                        }}
                        key={4}
                        loading={{ indicator: <Spin size="large" />, spinning: IsLoading }}
                        rowSelection={rowSelectionFuturesId}
                        columns={columnFutures}
                        dataSource={futuresData}
                        scroll={{ y: 230, x: 400 }}
                        onRow={(record) => {
                            return {
                                onDoubleClick: (event) => onRowFuturesClick(record),
                                style: {
                                    backGround: "#FFFFFF",
                                    boxShadow: "inset 0px -1px 0px #E6E6E6",
                                }
                            };
                        }}
                    />
                    <Modal
                        key={30}
                        footer={null}
                        onCancel={onCloseFutureModal}
                        title={`${"Шкала"}`}
                        visible={isModalFutureVisible}
                    >
                        <Form
                            key={31}
                            form={futuresForm}
                            onFinish={(data) => createFutureData(data)}
                            initialValues={{
                                selectedRowFuturesData
                            }}
                        >
                            <span key={32} className="form-description">{`${t("common:From")}`}</span>
                            <Form.Item
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                key={33}
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                                name="fromValue"
                            >
                                <Input type="number" key={34} />
                            </Form.Item>
                            <span key={35} className="form-description">{`${t("common:Percent")}`}</span>
                            <Form.Item
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                key={36}
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                                name="percent"
                            >
                                <Input type="number" key={37} />
                            </Form.Item>
                            <span key={14} className="form-description">{`${t("common:TableUnique")}`}</span>
                            <Form.Item
                                rules={[
                                    {
                                        required: true,
                                        message: `${t("common:ThisFieldRequired")}`,
                                    },
                                ]}
                                key={15}
                                style={{
                                    marginTop: "4px",
                                    marginBottom: "16px",
                                }}
                                name="uniqueId"
                            >
                                <Select
                                    key={10}
                                >
                                    {uniquesData.length && uniquesData?.map((item: any) => {
                                        return (
                                            <Option
                                                value={item.name}
                                                key={item.key}
                                            >
                                                {item.name}
                                            </Option>)
                                    })}
                                </Select>
                            </Form.Item>
                            <Space style={{ marginTop: "16px" }}>
                                <Button
                                    key={6}
                                    type="primary"
                                    htmlType="submit"
                                    className="login-form-button"
                                >
                                    {`${t("common:ButtonSave")}`}
                                </Button>
                                <Button key={7}
                                    type="default"
                                    className="login-form-button"
                                    onClick={onCloseFutureModal}
                                >
                                    {`${t("common:ButtonClose")}`}
                                </Button>
                            </Space>
                        </Form>
                    </Modal>
                    <Space style={{ marginTop: "16px" }}>
                        <Button
                            key={6}
                            type="primary"
                            onClick={() => {
                                setIsModalFutureVisible(true)
                                isNewFutureData.current = true;
                            }}
                            className="login-form-button"
                        >
                            {`${t("common:ButtonCreate")}`}
                        </Button>
                        <Button key={7}
                            type="default"
                            className="login-form-button"
                            onClick={() => removeFutures(selectedFuturesId?.selectedRowKeysFutures)}
                        >
                            {`${t("common:ButtonDelete")}`}
                        </Button>
                    </Space>
                </TabPane>
            </Tabs>
            <Table
                locale={{
                    triggerDesc: t("common:TriggerDesc"),
                    triggerAsc: t("common:TriggerAsc"),
                    cancelSort: t("common:CancelSort")
                }}
                key={20}
                loading={{ indicator: <Spin size="large" />, spinning: IsLoading }}
                rowSelection={rowSelectionUniquessId}
                columns={columnUniques}
                dataSource={uniquesData}
                style={{ marginTop: "32px", maxWidth: "400px" }}
                scroll={{ y: 170 }}
                onRow={(record) => {
                    return {
                        onDoubleClick: (event) => onRowUniqueClick(record),
                        style: {
                            backGround: "#FFFFFF",
                            boxShadow: "inset 0px -1px 0px #E6E6E6",
                        }
                    };
                }}
            />
            <Modal
                key={21}
                // forceRender={true}
                footer={null}
                onCancel={onCloseUniqueModal}
                title={`${"Unique"}`}
                visible={isModalUniqueVisible}
            >
                <Form
                    key={23}
                    form={uniqueForm}
                    onFinish={(data) => createUniqueData(data)}
                    initialValues={{
                        selectedRowUniqueData
                    }}
                >
                    <span key={22} className="form-description">{`${t("common:TableUnique")}`}</span>
                    <Form.Item
                        rules={[
                            {
                                required: true,
                                message: `${t("common:ThisFieldRequired")}`,
                            },
                        ]}
                        name="name"
                        key={24}
                        style={{
                            marginTop: "4px",
                            marginBottom: "16px",
                        }}

                    >
                        <Input type="text" key={23} />
                    </Form.Item>
                    <Space style={{ marginTop: "16px" }}>
                        <Button
                            key={24}
                            type="primary"
                            htmlType="submit"
                            className="login-form-button"
                        >
                            {`${t("common:ButtonSave")}`}
                        </Button>
                        <Button key={25}
                            type="default"
                            className="login-form-button"
                            onClick={onCloseUniqueModal}
                        >
                            {`${t("common:ButtonClose")}`}
                        </Button>
                    </Space>
                </Form>
            </Modal>
            <Space style={{ marginTop: "16px" }}>
                <Button
                    key={6}
                    type="primary"
                    onClick={() => {
                        setIsModalUniqueVisible(true)
                        isNewUniqueData.current = true;
                    }}
                    className="login-form-button"
                >
                    {`${t("common:ButtonCreate")}`}
                </Button>
                <Button key={7}
                    type="default"
                    className="login-form-button"
                    disabled={selectedUniquesId?.selectedRowKeysUniques.length > 1}
                    onClick={() => removeUniques(selectedUniquesId?.selectedRowKeysUniques)}
                >
                    {`${t("common:ButtonDelete")}`}
                </Button>
            </Space>
        </>
    );

}

export default Scales;
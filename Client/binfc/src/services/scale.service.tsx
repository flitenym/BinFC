import axios from "axios";

const API_URL = "scale";

const getScaleSpotData = async () => {
    return await axios
        .get(API_URL + "/spot")
        .then((response) => {
            return response.data
        })
};

const getScaleFuturesData = async () => {
    return await axios
        .get(API_URL + "/futures")
        .then((response) => {
            return response.data
        })
};

const deleteScaleSpotData = async (array: number[]) => {
    return await axios
        .post(API_URL + "/deletespot", array)
};

const deleteScaleFutureData = async (array: number[]) => {
    return await axios
        .post(API_URL + "/deletefutures", array)
};

const updateScalePostsData = async (data: any) => {
    return await axios
        .put(
            API_URL + `/updatespot/${data?.id}`, data,
        )
};

const updateScaleFutureData = async (data: any) => {
    return await axios
        .put(
            API_URL + `/updatefutures/${data?.id}`, data,
        )
};

const createScalePostData = async (data: any) => {
    return await axios
        .post(API_URL + "/createspot", data)
};

const createScaleFutureData = async (data: any) => {
    return await axios
        .post(API_URL + "/createfutures", data)
};

const scaleService = {
    getScaleSpotData,
    getScaleFuturesData,
    deleteScaleSpotData,
    deleteScaleFutureData,
    updateScalePostsData,
    updateScaleFutureData,
    createScalePostData,
    createScaleFutureData
}

export default scaleService;
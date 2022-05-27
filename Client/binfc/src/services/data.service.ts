import axios from "axios";

const API_URL = "data";

const getSpotData = async () => {
    return await axios
        .get(API_URL + "/spot")
        .then((response) => {
            return response.data
        })
};

const getFuturesData = async () => {
    return await axios
        .get(API_URL + "/futures")
        .then((response) => {
            return response.data
        })
};

const deleteSpotData = async (array: number[]) => {
    return await axios
        .post(API_URL + "/deletespot", array)
        .then((response) => {
            return response.data
        })
};

const deleteAllSpotData = async () => {
    return await axios
        .post(API_URL + "/deleteallspot")
        .then((response) => {
            return response.data
        })
};

const deleteFuturesData = async (array: number[]) => {
    return await axios
        .post(API_URL + "/deletefutures", array)
        .then((response) => {
            return response.data
        })
};

const deleteAllFuturesData = async () => {
    return await axios
        .post(API_URL + "/deleteallfutures")
        .then((response) => {
            return response.data
        })
};

const dataService = {
    getSpotData,
    getFuturesData,
    deleteSpotData,
    deleteFuturesData,
    deleteAllSpotData,
    deleteAllFuturesData
};

export default dataService;
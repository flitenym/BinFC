import axios from "axios";

const API_URL = "payhistory";

const getPayHistory = async () => {
    return axios
        .get(API_URL).then((response) => {
            return response.data
        })
};

export const payHistoryService = {
    getPayHistory
}
import axios from "axios";

const API_URL = "payment";

const getPaymentData = async () => {
    return axios
        .get(API_URL).then((response) => {
            return response
        })
};

const getBalanceData = async () => {
    return axios
        .get(API_URL + "/balance").then((response) => {
            return response
        })
};

const postPaymentData = async (data: any) => {
    return axios
        .post(API_URL, data).then((response) => {
            return response
        })
};


export const paymentService = {
    getPaymentData,
    getBalanceData,
    postPaymentData
}
import axios from "axios";

const API_URL = "payment";

const getPaymentData = async () => {
    return axios
        .get(API_URL).then((response) => {
            return response.data
        })
};

const getBalanceData = async () => {
    return axios
        .get(API_URL + "/balance").then((response) => {
            return response.data
        })
};

const postPaymentData = async (data: any) => {
    return axios
        .post(API_URL, {
            body: data
        }).then((response) => {
            console.log(response);
            return response.data
        })
};


export const paymentService = {
    getPaymentData,
    getBalanceData,
    postPaymentData
}
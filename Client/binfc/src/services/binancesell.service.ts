import axios from "axios";

const API_URL = "binancesell";

const start = async () => {
    return axios
        .get(API_URL + "/start", {
        })
};

const stop = async () => {
    return axios
        .get(API_URL + "/stop", {
        })
};

const restart = async () => {
    return axios
        .get(API_URL + "/restart", {
        })
};

export const binancesellService = {
    start,
    stop,
    restart
};

export default binancesellService;
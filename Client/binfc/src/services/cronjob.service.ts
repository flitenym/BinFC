import axios from "axios";

const API_URL = "cronjob";

const check = async (cronExpression: string) => {
    return await axios
        .get(API_URL + "/check", {
            params: {cron:cronExpression}
        })
        .then((response) => {        
            return response.status
        })
};

const next = async (cronExpression: string) => {
    return await axios
        .get(API_URL + "/next", {
            params: {cron:cronExpression}
        })
        .then((response) => {        
            return response
        })
};


const cronService = {
    check,
    next
};

export default cronService;
import axios from "axios";

const API_URL = "unique";

const getUnique = async () => {
    return await axios
        .get(API_URL)
        .then((response) => {
            return response.data
        })
};

const uniqueService = {
    getUnique,
};

export default uniqueService;
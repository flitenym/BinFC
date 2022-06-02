import axios from "axios";

const API_URL = "unique";

const getUniqueData = async () => {
    return await axios
        .get(API_URL)
        .then((response) => {
            return response.data
        })
};

const deleteScaleUniqueData = async (key: number) => {
    return await axios
        .delete(API_URL + `/${key}`)
};

const updateScaleUniqueData = async (data: any) => {
    return await axios
        .put(
            API_URL+ `/${data.id}`, data,
        )
};

const createScaleUniqueData = async (data: any) => {
    return await axios
        .post(API_URL, data)
};

const uniqueScaleService = {
    getUniqueData,
    deleteScaleUniqueData,
    updateScaleUniqueData,
    createScaleUniqueData
}

export default uniqueScaleService;
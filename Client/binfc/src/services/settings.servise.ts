import axios from "axios";

const API_URL = "settings";

const getSettings = async () => {
    return await axios
        .get(API_URL)
        .then((response) => {
            return response.data
        })
};

const saveSettings = async (formData: any) => {
    return await axios
        .post(API_URL,
            formData, {
            headers: { 'Content-Type': 'multipart/form-data' },
        }
        )
        .then((response) => {
            return response.data
        })
};

const settingsService = {
    getSettings,
    saveSettings
};

export default settingsService;
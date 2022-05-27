import axios from "axios";

const API_URL = "import";

const importData = async (formData: any) => {
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

export const importService = {
    importData,
};

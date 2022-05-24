import axios from "axios";

const API_URL = "userInfo";

const getUsers = async () => {
    return await axios
        .get(API_URL)
        .then((response) => {
            return response.data
        })
};

// const saveSettings = async (formData: any) => {
//     return await axios
//         .post(API_URL,
//             formData, {
//             headers: { 'Content-Type': 'multipart/form-data' },
//         }
//         )
//         .then((response) => {
//             return response.data
//         })
// };

const usersService = {
    getUsers,
    // saveSettings
};

export default usersService;
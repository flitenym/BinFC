import axios from "axios";

const API_URL = "userInfo";

const getUsers = async () => {
    return await axios
        .get(API_URL)
        .then((response) => {
            return response.data
        })
};

const upadeteUser = async (data: any) => {
    return await axios
        .put(
        API_URL + `/${data?.id}`, data,
    )
};

const usersService = {
    getUsers,
    upadeteUser
};

export default usersService;
import axios from "axios";

const API_URL = "userinfo";

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

const approveUsers = async (usersId: number[]) => {
    return await axios
        .post(API_URL + "/approve", usersId)
        .then((response) => {
            return response.data
        })
};

const approveAllUsers = async () => {
    return await axios
        .post(API_URL + "/approveall")
};

const notApproveAllUsers = async () => {
    return await axios
        .post(API_URL + "/notapproveall")
};


const notApproveUsers = async (usersId: number[]) => {
    return await axios
        .post(API_URL + "/notapprove", usersId)
        .then((response) => {
            return response.data
        })
};

const usersService = {
    getUsers,
    upadeteUser,
    approveUsers,
    notApproveUsers,
    approveAllUsers,
    notApproveAllUsers
};

export default usersService;
import axios from "axios";

const API_URL = "export";

const exportPayHistory = async (ids: number[]) => {
    const FileDownload = require('js-file-download');
    return axios
        .post(API_URL + "/payhistory", {
            ids: ids,
            responseType: 'blob',

        }).then((response: any) => {
            if (response.status == 200){
                const typeFile = response?.headers?.['content-type'].split('/')[1]
                FileDownload(response.data, `Table.${typeFile}`, 'text/csv;charset=utf-8', '\uFEFF')
            }
        })
};

export const exportPayHistoryService = {
    exportPayHistory
}
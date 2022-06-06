import { t } from "i18next";

export const sortedString = (a: any, b: any) => {
    if (b === t("common:noData") && a !== t("common:noData")) {
        return -1;
    }
    if (a === t("common:noData") && b !== t("common:noData")) {
        return 1;
    }
    if (a !== t("common:noData") && b !== t("common:noData")) {
        return a.localeCompare(b);
    }
    return 0;
};

export const sortedNumbers = (a: any, b: any) => {
    if (b < a) {
        return -1;
    }
    if (b > a) {
        return 1;
    }
    if (typeof (a) === "string" && typeof (b) !== "string") {
        return 1;
    }
    if (typeof (b) === "string" && typeof (a) !== "string") {
        return -1;
    }
    return 0;
}


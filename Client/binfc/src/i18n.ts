import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import ru from "./locales/ru/ru";
import en from "./locales/en/en";

i18n
    .use(initReactI18next)
    .init({
        resources: {
            ru: ru,
            en: en,
          },
        fallbackLng: "ru",
        debug: false,
        detection: {
            order: ["localStorage", "cookie"],
            caches: ["localStorage", "cookie"],
        },
        interpolation: {
            escapeValue: false,
        },
    });

export default i18n;

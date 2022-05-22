import {
    LOG_IN_SUCCESS,
    LOG_OUT_SUCCESS,
    LANGUAGE_CHANGE,
    Token,
    Language,
    AuthActionTypes,
    LanguageActionTypes,
} from './types';

export const initCommonState: Token = {
    username: null,
    token: null
};

export const languageState: Language = {
    locale: null,
};

export const authReducer = (state = initCommonState, action: AuthActionTypes) => {
    switch (action.type) {
        case LOG_IN_SUCCESS: {
            return {
                ...state,
                username: action?.payload?.username,
                token: action?.payload?.token
            };
        }
        case LOG_OUT_SUCCESS: {
            return {
                ...state,
                username: null,
                token: null
            }
        }
        default:
            return state;
    }
}

export const languageReducer = (state = languageState, action: LanguageActionTypes) => {
    switch (action.type) {
        case LANGUAGE_CHANGE: {
            return {
                ...state,
                locale: action?.payload?.locale,
            };
        }
        default:
            return state;
    }
}
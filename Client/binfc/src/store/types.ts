/* Login */
export const LOG_IN_SUCCESS = 'LOG_IN_SUCCESS';
export const LOG_OUT_SUCCESS = 'LOG_OUT_SUCCESS';

/* Language */
export const LANGUAGE_CHANGE = 'LANGUAGE_CHANGE';

/* Mode */
export const MODE_CHANGE = 'MODE_CHANGE';

export interface AuthActionTypes {
    type: typeof LOG_IN_SUCCESS | typeof LOG_OUT_SUCCESS;
    payload: Token
}

export interface LanguageActionTypes {
    type: typeof LANGUAGE_CHANGE;
    payload: Language
}

export interface themeActionTypes {
    type: typeof MODE_CHANGE;
    payload: ThemeMode
}

export interface Language {
    locale: string | null;
}

export interface ThemeMode {
    mode: string | null;
}
export interface Token {
    username?: string | null;
    token?: string | null;
}

export interface AuthState {
    isSubmitting?: boolean;
    currentUser?: object | null;
    error?: Error | null;
}


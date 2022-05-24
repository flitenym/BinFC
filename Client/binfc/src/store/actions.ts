import {
  Token,
  LOG_IN_SUCCESS,
  LOG_OUT_SUCCESS,
  LANGUAGE_CHANGE,
  MODE_CHANGE,
  Language,
  ThemeMode
} from './types';

export function logInSuccess(data: Token) {
  return {
    type: LOG_IN_SUCCESS,
    payload: data,
  };
}

export function logOutSuccess() {
  return {
    type: LOG_OUT_SUCCESS,
  };
}

export function languageChange(data: Language) {
  return {
    type: LANGUAGE_CHANGE,
    payload: data,
  };
}

export function modeChange(data: ThemeMode) {
  return {
    type: MODE_CHANGE,
    payload: data,
  };
}
import { combineReducers, applyMiddleware } from 'redux';
import { legacy_createStore as createStore } from 'redux'
import { composeWithDevTools } from 'redux-devtools-extension';
import thunk from 'redux-thunk';
import { authReducer, languageReducer } from './reducers';

const rootReducer = combineReducers({
    authState: authReducer,
    languageState: languageReducer,
});

export const store = createStore(rootReducer, composeWithDevTools(applyMiddleware(thunk)));
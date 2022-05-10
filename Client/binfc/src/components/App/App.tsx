import React from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.scss';
import Dashboard from '../Dashboard/Dashboard';
import Login from '../Login/Login';
import useToken from './useToken';

function App() {
  debugger
  const { token, setToken } = useToken();

  if(!token) {
    return <Login setToken={setToken} />
  }

  return (
    <div className="wrapper">
      <h1>Application</h1>
      <BrowserRouter>
        <Routes>
          <Route path="/dashboard">
            <Dashboard />
          </Route>
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;

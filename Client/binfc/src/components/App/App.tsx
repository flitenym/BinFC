import React from 'react';
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import Dashboard from '../Dashboard/Dashboard';
import Login from '../Login/Login';
import useToken from '../Auth/UseToken';

function App() {
  const { token, setToken } = useToken();

  if(!token) {
    return <Login setToken={setToken} />
  }

  return (
      <BrowserRouter>
        <Routes>
          <Route path="/dashboard" element={<Dashboard/>} />
          <Route path="/" element={<Navigate replace to="/dashboard" />} />
          <Route path="*" element={<Navigate to ="/dashboard" />}/>
        </Routes>
      </BrowserRouter>
  );
}

export default App;

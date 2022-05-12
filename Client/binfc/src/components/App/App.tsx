import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import Dashboard from '../Dashboard/Dashboard';
import Login from '../Login/Login';
import AuthService from "../../services/auth.service";

function App() {

  const logOut = () => {
    AuthService.logout();
  };

  return (
      <BrowserRouter>
        <Routes>
          <Route path="/dashboard" element={<Dashboard/>} />
          <Route path="/" element={<Navigate replace to="/dashboard" />} />
          <Route path="*" element={<Navigate to ="/dashboard" />}/>
          <Route path="/login" element ={<Login />}/>
        </Routes>
      </BrowserRouter>
  );
}

export default App;
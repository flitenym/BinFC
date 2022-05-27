import { Route, Routes } from "react-router-dom";
import DashBoard from "../components/DashBoard";
import Login from "../components/Login";
import PaymentsSettings from "../components/Settings";

const Routers = () => {
  return (
    <Routes>
      {/* <Route
        key="1"
        // name="Dashboard"
        path="/dashboard/*"
        element={<DashBoard />}
      />
      <Route
        key="2"
        // name="Login"
        path="/login"
        // component={Login}
        element={<Login />}
      /> */}
      <Route
        key="3"
        path="/dashboard/PaymentsSettings"
        element={<PaymentsSettings />}
      />
    </Routes>
  )
}

export default Routers;

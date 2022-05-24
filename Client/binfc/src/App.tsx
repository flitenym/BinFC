import { FunctionComponent, useEffect } from "react";
import { Route, Routes, Navigate } from "react-router-dom";
import DashBoard from "./components/DashBoard";
import PaymentsSettings from "./components/PaymentsSettings";
import Users from "./components/Users";
import Payout from "./components/Payout";
import PaymentHistory from "./components/PaymentHistory";
import BinanceSettings from "./components/BinanceSettings";
import LoginPage from "./components/Login";
import { modeChange } from "./store/actions";
import { useDispatch, useSelector } from "react-redux";
import { useThemeSwitcher } from "react-css-theme-switcher";


const App: FunctionComponent = () => {
  const lightThemeIsSelected = localStorage.getItem("theme")
  const mode = useSelector((state: any) => state?.themeState?.mode === "Light")
  const { switcher, themes } = useThemeSwitcher();
  const dispatch = useDispatch()

  useEffect(() => {
    if (lightThemeIsSelected  === "Light") {
      dispatch(modeChange({ mode: "Light" }))
      switcher({ theme: themes.light });
    } else {
      dispatch(modeChange({ mode: "Dark" }))
      switcher({ theme: themes.dark });
    }
  }, [])

  return (
        <Routes>
          <Route key={0} path="/" element={<Navigate replace to="/login" />} />
          <Route key={1} path={`/dashboard/*`} element={<DashBoard />} >
            <Route key={2} path={`/dashboard/*/PaymentsSettings`} element={<PaymentsSettings />} />
            <Route key={3} path={`/dashboard/*Users`} element={<Users />} />
            <Route key={4} path={`/dashboard/*Payout`} element={<Payout />} />
            <Route key={5} path={`/dashboard/*PaymentHistory`} element={<PaymentHistory />} />
            <Route key={6} path={`/dashboard/*BinanceSettings`} element={<BinanceSettings />} />
          </Route>
          <Route key={7} path="/login" element={<LoginPage />} />
        </Routes>
  );
}

export default App;



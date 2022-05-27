import { FunctionComponent, useEffect } from "react"
import { useNavigate } from "react-router-dom";
import NavBar from "../NavBar";
import Header from "../Header";
import { logInSuccess } from "../../store/actions";
import { useDispatch } from "react-redux";
import { Layout } from "antd";


interface IProps { }

const DashBoard: FunctionComponent<IProps> = () => {
    const navigate = useNavigate();
    const token = localStorage.getItem("token");
    const username = localStorage.getItem("username");
    const dispatch = useDispatch()

    useEffect(() => {
        if (!token) {
            navigate("/login");
        } else {
            dispatch(logInSuccess({ username, token }));
        }
    }, [dispatch, navigate, token])

    return (
        <Layout key={1} style={{ display: "flex" }} >
            <Header key={3} />
            <NavBar key={2} />
        </Layout>
    )
}

export default DashBoard

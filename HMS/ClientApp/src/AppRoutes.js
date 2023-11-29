import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Main } from "./components/Main";
import { Login } from "./components/Login";
import { Appointments } from "./components/Appointments";


const AppRoutes = [
    {
        index: true,
        element: <Login />
    },
    {
        path: '/appointments',
        element: <Appointments />
    },
    {
        path: '/main',
        element: <Main />
    }
];

export default AppRoutes;

import { Main } from "./components/Main";
import { Login } from "./components/Login";
import { Appointments } from "./components/Appointments";
import { Prescriptions } from "./components/Prescriptions";
import { Journal } from "./components/Journal";
import { Rating } from "./components/Ratings";

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
        path: '/prescriptions',
        element: <Prescriptions />
    },
    {
        path: '/journal',
        element: <Journal />
    },
    {
        path: '/main',
        element: <Main />
    },
    {
        path: '/ratings',
        element: <Rating />
    }
];

export default AppRoutes;

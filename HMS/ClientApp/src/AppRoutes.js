import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Main } from "./components/Main";
import { Login } from "./components/Login";


const AppRoutes = [
  {
    index: true,
    element: <Login />
  },
  {
    //path: '/counter',
    //element: <Counter />
  },
  {
    //path: '/fetch-data',
    //element: <FetchData />
    },
    {
        path: '/main',
        element: <Main />
    }
];

export default AppRoutes;

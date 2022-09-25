import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { ACSTokenDemo } from "./components/ACSTokenDemo";
import { Home } from "./components/Home";
import { Join } from './components/Join';
import { Rooms } from './components/Rooms';
import ChatList from './components/ChatList';
import ChatDetails from './components/ChatDetails';
import UserList from './components/UserList';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    requireAuth: true,
    element: <FetchData />
  },
  {
    path:'/join',
    element: <Join />
  },
  {
    path:'/rooms',
    requireAuth: true,
    element: <Rooms />
  },
  {
    path: '/token',
    requireAuth: true,
    element: <ACSTokenDemo />
  },
  {
    path: '/chats',
    requireAuth: true,
    element: <ChatList />
  },
  {
    path: '/chats/:threadId',
    requireAuth: true,
    element: <ChatDetails />
  },
  {
    path: '/users',
    requireAuth: true,
    element: <UserList />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;

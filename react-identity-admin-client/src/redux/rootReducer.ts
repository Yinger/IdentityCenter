import { combineReducers } from "redux";
import role from "../features/role/redux/reducer";
import user from "../features/user/redux/reducer";

const reducers = {
  role,
  user,
};

export default combineReducers(reducers);

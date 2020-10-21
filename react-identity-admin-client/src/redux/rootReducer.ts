import { combineReducers } from "redux";
import role from "../features/role/redux/reducer";

const reducers = {
  role,
};

export default combineReducers(reducers);

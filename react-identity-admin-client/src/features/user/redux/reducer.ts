import { State } from "../../../interface/user";
import { Action } from "../../../interface/types";
import { reducer as getUserListReducer } from "./actionWithReducer/getUserList";

import { USER_SEARCH } from "../../../constants/actions";

const initialState: State = {
  userList: undefined,
};

export default function reducer(state = initialState, action: Action) {
  switch (action.type) {
    // Handle cross-topic actions here
    case USER_SEARCH:
      return getUserListReducer(state, action);
    default:
      return { ...state };
  }
}

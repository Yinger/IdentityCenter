import { reducer as getRoleListReducer } from "./actionWithReducer/getRoleList";
import { Action } from "../../../interface/types";
import { State } from "../../../interface/role";

import { ROLE_SEARCH } from "../../../constants/actions";

const initialState: State = {
  roleList: undefined,
};

export default function reducer(state = initialState, action: Action) {
  switch (action.type) {
    // Handle cross-topic actions here
    case ROLE_SEARCH:
      return getRoleListReducer(state, action);
    default:
      return { ...state };
  }
}

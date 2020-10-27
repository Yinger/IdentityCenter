import { State } from "../../../interface/user";
import { Action } from "../../../interface/types";
import { reducer as getUserListReducer } from "./actionWithReducer/getUserList";
import { reducer as getUserRoleNameListReducer } from "./actionWithReducer/getRoleNameList";
import { reducer as getUserClaimNameListReducer } from "./actionWithReducer/getClaimNameList";
import { reducer as updateUserReducer } from "./actionWithReducer/updateUser";

import { USER_SEARCH } from "../../../constants/actions";
import { USER_GET_ROLENAME_ALL } from "../../../constants/actions";
import { USER_GET_CLAIMNAME_ALL } from "../../../constants/actions";
import { USER_UPDATE } from "../../../constants/actions";

const initialState: State = {
  userList: undefined,
  roleNameList: [],
  claimNameList: [],
};

export default function reducer(state = initialState, action: Action) {
  switch (action.type) {
    // Handle cross-topic actions here
    case USER_SEARCH:
      return getUserListReducer(state, action);
    case USER_GET_ROLENAME_ALL:
      return getUserRoleNameListReducer(state, action);
    case USER_GET_CLAIMNAME_ALL:
      return getUserClaimNameListReducer(state, action);
    case USER_UPDATE:
      return updateUserReducer(state, action);
    default:
      return { ...state };
  }
}

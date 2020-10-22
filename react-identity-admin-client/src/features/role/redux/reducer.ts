import { reducer as getRoleListReducer } from "./actionWithReducer/getRoleList";
import { reducer as getCreateRoleReducer } from "./actionWithReducer/createRole";
import { reducer as getUpdateRoleReducer } from "./actionWithReducer/updateRole";
import { reducer as getDeleteRoleReducer } from "./actionWithReducer/deleteRole";
import { Action } from "../../../interface/types";
import { State } from "../../../interface/role";

import {
  ROLE_SEARCH,
  ROLE_CREATE,
  ROLE_UPDATE,
  ROLE_DELETE,
} from "../../../constants/actions";

const initialState: State = {
  roleList: undefined,
};

export default function reducer(state = initialState, action: Action) {
  switch (action.type) {
    // Handle cross-topic actions here
    case ROLE_SEARCH:
      return getRoleListReducer(state, action);
    case ROLE_CREATE:
      return getCreateRoleReducer(state, action);
    case ROLE_UPDATE:
      return getUpdateRoleReducer(state, action);
    case ROLE_DELETE:
      return getDeleteRoleReducer(state, action);
    default:
      return { ...state };
  }
}

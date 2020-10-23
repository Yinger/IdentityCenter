import { Dispatch } from "redux";
import { Action } from "../../../../interface/types";
import { ROLE_SEARCH } from "../../../../constants/actions";
import { RoleRequest, State } from "../../../../interface/role";
import { ROLE_SEARCH_URL } from "../../../../constants/urls";
import { post } from "../../../../utils/request";

export function getRoleList(param: RoleRequest, callback: () => void) {
  return (dispatch: Dispatch) => {
    post(ROLE_SEARCH_URL, param).then((res) => {
      dispatch({
        type: ROLE_SEARCH,
        payload: res.data,
      });
      callback();
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case ROLE_SEARCH:
      return {
        ...state,
        roleList: action.payload,
      };

    default:
      return state;
  }
}

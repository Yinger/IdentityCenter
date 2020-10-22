import { Dispatch } from "redux";
import { Action } from "../../../../interface/types";
import { ROLE_SEARCH } from "../../../../constants/actions";
import { RoleRequest, State } from "../../../../interface/role";
import { SEARCH_ROLE_URL } from "../../../../constants/urls";
import { post } from "../../../../utils/request";

export function getRoleList(param: RoleRequest, callback: () => void) {
  return (dispatch: Dispatch) => {
    post(SEARCH_ROLE_URL, param).then((res) => {
      // console.log(res.data);
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

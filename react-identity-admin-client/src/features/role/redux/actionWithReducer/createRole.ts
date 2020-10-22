import { Dispatch } from "redux";
import { post } from "../../../../utils/request";
import { Action } from "../../../../interface/types";
import { RoleInfo, RoleCreateRequest, State } from "../../../../interface/role";
import { ROLE_CREATE_URL } from "../../../../constants/urls";
import { ROLE_CREATE } from "../../../../constants/actions";

export function createRole(param: RoleCreateRequest, callback: () => void) {
  param.id = "-1";
  return (dispatch: Dispatch) => {
    post(ROLE_CREATE_URL, param).then((res) => {
      dispatch({
        type: ROLE_CREATE,
        payload: {
          // id: "-1",
          roleName: param.roleName,
          tag: param.tag,
          description: param.description,
          ...res.data,
        },
      });
      callback();
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case ROLE_CREATE:
      let newList = [action.payload, ...(state.roleList as RoleInfo[])];
      return {
        ...state,
        roleList: newList,
      };

    default:
      return { ...state };
  }
}

import { Dispatch } from "redux";
import _ from "lodash";
import { put } from "../../../../utils/request";
import { Action } from "../../../../interface/types";
import { RoleInfo, RoleUpdateRequest, State } from "../../../../interface/role";
import { ROLE_UPDATE_URL } from "../../../../constants/urls";
import { ROLE_UPDATE } from "../../../../constants/actions";

export function updateRole(param: RoleUpdateRequest, callback: () => void) {
  return (dispatch: Dispatch) => {
    put(ROLE_UPDATE_URL, param).then((res) => {
      dispatch({
        type: ROLE_UPDATE,
        payload: param,
      });
      callback();
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case ROLE_UPDATE:
      let updatedList = [...(state.roleList as RoleInfo[])];
      let item: RoleUpdateRequest = action.payload;
      let index = _.findIndex(updatedList, {
        id: item.id,
      });
      updatedList[index] = {
        id: item.id,
        roleName: item.roleName,
        tag: item.tag,
        description: item.description,
      };
      return {
        ...state,
        roleList: updatedList,
      };

    default:
      return state;
  }
}

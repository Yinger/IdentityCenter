import { Dispatch } from "redux";
import _ from "lodash";
import { remove } from "../../../../utils/request";
import { Action } from "../../../../interface/types";
import { RoleInfo, RoleDeleteRequest, State } from "../../../../interface/role";
import { ROLE_DELETE_URL } from "../../../../constants/urls";
import { ROLE_DELETE } from "../../../../constants/actions";

export function deleteRole(param: RoleDeleteRequest) {
  return (dispatch: Dispatch) => {
    let url = ROLE_DELETE_URL + "?id=" + param.id;
    remove(url).then((res) => {
      dispatch({
        type: ROLE_DELETE,
        payload: param.id,
      });
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case ROLE_DELETE:
      let reducedList = [...(state.roleList as RoleInfo[])];
      _.remove(reducedList, (item: RoleInfo) => {
        return item.id === action.payload;
      });
      return {
        ...state,
        roleList: reducedList,
      };

    default:
      return { ...state };
  }
}

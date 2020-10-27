import { Dispatch } from "redux";
import _ from "lodash";
import { post } from "../../../../utils/request";
import { Action } from "../../../../interface/types";
import { UserInfo, UserUpdateRequest, State } from "../../../../interface/user";
import { USER_UPDATE_URL } from "../../../../constants/urls";
import { USER_UPDATE } from "../../../../constants/actions";

export function updateUser(param: UserUpdateRequest, callback: () => void) {
  return (dispatch: Dispatch) => {
    post(USER_UPDATE_URL, param).then((res) => {
      dispatch({
        type: USER_UPDATE,
        payload: param,
      });
      callback();
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case USER_UPDATE:
      let updatedList = [...(state.userList as UserInfo[])];
      let item: UserUpdateRequest = action.payload;
      let index = _.findIndex(updatedList, {
        id: item.id,
      });
      updatedList[index] = {
        id: item.id,
        loginName: item.loginName,
        email: item.email,
        lgCode: item.lgCode,
        lgKaKakari: item.lgKaKakari,
        listRole: item.listRole,
        listClaim: item.listClaim,
      };
      return {
        ...state,
        userList: updatedList,
      };

    default:
      return state;
  }
}

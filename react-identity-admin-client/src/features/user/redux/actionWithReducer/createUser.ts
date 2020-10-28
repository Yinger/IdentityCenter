import { Dispatch } from "redux";
import { post } from "../../../../utils/request";
import { Action } from "../../../../interface/types";
import { UserInfo, UserCreateRequest, State } from "../../../../interface/user";
import { USER_CREATE_URL } from "../../../../constants/urls";
import { USER_CREATE } from "../../../../constants/actions";

export function createUser(param: UserCreateRequest, callback: () => void) {
  param.id = "-1";
  return (dispatch: Dispatch) => {
    post(USER_CREATE_URL, param).then((res) => {
      dispatch({
        type: USER_CREATE,
        payload: {
          // id: "-1",
          loginName: param.loginName,
          email: param.email,
          lgCode: param.lgCode,
          lgKaKakari: param.lgKaKakari,
          listRole: param.listRole,
          listClaim: param.listClaim,
          ...res.data,
        },
      });
      callback();
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case USER_CREATE:
      let newList = [action.payload, ...(state.userList as UserInfo[])];
      return {
        ...state,
        userList: newList,
      };

    default:
      return { ...state };
  }
}

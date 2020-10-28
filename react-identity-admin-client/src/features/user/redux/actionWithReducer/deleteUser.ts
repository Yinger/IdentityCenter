import { Dispatch } from "redux";
import _ from "lodash";
import { remove } from "../../../../utils/request";
import { Action } from "../../../../interface/types";
import { UserInfo, UserDeleteRequest, State } from "../../../../interface/user";
import { USER_DELETE_URL } from "../../../../constants/urls";
import { USER_DELETE } from "../../../../constants/actions";

export function deleteUser(param: UserDeleteRequest) {
  return (dispatch: Dispatch) => {
    let url = USER_DELETE_URL + "?id=" + param.id;
    remove(url).then((res) => {
      dispatch({
        type: USER_DELETE,
        payload: param.id,
      });
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case USER_DELETE:
      let reducedList = [...(state.userList as UserInfo[])];
      _.remove(reducedList, (item: UserInfo) => {
        return item.id === action.payload;
      });
      return {
        ...state,
        userList: reducedList,
      };

    default:
      return { ...state };
  }
}

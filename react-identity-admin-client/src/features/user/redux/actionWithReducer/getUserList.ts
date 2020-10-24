import { Dispatch } from "redux";
import { Action } from "../../../../interface/types";
import { USER_SEARCH } from "../../../../constants/actions";
import { UserSearchRequest, State } from "../../../../interface/user";
import { USER_SEARCH_URL } from "../../../../constants/urls";
import { post } from "../../../../utils/request";

export function getUserList(param: UserSearchRequest, callback: () => void) {
  return (dispatch: Dispatch) => {
    post(USER_SEARCH_URL, param).then((res) => {
      dispatch({
        type: USER_SEARCH,
        payload: res.data,
      });
      callback();
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case USER_SEARCH:
      return {
        ...state,
        userList: action.payload,
      };

    default:
      return state;
  }
}

import { Dispatch } from "redux";
import { Action } from "../../../../interface/types";
import { USER_GET_CLAIMNAME_ALL } from "../../../../constants/actions";
import { State } from "../../../../interface/user";
import { USER_GET_CLAIMNAME_ALL_URL } from "../../../../constants/urls";
import { get } from "../../../../utils/request";

export function getUserClaimNameList(param: any, callback: () => void) {
  return (dispatch: Dispatch) => {
    get(USER_GET_CLAIMNAME_ALL_URL, param).then((res) => {
      dispatch({
        type: USER_GET_CLAIMNAME_ALL,
        payload: res.data,
      });
      callback();
    });
  };
}

export function reducer(state: State, action: Action) {
  switch (action.type) {
    case USER_GET_CLAIMNAME_ALL:
      return {
        ...state,
        claimNameList:
          action.payload !== undefined && action.payload != null
            ? action.payload
            : [],
      };

    default:
      return state;
  }
}

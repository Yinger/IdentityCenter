import React, { useState, useEffect } from "react";
import { bindActionCreators, Dispatch } from "redux";
import { connect } from "react-redux";
import { UserResponse, UserSearchRequest } from "../../interface/user";
import { Table } from "antd";
import DataColumns from "./components/dataColumns";
import { getUserList } from "./redux/actions";

interface Props {
  onSearchUser(param: UserSearchRequest, callback: () => void): void;
  userList: UserResponse;
}
const User = (props: Props) => {
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    let param: UserSearchRequest = {};
    setLoading(true);
    props.onSearchUser(param, () => {
      setLoading(false);
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <>
      <Table
        columns={DataColumns()}
        dataSource={props.userList}
        loading={loading}
        className="table"
        size="middle"
        scroll={{ x: "fit-content" }}
      />
    </>
  );
};

const mapStateToProps = (state: any) => ({
  userList: state.user.userList,
});

const mapDispatchToProps = (dispatch: Dispatch) =>
  bindActionCreators(
    {
      onSearchUser: getUserList,
    },
    dispatch,
  );

export default connect(mapStateToProps, mapDispatchToProps)(User);

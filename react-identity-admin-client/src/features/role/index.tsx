import React, { useState } from "react";
import { bindActionCreators, Dispatch } from "redux";
import { connect } from "react-redux";
import QueryForm from "./components/queryForm";
import { Button, Table } from "antd";
import DataColumns from "./components/dataColumns";
import "./index.scss";
import { RoleRequest, RoleResponse } from "../../interface/role";
import { getRoleList } from "./redux/actions";

interface Props {
  onSearchRole(param: RoleRequest, callback: () => void): void;
  roleList: RoleResponse;
}

const Role = (props: Props) => {
  const [loading, setLoading] = useState(false);
  return (
    <>
      <QueryForm getData={props.onSearchRole} setLoading={setLoading} />
      <div className="toolbar">
        <Button type="primary">新規</Button>
      </div>

      <Table
        columns={DataColumns()}
        dataSource={props.roleList}
        loading={loading}
        className="table"
      />
    </>
  );
};

const mapStateToProps = (state: any) => ({
  roleList: state.role.roleList,
});

const mapDispatchToProps = (dispatch: Dispatch) =>
  bindActionCreators(
    {
      onSearchRole: getRoleList,
    },
    dispatch,
  );

export default connect(mapStateToProps, mapDispatchToProps)(Role);

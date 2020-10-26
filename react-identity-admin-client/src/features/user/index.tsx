import React, { useState, useEffect } from "react";
import { bindActionCreators, Dispatch } from "redux";
import { connect } from "react-redux";
import {
  UserInfo,
  UserResponse,
  UserSearchRequest,
} from "../../interface/user";
import { Button, PageHeader, Table } from "antd";
import {
  getUserClaimNameList,
  getUserList,
  getUserRoleNameList,
} from "./redux/actions";
import DataColumns from "./components/dataColumns";
import QueryForm from "./components/queryForm";
import InfoModal from "./components/infoModal";
import { PlusOutlined } from "@ant-design/icons";
import moment from "moment";

interface Props {
  onSearchUser(param: UserSearchRequest, callback: () => void): void;
  onInitRoleNameList(param: any, callback: () => void): void;
  onInitClaimNameList(param: any, callback: () => void): void;
  userList: UserResponse;
  roleNameList: string[];
  claimNameList: string[];
}
const User = (props: Props) => {
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [edit, setEdit] = useState(false);
  const [rowData, setRowData] = useState<Partial<UserInfo>>({});

  const hideModal = () => {
    setRowData({});
    setShowModal(false);
  };

  const handleCreate = () => {
    setRowData({});
    setShowModal(true);
    setEdit(false);
  };

  const handleUpdate = (record: UserInfo) => {
    setShowModal(true);
    setEdit(true);
    setRowData(record);
  };

  useEffect(
    () => {
      let param: UserSearchRequest = {};
      setLoading(true);
      props.onSearchUser(param, () => {
        setLoading(false);
      });
      props.onInitRoleNameList(moment.now().toString(), () => {});
      props.onInitClaimNameList(null, () => {});
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    []
  );

  return (
    <>
      <PageHeader
        className="site-page-header"
        title="ユーザー"
        subTitle="ユーザー情報を管理します"
        key={"user-page-header"}
      />
      <QueryForm
        getData={props.onSearchUser}
        setLoading={setLoading}
        key={"user-query-form"}
      />
      <Button
        type="primary"
        icon={<PlusOutlined />}
        onClick={handleCreate}
        style={{ float: "right", marginTop: -10 }}
      >
        新規
      </Button>
      <InfoModal
        visible={showModal}
        edit={edit}
        rowData={rowData}
        hide={hideModal}
        roleNameList={props.roleNameList}
        claimNameList={props.claimNameList}
        // createData={props.onCreateRole}
        // updateData={props.onUpdateRole}
      />
      <Table
        columns={DataColumns(handleUpdate)}
        dataSource={props.userList}
        loading={loading}
        className="table"
        size="middle"
        scroll={{ x: "fit-content" }}
        rowKey="loginName"
      />
    </>
  );
};

const mapStateToProps = (state: any) => ({
  userList: state.user.userList,
  roleNameList: state.user.roleNameList,
  claimNameList: state.user.claimNameList,
});

const mapDispatchToProps = (dispatch: Dispatch) =>
  bindActionCreators(
    {
      onSearchUser: getUserList,
      onInitRoleNameList: getUserRoleNameList,
      onInitClaimNameList: getUserClaimNameList,
    },
    dispatch
  );

export default connect(mapStateToProps, mapDispatchToProps)(User);

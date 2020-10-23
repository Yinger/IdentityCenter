import React, { useState } from "react";
import { bindActionCreators, Dispatch } from "redux";
import { connect } from "react-redux";
import QueryForm from "./components/queryForm";
import { Button, Table } from "antd";
import DataColumns from "./components/dataColumns";
import "./index.scss";
import {
  RoleCreateRequest,
  RoleDeleteRequest,
  RoleInfo,
  RoleRequest,
  RoleResponse,
  RoleUpdateRequest,
} from "../../interface/role";
import {
  getRoleList,
  createRole,
  updateRole,
  deleteRole,
} from "./redux/actions";
import { PlusOutlined } from "@ant-design/icons";
import InfoModal from "./components/infoModal";

interface Props {
  onSearchRole(param: RoleRequest, callback: () => void): void;
  onUpdateRole(param: RoleUpdateRequest, callback: () => void): void;
  onCreateRole(param: RoleCreateRequest, callback: () => void): void;
  onDeleteRole(param: RoleDeleteRequest): void;
  roleList: RoleResponse;
}

const Role = (props: Props) => {
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [edit, setEdit] = useState(false);
  const [rowData, setRowData] = useState<Partial<RoleInfo>>({});

  const hideModal = () => {
    setRowData({});
    setShowModal(false);
  };

  const handleCreate = () => {
    setRowData({});
    setShowModal(true);
    setEdit(false);
  };

  const handleUpdate = (record: RoleInfo) => {
    setShowModal(true);
    setEdit(true);
    setRowData(record);
  };

  const handleDelete = (param: RoleDeleteRequest) => {
    props.onDeleteRole(param);
  };

  return (
    <>
      <div className="toolbar">
        <QueryForm getData={props.onSearchRole} setLoading={setLoading} />
      </div>
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
        createData={props.onCreateRole}
        updateData={props.onUpdateRole}
      />
      <Table
        columns={DataColumns(handleUpdate, handleDelete)}
        dataSource={props.roleList}
        loading={loading}
        className="table"
        size="middle"
        scroll={{ x: "fit-content" }}
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
      onCreateRole: createRole,
      onUpdateRole: updateRole,
      onDeleteRole: deleteRole,
    },
    dispatch,
  );

export default connect(mapStateToProps, mapDispatchToProps)(Role);

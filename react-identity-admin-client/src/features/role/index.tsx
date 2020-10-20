import React from "react";
import QueryForm from "./components/queryForm";
import { Button, Table } from "antd";
import DataColumns from "./components/dataColumns";
import "./index.scss";

const Role = () => {
  return (
    <>
      <QueryForm />
      <div className="toolbar">
        <Button type="primary">新規</Button>
      </div>

      <Table
        columns={DataColumns()}
        // dataSource={props.roleList}
        // loading={loading}
        className="table"
      />
    </>
  );
};

export default Role;

import React from "react";
import { Button, Divider, Popconfirm } from "antd";
import { RoleInfo } from "../../../interface/role";

const DataColumns = () => {
  return [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
    },
    {
      title: "ロール",
      dataIndex: "roleName",
      key: "roleName",
    },
    {
      title: "tag",
      dataIndex: "tag",
      key: "tag",
    },
    {
      title: "説明",
      dataIndex: "description",
      key: "description",
    },
    {
      title: "操作",
      key: "action",
      render: (text: string, record: RoleInfo) => (
        <span>
          <Button
            size="small"
            // icon="edit"
            onClick={() => {
              // handleUpdate(record);
            }}
          >
            編集
          </Button>
          <Divider type="vertical" />
          <Popconfirm
            title={`${record.roleName} を削除しますか？`}
            onConfirm={() => {
              // handleDelete({ id: record.id });
            }}
          >
            <Button
              size="small"
              // icon="delete"
              danger
            >
              削除
            </Button>
          </Popconfirm>
        </span>
      ),
    },
  ];
};

export default DataColumns;

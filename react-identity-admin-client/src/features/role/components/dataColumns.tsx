import React from "react";
import { Button, Divider, Popconfirm, Tag } from "antd";
import { RoleDeleteRequest, RoleInfo } from "../../../interface/role";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";

const DataColumns = (
  handleUpdate: (record: RoleInfo) => void,
  handleDelete: (record: RoleDeleteRequest) => void
) => {
  return [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
      width: "25%",
    },
    {
      title: "ロール",
      dataIndex: "roleName",
      key: "roleName",
      width: "30%",
    },
    {
      title: "tag",
      dataIndex: "tag",
      key: "tag",
      width: "10%",
      render: (tag: string) => <Tag color="blue">{tag.toUpperCase()}</Tag>,
    },
    {
      title: "説明",
      dataIndex: "description",
      key: "description",
      width: "20%",
    },
    {
      title: "操作",
      key: "action",
      width: "15%",
      render: (text: string, record: RoleInfo) => (
        <span>
          <Button
            size="small"
            icon={<EditOutlined />}
            onClick={() => {
              handleUpdate(record);
            }}
          >
            編集
          </Button>
          <Divider type="vertical" />
          <Popconfirm
            title={`${record.roleName} を削除しますか？`}
            onConfirm={() => {
              console.log(record);
              handleDelete({ id: record.id });
            }}
          >
            <Button size="small" icon={<DeleteOutlined />} danger>
              削除
            </Button>
          </Popconfirm>
        </span>
      ),
    },
  ];
};

export default DataColumns;

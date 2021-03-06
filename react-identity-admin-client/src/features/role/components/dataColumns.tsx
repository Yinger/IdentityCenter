import React from "react";
import { Button, Divider, Popconfirm, Tag, Tooltip } from "antd";
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
      width: 300,
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
      render: (tag: string) => <Tag color="blue">{tag}</Tag>,
    },
    {
      title: "説明",
      dataIndex: "description",
      key: "description",
      // align: "center" as "center",
      width: 300,
    },
    {
      title: "操作",
      key: "action",
      align: "center" as "center",
      width: 200,

      render: (text: string, record: RoleInfo) => (
        <span>
          <Tooltip title="編集" color={"blue"} placement="bottom">
            <Button
              size="small"
              icon={<EditOutlined />}
              type="primary"
              onClick={() => {
                handleUpdate(record);
              }}
            />
          </Tooltip>

          <Divider type="vertical" />
          <Tooltip title="削除" color={"red"} placement="bottom">
            <Popconfirm
              title={`${record.roleName} を削除しますか？`}
              onConfirm={() => {
                console.log(record);
                handleDelete({ id: record.id });
              }}
            >
              <Button
                size="small"
                icon={<DeleteOutlined />}
                type="primary"
                danger
              />
            </Popconfirm>
          </Tooltip>
        </span>
        // <span style={{ float: "right" }}>
        //   <Button
        //     size="small"
        //     icon={<EditOutlined />}
        //     type="primary"
        //     onClick={() => {
        //       handleUpdate(record);
        //     }}
        //   >
        //     編集
        //   </Button>
        //   <Divider type="vertical" />
        //   <Popconfirm
        //     title={`${record.roleName} を削除しますか？`}
        //     onConfirm={() => {
        //       console.log(record);
        //       handleDelete({ id: record.id });
        //     }}
        //   >
        //     <Button
        //       size="small"
        //       icon={<DeleteOutlined />}
        //       type="primary"
        //       danger
        //     >
        //       削除
        //     </Button>
        //   </Popconfirm>
        // </span>
      ),
    },
  ];
};

export default DataColumns;

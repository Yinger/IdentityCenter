import React from "react";
import { Button, Divider, Popconfirm, Tag, Tooltip } from "antd";
import { UserDeleteRequest, UserInfo } from "../../../interface/user";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";

const DataColumns = (
  handleUpdate: (record: UserInfo) => void,
  handleDelete: (record: UserDeleteRequest) => void
) => {
  return [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
      width: 300,
      // ellipsis: true,
    },
    {
      title: "ユーザー名",
      dataIndex: "loginName",
      key: "loginName",
    },
    {
      title: "メール",
      dataIndex: "email",
      key: "email",
    },
    {
      title: "市区町村",
      dataIndex: "lgCode",
      key: "lgCode",
    },
    {
      title: "所属課",
      dataIndex: "lgKaKakari",
      key: "lgKaKakari",
    },
    {
      title: "ロール",
      dataIndex: "listRole",
      key: "listRole",
      render: (roleList: string[] | undefined | null) =>
        roleList !== undefined && roleList != null
          ? roleList.map((tag: string) => <Tag key={tag}>{tag}</Tag>)
          : "",
    },
    // {
    //   title: "Claims",
    //   dataIndex: "listClaim",
    //   key: "listClaim",
    //   render: (claimList: string[] | undefined | null) =>
    //     claimList !== undefined && claimList != null
    //       ? claimList.map((tag: string) => (
    //           <Tag color="purple" key={tag}>
    //             {tag}
    //           </Tag>
    //         ))
    //       : "",
    // },
    {
      title: "操作",
      key: "action",
      align: "center" as "center",
      // align: "center",

      render: (text: string, record: UserInfo) => (
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
              title={`${record.loginName} を削除しますか？`}
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
      ),
    },
  ];
};

export default DataColumns;

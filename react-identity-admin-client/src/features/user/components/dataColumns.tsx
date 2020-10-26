import React from "react";
import { Button, Divider, Popconfirm, Tag } from "antd";
import { UserInfo } from "../../../interface/user";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";

const DataColumns = (handleUpdate: (record: UserInfo) => void) =>
  //   handleDelete: (record: UserDeleteRequest) => void,
  {
    return [
      //   {
      //     title: "ID",
      //     dataIndex: "id",
      //     key: "id",
      //   },
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
            ? roleList.map((tag: string) => (
                <Tag color="blue" key={tag}>
                  {tag}
                </Tag>
              ))
            : "",
      },
      {
        title: "Claims",
        dataIndex: "listClaim",
        key: "listClaim",
        render: (claimList: string[] | undefined | null) =>
          claimList !== undefined && claimList != null
            ? claimList.map((tag: string) => (
                <Tag color="purple" key={tag}>
                  {tag}
                </Tag>
              ))
            : "",
      },
      {
        // title: "操作",
        key: "action",

        render: (text: string, record: UserInfo) => (
          <span style={{ float: "right" }}>
            <Button
              size="small"
              icon={<EditOutlined />}
              type="primary"
              onClick={() => {
                handleUpdate(record);
              }}
            >
              編集
            </Button>
            <Divider type="vertical" />
            <Popconfirm
              title={`${record.loginName} を削除しますか？`}
              onConfirm={() => {
                console.log(record);
                // handleDelete({ id: record.id });
              }}
            >
              <Button
                size="small"
                icon={<DeleteOutlined />}
                type="primary"
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

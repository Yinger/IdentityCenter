import React, { useState } from "react";
import { Modal, Form, Input, Select, Tag, Typography, message } from "antd";
import { FormProps } from "antd/lib/form";
import {
  UserCreateRequest,
  UserInfo,
  UserUpdateRequest,
} from "../../../interface/user";
import { USER_FIND_BY_NAME_URL } from "../../../constants/urls";
import { get } from "../../../utils/request";
import "antd/dist/antd.css";
import "./infoModal.scss";

const { Option } = Select;
const { Text } = Typography;
const layout = {
  labelCol: {
    span: 5,
  },
  wrapperCol: {
    span: 19,
  },
};

interface Props extends FormProps {
  visible: boolean;
  edit: boolean;
  rowData: Partial<UserInfo>;
  roleNameList: string[];
  claimNameList: string[];
  hide(): void;
  createData(param: UserCreateRequest, callback: () => void): void;
  updateData(param: UserUpdateRequest, callback: () => void): void;
}

const InfoModal = (props: Props) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  /**
   * 新規作成または編集保存時に発生します
   */
  const handleOk = () => {
    // console.log(userRoleNameList);
    //必須入力項目チェック
    form.validateFields().then(() => {
      let param = form.getFieldsValue();
      let paramFindUserByName = { name: param.loginName };

      //id非表示ので、ここで記入します
      param.id = props.rowData.id;
      param.listRole = props.rowData.listRole;

      //loading開始
      setConfirmLoading(true);

      //新規作成の場合 ---------------------------
      if (!props.edit) {
        //同じユーザー名既存チェック
        get(USER_FIND_BY_NAME_URL, paramFindUserByName).then((res) => {
          if (res.data == null) {
            //既存ではないの場合、新規処理行います
            props.createData(param as UserCreateRequest, close);
          } else {
            //loading終止
            setConfirmLoading(false);
            //メーセッじ表示
            message.info("該当ユーザー名は既存しました。");
          }
        });
      }
      //編集保存の場合 ---------------------------
      else {
        //ユーザー名変更の場合、変更後のユーザー名既存チェック
        if (props.rowData.loginName !== param.loginName) {
          //同じユーザー名既存チェック
          get(USER_FIND_BY_NAME_URL, paramFindUserByName).then((res) => {
            if (res.data == null) {
              //既存ではないの場合、保存処理行います
              props.updateData(param as UserUpdateRequest, close);
            } else {
              //loading終止
              setConfirmLoading(false);
              //メーセッじ表示
              message.info("該当ユーザー名は既存しました。");
            }
          });
        } else {
          //ユーザー名変更がない場合、直接保存します
          props.updateData(param as UserUpdateRequest, close);
        }
      }
    });
  };
  /**
   * 「キャンセル」ボタンをクリック時に発生します
   */
  const handleCancel = () => {
    close();
  };

  /**
   * Dialogクローズ時に発生します
   */
  const close = () => {
    props.hide();
    setConfirmLoading(false);
  };

  function handleRoleNameListChange(value: string[]) {
    props.rowData.listRole = value;
  }

  let title = props.edit ? "編集" : "新しいユーザー情報を作成";
  let {
    id,
    loginName,
    email,
    lgCode,
    lgKaKakari,
    listRole,
    // listClaim,
  } = props.rowData;

  return (
    <>
      <Modal
        destroyOnClose={true}
        title={title}
        visible={props.visible}
        onOk={handleOk}
        onCancel={handleCancel}
        confirmLoading={confirmLoading}
        width={560}
      >
        <Form
          form={form}
          {...layout}
          preserve={false}
          style={{ margin: 0, marginTop: -20 }}
        >
          <div
            className="space-align-block"
            style={{ marginTop: -2, marginBottom: 20 }}
          >
            <Text>
              クレーム
              <Text strong> (固定) </Text>
              <Text type="secondary">
                ログイン後下記ユーザー情報を取得できています
              </Text>
            </Text>
            <br />
            {props.claimNameList !== undefined && props.claimNameList != null
              ? props.claimNameList.map((claim: string) => (
                  <Tag key={claim} color="purple">
                    {claim}
                  </Tag>
                ))
              : ""}
          </div>
          <Form.Item
            label="ID"
            name="id"
            initialValue={props.edit ? id : "-1"}
            style={{ marginBottom: "5px" }}
          >
            <Input
              placeholder="ID"
              style={{ width: 390 }}
              // maxLength={20}
              readOnly={true}
              addonAfter={
                <Tag color="purple" style={{ margin: 0 }}>
                  {"userid"}
                </Tag>
              }
            />
          </Form.Item>
          <Form.Item
            label="ユーザー名"
            name="loginName"
            initialValue={loginName}
            style={{ marginBottom: "5px" }}
            rules={[
              {
                required: true,
                whitespace: true,
                message: "ユーザー名を入力してください",
              },
            ]}
          >
            <Input
              placeholder="ユーザー名"
              style={{ width: 390 }}
              // maxLength={20}
              addonAfter={
                <Tag color="purple" style={{ margin: 0 }}>
                  {"name"}
                </Tag>
              }
            />
          </Form.Item>

          <Form.Item
            label="メール"
            name="email"
            initialValue={email}
            style={{ marginBottom: "5px" }}
            rules={[
              {
                required: true,
                type: "email",
                message: "正しいメールアドレスを入力してください",
              },
            ]}
          >
            <Input
              placeholder="メール"
              style={{ width: 390 }}
              maxLength={50}
              addonAfter={
                <Tag color="purple" style={{ margin: 0 }}>
                  {"email"}
                </Tag>
              }
            />
          </Form.Item>
          <Form.Item
            label="市区町村"
            name="lgCode"
            initialValue={lgCode}
            style={{ marginBottom: "5px" }}
          >
            <Input
              placeholder="市区町村"
              style={{ width: 390 }}
              maxLength={20}
              addonAfter={
                <Tag color="purple" style={{ margin: 0 }}>
                  {"lgcode"}
                </Tag>
              }
            />
          </Form.Item>
          <Form.Item
            label="所属課"
            name="lgKaKakari"
            initialValue={lgKaKakari}
            style={{ marginBottom: "5px" }}
          >
            <Input placeholder="所属課" style={{ width: 390 }} maxLength={20} />
          </Form.Item>
          <Form.Item
            label="ロール"
            name="listRole"
            initialValue={listRole}
            key="listRole"
            style={{ marginBottom: "5px" }}
          >
            <div
              className="ant-input-group ant-input-group-compact"
              style={{ width: 390 }}
            >
              <Select
                mode="multiple"
                size="middle"
                placeholder="ロールを選択してください"
                defaultValue={listRole}
                onChange={handleRoleNameListChange}
                style={{ width: 330 }}
              >
                {props.roleNameList !== undefined && props.roleNameList != null
                  ? props.roleNameList.map((role: string) => (
                      <Option key={role} value={role}>
                        {role}
                      </Option>
                    ))
                  : ""}
              </Select>
              <div
                className="ant-input-group-addon"
                style={{
                  width: 60,
                  height: 32,
                  textAlign: "center",
                  marginLeft: 1,
                }}
              >
                {
                  <Tag color="purple" style={{ marginTop: 4 }}>
                    {"role"}
                  </Tag>
                }
              </div>
            </div>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default InfoModal;

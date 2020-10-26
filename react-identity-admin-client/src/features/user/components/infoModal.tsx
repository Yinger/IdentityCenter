import React, { useState } from "react";
import { Modal, Form, Input, Select, Tag } from "antd";
import { FormProps } from "antd/lib/form";
import { UserInfo } from "../../../interface/user";
// import { PlusOutlined } from "@ant-design/icons";

const { Option } = Select;
const layout = {
  labelCol: {
    span: 6,
  },
  wrapperCol: {
    span: 18,
  },
};

interface Props extends FormProps {
  visible: boolean;
  edit: boolean;
  rowData: Partial<UserInfo>;
  roleNameList: string[];
  claimNameList: string[];
  hide(): void;
  // createData(param: UserCreateRequest, callback: () => void): void;
  // updateData(param: UserUpdateRequest, callback: () => void): void;
}

const InfoModal = (props: Props) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [userRoleNameList, setUserRoleNameList] = useState<string[]>([]);

  const handleOk = () => {
    console.log(userRoleNameList);
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
    setUserRoleNameList(value);
  }

  let title = props.edit ? "編集" : "新しいユーザー情報を作成";
  let {
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
      >
        <Form form={form} {...layout} preserve={false}>
          <Form.Item
            label="クレーム(固定)"
            name="listClaim"
            initialValue={props.claimNameList}
            key="listClaim"
          >
            {props.claimNameList !== undefined && props.claimNameList != null
              ? props.claimNameList.map((claim: string) => (
                  <Tag color="purple" key={claim}>
                    {claim}
                  </Tag>
                ))
              : ""}
          </Form.Item>
          <Form.Item
            label="ユーザー名"
            name="loginName"
            initialValue={loginName}
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
              style={{ width: 300 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="メール" name="email" initialValue={email}>
            <Input
              placeholder="メール"
              style={{ width: 300 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="市区町村" name="lgCode" initialValue={lgCode}>
            <Input
              placeholder="市区町村"
              style={{ width: 300 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="所属課" name="lgKaKakari" initialValue={lgKaKakari}>
            <Input
              placeholder="所属課"
              style={{ width: 300 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item
            label="ロール"
            name="listRole"
            initialValue={listRole}
            key="listRole"
          >
            <Select
              mode="multiple"
              size="middle"
              placeholder="ロールを選択してください"
              defaultValue={listRole}
              onChange={handleRoleNameListChange}
              style={{ width: 300 }}
            >
              {props.roleNameList !== undefined && props.roleNameList != null
                ? props.roleNameList.map((role: string) => (
                    <Option key={role} value={role}>
                      {role}
                    </Option>
                  ))
                : ""}
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default InfoModal;

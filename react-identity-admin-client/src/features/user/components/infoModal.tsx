import React, { useState } from "react";
import { Modal, Form, Input, Tag } from "antd";
import { FormProps } from "antd/lib/form";
import { UserInfo } from "../../../interface/user";

const layout = {
  labelCol: {
    span: 8,
  },
  wrapperCol: {
    span: 16,
  },
};

interface Props extends FormProps {
  visible: boolean;
  edit: boolean;
  rowData: Partial<UserInfo>;
  hide(): void;
  // createData(param: UserCreateRequest, callback: () => void): void;
  // updateData(param: UserUpdateRequest, callback: () => void): void;
}

const InfoModal = (props: Props) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  const handleOk = () => {};

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
              style={{ width: 200 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="メール" name="email" initialValue={email}>
            <Input
              placeholder="メール"
              style={{ width: 200 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="市区町村" name="lgCode" initialValue={lgCode}>
            <Input
              placeholder="市区町村"
              style={{ width: 200 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="所属課" name="lgKaKakari" initialValue={lgKaKakari}>
            <Input
              placeholder="所属課"
              style={{ width: 200 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="ロール" name="listRole" initialValue={listRole}>
            {listRole !== undefined && listRole != null
              ? listRole.map((role, index) => {
                  return (
                    <>
                      <Tag
                        className="edit-tag"
                        key={role}
                        closable={index !== 0}
                        // onClose={}
                      ></Tag>
                    </>
                  );
                })
              : ""}
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default InfoModal;

import React, { useState } from "react";
import { Modal, Form, Input, Select, Tag, Card, message } from "antd";
import { FormProps } from "antd/lib/form";
import { UserInfo, UserUpdateRequest } from "../../../interface/user";
import { USER_FIND_BY_NAME_URL } from "../../../constants/urls";
import { get } from "../../../utils/request";

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
      // param.listRole = userRoleNameList;

      //loading開始
      setConfirmLoading(true);

      //新規作成の場合 ---------------------------
      if (!props.edit) {
        //同じユーザー名既存チェック
        get(USER_FIND_BY_NAME_URL, paramFindUserByName).then((res) => {
          if (res.data == null) {
            //既存ではないの場合、新規処理行います
            // props.createData(param as UserCreateRequest, close);
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
          <Card
            type="inner"
            size="small"
            title="クレーム(固定)"
            style={{ marginTop: -20, marginBottom: 10 }}
          >
            {/* <p>※ログイン後に下記ユーザー情報を取得できています</p> */}
            {props.claimNameList !== undefined && props.claimNameList != null
              ? props.claimNameList.map((claim: string) => (
                  <Tag key={claim} color="purple">
                    {claim}
                  </Tag>
                ))
              : ""}
          </Card>
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

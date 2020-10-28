import React, { useState } from "react";
import { Modal, Form, Input, message } from "antd";
import { FormProps } from "antd/lib/form";
import { ROLE_FIND_BY_NAME_URL } from "../../../constants/urls";
import { get } from "../../../utils/request";
import {
  RoleCreateRequest,
  RoleInfo,
  RoleUpdateRequest,
} from "../../../interface/role";

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
  rowData: Partial<RoleInfo>;
  hide(): void;
  createData(param: RoleCreateRequest, callback: () => void): void;
  updateData(param: RoleUpdateRequest, callback: () => void): void;
}

const InfoModal = (props: Props) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  /**
   * 新規作成または編集保存時に発生します
   */
  const handleOk = () => {
    //必須入力項目チェック
    form.validateFields().then(() => {
      let param = form.getFieldsValue();
      let paramFindRoleByName = { name: param.roleName };

      //id非表示ので、ここで記入します
      param.id = props.rowData.id;

      //loading開始
      setConfirmLoading(true);

      //新規作成の場合 ---------------------------
      if (!props.edit) {
        //同じロール名既存チェック
        get(ROLE_FIND_BY_NAME_URL, paramFindRoleByName).then((res) => {
          if (res.data == null) {
            //既存ではないの場合、新規処理行います
            props.createData(param as RoleCreateRequest, close);
          } else {
            //loading終止
            setConfirmLoading(false);
            //メーセッじ表示
            message.info("該当ロール名は既存しました。");
          }
        });
      }
      //編集保存の場合 ---------------------------
      else {
        //ロール名変更の場合、変更後のロール名既存チェック
        if (props.rowData.roleName !== param.roleName) {
          //同じロール名既存チェック
          get(ROLE_FIND_BY_NAME_URL, paramFindRoleByName).then((res) => {
            if (res.data == null) {
              //既存ではないの場合、保存処理行います
              props.updateData(param as RoleUpdateRequest, close);
            } else {
              //loading終止
              setConfirmLoading(false);
              //メーセッじ表示
              message.info("該当ロール名は既存しました。");
            }
          });
        } else {
          //ロール名変更がない場合、直接保存します
          props.updateData(param as RoleUpdateRequest, close);
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

  let title = props.edit ? "編集" : "新しいロールを作成";
  let { roleName, tag, description } = props.rowData;

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
        <Form form={form} {...layout} preserve={false}>
          <Form.Item
            label="ロール名"
            name="roleName"
            initialValue={roleName}
            style={{ marginBottom: "5px" }}
            rules={[
              {
                required: true,
                whitespace: true,
                message: "ロール名を入力してください",
              },
            ]}
          >
            <Input
              placeholder="ロール名"
              style={{ width: 390 }}
              maxLength={50}
              // allowClear
            />
          </Form.Item>
          <Form.Item
            label="タグ"
            name="tag"
            initialValue={tag}
            style={{ marginBottom: "5px" }}
          >
            <Input
              placeholder="タグ"
              style={{ width: 390 }}
              maxLength={50}
              // allowClear
            />
          </Form.Item>
          <Form.Item
            label="説明"
            name="description"
            initialValue={description}
            style={{ marginBottom: "5px" }}
          >
            <Input.TextArea
              placeholder="説明"
              style={{ width: 390 }}
              maxLength={200}
              // allowClear
            />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};
export default InfoModal;

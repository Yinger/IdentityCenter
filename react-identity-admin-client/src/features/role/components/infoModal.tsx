import React, { useState } from "react";
import { Modal, Form, Input } from "antd";
import { FormProps } from "antd/lib/form";
import {
  RoleCreateRequest,
  RoleInfo,
  RoleUpdateRequest,
} from "../../../interface/role";

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
  rowData: Partial<RoleInfo>;
  hide(): void;
  createData(param: RoleCreateRequest, callback: () => void): void;
  updateData(param: RoleUpdateRequest, callback: () => void): void;
}

const InfoModal = (props: Props) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  const handleOk = () => {
    form.validateFields().then(() => {
      setConfirmLoading(true);
      let param = form.getFieldsValue();
      param.id = props.rowData.id;
      if (!props.edit) props.createData(param as RoleCreateRequest, close);
      else props.updateData(param as RoleUpdateRequest, close);
    });
  };

  const handleCancel = () => {
    close();
  };

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
      >
        <Form form={form} {...layout} preserve={false}>
          <Form.Item
            label="ロール名"
            name="roleName"
            initialValue={roleName}
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
              style={{ width: 200 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="タグ" name="tag" initialValue={tag}>
            <Input
              placeholder="タグ"
              style={{ width: 200 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
          <Form.Item label="説明" name="description" initialValue={description}>
            <Input
              placeholder="説明"
              style={{ width: 200 }}
              maxLength={20}
              allowClear
            />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};
export default InfoModal;

import React, { useState, useEffect } from "react";
import { Button, Form, Input } from "antd";
import { FormProps } from "antd/lib/form";
import { RoleRequest } from "../../../interface/role";
import { SearchOutlined, ClearOutlined } from "@ant-design/icons";

interface Props extends FormProps {
  //   onDataChange(data: RoleSearchResponse): void;
  getData(param: RoleRequest, callback: () => void): void;
  setLoading(loading: boolean): void;
}

const QueryForm = (props: Props) => {
  const [roleName, setRoleName] = useState("");
  const [tag, setTag] = useState("");
  const [description, setDescription] = useState("");

  const handleRoleNameChange = (e: React.FormEvent<HTMLInputElement>) => {
    setRoleName(e.currentTarget.value);
  };

  const handleTagChange = (e: React.FormEvent<HTMLInputElement>) => {
    setTag(e.currentTarget.value);
  };

  const handleDescriptionChange = (e: React.FormEvent<HTMLInputElement>) => {
    setDescription(e.currentTarget.value);
  };

  const queryRole = (param: RoleRequest) => {
    props.setLoading(true);
    // console.log(param);
    props.getData(param, () => {
      props.setLoading(false);
    });
  };

  const handleReset = () => {
    setRoleName("");
    setTag("");
    setDescription("");
    queryRole({ roleName: "", tag: "", description: "" } as RoleRequest);
  };

  const handleSubmit = () => {
    queryRole({ roleName, tag, description });
  };

  useEffect(() => {
    queryRole({ roleName, tag, description });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Form layout="inline">
      <Form.Item>
        <Input
          placeholder="ロール"
          style={{ width: 200 }}
          value={roleName}
          onChange={handleRoleNameChange}
        />
      </Form.Item>
      <Form.Item>
        <Input
          placeholder="Tag"
          style={{ width: 200 }}
          value={tag}
          onChange={handleTagChange}
        />
      </Form.Item>
      <Form.Item>
        <Input
          placeholder="説明"
          style={{ width: 200 }}
          value={description}
          onChange={handleDescriptionChange}
        />
      </Form.Item>
      <Form.Item>
        <Button type="primary" onClick={handleSubmit} icon={<SearchOutlined />}>
          検索
        </Button>
      </Form.Item>
      <Form.Item>
        <Button onClick={handleReset} icon={<ClearOutlined />}>
          クリア
        </Button>
      </Form.Item>
    </Form>
  );
};
export default QueryForm;

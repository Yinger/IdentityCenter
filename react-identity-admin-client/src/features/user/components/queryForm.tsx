import React, { useState, useEffect } from "react";
import { Button, Form, Input } from "antd";
import { FormProps } from "antd/lib/form";
import { SearchOutlined, ClearOutlined } from "@ant-design/icons";
import { UserSearchRequest } from "../../../interface/user";
import "./queryForm.scss";

interface Props extends FormProps {
  getData(param: UserSearchRequest, callback: () => void): void;
  setLoading(loading: boolean): void;
}

const QueryForm = (props: Props) => {
  const [loginName, setLoginName] = useState("");
  const [email, setEmail] = useState("");
  const [lgCode, setLgCode] = useState("");
  const [lgKaKakari, setLgKaKakari] = useState("");
  const [roleName, setRoleName] = useState("");

  const handleLoginNameChange = (e: React.FormEvent<HTMLInputElement>) => {
    setLoginName(e.currentTarget.value);
  };

  const handleEmailChange = (e: React.FormEvent<HTMLInputElement>) => {
    setEmail(e.currentTarget.value);
  };

  const handleLgCodeChange = (e: React.FormEvent<HTMLInputElement>) => {
    setLgCode(e.currentTarget.value);
  };

  const handleLgKaKakariChange = (e: React.FormEvent<HTMLInputElement>) => {
    setLgKaKakari(e.currentTarget.value);
  };

  const handleRoleNameChange = (e: React.FormEvent<HTMLInputElement>) => {
    setRoleName(e.currentTarget.value);
  };

  const queryUser = (param: UserSearchRequest) => {
    props.setLoading(true);
    props.getData(param, () => {
      props.setLoading(false);
    });
  };

  const handleReset = () => {
    setLoginName("");
    setEmail("");
    setLgCode("");
    setLgKaKakari("");
    setRoleName("");
    queryUser({
      loginName: "",
      email: "",
      lgCode: "",
      lgKaKakari: "",
      roleName: "",
    } as UserSearchRequest);
  };

  const handleSubmit = () => {
    queryUser({ loginName, email, lgCode, lgKaKakari, roleName });
  };

  useEffect(() => {
    queryUser({ loginName, email, lgCode, lgKaKakari, roleName });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Form layout="inline" className="ant-form-item">
      <Form.Item>
        <Input
          placeholder="ユーザー名"
          style={{ width: 200 }}
          value={loginName}
          onChange={handleLoginNameChange}
        />
      </Form.Item>
      <Form.Item>
        <Input
          placeholder="メール"
          style={{ width: 200 }}
          value={email}
          onChange={handleEmailChange}
        />
      </Form.Item>
      <Form.Item>
        <Input
          placeholder="市区町村"
          style={{ width: 200 }}
          value={lgCode}
          onChange={handleLgCodeChange}
        />
      </Form.Item>
      <Form.Item>
        <Input
          placeholder="所属課"
          style={{ width: 200 }}
          value={lgKaKakari}
          onChange={handleLgKaKakariChange}
        />
      </Form.Item>
      <Form.Item>
        <Input
          placeholder="ロール"
          style={{ width: 200 }}
          value={roleName}
          onChange={handleRoleNameChange}
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

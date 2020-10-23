import React from "react";
import { Route, Link } from "react-router-dom";
import { Layout, Menu, ConfigProvider } from "antd";
import ja_JP from "antd/lib/locale-provider/ja_JP";
import "moment/locale/ja";
import "antd/dist/antd.css";
import "./App.scss";
import {
  TeamOutlined,
  IdcardOutlined,
  ClusterOutlined,
  CloudServerOutlined,
} from "@ant-design/icons";

import Client from "./client";
import ApiResource from "./apiResource";
import Role from "./role";
import User from "./user";

const { Content, Sider } = Layout;

const App = ({ match }: any) => {
  let defaultKey = match.url.replace("/", "") || "user";
  return (
    <ConfigProvider locale={ja_JP}>
      <Layout>
        <Sider
          style={{
            overflow: "auto",
            height: "100vh",
            position: "fixed",
            left: 0,
          }}
        >
          <div className="logo">Ids Admin</div>
          <Menu theme="dark" mode="inline" defaultSelectedKeys={defaultKey}>
            <Menu.Item
              key="apiResource"
              icon={<CloudServerOutlined style={{ fontSize: 18 }} />}
            >
              <Link to="/apiResource">API</Link>
            </Menu.Item>
            <Menu.Item
              key="client"
              icon={<ClusterOutlined style={{ fontSize: 18 }} />}
            >
              <Link to="/client">クライアント</Link>
            </Menu.Item>
            <Menu.Item
              key="role"
              icon={<TeamOutlined style={{ fontSize: 18 }} />}
            >
              <Link to="/role">ロール</Link>
            </Menu.Item>
            <Menu.Item
              key="user"
              icon={<IdcardOutlined style={{ fontSize: 18 }} />}
            >
              <Link to="/user">ユーザー</Link>
            </Menu.Item>
          </Menu>
        </Sider>
        <Layout className="site-layout" style={{ marginLeft: 200 }}>
          <Content style={{ marginTop: 3, overflow: "initial" }}>
            <div className="site-layout-background" style={{ padding: 12 }}>
              <Route path="/" exact component={User} />
              <Route path="/apiResource" component={ApiResource} />
              <Route path="/client" component={Client} />
              <Route path="/role" component={Role} />
              <Route path="/user" component={User} />
            </div>
          </Content>
        </Layout>
      </Layout>
    </ConfigProvider>
  );
};

export default App;

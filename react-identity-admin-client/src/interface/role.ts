export interface RoleInfo {
  id: string;
  roleName: string;
  tag: string;
  description: string;
}

export interface RoleRequest {
  roleName?: string;
  tag?: string;
  description?: string;
}

export type RoleResponse = RoleInfo[] | undefined;

export type State = Readonly<{
  roleList: RoleResponse;
}>;

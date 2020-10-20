export interface RoleInfo {
  id: string;
  roleName: string;
  tag: string;
  description: string;
}

export interface RoleSearchRequest {
  roleName?: string;
  tag?: string;
  description?: string;
}

export type RoleSearchResponse = RoleInfo[] | undefined;

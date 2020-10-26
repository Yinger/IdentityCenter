export interface UserInfo {
  id: string;
  loginName: string;
  email: string;
  lgCode: string;
  lgKaKakari: string;
  listRole?: string[] | undefined;
  listClaim?: string[] | undefined;
}

export interface UserSearchRequest {
  loginName?: string;
  email?: string;
  lgCode?: string;
  lgKaKakari?: string;
  roleName?: string;
}

export type UserResponse = UserInfo[] | undefined;

export type State = Readonly<{
  userList: UserResponse;
  roleNameList: string[];
  claimNameList: string[];
}>;

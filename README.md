# 数据库表解析

## （一） 操作

对授权的体现，授权时自动添加

- DeviceCodes
- PersistedGrants

## （二） 配置（资源）

Api 资源，用户，客户端

- Identity \*\*\*
  - IdentityResources
  - IdentityClaims
- API \*\*\*
  - ApiClaims （1 个 ApiResource 对应多个 ApiClaims ➡️ access token）
  - ApiResources （ApiResource ➡️ Client AllowedScopes ➡️ Token Claims）
- Client \*\*\*
  - Clients
  - ClientCorsOrigins
  - ClientGrantTypes
  - ClientScopes
  - ClientRedirectUris
  - ClientPostLogoutRedirectUris
  - ClientSecrets

## （三） 应用用户

MicrosoftIdentity 扩展 自定义上下文

- 用户
  - AspNetUsers
- 角色
  - Role
- 声明
  - AspNetUserClaims（➡️ Token）

# 数据库表解析

## 一 操作

对授权的体现，授权时自动添加

1. DeviceCodes
2. PersistedGrants

## 二 配置（资源）

Api 资源，用户，客户端

- Identity \*\*\*
  1. IdentityResources
  2. IdentityClaims
- API \*\*\*
  1. ApiClaims （1ApiResource ： \*ApiClaims ➡️ access token）
  2. ApiResources （ApiResource ➡️ Client AllowedScopes ➡️ Token Claims）
- Client \*\*\*
  1. Clients
  2. ClientCorsOrigins
  3. ClientGrantTypes
  4. ClientScopes
  5. ClientRedirectUris
  6. ClientPostLogoutRedirectUris
  7. ClientSecrets

## 三 应用用户

MicrosoftIdentity 扩展 自定义上下文

- 用户
  - AspNetUsers
- 角色
  - Role
- 声明
  - AspNetUserClaims（➡️ Token）

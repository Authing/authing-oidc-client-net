# Authing C# OIDC SDK

[Authing](https://authing.cn/) 是一个企业级身份认证提供商，其集成了 OAuth、LDAP、OIDC 等多种身份认证和授权解决方案。

Authing C# OIDC SDK 是 Authing 提供的在 Windows 平台上进行 OIDC 登录的解决方案，
开发者可以在 Winforms、WPF 应用程序中快速接入，并使用其进行认证。

## 安装

### Winforms

```bash
$ Install-Package Authing.OidcClient.Winforms
```

### WPF

```bash
$ Install-Package Authing.OidcClient.WPF
```

## 使用

### 初始化

首先确保拥有一个 Authing 账号，如果还没有，请先[注册](https://authing.cn/sign-up)。

然后从控制台取得 OIDC 应用的 AppId、AppDomain 和 回调地址，
在项目中可以使用这些参数创建一个 `AuthingClient` 实例。

```c#
var client = new AuthingClient(new AuthingClientOptions() {
    AppId = "your_app_id",
    AppDomain = "your_app_domain",
    // 登录成功后的回调地址
    RedirectUri = "your_redirect_uri",
    // 注销后的回调地址
    PostLogoutRedirectUri = "your_post_logout_redirect_uri",
});
```

### 登录

登录为异步操作，返回的结果中包括 token 和用户信息，完整的示例在 [test](./test) 目录下。

```c#
// 获取登录结果
var loginResult = await client.LoginAsync();

// token 信息
print($"id_token: {loginResult.IdentityToken}");
print($"access_token: {loginResult.AccessToken}");
print($"refresh_token: {loginResult.RefreshToken}");

// 用户信息
foreach (var claim in loginResult.User.Claims)
{
    print($"{claim.Type} = {claim.Value}");
}
```

如果需要获得用户的某种信息，例如电话和 email，可以使用如下方法：

```c#
var phone = loginResult.User.FindFirst("phone");
var email = loginResult.User.FindFirst("email");
```

### 注销

注销为异步操作，默认为静默注销，不显示窗口。

```c#
var LogoutResult = await client.LogoutAsync();
```

## 获取帮助

1. Gitter: [#authing-chat](https://gitter.im/authing-chat/community)

## 开发者信息

[Authing](https://authing.cn)

## License

This project is licensed under the MIT license. See the LICENSE file for more info.

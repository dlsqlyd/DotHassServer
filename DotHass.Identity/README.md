# dothass


exterallogin  是外部登录.第三方登录


TwoFactor  两步验证 sms 或者其他


AuthenticationScheme  认证方案


二次登录 和 扩展登录的区别


二次登录..类似 短消息验证等

扩展登录 即是第三方登录




UserOnlyStore 没有实现role功能


UserStore  实现了角色功能


TokenProviders   两部验证的提供器



TUserLogin   第三方登录的名字和key和第三方登录的显示名

TUserToken  存储用户二次登录code.和忘记密码token等

第三方登录流程
1.先请求第三方认证..如果认证成功 返回认证结果
2.根据认证结果从logininfo表获取uid..再获取用户信息
3.如果没有获取到用户信息..则创建用户信息,并登录

无论是密码登录..还是第三方登陆了..都会走二次登录的接口SignInOrTwoFactorAsync..可用参数bypassTwoFactor控制




登录和认证流程 ,Identity系统会引用认证系统

1.Identity系统验证账号密码
2.登录成功后.user会变成userPrincipal,会把sid包装成authenticationProperties.传给_authService
3. _authService执行SignInAsync
4.SessionAuthenticationHandler 会执行登录









































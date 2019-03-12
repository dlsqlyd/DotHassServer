1 web认证的方式和游戏服务器认证的方式不同
web认证生成的token是不变的.
游戏服务器生成的sign每条都是改变的


4.ActivatorUtilities.CreateInstance...只能是class..将proiver注册到class中的构造函数..和proiver中没有关系


5.Authentication 认证..用户登录,或者第三方登录后,会在authservice中存储认证信息.
SignInAsync
如果user已经存在. 则顶号,清除之前的session

认证中间件:
获取user到上下文中.供授权使用....
AuthenticateAsync
如果由user 的话.没过期则更新过期时间
如果user过期则清除..不踢掉线(再登录Authorization就会检查不到user.而提示没有权限)





6.Authorization 授权

主要是配合控制器,根据user检查是否有权限执行控制器





 

 

 


 Resources文件。访问修饰符中。改成无代码生成。就可以自定义resources.designer.cs文件了











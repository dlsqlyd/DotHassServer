1 web认证的方式和游戏服务器认证的方式不同
web认证生成的token是不变的.
游戏服务器生成的sign每条都是改变的


2.
//生成一个特性
AttributeKey<NetServerOptions>.NewInstance(Options.Name)
child.GetAttribute(this.attributeKey).Set(this.channelGroup);

//特性的key是全局唯一的..因为key的生成使用的是一个静态的AttributeConstantPool
//特性的值是跟随存储到channel的..而不是全局的
//如果开两个channel,注意key不要重复



3.Sharable
标注一个channel handler可以被多个channel安全地共享。

ChannelHandlerAdapter还提供了实用方法isSharable()。如果其对应的实现被标注为Sharable，那么这个方法将返回true，表示它可以被添加到多个ChannelPipeline中。

因为一个ChannelHandler可以从属于多个ChannelPipeline，所以它也可以绑定到多个ChannelHandlerContext实例。用于这种用法的ChannelHandler必须要使用@Sharable注解标注；否则，试图将它添加到多个ChannelPipeline时将会触发异常。显而易见，为了安全地被用于多个并发的Channel（即连接），这样的ChannelHandler必须是线程安全的。



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





7.kcp的一些信息

1 如果真的需要即时的发出数据,那么需要使用client的flush方法.否则你总会有那么一个小延迟
2 kcp方法调用是不支持多线程的,如果你使用了flush,那么请不要用后台线程去DoWork().否则不安全.
3 即便你都是单线程的,也不能无脑的while true去发,这样会把缓冲区爆掉.需要在恰当的时机去看一下waitsnd的值.毕竟带宽不是无限的.
4 开一个客户端的时候,你的pingpong测试数据可能只有32qps 或者64qps.原因是这种测试正好命中了最糟糕的情况.因为消息的执行并不是收到后立刻执行的,在服务器收到数据以后,先堆积到线程池,每个worker在没有任务的时候会有一个短暂的SpinOnce(),于是这种测试正好全部命中SpinOnce(),这里如果觉得每个用户32qps不够用,就需要修改任务调度这部分.
5 一定要熟悉kcp的参数设置.要理解每个参数的意义.



 

 


 Resources文件。访问修饰符中。改成无代码生成。就可以自定义resources.designer.cs文件了











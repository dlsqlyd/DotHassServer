1.netcore mvc 的aop实现是使用中间件实现的

ControllerActionInvoker..

并不向传统的aop实现方式

因为控制器的创建和action的执行都是固定的...所以在执行前后找到该函数的ActionFilterAttribute或者类的特性
按照顺序执行就可以了

2.applicationparts  service中批量注册控制器和其他一些东西
ApplicationAssembliesProvider
- 根据入口类的assmery获取所有的依赖assmery
- 将ReferenceAssemblies内的包设置为  DependencyClassification.MvcReference 
- 依赖的包中含有ReferenceAssemblies的assmery,才会被设置DependencyClassification.ReferencesMvc 
- 将assmery封装成AssemblyItem,根据他的特性获取他的相关的assembly
- 排序取出重复的assembly
简单的说就是谁引用了ReferenceAssemblies里的包.谁的assmery就是可用的
这个类主要就是根据入口类,找到可使用的assmery


DefaultApplicationPartFactory
根据上一步的assmbly实例化AssemblyPart


applicationPart    内容块..根据assembly或者其他,创建的源
TFeature     根据特征在从源中查找所需要的东西,并存储起来


2.mvc 有两种方式,一种是默认的..一种是使用service
//默认方式.




//将所有控制器注册到di中,
ServiceBasedControllerActivator : IControllerActivator

    var feature = new ControllerFeature();
    builder.PartManager.PopulateFeature(feature);

    foreach (var controller in feature.Controllers.Select(c => c.AsType()))
    {
        builder.Services.TryAddTransient(controller, controller);
    }

    builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());



http://www.cnblogs.com/savorboard/p/aspnetcore-mvc-routing-action.html





ActionDescriptorCollectionProvider




ControllerActionDescriptorProvider




DefaultApplicationModelProvider





System.Text.Encodings.Web  用于mvc的MessageModel
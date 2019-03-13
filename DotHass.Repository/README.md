# dothass






1.DictionaryCacheHandle or SystemRuntimeCaching  以及msmemorycache都是不会序列化的



//为某个类型.单独配置缓存

   services.AddCacheManager<CacheSession>(configure: builder =>
                {
                    builder.WithMicrosoftMemoryCacheHandle();
                });



//  根据配置文件配置一个公共的缓存管理

services.AddCacheManagerConfiguration(Configuration, cfg => cfg.WithMicrosoftLogging(l => l.AddConsole()));

services.AddCacheManager();



//为某个类型.根据配置文件配置..注意是基于Configuration的

   services.AddCacheManager<CacheSession>(Configuration,configure: builder =>
                {
                    builder.WithMicrosoftMemoryCacheHandle();
                });



关于缓存为什么没有findall

https://github.com/MichaCo/CacheManager/issues/225




缓存和mysql

TODO:使用Hangfire 存储数据库操作队列..主要是为了便于监控和重试数据库操作出现的意外..可以重试等..
//未实现.
合并相同操作..
比如二十秒内..存储一次....如果二十秒内..有两次存储的动作是一样的..则只存储一次


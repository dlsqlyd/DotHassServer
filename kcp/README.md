1.配置管理器  里可以改变默认的平台..
当选择anycpu的时候,c#项目会根据当前系统生成程序
而c++项目默认是win32
release下拉菜单找到配置管理器进行修改.debug和release都得修改


2.从dll导出
2015版本
https://msdn.microsoft.com/zh-cn/library/z4zxe9k8.aspx
2017版本
https://docs.microsoft.com/zh-cn/cpp/build/exporting-from-a-dll

https://docs.microsoft.com/zh-cn/dotnet/framework/interop/consuming-unmanaged-dll-functions
3.如果找不到入口点,重新建立def文件


4.关于调试
pdb和lib记得copy过去

dll调试
在dll项目中执行exe
https://docs.microsoft.com/zh-cn/visualstudio/debugger/how-to-debug-from-a-dll-project

混合调试
nercore需要在
add the following to your profile in launchsettings.json
"nativeDebugging" : true
https://docs.microsoft.com/zh-cn/visualstudio/debugger/how-to-debug-in-mixed-mode
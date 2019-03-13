
# 先创建迁移的数据表
dotnet ef migrations add init -o Data/Migrations -v

dotnet ef migrations add init -o Data/Identity  --context DotHass.Sample.Data.GameIdentityDbContext  -v

dotnet ef migrations add init -o Data/Data  --context DotHass.Sample.Data.GameDataDbContext  -v

# 按照迁移的数据表。修改数据库

dotnet ef database update -v

dotnet ef database update --context DotHass.Sample.Data.GameIdentityDbContext -v
dotnet ef database update --context DotHass.Sample.Data.GameDataDbContext -v

# 一个bug...执行dotnet ef命令的时候回生成所有项目..项目生成事件.如果由$(ProjectDir)这样的东西.会解析不了..造成命令执行不下去..

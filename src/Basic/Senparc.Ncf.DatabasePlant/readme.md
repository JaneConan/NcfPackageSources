﻿## Senparc.Ncf.DatabasePlant 项目说明

Plant 意为“停机坪”，这意味着当你准备“检修”模块的时候，需要用到它。


### 原理

Senparc.Ncf.DatabasePlant 引用了 NCF 官方实现的所有数据库的 DatabaseConfiguration 的项目，如：Senparc.Ncf.Database.MySql、Senparc.Ncf.Database.SqlServer，等等。

### 通途

因为有了所有数据库 DatabaseConfiguration 的引用，这就意味着：一旦项目（模块）引用了 Senparc.Ncf.DatabasePlant，那么就可以获得操作所有（已经实现的）数据库的能力。

但是我们也知道，如果带着一堆数据库的 Providers 部署到生产环境，是一个负担（虽然通常对运行效率并不会有影响），
因此，我们只建议在“检修”（Debug）的时候去使用它，而在生产环境屏蔽它，为了能够顺利切换这两个场景，我们可以在引用 Senparc.Ncf.DatabasePlant 的时候，加上编译条件，如：

``` XML
<ProjectReference Condition=" '$(Configuration)' != 'Release' " Include="..\..\..\Basic\Senparc.Ncf.DatabasePlant\Senparc.Ncf.DatabasePlant.csproj" />
```

这也正是“停机坪”名称的由来：我们只在 Debug 的时候让项目“躺”在停机坪上，可以对数据库进行比如 Migration（迁移） 等各类针对多有数据库的批量操作，
而当 NCF 起飞（Release）后，这个包会被自动忽略，不会给系统带来额外的负担。

## 更新说明

当任何 `Senparc.Ncf.Database.xx` 类库版本升级后都应该同样升级当前项目版本，以确保随时引用最新的类库版本。
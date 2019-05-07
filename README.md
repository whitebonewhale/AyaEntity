# AyaEntity

## 说明

基于dapper轻量简单的orm框架，实现简单及稍复杂的sql自动生成。
可自定义扩展DB Service实现业务复杂sql。

***目前仅支持mysql数据库***



## 简单使用

> DBService类默认提供了6个基本的sql操作方法
>
> 1. GetEntity：获取一个实体
> 2. GetEntityList：获取实体列表
> 3. Update：更新
> 4. Delete：删除
> 5. Insert：增加一个实体
> 6. InsertList：增加实体列表

```c#
// 使用连接字符串 初始化manage  
SqlManager manage = new SqlManager("数据库连接字符串");
// 使用默认db service
DBService dbService = this.manage.UseService<DBService>();
// 查询id=3的 article
Article article = dbService.GetEntity<Article>(new Article { Id = 3 });
```



## 自定义扩展service

```c#
public class ArticleDBService : DBService
{
  public ArticleDBService(IDbConnection conn) : base(conn)
  {
  }
 
  public Dictionary<string, int> GetArticleDictionaryCounts()
  {
    return this.Connection.Query<KeyValuePair<string, int>>(
        // 使用框架的sql语句生成对象
        new MysqlSelectStatement()
            .Select("count(*) as `Value`", "article_name as `Key`")
            .Group("article_name")
            .From(SqlAttribute.GetTableName(typeof(Article))).ToSql()
       ).ToDictionary(m => m.Key, m => m.Value);
  }
}
```



## asp net core中使用

> 增加一个“AddDbManager”扩展方法

```c#
public static SqlManager AddDbManager(this IServiceCollection services, string conn)
{
  SqlManager sqlmanager= new SqlManager(conn);
  services.AddSingleton(sqlmanager);
  return sqlmanager;
}
```

> 在ConfigureServices方法中配置并注入

```c#
services.AddDbManager(config.GetConnectionString("dbConnection"))
	    .AddService<ArticleDBService>();
```

> 控制器中接收

```c#
public class HomeController : Controller
{
  ArticleDBService blogService;

  public HomeController(SqlManager manager)
  {
    blogService = manager.UseService<ArticleDBService>();
  }
  
  public Index()
  {
  	var modal = blogService.GetArticleDictionaryCounts();
  	return View(modal);
  }
}
```










## Nuget

Visual Studio Nuget 搜索 "AyaEntity" 下载安装，不过更新不及时，可以clone项目，本地打包使用。

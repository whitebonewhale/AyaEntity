# AyaEntity

## 说明

基于dapper的轻量简单的orm框架，实现简单及稍复杂的sql自动生成。
可自定义扩展Sql service实现业务复杂sql。

***目前仅支持mysql数据库***



> 简单使用

```c#
// 使用连接字符串 初始化manage  
SqlManager manage = new SqlManager("数据库连接字符串");
// 使用默认db service
DBService dbService = this.manage.UseService<DBService>();
// 查询id=3的 article
Article article = dbService.GetEntity<Article>(new Article { Id = 3 });

// 添加自定义sql service 并使用
this.manage.AddService<ArticleDBService>();
this.articleService = this.manage.UseService<ArticleDBService>();
// 自定扩展service, 查询id最大的article
Article max = articleService.GetMaxIdArticle();
```





## asp net core中使用

> 自行“AddDbManager”扩展方法
>
> 在ConfigureServices方法中配置并注入

```c#
services.AddDbManager(config.GetConnectionString("blog"))
	    .AddService<BlogDbService>();
```

> 控制器中接收

```c#
public class HomeController : Controller
{
  BlogDbService blogService;

  public HomeController(SqlManager manager)
  {
    blogService = manager.UseService<BlogDbService>();
  }
}

```






## Nuget

Visual Studio Nuget 搜索 "AyaEntity" 下载安装，不过更新不及时，可以clone项目，本地打包使用。

using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using AyaEntity.Statement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AyaEntity.Tests
{
  [TableName("blog_article")]
  class Article
  {

    [PrimaryKey]
    [ColumnName("id")]
    public int Id { get; set; }

    [ColumnName("article_name")]
    public string Name { get; set; }

    [ColumnName("article_title")]
    public string Title { get; set; }
  }


  class blog_article
  {
    public int id { get; set; }
    public string article_name { get; set; }
    public string article_title { get; set; }
  }



  [TestClass]
  public class BaseServiceTest
  {

    private SqlManager manage;

    public BaseServiceTest()
    {

      this.manage = new SqlManager($"Server={Config.server};Database={Config.dbName}; User={Config.username};Password={Config.pwd};charset=UTF8");

      //string m = "2";
      //// 自定义sql语句生成器，模糊匹配获取一个实体
      //ArticleSqlService service = new ArticleSqlService();
      //// 生成如下语句
      //// select top 1 * from tableName where name like '%' +@name +'%';
      //Article m2 = this.manage
      //                // 设置使用自定义的sql生成器，自定义业务）
      //                .AddService("ArticleService",service)
      //                .UseService(option =>
      //                {
      //                  option.CurrentServiceKey = "ArticleService";
      //                  option.ServiceMethod= "LikeName";
      //                })
      //                .Get<Article>(new {name = "321" });
    }

    /// <summary>
    /// 测试获取entity
    /// </summary>
    [TestMethod]
    public void TestGet()
    {
      //特性实体类
      Article artile = this.manage.Get<Article>(new Article { Name = "123" }
      );
      Assert.IsTrue(artile.Name.Equals("123"), "Article:查询出的name不为123");


      // 测试非特性实体类
      Article b_article = this.manage.Get<Article>(
        new  { article_name = "321" }
      );
      Assert.IsTrue(b_article.Name.Equals("321"), "blog_article:查询出的name不为321");


      // 测试动态参数
      artile = this.manage.Get<Article>(new { article_name = "321" });
      Assert.IsTrue(artile.Name.Equals("321"), "dynamic param: 查询出的name不为321");

    }



    /// <summary>
    /// 测试获取list entity
    /// </summary>
    [TestMethod]
    public void TestGetList()
    {
      // 无条件获取所有
      IEnumerable<Article> list = this.manage.GetList<Article>();
      Assert.IsTrue(list.Count() > 0, "查询列表失败");

      // 简单条件
      list = this.manage.GetList<Article>(new Article { Name = "123" });
      Assert.IsTrue(list.Count() > 0 && list.All(m => m.Name.Equals("123")), "123:查询列表失败");

      // 简单自定义条件
      list = this.manage.GetList<Article>(new Article { Name = "%3%" }, "article_name like @Name");
      Assert.IsTrue(list.Count() > 0 && list.All(m => m.Name.Contains("3")), "%3%:查询列表失败");



    }



    /// <summary>
    /// 测试自定义sql语句
    /// </summary>
    [TestMethod]
    public void TestExcuteCustom()
    {
      // 执行自定义sql，模糊匹配，获取一个实体
      Article artile = this.manage.ExcuteCustomGet<Article>(
        new MysqlSelectStatement()
            .Select(SqlAttribute.GetColumns(typeof(Article)))
            .Where(new { name = "%me" }, "article_name like @name")
            .From(SqlAttribute.GetTableName(typeof(Article)))
        );
      Assert.IsTrue(artile.Name.Equals("likeme"), "模糊查询匹配likeme失败");

      // 执行自定义sql： 分组查询 获取字典
      Dictionary<string,string> d = this.manage.ExcuteCustomGetList<KeyValuePair<string,string>>(
        new MysqlSelectStatement()
            .Select("count(*) as `Value`","article_name as `Key`")
            .Group("article_name")
            .From(SqlAttribute.GetTableName(typeof(Article)))
       ).ToDictionary(m=>m.Key,m=>m.Value);
      Assert.IsTrue(d.Count > 0 && d["kaakira"].Equals("2"), "字典不匹配");
    }



    private string GetSqlString()
    {
      StatementService service = new BaseStatementService();
      ISqlStatementToSql sql = service.GetExcuteSQLString("Get", typeof(Article),
               new Article { Name = "123" }
             );

      return sql.ToSql();
    }








    //// 使用默认的sql语句生成器，简单获取一个model实体
    //// 会生成如下语句
    //// select * from blog_article where name = @name
    //Article m = this.manage
    //                // 使用默认sql生成器（设置为默认,ps:可不用)
    //                //.UseService(opt=>opt.CurrentServiceKey="default")
    //                .Get<Article>(new { name = "123", });



    //// 上面设置过了，当前service sql生成类为ArticleService，直接调用和上面效果一样
    //this.manage.Get<Article>();


    //// 切换回default service sql生成器；
    //this.manage.UseService(opt => opt.CurrentServiceKey = "default")
    //            .Get<Article>();


    //// 再切换回articleservice 
    //this.manage.UseService(opt => opt.CurrentServiceKey = "ArticleService")
    //            .Get<Article>();

    //// 执行update，新增评论（文章评论数量自增+1）
    //this.manage.UseService(opt => opt.ServiceMethod = "NewCommit")
    //            .Update<Article>(new { articleId = "123" });

    //// 执行update，更改文章头部信息
    //this.manage.UseService(opt => opt.ServiceMethod = "UpdateHead")
    //             .Update<Article>(new Article { Name = "我要改名字", Title = "我要改标题", Id = "我是主键，where我" });

  }


  /// <summary>
  /// Demo：service自定义扩展
  /// </summary>
  public class ArticleSqlService : StatementService
  {


    /// <summary>
    /// 优化：只生成一次
    /// </summary>
    private MysqlSelectStatement selectSql = new MysqlSelectStatement();
    private UpdateStatement updateSql = new UpdateStatement();
    private DeleteStatement deleteSql;
    private InsertStatement insertSql;


    protected override SqlStatement CreateSql(string funcName, object conditionParameters)
    {
      string flag = funcName + ":" + this.methodName;
      switch (flag)
      {
        case "Get:LikeName":
          return this.LikeName();
        case "Update:NewCommit":
          return this.NewCommit(conditionParameters);
        // 根据主键，更新文章头部信息（文章名字，文章标题）
        case "Update:UpdateHead":
          return this.updateSql.Update(conditionParameters)
                                .Set("article_name=@Name", "article_title=@Title")
                                .Where(SqlAttribute.GetPrimaryColumn(this.entityType));
        default:
          throw new NotImplementedException("未实现方法：" + flag);
      }
    }

    /// <summary>
    /// 文章有了新评论，评论数自增+1
    /// </summary>
    /// <returns></returns>
    private SqlStatement NewCommit(object param)
    {
      return this.updateSql.Update(param).Set("commit_count+=1").Where("article_id=@articleId");
    }


    /// <summary>
    /// 模糊名字查询
    /// </summary>
    /// <returns></returns>
    private SqlStatement LikeName()
    {
      return this.selectSql
                  .Select("top 1 *")
                  .From(SqlAttribute.GetTableName(this.entityType));
    }


  }
}





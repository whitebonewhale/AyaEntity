using AyaEntity.Base;
using AyaEntity.Command;
using AyaEntity.DataUtils;
using AyaEntity.Services;
using AyaEntity.Statement;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AyaEntity.Tests
{
  [TableName("blog_article")]
  public class Article
  {
    [PrimaryKey]
    [IdentityKey]
    [ColumnName("id")]
    public int Id { get; set; }

    [ColumnName("article_name")]
    public string Name { get; set; }

    [ColumnName("article_title")]
    public string Title { get; set; }
    [ColumnName("state")]
    public byte State { get; set; }
  }

  [TableName("blog_article")]
  class Article_NotSelect
  {

    [PrimaryKey]
    [IdentityKey]
    [ColumnName("id")]
    public int Id { get; set; }

    [NotSelect]
    [ColumnName("article_name")]
    public string Name { get; set; }

    [ColumnName("article_title")]
    public string Title { get; set; }
    [ColumnName("state")]
    public byte State { get; set; }
  }
  /// <summary>
  /// 解除掉此注释，配置自己的测试数据库
  /// </summary>
  //public class Config
  //{
  //  public const string server="服务器地址";
  //  public const string username="用户名";
  //  public const string pwd ="密码";
  //  public const string dbName ="数据库名";
  //}



  [TestClass]
  public class BaseServiceTest
  {



    private SqlManager manage;
    private ArticleDBService articleService;

    public BaseServiceTest()
    {

      // 使用连接字符串 初始化manage  
      manage = new SqlManager($"Server={Config.server};Database={Config.dbName}; User={Config.username};Password={Config.pwd};charset=UTF8");
      // 使用默认db service
      DBService dbService = this.manage.UseService<DBService>();
      Article article = dbService.GetEntity<Article>(new Article { Id = 3 });

      // 添加自定义sql service 并使用
      this.manage.AddService<ArticleDBService>();
      this.articleService = this.manage.UseService<ArticleDBService>();
      Article max = articleService.GetMaxIdArticle();

    }


    [TestMethod]
    public void GetLastInsertId()
    {
      // 插入实体数据
      int id = articleService.Insert(new Article { Name = "test insert", Title = "测试插入数据" }, true);
      Assert.IsTrue(id > 1, "插入单条数据错误，v:" + id);
    }

    /// <summary>
    /// 测试not select 特性
    /// </summary>
    [TestMethod]
    public void TestNotSelect()
    {
      IEnumerable<Article_NotSelect> list = articleService.GetEntityList<Article_NotSelect>();
      Assert.IsTrue(list.All(m => m.Name == null));
    }



    /// <summary>
    /// 测试获取entity
    /// </summary>
    [TestMethod]
    public void TestGet()
    {
      //特性实体类参数，根据属性特性自动解析
      Article article = articleService.GetEntity<Article>(new Article { Name = "3 insert list 3" }
      );
      Assert.IsTrue(article.Name.Equals("3 insert list 3"), "1");


      // 测试非特性实体类参数，
      article = articleService.GetEntity<Article>(new { article_name = "3 insert list 3" });
      Assert.IsTrue(article.Name.Equals("3 insert list 3"), "2");

    }


    /// <summary>
    /// 测试获取list entity
    /// </summary>
    [TestMethod]
    public void TestGetList()
    {
      // 无条件获取所有
      IEnumerable<Article> list = articleService.GetEntityList<Article>();
      Assert.IsTrue(list.Count() > 0, "查询列表失败");

      // 简单条件
      list = articleService.GetEntityList<Article>();
      Assert.IsTrue(list.Count() > 0, "列表查询异常：简单条件");
      // 简单自定义条件
      list = articleService.GetEntityList<Article>(new { id = "%9%" }, "id like @id");
      Assert.IsTrue(list.Count() > 0 && list.All(m => m.Id.ToString().Contains('9')), "%3%:查询列表失败");

    }



    /// <summary>
    /// 测试插入数据
    /// </summary>
    [TestMethod]
    public void TestInsert()
    {
      // 插入实体数据
      int row = articleService.Insert(new Article { Name = "test insert", Title = "测试插入数据" });
      Assert.IsTrue(row == 1, "插入单条数据错误，影响行数:" + row);

      // 插入多条实体数据
      List<Article> list = new List<Article>();
      for (int i = 0; i < 10; i++)
      {
        list.Add(new Article { Name = i + " insert list " + i, Title = "测试插入数据 " + i });
      }

      row = articleService.InsertList(list);
      Assert.IsTrue(row == list.Count, "插入多条数据错误，影响行数:" + row);
    }

    //[TestMethod]
    //public void TestDeleteAll()
    //{
    //  int row = articleService.Delete<Article>(new Article { Id = 0 }, "1=1");
    //  Assert.IsTrue(row > 0);
    //}


    [TestMethod]
    public void TestDelete()
    {

      //BaseStatementService s = new BaseStatementService();
      //string str= s.GetExcuteSQLString("Delete", typeof(Article), new {id = 0 }).ToSql();
      //Assert.IsFalse(true, str);

      // 插入数据 已供删除
      articleService.Insert(new Article { Name = "temp delete ", Title = "测试插入数据" });
      int row = articleService.Delete<Article>(new Article { Name = "temp delete" });
      Assert.IsTrue(row == 1, "删除数据失败，影响行数:" + row);


      try
      {
        // 测试空where条件，抛出异常，必须携带where参数，为了数据安全
        articleService.Delete<Article>(null);
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex.Message.Contains("1=1"), "不安全的删除");

        // 测试删除所有，手动加上参数，起一个确认删除所有的功能
        IEnumerable<Article> list = articleService.GetEntityList<Article>();
        row = articleService.Delete<Article>("1=1");
        Assert.IsTrue(list.Count() == row, "未能删除所有数据,原有数据行数:" + list.Count() + ",影响行数:" + row);
        // 删除完再把数据加回去
        row = articleService.InsertList(list);
        Assert.IsTrue(row == list.Count(), "插入多条数据错误，影响行数:" + row);
      }
    }


    /// <summary>
    /// 测试删除2个maxid
    /// </summary>
    [TestMethod]
    public void TestDeleteMaxId()
    {
      // 使用自定义sql，查询最大id
      int maxid = articleService.GetMaxId();
      // 测试自定义参数
      int row = articleService.Delete<Article>(new Article { Id = maxid - 2 }, "id > @Id");
      Assert.IsTrue(row > 1 && row < 3, "自定义参数删除异常，影响行数:" + row);
    }



    [TestMethod]
    public void TestGetMaxId()
    {
      int maxid = articleService.GetMaxId();
      Assert.IsTrue(maxid > 0, "获取maxid异常，maxid:" + maxid);
    }


    /// <summary>
    /// 测试自定义sql语句
    /// </summary>
    [TestMethod]
    public void TestExcuteCustom()
    {

      // 使用自定义sql，查询最大id

      int maxid = articleService.GetMaxId();
      Article a = articleService.GetEntity<Article>(new Article { Id = maxid });
      // 执行自定义sql，模糊匹配，获取一个实体
      Article artile = articleService.LikeArticleName(a.Name);

      Assert.IsTrue(artile.Name.Equals(a.Name), "模糊查询匹配likeme失败");

    }

    /// <summary>
    /// 测试自定义SQL，获取字典对象
    /// </summary>
    [TestMethod]
    public void TestExcuteCustomToDict()
    {
      // 执行自定义sql： 分组查询 获取字典
      Dictionary<string, int> d = articleService.GetArticleDictionaryCounts();
      Assert.IsTrue(d.Count > 0);
    }

    /// <summary>
    /// 测试更新
    /// </summary>
    [TestMethod]
    public void TestUpdate()
    {
      Article max = articleService.GetMaxIdArticle();
      // 默认按照主键id更新数据
      string name = "update max name";
      int row = articleService.Update<Article>(new Article { Name = name, Id = max.Id });
      Article a = articleService.GetEntity<Article>(new { id = max.Id });
      Assert.AreEqual(a.Name, name);
    }


    /// <summary>
    /// 测试更新自定义更新
    /// </summary>
    [TestMethod]
    public void TestUpdateCustom()
    {
      Article max = articleService.GetMaxIdArticle();
      // 默认按照主键id更新数据 
      string title = "update title";
      int row = articleService.Update<Article>(new { Name = max.Name, Title = title, Id = max.Id }, "article_name=@Name AND id=@Id", "article_title=@Title");

      Assert.AreEqual(row, 1);

    }


    /// <summary>
    /// 测试更新,主键为0（测试默认值排除机制
    /// </summary>
    [TestMethod]
    public void TestUpdateDefalutId()
    {
      try
      {
        // 默认按照主键id更新数据
        Article max = articleService.GetMaxIdArticle();
        int row = articleService.Update<Article>(new Article { Name = "123", Id = 0 });
      }
      catch (Exception ex)
      {
        if (!ex.Message.Contains("1=1"))
        {
          Assert.Fail();
        }
      }

    }

  }

}





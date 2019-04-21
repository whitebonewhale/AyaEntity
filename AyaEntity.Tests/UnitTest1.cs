using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using AyaEntity.Statement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AyaEntity.Tests
{
  [TestClass]
  public class UnitTest1
  {
    const string connectionString ="";
    private SqlManager manage;


    public UnitTest1()
    {
      this.manage = new SqlManager(connectionString);

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

    [TestMethod]
    public void TestMethod1()
    {

      // 使用默认的sql语句生成器，简单获取一个model实体
      // 会生成如下语句
      // select * from blog_article where name = @name
      Article m = this.manage
                      // 使用默认sql生成器（设置为默认,ps:可不用)
                      //.UseService(opt=>opt.CurrentServiceKey="default")
                      .Get<Article>(new { name = "123", });



      // 上面设置过了，当前service sql生成类为ArticleService，直接调用和上面效果一样
      this.manage.Get<Article>();


      // 切换回default service sql生成器；
      this.manage.UseService(opt => opt.CurrentServiceKey = "default")
                  .Get<Article>();


      // 再切换回articleservice 
      this.manage.UseService(opt => opt.CurrentServiceKey = "ArticleService")
                  .Get<Article>();

      // 执行update，新增评论（文章评论数量自增+1）
      this.manage.UseService(opt => opt.ServiceMethod = "NewCommit")
                  .Update<Article>(new { articleId = "123" });

      // 执行update，更改文章头部信息
      this.manage.UseService(opt => opt.ServiceMethod = "UpdateHead")
                   .Update<Article>(new Article { Name = "我要改名字", Title = "我要改标题", Id = "我是主键，where我" });

      Assert.IsFalse(result, "1 should not be prime");
    }


  }




}

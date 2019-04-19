using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using AyaEntity.SqlStatement;
using System;

[TableName("blog_article")]
class Article
{

  [PrimaryColumn("id")]
  public string Id { get; set; }

  [ColumnName("article_name")]
  public string Name { get; set; }

  [ColumnName("article_title")]
  public string Title { get; set; }
}

class TestClass
{
  private SqlManager manage;


  public void Test()
  {
    this.manage = new SqlManager("connectString");

    // 使用默认的sql语句生成器，简单获取一个model实体
    // 会生成如下语句
    // select * from blog_article where name = @name
    Article m = this.manage
                      // 使用默认sql生成器（设置为默认,ps:可不用)
                      //.UseService(opt=>opt.CurrentServiceKey="default")
                      .Get<Article>(new { name = "123", });


    // 自定义sql语句生成器，模糊匹配获取一个实体
    ArticleSqlService service = new ArticleSqlService();
    // 生成如下语句
    // select top 1 * from tableName where name like '%' +@name +'%';
    Article m2 = this.manage
                      // 设置使用自定义的sql生成器，自定义业务）
                      .AddService("ArticleService",service)
                      .UseService(option =>
                      {
                        option.CurrentServiceKey = "ArticleService";
                        option.ServiceMethod= "LikeName";
                      })
                      .Get<Article>(new {name = "321" });

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

  }


}
/// 例子：自定义sql语句生成器 文章类，改变Get方法的行为
class ArticleSqlService : StatementService
{


  /// <summary>
  /// 优化：只生成一次
  /// </summary>
  private SelectStatement selectSql = new SelectStatement();
  private UpdateStatement updateSql = new UpdateStatement();
  private DeleteStatement deleteSql;
  private InsertStatement insertSql;


  protected override ISqlStatementToSql CreateSql(string funcName, object conditionParameters)
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

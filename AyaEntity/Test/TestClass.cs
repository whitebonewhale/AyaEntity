using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using AyaEntity.SqlStatement;

[TableName("blog_article")]
class Article
{
  public string Name { get; set; }
}
///  
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
    this.manage
          .UseService(opt => opt.CurrentServiceKey = "default")
          .Get<Article>();


    // 再切换回articleservice 
    this.manage
          .UseService(opt => opt.CurrentServiceKey = "ArticleService")
          .Get<Article>();


  }


}
/// 例子：自定义sql语句生成器 文章类，改变Get方法的行为
class ArticleSqlService : StatementService
{



  protected override ISqlStatement CreateSql(string funcName,object caluseParameters,object updateEntity)
  {
    SelectStatement sql = new SelectStatement();
    switch (funcName)
    {
      case "Get":
      default:
        sql = this.methodName.Equals("LikeName")
          ? this.LikeName()
          : new SelectStatement().From(SqlAttribute.GetTableName(this.entityType)).WhereAutoCaluse(caluseParameters);
        break;
    }
    return sql;
  }


  private SelectStatement LikeName()
  {
    return new SelectStatement()
                .Select("top 1 *")
                .From(SqlAttribute.GetTableName(this.entityType))
                .Where("name like '%'+@name+'%'");
  }


}

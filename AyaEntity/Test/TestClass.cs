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
    // 使用默认的sql语句生成器，简单获取一个model实体
    Article m = this.manage.Get<Article>(new { name = "123", });

    // 自定义sql语句生成器，模糊匹配获取一个实体
    ArticleSqlService service = new ArticleSqlService();
    Article m2 = this.manage
                      // 设置使用自定义的sql生成器，自定义业务）
                      .AddService("ArticleService",service)
                      .UseService(option =>
                      {
                        option.CurrentServiceKey = "ArticleService";
                        option.ServiceMethod= "LikeName";
                      })
                      .Get<Article>();


  }


}
/// 例子：自定义sql语句生成器 文章类，改变Get方法的行为
class ArticleSqlService : StatementService
{
  bool like = false;


  public ArticleSqlService(bool like = true)
  {
    this.like = like;
  }


  protected override ISqlStatement CreateSql(string funcName)
  {
    SelectStatement sql = new SelectStatement();
    switch (funcName)
    {
      case "Get":
      default:
        sql = this.methodName.Equals("LikeName")
          ? this.LikeName()
          : new SelectStatement().From(SqlAttribute.GetTableName(this.entityType)).WhereAutoCaluse(this.caluseParam);

        break;
    }
    return sql;
  }


  private SelectStatement LikeName()
  {
    // 生成如下语句
    // select top 1 * from tableName where name like '%' +@name +'%';
    return new SelectStatement()
                .Select("top 1 *")
                .From(SqlAttribute.GetTableName(this.entityType))
                .Where("name like '%'+@name+'%'");
  }


}

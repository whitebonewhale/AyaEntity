using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using AyaEntity.Statement;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.Tests
{
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



  /// 例子：自定义sql语句生成器 文章类，改变Get方法的行为
  public class ArticleSqlService : StatementService
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


}

using AyaEntity.Command;
using AyaEntity.DataUtils;
using AyaEntity.Services;
using AyaEntity.Statement;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AyaEntity.Tests
{


  //public class ArticleDBService : DBService
  //{
  //  public ArticleDBService(IDbConnection conn) : base(conn)
  //  {
  //  }

  //  public Dictionary<string, int> GetArticleDictionaryCounts()
  //  {
  //    return this.Connection.Query<KeyValuePair<string, int>>(
  //        new MysqlSelectStatement()
  //            .Select("count(*) as `Value`", "article_name as `Key`")
  //            .Group("article_name")
  //            .From(SqlAttribute.GetTableName(typeof(Article))).ToSql()
  //       ).ToDictionary(m => m.Key, m => m.Value);
  //  }
  //}
  /// <summary>
  /// Demo：文章 service自定义扩展
  /// </summary>
  public class ArticleDBService : DBService
  {
    public ArticleDBService()
    {
    }

    public int GetMaxId()
    {
      Type entityType = typeof(Article);
      ISqlStatementToSql sql = CommandBuilder.BuildSelect(null, entityType)
                                              .Select("Max(id)")
                                              .From(SqlAttribute.GetTableName(entityType));
      return Connection.QueryFirst<int>(sql.ToSql());
    }


    public Article GetMaxIdArticle()
    {
      Type entityType = typeof(Article);
      string tn = SqlAttribute.GetTableName(entityType);
      ISqlStatementToSql sql = CommandBuilder.BuildSelect(null, entityType)
                                             .Select(SqlAttribute.GetSelectColumns(entityType))
                                             .From(tn)
                                             .Where("Id=("
                                             + new MysqlSelectStatement()
                                                   .Select("Max(id) as Id")
                                                   .From(tn).ToSql()
                                             + ")");
      return Connection.QueryFirst<Article>(sql.ToSql());
    }


    public Article LikeArticleName(string name)
    {
      string sql = new MysqlSelectStatement()
          .Select(SqlAttribute.GetSelectColumns(typeof(Article)))
          .Where("article_name like @name")
          .From(SqlAttribute.GetTableName(typeof(Article)))
          .ToSql();

      return this.Connection.QueryFirst<Article>(sql, new { name = name.Substring(0, 2) + "%" });


    }


    public Dictionary<string, int> GetArticleDictionaryCounts()
    {
      return this.Connection.Query<KeyValuePair<string, int>>(
          new MysqlSelectStatement()
              .Select("count(*) as `Value`", "article_name as `Key`")
              .Group("article_name")
              .From(SqlAttribute.GetTableName(typeof(Article))).ToSql()
         ).ToDictionary(m => m.Key, m => m.Value);
    }
  }
}
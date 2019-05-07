using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using AyaEntity.Statement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AyaEntity.Tests
{

  public enum ArticleMehtod
  {
    LikeName,
    MaxId,
    MaxIdEntity,
    StateAdd
  }


  /// <summary>
  /// Demo：文章 service自定义扩展
  /// </summary>
  public class ArticleSqlService : BaseStatementService
  {

    public override SqlStatement GetSql(object conditionParameters)
    {
      ArticleMehtod flag = (ArticleMehtod)method;
      switch (flag)
      {
        case ArticleMehtod.LikeName:
          return this.selectSql
                  .Limit(1)
                  .Where("article_name like @Name")
                  .From(SqlAttribute.GetTableName(this.entityType));

        case ArticleMehtod.MaxId:
          return this.selectSql
                  .Select("Max(id)")
                  .Limit(1)
                  .Where(conditionParameters)
                  .From(SqlAttribute.GetTableName(this.entityType));

        case ArticleMehtod.MaxIdEntity:
          string tn = SqlAttribute.GetTableName(this.entityType);
          return this.selectSql
                  .Select(SqlAttribute.GetSelectColumns(this.entityType))
                  .From(tn)
                  .Where("Id=("
                  + new MysqlSelectStatement()
                        .Select("Max(id) as Id")
                        .Limit(1)
                        .Where(conditionParameters)
                        .From(tn).ToSql()
                  + ")");

        case ArticleMehtod.StateAdd:
          return this.updateSql
                  .Set("state+=1")
                  .Where(conditionParameters)
                  .From(SqlAttribute.GetPrimaryColumn(this.entityType));
        default:
          throw new NotImplementedException("Method not implement:" + flag);
      }
    }
  }
}
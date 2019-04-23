using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using AyaEntity.Statement;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.Tests
{

  /// <summary>
  /// Demo：文章 service自定义扩展
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
        // 模糊名字查询
        case "Get:LikeName":
          return this.selectSql
                  .Limit(1)
                  .Where("article_name like @Name")
                  .From(SqlAttribute.GetTableName(this.entityType));
        // 获取当前最大的自增id
        case "Get:GetMaxId":
          return this.selectSql
                  .Select("Max(id)")
                  .Limit(1)
                  .Where(conditionParameters)
                  .From(SqlAttribute.GetTableName(this.entityType));
        // 获取当前最大的自增id 及entity
        case "GetEntity:GetMaxIdEntity":
          string tn = SqlAttribute.GetTableName(this.entityType);
          return this.selectSql
                  .Select(SqlAttribute.GetColumns(this.entityType))
                  .From(tn)
                  .Where("Id=("
                  + new MysqlSelectStatement()
                        .Select("Max(id) as Id")
                        .Limit(1)
                        .Where(conditionParameters)
                        .From(tn).ToSql()
                  + ")");
        // state 自增加一
        case "Update:StateAdd":
          return this.updateSql
                  .Set("state+=1")
                  .Where(conditionParameters)
                  .From(SqlAttribute.GetPrimaryColumn(this.entityType));
        default:
          throw new NotImplementedException("未实现方法：" + flag);
      }
    }
  }
}
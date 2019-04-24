using AyaEntity.DataUtils;
using AyaEntity.Statement;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlServices
{
  public class BaseStatementService : StatementService
  {

    /// <summary>
    /// 优化：只生成一次
    /// </summary>
    private MysqlSelectStatement selectSql;
    private UpdateStatement updateSql;
    private DeleteStatement deleteSql;
    private InsertStatement insertSql;



    protected override SqlStatement CreateSql(string funcName, object conditionParameters)
    {
      SqlStatement sql = null;
      //typeof(SqlManager).GetMethod(funcName).GetCustomAttributes(typeof(StatementOperateAttribute), false)
      switch (funcName)
      {
        case "GetEntity":
          sql = this.Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(this.entityType)).Limit(1);
          break;
        case "GetEntityList":
          sql = this.Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(this.entityType));
          break;
        case "Update":
          sql = this.Update(conditionParameters);
          break;
        case "Delete":
          sql = this.Delete(conditionParameters);
          break;
        case "Insert":
          sql = this.Insert(conditionParameters);
          break;
        case "InsertList":
          sql = this.Insert(conditionParameters);
          break;
        default:
          throw new NotImplementedException("service类未实现SqlManage方法 case: " + funcName);
      }
      return sql;
    }



    #region 默认方法，无关紧要

    /// <summary>
    /// 生成默认select sql方法 
    /// </summary>
    /// <returns></returns>
    private MysqlSelectStatement Select(object conditionParam)
    {
      if (this.selectSql == null)
      {
        this.selectSql = new MysqlSelectStatement();
      }
      this.selectSql.From(SqlAttribute.GetTableName(this.entityType))
                    .Where(conditionParam);
      return this.selectSql;
    }

    /// <summary>
    /// 生成默认update sql方法
    /// </summary>
    /// <returns></returns>
    private UpdateStatement Update(object conditionParameters)
    {
      if (this.updateSql == null)
      {
        this.updateSql = new UpdateStatement();
      }

      this.updateSql.Set(conditionParameters)
                    .UpdateSetColumns(SqlAttribute.GetUpdateColumns(conditionParameters, out string primaryKey).ToArray())
                    .WherePrimaryKey(primaryKey)
                    .From(SqlAttribute.GetTableName(this.entityType));
      return this.updateSql;
    }


    private DeleteStatement Delete(object conditionParam)
    {
      if (this.deleteSql == null)
      {
        this.deleteSql = new DeleteStatement();
      }
      this.deleteSql.From(SqlAttribute.GetTableName(this.entityType))
                    .Where(conditionParam);
      return this.deleteSql;
    }

    private InsertStatement Insert(object conditionParam)
    {

      if (this.insertSql == null)
      {
        this.insertSql = new InsertStatement();
      }
      this.insertSql.Insert(SqlAttribute.GetInsertCoulmn(this.entityType), conditionParam)
                   .From(SqlAttribute.GetTableName(this.entityType));
      return this.insertSql;
    }

    #endregion
  }
}

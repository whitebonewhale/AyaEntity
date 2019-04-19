using AyaEntity.DataUtils;
using AyaEntity.SqlStatement;
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
    private SelectStatement selectSql;
    private UpdateStatement updateSql;
    private DeleteStatement deleteSql;
    private InsertStatement insertSql;


    protected override ISqlStatementToSql CreateSql(string funcName, object conditionParameters)
    {
      ISqlStatementToSql sql = null;
      //typeof(SqlManager).GetMethod(funcName).GetCustomAttributes(typeof(StatementOperateAttribute), false)
      switch (funcName)
      {
        case "Get":
          sql = this.Select(conditionParameters).Select("top 1 *");
          break;
        case "GetList":
          sql = this.Select(conditionParameters).Select("*");
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
    private SelectStatement Select(object conditionParam)
    {
      if (this.selectSql == null)
      {
        this.selectSql = new SelectStatement();
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

      this.updateSql.Update(conditionParameters)
                    .Set(SqlAttribute.GetUpdateColumns(this.entityType, out string primaryKey).ToArray())
                    .WherePrimaryKey(primaryKey)
                    .From(SqlAttribute.GetTableName(this.entityType));
      return this.updateSql;
    }


    private ISqlStatementToSql Delete(object conditionParam)
    {
      throw new NotImplementedException();
    }

    private ISqlStatementToSql Insert(object conditionParam)
    {

      if (this.insertSql == null)
      {
        this.insertSql = new InsertStatement();
      }
      return this.insertSql.Insert(SqlAttribute.GetInsertCoulmn(this.entityType), conditionParam)
                    .From(SqlAttribute.GetTableName(this.entityType));
    }

    #endregion
  }
}

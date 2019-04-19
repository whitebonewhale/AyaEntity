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


    protected override ISqlStatement CreateSql(string funcName, object caluseParameters, object updateEntity)
    {
      ISqlStatement sql = null;
      //typeof(SqlManager).GetMethod(funcName).GetCustomAttributes(typeof(StatementOperateAttribute), false)
      switch (funcName)
      {
        case "Get":
          sql = this.Select(caluseParameters).Select("top 1 *");
          break;
        case "GetList":
          sql = this.Select(caluseParameters).Select("*");
          break;

        case "Update":
          sql = this.Update(caluseParameters, updateEntity);
          break;
        case "Delete":
          sql = this.Delete(caluseParameters);
          break;

        case "Insert":
          sql = this.Insert(caluseParameters);
          break;
        case "InsertList":
          sql = this.Insert(caluseParameters);
          break;
        default:
          throw new NotImplementedException("service类未实现CreateSql case: " + funcName);
      }
      return sql;
    }



    #region 默认方法，无关紧要

    /// <summary>
    /// 生成默认select sql方法 
    /// </summary>
    /// <returns></returns>
    private SelectStatement Select(object caluseParam)
    {
      if (this.selectSql == null)
      {
        this.selectSql = new SelectStatement();
      }
      this.selectSql.From(SqlAttribute.GetTableName(this.entityType))
                    .WhereAutoCaluse(caluseParam);
      return this.selectSql;
    }

    /// <summary>
    /// 生成默认update sql方法
    /// </summary>
    /// <returns></returns>
    private UpdateStatement Update(object caluseParameters, object updateEntity)
    {
      if (this.updateSql == null)
      {
        this.updateSql = new UpdateStatement();
      }
      this.updateSql.Update(SqlAttribute.GetTableName(this.entityType))
                    .Set(updateEntity)
                    .WhereAutoCaluse(caluseParameters);
      return this.updateSql;
    }


    private ISqlStatement Delete(object caluseParam)
    {
      throw new NotImplementedException();
    }

    private ISqlStatement Insert(object caluseParam)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}

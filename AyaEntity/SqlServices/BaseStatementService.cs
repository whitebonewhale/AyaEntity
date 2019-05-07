using AyaEntity.DataUtils;
using AyaEntity.Statement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AyaEntity.SqlServices
{
  public class BaseStatementService : IStatementService
  {

    /// <summary>
    /// 方法枚举（整型）
    /// </summary>
    protected Enum method;

    /// <summary>
    /// sql语句操作的表实体类类型
    /// </summary>
    protected Type entityType;

    /// <summary>
    /// 优化：只生成一次
    /// </summary>
    protected MysqlSelectStatement selectSql;
    protected UpdateStatement updateSql;
    protected DeleteStatement deleteSql;
    protected InsertStatement insertSql;



    public virtual IStatementService Config(Type type, Enum method = null)
    {
      this.entityType = type;
      this.method = method;
      return this;
    }


    public virtual SqlStatement GetSql(object conditionParameters)
    {
      return this.Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(this.entityType)).Limit(1);
    }

    public virtual SqlStatement GetEntitySql(object conditionParameters)
    {
      return this.Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(this.entityType)).Limit(1);
    }

    public virtual SqlStatement GetListSql(object conditionParameters)
    {
      return this.Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(this.entityType)); ;
    }

    public virtual SqlStatement GetEntityListSql(object conditionParameters)
    {
      return this.Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(this.entityType)); ;
    }

    public virtual SqlStatement UpdateSql(object conditionParameters)
    {
      return this.Update(conditionParameters); ;
    }

    public virtual SqlStatement DeleteSql(object conditionParameters)
    {
      return this.Delete(conditionParameters);
    }



    public virtual SqlStatement InsertSql(object conditionParameters)
    {
      return this.Insert(conditionParameters);
    }

    public virtual SqlStatement InsertListSql(object conditionParameters)
    {
      return this.Insert(conditionParameters);
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

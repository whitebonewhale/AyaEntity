using AyaEntity.DataUtils;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlStatement
{
  public abstract class SqlStatement : ISqlStatementToSql
  {
    // where语句连接运算符: and/or
    public ConditionOpertor conditionOpertor = ConditionOpertor.AND;
    protected IEnumerable<string> columns;
    protected string tableName;

    protected object conditionParam;
    protected string whereCondition;


    protected virtual string getWhereCondition
    {
      get
      {
        return string.IsNullOrEmpty(this.whereCondition)
          ? SqlAttribute.GetWhereCondition(this.conditionParam, this.conditionOpertor)
          : this.whereCondition;
      }
    }


    public SqlStatement From(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    public abstract object GetParameters();
    public abstract string ToSql();



    /// <summary>
    /// 自定义where子句
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public SqlStatement Where(object sqlParam = null, params string[] whereCondition)
    {
      this.conditionParam = sqlParam;
      // 根据参数自动生成默认condition语句
      if (whereCondition != null)
      {
        this.whereCondition = "(" + string.Join(") " + this.conditionOpertor.ToString() + " (", this.whereCondition) + ")";
      }
      return this;
    }

    /// <summary>
    /// 自定义条件
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public SqlStatement Where(params string[] condition)
    {
      if (condition == null)
      {
        throw new ArgumentNullException("where 条件参数为空：condition");
      }
      this.whereCondition = "(" + string.Join(") " + this.conditionOpertor.ToString() + " (", this.whereCondition) + ")";
      return this;
    }

  }
}

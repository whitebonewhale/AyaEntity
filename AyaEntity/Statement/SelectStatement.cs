using AyaEntity.DataUtils;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace AyaEntity.Statement
{


  /// <summary>
  /// select sql语句生成,实现其他select复杂语句可继承此类扩展重写即可
  /// </summary>
  public class SelectStatement : SqlStatement
  {
    public SortType sortType = SortType.DESC;
    public string sortField;
    protected string[] groupFields;



    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public override string ToSql()
    {
      StringBuilder buffer = new StringBuilder();
      // select
      buffer.Append("SELECT ").Append((this.columns == null) ? "*" : this.columns.Join(",", m => m));
      // from
      buffer.Append(" FROM ").Append(this.tableName);
      // where
      if (string.IsNullOrEmpty(this.getWhereCondition))
      {
        buffer.Append(" WHERE ").Append(this.getWhereCondition);
      }
      // group
      if (!this.groupFields.IsEmpty())
      {
        buffer.Append(" GROUP BY " + this.groupFields.Join(",", m => m));
      }
      // sort 
      if (!string.IsNullOrEmpty(this.sortField))
      {
        buffer.Append(" ORDER BY ").Append(this.sortField).Append(" " + this.sortType.ToString());
      }
      
      return buffer.ToString();
    }


    public override object GetParameters()
    {
      return this.conditionParam;
    }




    public SelectStatement Select(params string[] columns)
    {
      this.columns = columns;
      return this;
    }


    /// <summary>
    /// 自定义分组
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public SelectStatement Group(params string[] fields)
    {
      this.groupFields = fields;
      return this;
    }

  }

}

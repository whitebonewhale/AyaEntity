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
  public class MysqlSelectStatement : SqlStatement
  {
    public SortType sortType = SortType.DESC;
    public string sortField;
    protected string[] groupFields;
    private int limitSize;
    private int limitOffset;

    private List<string> joinSelects = new List<string>();


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

      // join
      if (this.joinSelects.Count > 0)
      {
        buffer.Append(" ").Append(string.Join(" ", joinSelects));
      }
      // where
      if (!string.IsNullOrEmpty(this.getWhereCondition))
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

      if (this.limitSize > 0)
      {
        buffer.Append($" LIMIT {this.limitOffset},{this.limitSize}");
      }
      return buffer.ToString();
    }

    public MysqlSelectStatement Limit(int size, int offset = 0)
    {
      this.limitSize = size;
      this.limitOffset = offset;
      return this;
    }


    public override object GetParameters()
    {
      return this.conditionParam;
    }



    public MysqlSelectStatement Join(string joinSelect)
    {
      this.joinSelects.Add(joinSelect);
      return this;
    }



    public MysqlSelectStatement Select(params string[] columns)
    {
      this.joinSelects.Clear();
      this.columns = columns;
      return this;
    }


    /// <summary>
    /// 自定义分组
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public MysqlSelectStatement Group(params string[] fields)
    {
      this.groupFields = fields;
      return this;
    }

  }

}

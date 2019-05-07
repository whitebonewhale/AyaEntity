using AyaEntity.DataUtils;
using AyaEntity.Command;
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
  public class DeleteStatement : SqlStatement
  {


    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public override string ToSql()
    {
      StringBuilder buffer = new StringBuilder();
      // from
      buffer.Append("DELETE FROM ").Append(this.tableName);
      // where
      if (!string.IsNullOrEmpty(this.getWhereCondition))
      {
        buffer.Append(" WHERE ").Append(this.getWhereCondition);
      }
      else
      {
        throw new InvalidOperationException("delete操作必须指定where参数，ps:可指定where 1=1");
      }
      return buffer.ToString();
    }


    public override object GetParameters()
    {
      return this.conditionParam;
    }


  }

}

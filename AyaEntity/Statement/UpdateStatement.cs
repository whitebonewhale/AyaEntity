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
  /// update sql语句生成,实现其他update复杂语句可继承此类扩展重写即可
  /// </summary>
  public class UpdateStatement : SqlStatement
  {


    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public override string ToSql()
    {
      StringBuilder buffer = new StringBuilder();

      // from
      buffer.Append("UPDATE ").Append(this.tableName);
      // set fields
      buffer.Append(" SET ").Append(this.columns.Join(",", m => m));
      // where
      if (!string.IsNullOrEmpty(this.whereCondition))
      {
        buffer.Append(" WHERE ").Append(this.whereCondition);
      }
      else
      {
        throw new InvalidOperationException("update 操作必须指定 where 参数，ps:可指定 where 1=1");
      }
      return buffer.ToString();
    }


    /// <summary>
    /// 获取sql参数，参数key特殊前缀处理
    /// </summary>
    /// <returns></returns>
    public override object GetParameters()
    {
      return this.conditionParam;
    }



    public UpdateStatement Set(object conditionParameters)
    {
      this.conditionParam = conditionParameters;
      return this;
    }
  }
}

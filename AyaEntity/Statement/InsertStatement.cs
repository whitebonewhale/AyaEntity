using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
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
  public class InsertStatement : SqlStatement
  {

    Dictionary<string,string> insertColumns;



    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public override string ToSql()
    {
      StringBuilder buffer = new StringBuilder();
      // from
      buffer.Append("INSERT INTO ").Append(this.tableName);

      // set fields
      buffer.Append("(").Append(this.insertColumns.Keys.Join(",", m => m)).Append(")");
      buffer.Append(" VALUSE(").Append(this.insertColumns.Values.Join(",", m => "@" + m)).Append(")");
      return buffer.ToString();
    }




    //git @github.com:whitebonewhale/AyaEntity.git
    public InsertStatement Insert(Dictionary<string, string> insertColumns, object sqlParam)
    {
      this.conditionParam = sqlParam;
      this.insertColumns = insertColumns;
      return this;
    }




    public override object GetParameters()
    {
      return this.conditionParam;
    }

  }

}

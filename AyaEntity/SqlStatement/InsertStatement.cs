using AyaEntity.DataUtils;
using AyaEntity.SqlServices;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace AyaEntity.SqlStatement
{

  /// <summary>
  /// select sql语句生成,实现其他select复杂语句可继承此类扩展重写即可
  /// </summary>
  public class InsertStatement : ISqlStatement
  {
    // where语句连接运算符: and/or
    public CaluseOpertor caluseOpertor = CaluseOpertor.and;


    private string[] columns;
    private string tableName;
    private string[] caluseFields ;


    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public string ToSql()
    {
      StringBuilder buffer = new StringBuilder();

      // from
      buffer.Append("INSERT INTO ").Append(this.tableName);
      // set fields
      buffer.Append("(").Append(this.columns.Join(",", m => m)).Append(")");
      buffer.Append(" VALUSE(").Append(this.columns.Join(",", m => "@" + m)).Append(")");
      return buffer.ToString();
    }




    public InsertStatement From(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    public InsertStatement Valuse(params string[] columns)
    {
      this.columns = columns;
      return this;
    }

    public DynamicParameters GetParameters()
    {
      throw new NotImplementedException();
    }
  }

}

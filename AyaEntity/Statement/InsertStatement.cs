using AyaEntity.Command;
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
  public class MysqlInsertStatement : SqlStatement
  {

    Dictionary<string, string> insertColumns;

    public const string LAST_INSERT_ID = "SELECT LAST_INSERT_ID();";
    bool lastInsertId = false;


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
      buffer.Append(" VALUES(").Append(this.insertColumns.Values.Join(",", m => "@" + m)).Append(")");
      if (this.lastInsertId)
      {
        buffer.Append(";").Append(LAST_INSERT_ID);
      }
      return buffer.ToString();
    }


    public MysqlInsertStatement LastInsertId(bool flag)
    {
      this.lastInsertId = flag;
      return this;
    }


    //git @github.com:whitebonewhale/AyaEntity.git
    public MysqlInsertStatement Insert(Dictionary<string, string> insertColumns, object sqlParam)
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

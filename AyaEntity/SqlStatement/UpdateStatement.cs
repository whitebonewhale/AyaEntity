using AyaEntity.DataUtils;
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
  public class UpdateStatement : ISqlStatement
  {
    // where语句连接运算符: and/or
    public CaluseOpertor caluseOpertor = CaluseOpertor.and;


    private string[] columns;
    private string tableName;
    private string whereCaluseString ;


    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public string ToSql()
    {
      StringBuilder buffer = new StringBuilder();
      // from
      buffer.Append("UPDATE FROM ").Append(this.tableName);
      // set fields
      buffer.Append(" SET ").Append(this.columns.Join(",", m => m + "=@" + m));
      // where
      if (!string.IsNullOrEmpty(this.whereCaluseString))
      {
        buffer.Append(" WHERE ").Append(this.whereCaluseString);
      }
      return buffer.ToString();
    }




    public UpdateStatement Update(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    public UpdateStatement Set(params string[] columns)
    {
      this.columns = columns;
      return this;
    }

    /// <summary>
    /// 自定义where子句
    /// </summary>
    /// <param name="caluse"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public UpdateStatement Where(params string[] whereCaluse)
    {
      this.whereCaluseString = whereCaluse.Join($" {this.caluseOpertor.ToString()} ", m => m);
      return this;
    }


    /// <summary>
    /// 根据参数自动生成默认caluse语句
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public UpdateStatement WhereAutoCaluse(object sqlParam)
    {
      if (sqlParam != null)
      {
        this.whereCaluseString = SqlAttribute.GetCaluse(sqlParam, this.caluseOpertor.ToString());
      }
      return this;
    }



  }

}

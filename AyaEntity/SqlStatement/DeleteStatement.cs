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
  public class DeleteStatement : ISqlStatement
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
      buffer.Append("DELETE FROM ").Append(this.tableName);
      // where
      if (this.caluseFields != null && this.caluseFields.Length > 0)
      {
        buffer.Append(" WHERE ").Append(this.caluseFields.Join($" {this.caluseOpertor.ToString()} ", m => m + "=@" + m));
      }
      return buffer.ToString();
    }




    public DeleteStatement Update(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    public DeleteStatement Set(params string[] columns)
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
    public DeleteStatement Where(params string[] whereCaluse)
    {
      this.caluseFields = whereCaluse;
      return this;
    }


    /// <summary>
    /// 根据参数自动生成默认caluse语句
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public DeleteStatement WhereAutoCaluse(object sqlParam)
    {
      if (sqlParam != null)
      {
        PropertyInfo[] fields = sqlParam.GetType().GetProperties();
        this.caluseFields = SqlAttribute.GetWhereCaluse(sqlParam);
      }
      return this;
    }

    public DynamicParameters GetParameters()
    {
      throw new NotImplementedException();
    }
  }

}

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
  public class UpdateStatement : ISqlStatement
  {
    // where语句连接运算符: and/or
    public CaluseOpertor caluseOpertor = CaluseOpertor.and;


    private string[] columns;
    private string tableName;
    private string[] caluseFields;

    protected object setEntity;
    protected object caluseParam;

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
      buffer.Append(" SET ").Append(this.columns.Join(",", m => m + "=@param_" + m));
      // where
      if (this.caluseFields != null && this.caluseFields.Length > 0)
      {
        buffer.Append(" WHERE ").Append(this.caluseFields.Join($" {this.caluseOpertor.ToString()} ", m => m + "=@cparam_" + m));
      }
      return buffer.ToString();
    }

    public DynamicParameters GetParameters()
    {
      DynamicParameters parameters = new DynamicParameters();
      PropertyInfo[] properties = this.setEntity.GetType().GetProperties();
      // 添加setEntity参数
      foreach (PropertyInfo property in properties)
      {
        parameters.Add("@param_" + property.Name, property.GetValue(this.setEntity));
      }

      if (this.caluseParam != null)
      {

        properties = this.caluseParam.GetType().GetProperties();
        // 添加setEntity参数
        foreach (PropertyInfo property in properties)
        {
          parameters.Add("@cparam_" + property.Name, property.GetValue(this.caluseParam));
        }
      }

      return parameters;
    }



    public UpdateStatement Update(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    public UpdateStatement Set(object setEntity)
    {
      this.columns = SqlAttribute.GetUpdateColumns(setEntity);
      this.setEntity = setEntity;
      return this;
    }

    /// <summary>
    /// 自定义where子句
    /// </summary>
    /// <param name="caluse"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public UpdateStatement Where(object caluseParam, params string[] whereCaluse)
    {
      this.caluseParam = caluseParam;
      this.caluseFields = whereCaluse;
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
        this.caluseParam = sqlParam;
        this.caluseFields = SqlAttribute.GetWhereCaluse(sqlParam);
      }
      return this;
    }

  }

}

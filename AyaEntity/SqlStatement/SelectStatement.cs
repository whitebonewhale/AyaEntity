﻿using AyaEntity.DataUtils;
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
  public class SelectStatement : ISqlStatement
  {
    // where语句连接运算符: and/or
    public CaluseOpertor caluseOpertor = CaluseOpertor.and;
    public SortType sortType = SortType.desc;
    public string sortField;


    private string[] columns={"*"};
    private string tableName;
    private string whereCaluseString ;
    private string[] groupFields;


    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public string ToSql()
    {
      StringBuilder buffer = new StringBuilder();
      // select
      buffer.Append("SELECT ").Append(this.columns.Join(",", m => m));
      // from
      buffer.Append(" FROM ").Append(this.tableName);
      // where
      if (!string.IsNullOrEmpty(this.whereCaluseString))
      {
        buffer.Append(" WHERE ").Append(this.whereCaluseString);
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



    public SelectStatement Select(params string[] columns)
    {
      this.columns = columns;
      return this;
    }


    public SelectStatement From(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    /// <summary>
    /// 自定义where子句
    /// </summary>
    /// <param name="caluse"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public SelectStatement Where(params string[] whereCaluse)
    {
      this.whereCaluseString = whereCaluse.Join($" {this.caluseOpertor.ToString()} ", m => m);
      return this;
    }


    /// <summary>
    /// 根据参数自动生成默认caluse语句
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public SelectStatement WhereAutoCaluse(object sqlParam)
    {
      this.whereCaluseString = SqlAttribute.GetCaluse(sqlParam, this.caluseOpertor.ToString());
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

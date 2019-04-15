using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlStatement
{
  /// <summary>
  /// sql语句生成器
  /// </summary>
  public interface ISqlBuilder
  {
    DynamicParameters Parameters { get; }

    
    /// <summary>
    /// 生成sql
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    string Build();

    ISqlBuilder PagingSelect();

    #region  select

    /// <summary>
    /// 生成select部分：select T.Properties
    /// </summary>
    /// <typeparam name="T">默认实体类，查询所有公开属性字段</typeparam>
    /// <returns></returns>
    ISqlBuilder Select();

    /// <summary>
    /// 生成：select *
    /// </summary>
    /// <returns></returns>
    ISqlBuilder SelectAll();


    /// <summary>
    /// 生成select部分：select xxx,aaa,bbb 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fields">要查询的字段</param>
    /// <returns></returns>
    ISqlBuilder Select(string[] fields);

    ISqlBuilder SelectFrom();
    #endregion


    #region from
    /// <summary>
    /// 生成From部分：... from tableName
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <returns></returns>
    ISqlBuilder From(string tableName);
    ISqlBuilder From();


    #endregion
    #region where

    ISqlBuilder Where();
    /// <summary>
    /// 生成where部分
    /// </summary>
    /// <param name="caluse"></param>
    /// <returns></returns>
    ISqlBuilder Where(string caluse);
    #endregion

  }
}

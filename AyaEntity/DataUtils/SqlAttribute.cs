using AyaEntity.Statement;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AyaEntity.DataUtils
{
  public static class SqlAttribute
  {
    public static string GetTableName(Type type)
    {
      object[] name = type.GetCustomAttributes(typeof(TableNameAttribute), false);
      if (name.Length > 0)
      {
        TableNameAttribute attr = name[0] as TableNameAttribute;
        return attr.TableName;
      }
      else
      {
        return type.Name;
      }
    }


    /// <summary>
    /// 获取实体所有列名（公开属性）
    /// </summary>
    /// <returns></returns>
    public static string[] GetColumns(Type entity)
    {
      return Array.ConvertAll(entity.GetProperties(), mbox =>
      {
        ColumnNameAttribute column = mbox.GetCustomAttribute<ColumnNameAttribute>();
        if (column != null)
        {
          return column.ColumnName;
        }
        else
        {
          return mbox.Name;
        }
      });
    }


    public static string GetWhereCondition(object conditionParam, ConditionOpertor conditionOpertor)
    {
      PropertyInfo[] fields = conditionParam.GetType().GetProperties();
      return fields.Join(" " + conditionOpertor.ToString() + " ", m =>
      {
        ColumnNameAttribute column = m.GetCustomAttribute<ColumnNameAttribute>();
        string cName = (column != null) ?column.ColumnName:m.Name;
        if ((typeof(IEnumerable<object>).IsAssignableFrom(m.PropertyType)))
        {
          return cName + " in @" + m.Name;
        }
        else
        {
          return cName + "=@" + m.Name;
        }
      });
    }



    /// <summary>
    /// 根据特性获取可更新列(排除主键列），并输出主键列名
    /// </summary>
    /// <param name="updateEnity"></param>
    /// <returns></returns>
    public static List<string> GetUpdateColumns(Type updateEnity, out string primaryColumn)
    {
      List<string> results = new List<string>();
      primaryColumn = null;
      foreach (var mbox in updateEnity.GetProperties())
      {
        ColumnNameAttribute column = mbox.GetCustomAttribute<ColumnNameAttribute>();
        PrimaryColumnAttribute primary = mbox.GetCustomAttribute<PrimaryColumnAttribute>();
        if (primary != null)
        {
          primaryColumn = (string.IsNullOrEmpty(primary.ColumnName)) ? mbox.Name : primary.ColumnName;
        }
        else
        {
          results.Add((column != null ? column.ColumnName : mbox.Name) + "=@" + mbox.Name);
        }
      }
      return results;
    }


    /// <summary>
    /// 获取实体类主键列名
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public static string GetPrimaryColumn(Type entityType)
    {
      List<string> results = new List<string>();
      foreach (var mbox in entityType.GetProperties())
      {
        PrimaryColumnAttribute m = mbox.GetCustomAttribute<PrimaryColumnAttribute>();
        if (m != null)
        {
          return (string.IsNullOrEmpty(m.ColumnName)) ? mbox.Name : m.ColumnName;
        }
      }
      throw new InvalidOperationException("获取主键列错误，必须指定一个属性为主键，请为实体类“" + entityType.FullName + "”添加主键特性列");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="valuesColumn"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetInsertCoulmn(Type entityType)
    {

      Dictionary<string,string>  results = new Dictionary<string,string> ();
      foreach (var mbox in entityType.GetProperties())
      {
        PrimaryColumnAttribute m = mbox.GetCustomAttribute<PrimaryColumnAttribute>();
        if (m != null)
        {
          results.Add(string.IsNullOrEmpty(m.ColumnName) ? mbox.Name : m.ColumnName, mbox.Name);
        }
      }
      return results;
    }

    /// <summary>
    /// 根据类型与实体生成update set字段
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="updateParam"></param>
    /// <returns></returns>
    //public static string[] GetUpdateColumns(Type entityType, object updateParam)
    //{
    //  PropertyInfo[] fields =entityType.GetProperties();

    //  for (int i = 0; i < fields.Length; i++)
    //  {
    //    IEnumerable<Attribute> attributes= fields[i].GetCustomAttributes();
    //    if (attributes != null)
    //    {
    //      foreach (Attribute attribute in attributes)
    //      {
    //        // 字段默认值根据特性特殊处理
    //        if (attribute is SetDefalutValueAttribute defaultValue)
    //        {
    //          if (fields[i].GetValue(updateParam) == defaultValue.Value && fields[i].PropertyType.IsValueType)
    //          {
    //            fields[i].SetValue(updateParam, Activator.CreateInstance(fields[i].PropertyType));
    //          }

    //        }
    //      }
    //    }
    //  }
    //  var set_fi = fields.Where(m => !id_clause.Contains(m.Name));


    //  sqlmem.Append(" set ");
    //  sqlmem.Append(set_fi.Join(", ", m => m.Name + "=@" + m.Name));
    //  sqlmem.Append(" where ").Append(id_clause.Join(" and ", m => m + "=@" + m));
    //  return sqlmem.ToString();
    //}
  }

}

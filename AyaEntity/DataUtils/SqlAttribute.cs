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
    public static string[] GetSelectColumns(string alias, Type entity, params string[] customs)
    {
      List<string> names = new List<string>();
      foreach (PropertyInfo mbox in entity.GetProperties())
      {
        if (mbox.GetCustomAttribute<NotSelectAttribute>() == null)
        {
          ColumnNameAttribute column = mbox.GetCustomAttribute<ColumnNameAttribute>();
          if (column != null)
          {
            names.Add(alias + "." + column.ColumnName + " as " + mbox.Name);
          }
          else
          {
            names.Add(alias + "." + mbox.Name);
          }
        }
      }
      if (customs != null && customs.Length > 0)
      {
        names.AddRange(customs);
      }
      return names.ToArray();
    }




    /// <summary>
    /// 获取实体所有列名（公开属性）
    /// </summary>
    /// <returns></returns>
    public static string[] GetSelectColumns(Type entity, params string[] customs)
    {
      List<string> names = new List<string>();
      foreach (PropertyInfo mbox in entity.GetProperties())
      {
        if (mbox.GetCustomAttribute<NotSelectAttribute>() == null)
        {
          ColumnNameAttribute column = mbox.GetCustomAttribute<ColumnNameAttribute>();
          if (column != null)
          {
            names.Add(column.ColumnName + " as " + mbox.Name);
          }
          else
          {
            names.Add(mbox.Name);
          }
        }
      }
      if (customs != null && customs.Length > 0)
      {
        names.AddRange(customs);
      }
      return names.ToArray();
    }

    /// <summary>
    /// 根据参数，自动生成where条件语句
    /// 
    /// </summary>
    /// <param name="conditionParam"></param>
    /// <param name="conditionOpertor"></param>
    /// <returns></returns>
    public static string GetWhereCondition(object conditionParam, ConditionOpertor conditionOpertor)
    {
      // TODO:丑陋的代码，这里将来肯定要进行优化，暂时没有想到好办法
      IEnumerable<PropertyInfo> fields = conditionParam.GetType().GetProperties().Where(m =>
      {
        object value = m.GetValue(conditionParam);
        if (ValueVerify(value, m.PropertyType))
        {
          return false;
        }
        return true;
      });
      return fields.Join(" " + conditionOpertor.ToString() + " ", m =>
      {
        // 对属性值进行自定义判断，决定是否拼接进sql where语句中
        ColumnNameAttribute column = m.GetCustomAttribute<ColumnNameAttribute>();
        string cName = (column != null) ? column.ColumnName : m.Name;
        if (typeof(IEnumerable<object>).IsAssignableFrom(m.PropertyType))
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
    /// 验证实体类的属性,过滤掉值等于默认值、等于null的属性，使其不参与进sql拼接逻辑
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool ValueVerify(object value, Type propertyType)
    {
      // 1:如果参数value为null，则不拼接进sql where语句中
      // 2:暂不处理事件类型属性字段，自定义扩展service解决
      // 3:值类型 需要判断对应的默认值，如果是默认值，则判定为未赋值，不参与sql语句
      return (value == null
          || propertyType == typeof(DateTime)
          || (propertyType.IsValueType && Convert.ToDouble(value) == 0));
    }

    /// <summary>
    /// 根据特性获取可更新列(排除主键列），并输出主键列名
    /// </summary>
    /// <param name="updateEnity"></param>
    /// <returns></returns>
    public static List<string> GetUpdateColumns(object conditionParam, out string primaryColumn)
    {
      List<string> results = new List<string>();
      primaryColumn = null;
      foreach (var mbox in conditionParam.GetType().GetProperties())
      {
        object value = mbox.GetValue(conditionParam);
        if (ValueVerify(value, mbox.PropertyType))
        {
          continue;
        }

        ColumnNameAttribute column = mbox.GetCustomAttribute<ColumnNameAttribute>();
        PrimaryKeyAttribute primary = mbox.GetCustomAttribute<PrimaryKeyAttribute>();
        if (primary != null)
        {
          primaryColumn = (string.IsNullOrEmpty(column.ColumnName)) ? mbox.Name : column.ColumnName;
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
        PrimaryKeyAttribute m = mbox.GetCustomAttribute<PrimaryKeyAttribute>();
        if (m != null)
        {
          ColumnNameAttribute column = mbox.GetCustomAttribute<ColumnNameAttribute>();
          return (string.IsNullOrEmpty(column.ColumnName)) ? mbox.Name : column.ColumnName;
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

      Dictionary<string, string> results = new Dictionary<string, string>();
      foreach (var mbox in entityType.GetProperties())
      {
        // 非自增列
        IdentityKeyAttribute identity = mbox.GetCustomAttribute<IdentityKeyAttribute>();
        NotInsertAttribute not = mbox.GetCustomAttribute<NotInsertAttribute>();
        if (identity == null && not == null)
        {
          ColumnNameAttribute m = mbox.GetCustomAttribute<ColumnNameAttribute>();
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

using AyaEntity.SqlStatement;
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
    /// 获取实体所有列（公开属性）
    /// </summary>
    /// <returns></returns>
    public static string[] GetColumns(Type entity)
    {
      return Array.ConvertAll(entity.GetProperties(), mbox => mbox.Name);
    }


    /// <summary>
    /// 根据参数与特性获取sql条件从句
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static string GetCaluse<T>(T entity, string opertor)
    {
      PropertyInfo[] fields = typeof(T).GetProperties();
      return fields.Join(" " + opertor + " ", m =>
          {
            if ((typeof(IEnumerable<object>).IsAssignableFrom(m.PropertyType)))
            {
              return m.Name + " in @" + m.Name;
            }
            else
            {
              return m.Name + "=@" + m.Name;
            }
          });
    }

    public static string[] GetUpdateColumns(object updateParam)
    {
      return Array.ConvertAll(updateParam.GetType().GetProperties(), mbox => mbox.Name);
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

using AyaEntity.SqlStatement;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AyaEntity.SqlStatement
{
  public class SqlAttribute
  {
    public string GetTableName(Type type)
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
    public IEnumerable<string> GetColumns(Type entity)
    {
      return entity.GetProperties().Select(mbox => mbox.Name);
    }


    /// <summary>
    /// 根据参数与特性获取sql条件从句
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public string GetCaluse(object entity)
    {
      IEnumerable<PropertyInfo> fields = entity.GetType().GetProperties();
      return fields.Join(" and ", m =>
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

  }
}

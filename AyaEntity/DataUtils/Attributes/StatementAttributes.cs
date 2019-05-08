using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaEntity.DataUtils
{

  public enum SqlOperate
  {
    Update,
    Insert,
    Select,
    Delete
  }


  public enum SortType
  {
    ASC,
    DESC
  }

  public enum ConditionOpertor
  {
    AND,
    OR,
  }


  /// <summary>
  /// 因为一些原因，不想插入此字段
  /// </summary>
  public class NotInsertAttribute : Attribute
  {
  }



  /// <summary>
  /// 因为一些原因，不想查询此字段
  /// </summary>
  public class NotSelectAttribute : Attribute
  {
  }


  /// <summary>
  /// 主键列特性标识
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class PrimaryKeyAttribute : Attribute
  {
    public PrimaryKeyAttribute()
    {
    }

  }


  /// <summary>
  /// 列名
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class ColumnNameAttribute : Attribute
  {
    public string ColumnName { get; set; }
    public ColumnNameAttribute(string name)
    {
      this.ColumnName = name;
    }
  }



  /// <summary>
  /// 自增主键
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class IdentityKeyAttribute : Attribute
  {

  }


  [AttributeUsage(AttributeTargets.Class)]

  public class TableNameAttribute : Attribute
  {
    public string TableName { get; set; }
    public TableNameAttribute(string tbName)
    {
      this.TableName = tbName;
    }
  }
}

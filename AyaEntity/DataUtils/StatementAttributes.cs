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


  public class StatementOperateAttribute : Attribute
  {
    public SqlOperate Operate;
    public StatementOperateAttribute(SqlOperate operate)
    {
      this.Operate = operate;
    }
  }


  /// <summary>
  /// 主键列特性标识
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class PrimaryColumnAttribute : Attribute
  {
    public string ColumnName { get; set; }
    public PrimaryColumnAttribute(string name)
    {
      this.ColumnName = name;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaEntity.SqlStatement

{
  /// <summary>
  /// sql语句操作类型
  /// </summary>
  public enum SqlClause
  {
    Like,
    NotIn,
    In,
    Equal,
  }
  public enum SqlOperate
  {
    Update,
    Insert,
    Select,
    Delete
  }

  public class NotMappedAttribute : Attribute
  {

  }

  public class KeyAttribute : Attribute
  {

  }

  public class IdentityKeyAttribute : Attribute
  {

  }

  public class TableNameAttribute : Attribute
  {
    public string TableName { get; set; }
    public TableNameAttribute(string tbName)
    {
      TableName = tbName;
    }
  }
}

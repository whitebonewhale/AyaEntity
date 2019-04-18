using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaEntity.SqlStatement
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
    asc,
    desc
  }

  public enum CaluseOpertor
  {
    and,
    or
  }


  public class StatementOperateAttribute : Attribute
  {
    public SqlOperate Operate;
    public StatementOperateAttribute(SqlOperate operate)
    {
      this.Operate = operate;
    }
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
      this.TableName = tbName;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaEntity.Base
{
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

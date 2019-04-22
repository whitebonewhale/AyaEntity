using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.DataUtils
{
  public class SqlManageException : Exception
  {

    public SqlManageException(string message, Exception ex) : base(message, ex)
    {
    }



  }
}

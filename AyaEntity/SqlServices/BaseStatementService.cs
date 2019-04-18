using AyaEntity.SqlStatement;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlServices
{
  public class BaseStatementService : StatementService
  {



    protected override ISqlStatement CreateSql(string funcName)
    {
      ISqlStatement sql = null;

      //typeof(SqlManager).GetMethod(funcName).GetCustomAttributes(typeof(StatementOperateAttribute), false)

      switch (funcName)
      {
        case "Get":
          sql = this.Select().Select("top 1 *");
          break;
        case "GetList":
          sql = this.Select().Select("*");
          break;

        case "Update":
          sql = this.Update();
          break;

        case "Delete":
          sql = this.Select();
          break;

        case "Insert":
          sql = this.Select();
          break;
        case "InsertList":
          sql = this.Select();
          break;
      }
      return sql;
    }


  }

}

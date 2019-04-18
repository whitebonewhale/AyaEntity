//using AyaEntity.SqlStatement;
//using System;
//using System.Collections.Generic;

//namespace AyaEntity.Base.Mysql
//{
//  public class MysqlDBSqlBuilder : SqlBuilder
//  {
//    public MysqlDBSqlBuilder(Type entityType = null, object param = null) : base(entityType, param)
//    {

//    }

//    private override string CreatePagingQuery(PagingParameter pag)
//    {
//      IEnumerable<string> fields = Parameters.ParameterNames;
//      this.Select()
//      StringBuilder sqlmem = new StringBuilder("select " + pag.Columns + " from " + pag.TableName);
//      sqlmem.Append(pag.Caluse);
//      sqlmem.Append(" Order by ").Append(pag.OrderField).Append(" ").Append(pag.OrderType);
//      sqlmem.Append(" limit @StartRow,@RowSize;");
//      // 查询总条数
//      if (pag.RequiredTotal)
//      {
//        sqlmem.Append("SELECT count(*) from ").Append(pag.TableName).Append(pag.Caluse);
//      }
//      return sqlmem.ToString();
//    }
//  }
//}

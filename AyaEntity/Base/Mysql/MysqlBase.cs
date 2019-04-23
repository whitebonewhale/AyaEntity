//using AyaEntity.Base;
//using AyaEntity.SqlStatement;
//using AyaEntity.SQLTools;
//using Dapper;
//using MySql.Data.MySqlClient;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AyaEntity.Base.Mysql
//{
//  public class MySqlBase : SqlClientBase
//  {
//    public MySqlBase(string connstr) : base(connstr)
//    {
//      Connection = new MySqlConnection(this.ConnectionString);
//    }




public PagingResult<T> PagingQuery<T>(PagingParameter pag, string condition)
{
  IEnumerable<string> fields = pag.Parameters.ParameterNames;
  if (pag.TableName == null)
  {
    pag.SetTableName(this.GetTableName(typeof(T)));
  }
  string sql = CreatePagingQuery(pag);

  var multi = Connection.QueryMultiple(sql, pag.PageParameters);

  PagingResult<T> result = new PagingResult<T>();


  result.PageIndex = pag.PageIndex;
  result.RowSize = pag.RowSize;
  result.Rows = multi.Read<T>();
  if (pag.RequiredTotal)
  {
    result.Total = multi.ReadSingle<int>();
  }
  return result;
}


//    public int AutoIdByAdd<T>(T dyparam)
//    {
//      if (dyparam == null)
//        throw new ArgumentNullException("dyparam");
//      string tableName = GetTableName(typeof(T));
//      return Connection.ExecuteScalar<int>(SqlBuilder.ToInsert(tableName, dyparam?.GetType()) + ";select @@IDENTITY;", dyparam);
//    }



//  }
//}

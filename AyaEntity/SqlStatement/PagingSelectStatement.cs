using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlStatement
{
  public class PagingSelectStatement : SelectStatement
  {
   
    /// <summary>
    /// 每页数据条数
    /// </summary>
    public int RowSize = 10;
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex = 1;

    /// <summary>
    /// 起始行计算
    /// </summary>
    public int StartRow { get { return (this.PageIndex - 1) * this.RowSize; } }
    /// <summary>
    /// 要查询的表名
    /// </summary>
    public string TableName { get; private set; }

    /// <summary>
    /// 是否需要数据总条数
    /// </summary>
    public bool RequiredTotal;



    /// <summary>
    /// 重写build方法
    /// </summary>
    /// <returns></returns>
    public override string Build()
    {
      StringBuilder buffer = new StringBuilder();


    }


    private void QueryListBuild(StringBuilder buffer)
    {
      // select
      buffer.Append("SELECT ").Append(this.columns.Join(",", m => m));
      // from
      buffer.Append(" FROM ").Append(this.tableName);
      // where
      if (!string.IsNullOrEmpty(this.whereCaluseString))
      {
        buffer.Append(" WHERE ").Append(this.whereCaluseString);
      }
      // group
      if (!this.groupFields.IsEmpty())
      {
        buffer.Append(" GROUP BY " + this.groupFields.Join(",", m => m));
      }
      // sort 
      if (!string.IsNullOrEmpty(this.sortField))
      {
        buffer.Append(" ORDER BY ").Append(this.sortField).Append(" " + this.sortType.ToString());
      }
      buffer.Append(" limit @StartRow,@PageSize;");
    }

    /// 构建分页查询数据总条数语句
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="caluse"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    private StringBuilder BuildPageQueryTotal(string tableName, string caluse)
    {
      StringBuilder sqlmem = new StringBuilder("SELECT count(*) from " + tableName);
      if (string.IsNullOrEmpty(caluse))
      {
        sqlmem.Append(" where ").Append(caluse);
      }
      return sqlmem;
    }


    public override IEnumerable<T> GetCustomPageList<T>(Pagination pag, string tableName, string caluse, string columns = null)
    {
      IEnumerable<string> fields = pag.DyParameters.ParameterNames;
      if (string.IsNullOrEmpty(caluse))
      {
        caluse = fields.Join(" and ", m => m + "=@" + m);
      }
      StringBuilder sqlmem = BuildePageQuerySql(pag, tableName, caluse);
      pag.TotalCount = this.Connection.ExecuteScalar<int>(this.BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters);
      return this.Connection.Query<T>(sqlmem.ToString(), pag.PageDyParameters);
    }


    public override IEnumerable<T> GetPageList<T>(Pagination pag, string caluse = null)
    {
      IEnumerable<string> fields = pag.DyParameters.ParameterNames;
      if (string.IsNullOrEmpty(caluse))
      {
        caluse = fields.Join(" and ", m => m + "=@" + m);
      }
      string tableName = this.GetTableName(typeof(T));
      StringBuilder sqlmem = BuildePageQuerySql(pag, tableName, caluse);
      pag.TotalCount = this.Connection.ExecuteScalar<int>(this.BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters);
      return this.Connection.Query<T>(sqlmem.ToString(), pag.PageDyParameters);
    }

    /// <summary>
    /// 完整参数（包含分页条件数
    /// </summary>
    public DynamicParameters PageParameters
    {
      get
      {
        DynamicParameters param = new DynamicParameters();
        param.Add("@StartRow", this.StartRow);
        param.Add("@RowSize", this.RowSize);
        param.Add("@OrderField", this.OrderField);
        return param;
      }
    }


  }
}

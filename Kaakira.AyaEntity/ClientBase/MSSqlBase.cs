using Dapper;
using KiraEntity.SQLTools;
using KiraEntity.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KiraEntity.Base
{


	/// <summary>
	/// 介绍：微软sql-server Base类 
	/// 版本：1.0
	/// 作者：https://kaakira.com
	/// </summary>
	public class MSSqlBase : SqlClientBase
	{

		public MSSqlBase(string connstr) : base(connstr)
		{
			Connection = new SqlConnection(this.ConnectionString);
		}
		#region 私有方法



		/// <summary>
		/// 构建分页查询语句
		/// </summary>
		/// <param name="pag"></param>
		/// <param name="tableName"></param>
		/// <param name="caluse"></param>
		/// <returns></returns>
		private StringBuilder BuildePageQuerySql(Pagination pag, string tableName, string caluse, string columns = null)
		{
			if (string.IsNullOrEmpty(columns))
			{
				columns = "*";
			}
			StringBuilder sqlmem = new StringBuilder("SELECT TOP " + pag.PageSize + " " + columns + " FROM ");
			sqlmem.Append("( SELECT ").Append(" ROW_NUMBER() OVER(ORDER BY " + pag.OrderField + " " + pag.OrderType);
			sqlmem.Append(") AS RowNo,").Append(columns).Append(" from ").Append(tableName);
			if (!string.IsNullOrEmpty(caluse))
			{
				sqlmem.Append(" where ").Append(caluse);
			}
			sqlmem.Append(") as pageTable ");
			sqlmem.Append("WHERE RowNo > " + pag.StartRow);
			return sqlmem;
		}


		/// <summary>
		/// 构建分页查询数据总条数语句
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="caluse"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		private StringBuilder BuildPageQueryTotal(string tableName, string caluse)
		{
			StringBuilder sqlmem = new StringBuilder("SELECT count(*) from " + tableName);
			if (!string.IsNullOrEmpty(caluse))
			{
				sqlmem.Append(" where ").Append(caluse);
			}
			return sqlmem;
		}
		#endregion


		/// <summary>
		/// 分页查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pag"></param>
		/// <param name="tableName"></param>
		/// <param name="caluse"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public override PagingResult<T> GetCustomPageList<T>(Pagination pag, string tableName, string caluse = null, string column = null)
		{
			if (string.IsNullOrEmpty(caluse))
			{
				IEnumerable<string> fields = pag.DyParameters.ParameterNames;
				caluse = fields.Join(" and ", m => m + "=@" + m);
			}
			StringBuilder sqlmem = BuildePageQuerySql(pag, tableName, caluse, column);
			return new PagingResult<T>
			{
				Total = this.Connection.ExecuteScalar<int>(BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters),
				Rows = this.Connection.Query<T>(sqlmem.ToString(), pag.PageDyParameters)
			};
		}


		/// <summary>
		/// 分页查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pag"></param>
		/// <returns></returns>
		public override PagingResult<T> GetPageList<T>(Pagination pag, string caluse = null)
		{
			if (string.IsNullOrEmpty(caluse))
			{
				IEnumerable<string> fields = pag.DyParameters.ParameterNames;
				caluse = fields.Join(" and ", m => m + "=@" + m);
			}

			String tableName = typeof(T).Name;
			StringBuilder sqlmem = BuildePageQuerySql(pag, tableName, caluse);
			return new PagingResult<T>
			{
				Total = this.Connection.ExecuteScalar<int>(BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters),
				Rows = this.Connection.Query<T>(sqlmem.ToString(), pag.PageDyParameters)
			};
		}

		/// <summary>
		/// 递归查询父级
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <param name="param"></param>
		/// <param name="cteParent"></param>
		/// <returns></returns>
		public IEnumerable<T> TraceParentsCTE<T>(string id, object param, string cteParent = "ParentId")
		{
			String tableName = typeof(T).Name;

			string sql = $@"
WITH CTE AS 
(
    SELECT * from {tableName} where {id}=@{id}
    UNION ALL 
    SELECT {tableName}.* from CTE
    JOIN {tableName} on {tableName}.{id}=CTE.{cteParent}
)
SELECT * FROM CTE";
			return this.Connection.Query<T>(sql, param);
		}

		public override int AutoIdByAdd<T>(T dyparam)
		{

			if (dyparam == null)
				throw new ArgumentNullException("dyparam");
			string tableName = GettableName(typeof(T));

			return Connection.ExecuteScalar<int>(state.ToInsert(tableName, dyparam?.GetType()) + ";select @@identity", dyparam);
		}



	}

}

using AyaEntity.SQLTools;
using System.Collections.Generic;
using System.Data;

namespace AyaEntity.Base
{
	/// <summary>
	/// Connection 接口
	/// </summary>
	public interface ISqlClientBase
	{
		string ConnectionString { get; }
		IDbConnection Connection { get; }


        int Execute(string sql, object param = null);
        IEnumerable<T> Query<T>(string sql, object param = null);

        /// <summary>
        /// 简单条件获取单个对象
        /// </summary>
        T GetFirst<T>(object dyparam = null);
		T Get<T>(object dyparam = null);
		/// <summary>
		/// 自定义条件获取单个对象
		/// </summary>
		T GetCustom<T>(string tablename, object dyparam, string clause = null);
		T GetCustom<T>(string tbName, string column, object param, string clause = null);

		/// <summary>
		/// 简单条件获取多个对象
		/// </summary>
		IEnumerable<T> GetList<T>(object dyparam = null);
		/// <summary>
		/// 自定义条件获取多个对象
		/// </summary>
		IEnumerable<T> GetCustomList<T>(string tbName, string column, object param, string clause = null);
		IEnumerable<T> GetCustomList<T>(string tableName, object sqlParameter, string clause);
		/// <summary>
		/// 分页获取多个对象
		/// </summary>
		PagingResult<T> GetPageList<T>(Pagination pag, string clause);

		/// <summary>
		/// 自定义条件，分页获取多个对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pag">分页</param>
		/// <param name="tableName">表名</param>
		/// <param name="clause">条件语句</param>
		/// <param name="columns">查询的列</param>
		PagingResult<T> GetCustomPageList<T>(Pagination pag, string tableName, string clause = null, string columns = null);

		void Add<T>(T data);
		int AutoIdByAdd<T>(T data);
		int AddList<T>(IEnumerable<T> entitylist);
		int Clear<T>();

		int Update<T>(object dyparam, params string[] idkey);

		int Delete<T>(object dyparam);
	}
}

using Dapper;
using AyaEntity.SQLTools;
using AyaEntity.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace AyaEntity.Base
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SqlClientBase : ISqlClientBase
	{
		public string ConnectionString { get; private set; }
		protected SqlStatement state = new SqlStatement();
		public SqlClientBase(string connstr)
		{
			this.ConnectionString = connstr;
		}
		public IDbConnection ConnectionAsync
		{
			get
			{
				return new SqlConnection(ConnectionString);
			}
		}



        public int Execute(string sql,object param)
        {
            return this.Connection.Execute(sql, param);
        }
		protected string GetTableName(Type type)
		{
			object[] name = type.GetCustomAttributes(typeof(TableNameAttribute), false);
			if (name.Length > 0)
			{
				TableNameAttribute attr = name[0] as TableNameAttribute;
				return attr.TableName;
			}
			else
			{
				return type.Name;
			}

		}


		public IDbConnection Connection { get; protected set; }



		public T GetFirst<T>(object dyparam = null)
		{
			string tableName = GetTableName(typeof(T));
			return this.Connection.QueryFirstOrDefault<T>(state.ToSelect(tableName, dyparam?.GetType()), dyparam);
		}
		public T Get<T>(object dyparam = null)
		{
			string tableName = GetTableName(typeof(T));
			return this.Connection.QuerySingleOrDefault<T>(state.ToSelect(tableName, dyparam?.GetType()), dyparam);
		}
		public T GetCustom<T>(string tableName, object sqlParameter, string clause = null)
		{
			return this.Connection.QuerySingleOrDefault<T>(state.ToSelect(tableName, clause), sqlParameter);
		}

		public T GetCustom<T>(string tbName, string column, object param, string clause = null)
		{
			string sql = state.CustomSelect(tbName, clause, param.GetType(), column);
			return this.Connection.QuerySingle<T>(sql, param);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dyparam"></param>
		/// <returns></returns>
		public IEnumerable<T> GetList<T>(object dyparam = null)
		{
			string tableName = GetTableName(typeof(T));

			return this.Connection.Query<T>(state.ToSelect(tableName, dyparam?.GetType()), dyparam);
		}



		public IEnumerable<T> GetCustomList<T>(string tbName, string column, object param, string clause = null)
		{

			string sql = state.CustomSelect(tbName, clause, param.GetType(), column);
			return this.Connection.Query<T>(sql, param);
		}

		public IEnumerable<T> GetCustomList<T>(string tableName, object sqlParameter, string clause = null)
		{

			return this.Connection.Query<T>(state.ToSelect(tableName, clause), sqlParameter);
		}


		/// <summary>
		/// 添加数据，
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dyparam"></param>
		/// <param name="autoId">是否返回自增identityid</param>
		/// <returns></returns>
		public void Add<T>(T dyparam)
		{
			if (dyparam == null)
				throw new ArgumentNullException("dyparam");
			string tableName = GetTableName(typeof(T));

			Connection.Execute(state.ToInsert(tableName, dyparam?.GetType()), dyparam);
		}

		public int Update<T>(object dyparam, params string[] idkey)
		{
			if (dyparam == null)
				throw new ArgumentNullException("dyparam");
			if(idkey.Length == 0)
			{
				throw new ArgumentException("where idkey参数至少有一个");
			}

			string tableName = GetTableName(typeof(T));

			return this.Connection.Execute(state.ToUpdate(tableName, dyparam?.GetType(), idkey), dyparam);
		}

		public int AddList<T>(IEnumerable<T> entitylist)
		{
			Type objType = typeof(T);
			string tableName = GetTableName(objType);

			return this.Connection.Execute(state.ToInsert(tableName, objType), entitylist);
		}

		/// <summary>
		/// 清空表，返回影响行数，
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public int Clear<T>()
		{
			string tableName = GetTableName(typeof(T));
			return this.Connection.Execute("truncate table " + tableName);
		}

		public int Delete<T>(object dyparam)
		{
			string tableName = GetTableName(typeof(T));

			return this.Connection.Execute(state.ToDelete(tableName, dyparam?.GetType()), dyparam);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pag"></param>
		/// <param name="clause"></param>
		/// <returns></returns>
		public abstract PagingResult<T> GetPageList<T>(Pagination pag, string clause);
		public abstract PagingResult<T> GetCustomPageList<T>(Pagination pag, string tableName, string clause, string columns);

		public abstract int AutoIdByAdd<T>(T data);
	}
}


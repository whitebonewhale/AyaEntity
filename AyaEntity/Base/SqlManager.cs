using AyaEntity.Command;
using AyaEntity.DataUtils;
using AyaEntity.Services;
using AyaEntity.Statement;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AyaEntity.Base
{


  public enum SqlFunction
  {
    Get,
    GetEntity,
    GetList,
    GetEntityList,
    Delete,
    Insert,
    InsertList,
    Update
  }

  /// <summary>
  /// SQLDAO 数据交互类，
  /// 角色：兼职sql语句管理者（控制sql建造者）
  /// </summary>
  public class SqlManager
  {

    public Type CurrentServiceKey;
    public Dictionary<Type, DBService> servicesPool;
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; }
    /// <summary>
    /// 连接对象
    /// </summary>
    public IDbConnection Connection { get; protected set; }



    /// <summary>
    /// sql连接类构造函数。
    /// 参数：连接字符串，默认sql语句生成器
    /// </summary>
    /// <param name="conn"></param>

    public SqlManager(string conn, DBService defaultService = null)
    {
      ConnectionString = conn;
      Connection = new MySqlConnection(conn);
      servicesPool = new Dictionary<Type, DBService>();
      if (defaultService == null)
      {
        defaultService = new DBService(Connection);
      }
      this.servicesPool.Add(typeof(DBService), defaultService);
    }


    /// <summary>
    /// 添加sql service处理生成器
    /// </summary>
    /// <param name="key"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public SqlManager AddService<T>() where T : DBService
    {
      T service = (T)Activator.CreateInstance(typeof(T), Connection);
      // 参数不允许null
      if (service == null)
      {
        throw new ArgumentNullException("service");
      }
      this.servicesPool.Add(typeof(T), service);
      return this;
    }


    /// <summary>
    /// 根据具体业务需求生成sql语句
    /// </summary>
    public T UseService<T>() where T : DBService
    {
      return (T)this.servicesPool[typeof(T)];
    }

    ///// <summary>
    ///// 分页获取一个自定义类型列表
    ///// </summary>
    ///// <typeparam name="TOutput"></typeparam>
    ///// <typeparam name="TEntity"></typeparam>
    ///// <param name="parameters"></param>
    ///// <param name="whereCondition"></param>
    ///// <returns></returns>
    //public PagingResult<TOutput> GetPaging<TOutput, TEntity>(object parameters, bool total = false, string whereCondition = null)
    //{
    //  PagingResult<TOutput> result = new PagingResult<TOutput>();

    //  Type type = typeof(TEntity);
    //  ISqlStatementToSql sql =this.UseService<BaseStatementService>()
    //                          .Config("GetPaging", type, parameters)
    //                          .Where(whereCondition);

    //  result.Rows = this.Connection.Query<TOutput>(sql.ToSql(), sql.GetParameters());
    //  if (total)
    //  {
    //    ISqlStatementToSql t_sql = this.currentService
    //                          .Config("GetPaging:Total", type, parameters)
    //                          .Where(whereCondition);
    //    result.Total = this.Connection.QueryFirst<int>(t_sql.ToSql());
    //  }
    //  return result;
    //}




    ///// <summary>
    ///// 获取一个实体，不存在则返回默认值
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="sql"></param>
    ///// <returns></returns>
    //public T GetOrDefault<T>()
    //{
    //  return Connection.QueryFirstOrDefault<T>(sql.Build(), sql.Parameters);
    //}


    ///// <summary>
    ///// 查询list
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="sql"></param>
    ///// <returns></returns>

    //public IEnumerable<T> QueryList<T>()
    //{
    //  return Connection.Query<T>(sql.Build(), sql.Parameters);
    //}


    //#region old
    ///// <summary>
    ///// 使用连接字符串初始化对象
    ///// </summary>
    ///// <param name="connstr"></param>
    //public SqlClientBase(string connstr)
    //{
    //  ConnectionString = connstr;
    //}

    ///// <summary>
    ///// 执行任意sal语句，返回影响行数
    ///// </summary>
    ///// <param name="sql"></param>
    ///// <param name="param"></param>
    ///// <returns></returns>
    //public int Execute(string sql, object param)
    //{
    //  return Connection.Execute(sql, param);
    //}




    //public IDbConnection Connection { get; protected set; }



    //public T GetFirst<T>(object dyparam)
    //{
    //  string sql = SqlBuilder.Select<T>(dyparam);
    //  return Connection.QuerySingleOrDefault<T>(sql);
    //}
    //public T Get<T>(object dyparam = null)

    //{
    //  throw new NotImplementedException();
    //  //  string tableName = SqlBuilder.GetTableName(typeof(T));
    //  //  return this.Connection.QuerySingleOrDefault<T>(SqlBuilder.ToSelect(tableName, dyparam?.GetType()), dyparam);
    //}
    //public T GetCustom<T>(string tableName, object sqlParameter, string clause = null)
    //{
    //  return Connection.QuerySingleOrDefault<T>(SqlBuilder.ToSelect(tableName, clause), sqlParameter);
    //}

    //public T GetCustom<T>(string tbName, string column, object param, string clause = null)
    //{
    //  string sql = SqlBuilder.CustomSelect(tbName, clause, param.GetType(), column);
    //  return Connection.QuerySingle<T>(sql, param);
    //}


    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="dyparam"></param>
    ///// <returns></returns>
    //public IEnumerable<T> GetList<T>(object dyparam = null)
    //{
    //  throw new NotImplementedException();
    //  //string tableName = GetTableName(typeof(T));

    //  //return this.Connection.Query<T>(SqlBuilder.ToSelect(tableName, dyparam?.GetType()), dyparam);
    //}



    //public IEnumerable<T> GetCustomList<T>(string tbName, string column, object param, string clause = null)
    //{

    //  string sql = SqlBuilder.CustomSelect(tbName, clause, param.GetType(), column);
    //  return Connection.Query<T>(sql, param);
    //}

    //public IEnumerable<T> GetCustomList<T>(string tableName, object sqlParameter, string clause = null)
    //{

    //  return Connection.Query<T>(SqlBuilder.ToSelect(tableName, clause), sqlParameter);
    //}


    ///// <summary>
    ///// 添加数据，
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="dyparam"></param>
    ///// <param name="autoId">是否返回自增identityid</param>
    ///// <returns></returns>
    //public void Add<T>(T dyparam)
    //{
    //  throw new NotImplementedException();
    //  //if (dyparam == null)
    //  //  throw new ArgumentNullException("dyparam");
    //  //string tableName = GetTableName(typeof(T));

    //  //Connection.Execute(SqlBuilder.ToInsert(tableName, dyparam?.GetType()), dyparam);
    //}

    //public int Update<T>(object dyparam, params string[] idkey)
    //{
    //  throw new NotImplementedException();
    //  if (dyparam == null)
    //  {
    //    throw new ArgumentNullException("dyparam");
    //  }

    //  if (idkey.Length == 0)
    //  {
    //    throw new ArgumentException("where idkey参数至少有一个");
    //  }

    //  //string tableName = GetTableName(typeof(T));

    //  //return this.Connection.Execute(SqlBuilder.ToUpdate(tableName, dyparam?.GetType(), idkey), dyparam);
    //}

    //public int AddList<T>(IEnumerable<T> entitylist)
    //{
    //  throw new NotImplementedException();
    //  //Type objType = typeof(T);
    //  //string tableName = GetTableName(objType);

    //  //return this.Connection.Execute(SqlBuilder.ToInsert(tableName, objType), entitylist);
    //}

    ///// <summary>
    ///// 清空表，返回影响行数，
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <returns></returns>
    //public int Clear<T>()
    //{
    //  throw new NotImplementedException();
    //  //string tableName = GetTableName(typeof(T));
    //  //return this.Connection.Execute("truncate table " + tableName);
    //}

    //public int Delete<T>(object dyparam)
    //{
    //  throw new NotImplementedException();
    //  //string tableName = GetTableName(typeof(T));

    //  //return this.Connection.Execute(SqlBuilder.ToDelete(tableName, dyparam?.GetType()), dyparam);
    //}

    //#endregion
  }
}


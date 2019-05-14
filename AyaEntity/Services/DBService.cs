using AyaEntity.Command;
using AyaEntity.DataUtils;
using AyaEntity.Statement;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AyaEntity.Services
{
  public class DBService
  {

    /// <summary>
    /// 连接对象
    /// </summary>
    protected IDbConnection Connection;

    public void SetConnection(IDbConnection connection)
    {
      this.Connection = connection;
    }





    /// <summary>
    /// 自定义执行sql语句
    /// </summary>
    /// <typeparam name="TableEntity"></typeparam>
    /// <param name="sql"></param>
    /// <returns></returns>
    public int Excute<TableEntity>(ISqlStatementToSql sql)
    {
      return this.Connection.Execute(sql.ToSql(), sql.GetParameters());
    }




    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <typeparam name="TResult">实体类</typeparam>
    /// <param name="sql"></param>
    /// <returns></returns>
    public TEntity GetEntity<TEntity>(object parameters = null, string whereCondition = null)
    {
      Type type = typeof(TEntity);
      ISqlStatementToSql sql = CommandBuilder.BuildSelect(parameters, type)
                                          .Select(SqlAttribute.GetSelectColumns(type))
                                          .Limit(1)
                                          .Where(whereCondition);
      return this.Connection.QueryFirstOrDefault<TEntity>(sql.ToSql(), sql.GetParameters());
    }


    /// <summary>
    /// 获取一个列表
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public IEnumerable<TEntity> GetEntityList<TEntity>(object parameters = null, string whereCondition = null)
    {
      Type type = typeof(TEntity);
      ISqlStatementToSql sql = CommandBuilder.BuildSelect(parameters, type)
                                          .Select(SqlAttribute.GetSelectColumns(type))
                                          .Where(whereCondition);
      return this.Connection.Query<TEntity>(sql.ToSql(), sql.GetParameters());
    }


    /// <summary>
    /// 根据table泛型删除数据
    /// </summary>
    /// <typeparam name="TableEntity"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>

    public int Delete<TableEntity>(object parameters, string whereCondition = null)
    {
      Type type = typeof(TableEntity);
      ISqlStatementToSql sql = CommandBuilder.BuildDelete(parameters, type)
                                          .Where(whereCondition);
      return this.Connection.Execute(sql.ToSql(), sql.GetParameters());

    }

    /// <summary>
    /// 根据table泛型更新数据
    /// </summary>
    /// <param name="updateEntity">复合参数（set字段 和 where条件字段 都在这个对象里）</param>
    /// <returns></returns>
    public int Update<TableEntity>(object updateEntity, string whereCondition = null, params string[] setColumns)
    {
      Type type = typeof(TableEntity);
      ISqlStatementToSql sql = CommandBuilder.BuildUpdate(updateEntity, type)
                                            .UpdateSetColumns(setColumns)
                                            .Where(whereCondition);
      return this.Connection.Execute(sql.ToSql(), sql.GetParameters());
    }



    /// <summary>
    /// 根据泛型插入一条实体数据
    /// </summary>
    /// <typeparam name="TableEntity"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="lastInsertid">是否返回自增id，不返回行数</param>
    /// <returns></returns>
    public int Insert<TableEntity>(TableEntity parameters, bool lastInsertid = false)
    {
      Type type = typeof(TableEntity);
      ISqlStatementToSql sql = CommandBuilder.BuildInsert(parameters, type)
                                              .LastInsertId(lastInsertid);
      if (!lastInsertid)
      {
        return this.Connection.Execute(sql.ToSql(), sql.GetParameters());
      }
      return this.Connection.ExecuteScalar<int>(sql.ToSql(), sql.GetParameters());
    }




    /// <summary>
    /// 根据泛型插入多条实体数据
    /// </summary>
    /// <typeparam name="TableEntity"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public int InsertList<TableEntity>(IEnumerable<TableEntity> parameters)
    {
      Type type = typeof(TableEntity);
      ISqlStatementToSql sql = CommandBuilder.BuildInsert(parameters, type);
      return this.Connection.Execute(sql.ToSql(), sql.GetParameters());
    }

  }
}

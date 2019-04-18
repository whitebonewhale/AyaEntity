using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.SqlStatement;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlServices
{


  /**
   * 
   * 
   * 
   * 
   **/


  public abstract class StatementService
  {


    /// <summary>
    /// 更新sql比较特殊，会用到两个参数
    /// update set param 与where caluse  param
    /// </summary>
    protected object updateParam;


    /// <summary>
    /// sql语句参数实体类
    /// insert entity param
    /// delete where caluse param
    /// select where caluse param
    /// </summary>
    protected object caluseParam;


    public DynamicParameters GetParameters()
    {
      DynamicParameters parameters = new DynamicParameters(this.caluseParam);

      if (this.updateParam != null)
      {
        parameters.AddDynamicParams(this.updateParam);
      }
      return null;
    }



    /// <summary>
    /// sql语句操作的表实体类类型
    /// </summary>
    protected Type entityType;



    /// <summary>
    /// 调用方法名称，用于自定义功能扩展
    /// 根据此字段来进一步控制sql语句的生成
    /// 简单理解为一个标识即可，
    /// </summary>
    protected string methodName;


    protected SelectStatement selectSql;
    protected UpdateStatement updateSql;


    /// <summary>
    /// 配置一下(●'◡'●)就能生成想要的sql啦
    /// </summary>
    /// <param name="funcName">方法名</param>
    /// <param name="type">sql结果，实体类型</param>
    /// <param name="parameters">sql 参数</param>
    /// <returns></returns>
    public ISqlStatement Config(string funcName, Type type, object parameters, object updateEntity = null)
    {
      this.caluseParam = parameters;
      this.updateParam = updateEntity;
      ISqlStatement sql = this.CreateSql(funcName);
      this.entityType = type;
      return sql;
    }

    /// <summary>
    /// 使用方法名
    /// </summary>
    /// <param name="methodName"></param>
    public void UseMethod(string methodName)
    {
      this.methodName = methodName;
    }




    /// <summary>
    /// 生成默认select sql方法 
    /// </summary>
    /// <returns></returns>
    protected SelectStatement Select()
    {
      if (this.selectSql == null)
      {
        this.selectSql = new SelectStatement()
              .From(SqlAttribute.GetTableName(this.entityType))
              .WhereAutoCaluse(this.caluseParam);
      }
      return this.selectSql;
    }

    /// <summary>
    /// 生成默认update sql方法
    /// </summary>
    /// <returns></returns>
    protected UpdateStatement Update()

    {
      if (this.updateSql == null)
      {
        this.updateSql = new UpdateStatement()
              .Update(SqlAttribute.GetTableName(this.entityType))
              .Set(SqlAttribute.GetUpdateColumns(this.updateParam))
              .WhereAutoCaluse(this.caluseParam);
      }
      return this.updateSql;
    }

    /// <summary>
    /// 生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected abstract ISqlStatement CreateSql(string funcName);

  }

}

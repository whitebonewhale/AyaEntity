using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.SqlStatement;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlServices
{



  public abstract class StatementService
  {




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


    /// <summary>
    /// 配置一下(●'◡'●)就能生成想要的sql啦
    /// </summary>
    /// <param name="funcName">方法名</param>
    /// <param name="type">sql结果，实体类型</param>
    /// <param name="caluseParameters">sql 参数</param>
    /// <returns></returns>
    public ISqlStatement Config(string funcName, Type type, object caluseParameters, object updateEntity = null)
    {
      ISqlStatement sql = this.CreateSql(funcName, caluseParameters,  updateEntity);
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
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected abstract ISqlStatement CreateSql(string funcName, object caluseParameters, object updateEntity);

   


  }

}

using AyaEntity.Base;
using AyaEntity.DataUtils;
using AyaEntity.Statement;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AyaEntity.SqlServices
{



  public interface IStatementService
  {


    /// <summary>
    /// 配置一下(●'◡'●)就能生成想要的sql啦
    /// </summary>
    /// <param name="funcName">方法名</param>
    /// <param name="type">sql结果，实体类型</param>
    /// <param name="conditionParameters">sql 参数</param>
    /// <returns></returns>
    IStatementService Config(Type type, Enum method = null);


    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement GetSql(object conditionParameters);


    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement GetEntitySql(object conditionParameters);


    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement GetListSql(object conditionParameters);


    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement GetEntityListSql(object conditionParameters);


    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement UpdateSql(object conditionParameters);


    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement DeleteSql(object conditionParameters);



    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement InsertSql(object conditionParameters);

    /// <summary>
    /// 子类重写具体逻辑，生成sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    SqlStatement InsertListSql(object conditionParameters);

  }

}

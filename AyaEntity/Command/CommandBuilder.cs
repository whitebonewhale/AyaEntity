using AyaEntity.DataUtils;
using AyaEntity.Statement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AyaEntity.Command
{
  public class CommandBuilder
  {
    //public SqlStatement GetEntitySql(object conditionParameters)
    //{
    //  return Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(entityType)).Limit(1);
    //}

    //public SqlStatement GetListSql(object conditionParameters)
    //{
    //  return Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(entityType)); ;
    //}

    //public SqlStatement GetEntityListSql(object conditionParameters)
    //{
    //  return Select(conditionParameters).Select(SqlAttribute.GetSelectColumns(entityType)); ;
    //}

    //public SqlStatement UpdateSql(object conditionParameters)
    //{
    //  return Update(conditionParameters); ;
    //}

    //public SqlStatement DeleteSql(object conditionParameters)
    //{
    //  return Delete(conditionParameters);
    //}



    //public SqlStatement InsertSql(object conditionParameters)
    //{
    //  return Insert(conditionParameters);
    //}

    //public SqlStatement InsertListSql(object conditionParameters)
    //{
    //  return Insert(conditionParameters);
    //}



    #region 默认方法，无关紧要

    /// <summary>
    /// 生成默认select sql方法 
    /// </summary>
    /// <returns></returns>
    public static MysqlSelectStatement BuildSelect(object conditionParam, Type entityType)
    {

      MysqlSelectStatement selectSql = new MysqlSelectStatement();

      selectSql.From(SqlAttribute.GetTableName(entityType))
                    .Where(conditionParam);
      return selectSql;
    }

    /// <summary>
    /// 生成默认update sql方法
    /// </summary>
    /// <returns></returns>
    public static UpdateStatement BuildUpdate(object conditionParameters, Type entityType)
    {
      UpdateStatement updateSql = new UpdateStatement();

      updateSql.Set(conditionParameters)
                    .UpdateSetColumns(SqlAttribute.GetUpdateColumns(conditionParameters, out string primaryKey).ToArray())
                    .WherePrimaryKey(primaryKey)
                    .From(SqlAttribute.GetTableName(entityType));
      return updateSql;
    }


    public static DeleteStatement BuildDelete(object conditionParam, Type entityType)
    {
      DeleteStatement deleteSql = new DeleteStatement();
      deleteSql.From(SqlAttribute.GetTableName(entityType))
                    .Where(conditionParam);
      return deleteSql;
    }

    public static InsertStatement BuildInsert(object conditionParam, Type entityType)
    {

      InsertStatement insertSql = new InsertStatement();
      insertSql.Insert(SqlAttribute.GetInsertCoulmn(entityType), conditionParam)
                   .From(SqlAttribute.GetTableName(entityType));
      return insertSql;
    }


    #endregion
  }
}

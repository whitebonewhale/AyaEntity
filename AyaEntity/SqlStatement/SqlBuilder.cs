using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace AyaEntity.SqlStatement
{




  public abstract class SqlBuilder : ISqlBuilder
  {

    #region old

    //public string GetTableName(Type type)
    //{
    //  object[] name = type.GetCustomAttributes(typeof(TableNameAttribute), false);
    //  if (name.Length > 0)
    //  {
    //    TableNameAttribute attr = name[0] as TableNameAttribute;
    //    return attr.TableName;
    //  }
    //  else
    //  {
    //    return type.Name;
    //  }
    //}



    ///// <summary>
    ///// 最简单的select查询语句生成
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="dyparam"></param>
    ///// <returns></returns>
    //public string Select<T>(object dyparam)
    //{
    //  string tableName = GetTableName(typeof(T));
    //  StringBuilder sqlmem = new StringBuilder("select * from " + tableName);
    //  if (dyparam != null)
    //  {
    //    IEnumerable<PropertyInfo> fields = dyparam.GetType().GetProperties();
    //    sqlmem.Append(" where ").Append(fields.Join(" and ", m =>
    //    {
    //      if ((typeof(IEnumerable<object>).IsAssignableFrom(m.PropertyType)))
    //      {
    //        return m.Name + " in @" + m.Name;
    //      }
    //      else
    //      {
    //        return m.Name + "=@" + m.Name;
    //      }
    //    }));
    //  }
    //  return sqlmem.ToString();
    //}

    ///// <summary>
    ///// 构建获取自定义数据sql方法
    ///// </summary>
    ///// <param name="tableName"></param>
    ///// <param name="clause"></param>
    ///// <param name="cloumns"></param>
    ///// <returns></returns>
    //public string CustomSelect(string tableName, string clause, Type param, string columns = null)
    //{
    //  if (string.IsNullOrEmpty(columns))
    //  {
    //    columns = "*";
    //  }
    //  if (string.IsNullOrEmpty(clause))
    //  {
    //    IEnumerable<PropertyInfo> fields = param.GetProperties();
    //    clause = fields.Join(" and ", m => m.Name + "=@" + m.Name);

    //  }
    //  StringBuilder sqlmem = new StringBuilder($"select {columns} from {tableName}");
    //  if (!string.IsNullOrEmpty(clause))
    //  {
    //    sqlmem.Append(" where ").Append(clause);
    //  }
    //  return sqlmem.ToString();
    //}


    //public string ToSelect(string tableName, string clause)
    //{
    //  StringBuilder sqlmem = new StringBuilder("select * from " + tableName);
    //  if (!string.IsNullOrEmpty(clause))
    //  {
    //    sqlmem.Append(" where ").Append(clause);
    //  }
    //  return sqlmem.ToString();
    //}






    //public string ToDelete(string tableName, Type dyparam)
    //{
    //  StringBuilder sqlmem = new StringBuilder("delete " + tableName);
    //  if (dyparam != null)
    //  {
    //    sqlmem.Append(" where ");
    //    IEnumerable<PropertyInfo> fields = GetKeys(dyparam, SqlOperate.Insert);
    //    sqlmem.Append($"{ fields.Join(" and ", m => m.Name + "=@" + m.Name)}");
    //  }
    //  return sqlmem.ToString();
    //}


    //public string ToInsert(string tableName, Type dyparam)
    //{
    //  StringBuilder sqlmem = new StringBuilder("insert into " + tableName);
    //  IEnumerable<PropertyInfo> fields = GetKeys(dyparam, SqlOperate.Insert);
    //  sqlmem.Append($"({ fields.Join(",", m => m.Name)})values(@{ fields.Join(",@", m => m.Name)})");
    //  return sqlmem.ToString();
    //}



    //public string ToUpdate(string tableName, Type dyparam, params string[] id_clause)
    //{
    //  StringBuilder sqlmem = new StringBuilder();
    //  sqlmem.Append("update " + tableName);
    //  IEnumerable<PropertyInfo> fields = GetKeys(dyparam, SqlOperate.Update);
    //  var set_fi = fields.Where(m => !id_clause.Contains(m.Name));


    //  sqlmem.Append(" set ");
    //  sqlmem.Append(set_fi.Join(", ", m => m.Name + "=@" + m.Name));
    //  sqlmem.Append(" where ").Append(id_clause.Join(" and ", m => m + "=@" + m));
    //  return sqlmem.ToString();
    //}
    #endregion

    private Type entityType;

    private readonly SqlAttribute attribute = new SqlAttribute();
    private StringBuilder sqlBuffer =new StringBuilder();

    private DynamicParameters parameters;
    private object objectParameters;

    public DynamicParameters Parameters => parameters;


    public SqlBuilder(Type entityType, object param)
    {
      if (param != null)
      {
        parameters = new DynamicParameters();
        parameters.AddDynamicParams(param);
        objectParameters = param;
      }
      this.entityType = entityType;
    }


    public string Build()
    {
      return sqlBuffer.ToString();
    }


    #region paging 
    /// <summary>
    /// 分页查询语句
    /// </summary>
    /// <param name="pag"></param>
    /// <param name="tableName"></param>
    /// <param name="caluse"></param>
    /// <returns></returns>
    public abstract string CreatePagingQuery(PagingParameter pag);




    #endregion

    #region select

    public ISqlBuilder SelectDefault()
    {
      SelectAll()
        .From(attribute.GetTableName(entityType))
        .Where(attribute.GetCaluse(objectParameters));
      return this;
    }


    public ISqlBuilder Select()
    {
      sqlBuffer.Append("select ").Append(attribute.GetColumns(entityType));
      return this;
    }


    public ISqlBuilder SelectAll()
    {
      sqlBuffer.Append("select * ");
      return this;
    }


    public ISqlBuilder Select(string[] fields)
    {
      sqlBuffer.Append("select ").Append(fields.Join(",", m => m));
      return this;
    }

    public ISqlBuilder SelectFrom()
    {
      string tableName = attribute.GetTableName(entityType);
      sqlBuffer.Append("select * From ").Append(tableName);

      return this;
    }

    #endregion

    #region update
    #endregion

    #region delete
    #endregion

    #region add
    #endregion



    #region from


    public ISqlBuilder From()
    {
      throw new NotImplementedException();
    }

    public ISqlBuilder From(string tableName)
    {
      sqlBuffer.Append(" ").Append(tableName);
      return this;
    }

    #endregion

    #region where
    public ISqlBuilder Where()
    {
      throw new NotImplementedException();
    }

    public ISqlBuilder Where<T>()
    {
      throw new NotImplementedException();
    }


    public ISqlBuilder Where(string caluse)
    {
      sqlBuffer.Append(" ").Append(caluse);
      return this;
    }
    #endregion


  }
}

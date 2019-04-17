using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace AyaEntity.SqlStatement
{
  public enum SortType
  {
    asc,
    desc
  }

  public enum CaluseOpertor
  {
    and,
    or
  }

  /// <summary>
  /// select sql语句生成,实现其他select复杂语句可继承此类扩展重写即可
  /// </summary>
  public class SelectStatement : ISqlBuilder
  {


    // where语句连接运算符: and/or
    public CaluseOpertor caluseOpertor = CaluseOpertor.and;
    public SortType sortType = SortType.desc;
    public string sortField;



    public SqlAttribute SqlAttribute()
    {

      return this.attribute;
    }
    // sql参数
    private static SelectStatement self;
    public static SelectStatement Build()
    {
      if (self == null)
      {
        self = new SelectStatement();
      }
      return self;
    }

    public DynamicParameters SqlParameters => this.parameters;

    private SelectStatement()
    {

    }
    protected string[] columns={"*"};
    protected string tableName;
    protected string whereCaluseString ;
    protected string[] groupFields;
    protected SqlAttribute attribute = new SqlAttribute();
    protected DynamicParameters parameters;
    protected object entityParameters;




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


    /// <summary>
    /// 生成sql
    /// </summary>
    /// <returns></returns>
    public virtual string ToSql()
    {
      StringBuilder buffer = new StringBuilder();
      // select
      buffer.Append("SELECT ").Append(this.columns.Join(",", m => m));
      // from
      buffer.Append(" FROM ").Append(this.tableName);
      // where
      if (!string.IsNullOrEmpty(this.whereCaluseString))
      {
        buffer.Append(" WHERE ").Append(this.whereCaluseString);
      }
      // group
      if (!this.groupFields.IsEmpty())
      {
        buffer.Append(" GROUP BY " + this.groupFields.Join(",", m => m));
      }
      // sort 
      if (!string.IsNullOrEmpty(this.sortField))
      {
        buffer.Append(" ORDER BY ").Append(this.sortField).Append(" " + this.sortType.ToString());
      }
      return buffer.ToString();
    }



    public SelectStatement Select(params string[] columns)
    {
      this.columns = columns;
      return this;
    }


    public SelectStatement From(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    /// <summary>
    /// 自定义where子句
    /// </summary>
    /// <param name="caluse"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public SelectStatement Where(params string[] whereCaluse)
    {
      this.whereCaluseString = whereCaluse.Join($" {this.caluseOpertor.ToString()} ", m => m);
      return this;
    }

    /// <summary>
    /// 自定义where子句并设置参数
    /// </summary>
    /// <param name="caluse"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public SelectStatement Where<T>(T entity, params string[] caluse)
    {
      this.parameters = new DynamicParameters(entity);
      return this.Where(caluse);
    }


    /// <summary>
    /// 根据参数自动生成默认caluse语句
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public PagingQueryStatement WhereAutoCaluse<T>(T entity)
    {
      this.parameters = new DynamicParameters(entity);
      this.whereCaluseString = this.attribute.GetCaluse(this.parameters, this.caluseOpertor.ToString());
      return this;
    }



    /// <summary>
    /// 自定义分组
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public PagingQueryStatement Group(params string[] fields)
    {
      this.groupFields = fields;
      return this;
    }

  }

}

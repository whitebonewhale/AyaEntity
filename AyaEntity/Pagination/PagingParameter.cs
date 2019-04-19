using Dapper;
using System.Linq;

namespace AyaEntity
{
  /// <summary>
  /// 排序方式
  /// </summary>
  public enum SortMode
  {
    Asc,
    Desc
  }


  /// <summary>
  /// 执行分页查询时的参数
  /// </summary>
  public class PagingParameter
  {
    public PagingParameter(string field, SortMode type)
    {
      PageIndex = 1;
      RowSize = 10;
      OrderField = field;
      OrderType = type;
    }

    public PagingParameter(int pi, int ps, string field, SortMode type)
    {
      OrderType = type;
      OrderField = field;
      RowSize = ps;
      PageIndex = pi < 1 ? 1 : pi;
    }


    /// <summary>
    /// 每页数据条数
    /// </summary>
    public int RowSize = 10;
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex = 1;

    /// <summary>
    /// 起始行计算
    /// </summary>
    public int StartRow { get { return (PageIndex - 1) * RowSize; } }
    /// <summary>
    /// 要查询的表名
    /// </summary>
    public string TableName { get; private set; }

    /// <summary>
    /// 是否需要数据总条数
    /// </summary>
    public bool RequiredTotal;


    /// <summary>
    /// sql查询列名
    /// </summary>
    private string _sql_columns;
    public string Columns
    {
      get { return string.IsNullOrEmpty(_sql_columns) ? "*" : _sql_columns; }
      set { _sql_columns = value; }
    }

    /// <summary>
    /// sql查询where条件语句
    /// </summary>
    public string Condition
    {
      get;
      set;
    }
    public void SetTableName(string tableName)
    {
      TableName = tableName;
    }


    /// <summary>
    /// 排序字段
    /// </summary>
    public string OrderField;


    /// <summary>
    /// 排序方式
    /// </summary>
    public SortMode OrderType;

    /// <summary>
    /// 完整参数（包含分页条件数
    /// </summary>
    public DynamicParameters PageParameters
    {
      get
      {
        DynamicParameters param = new DynamicParameters();
        param.Add("@StartRow", StartRow);
        param.Add("@RowSize", RowSize);
        param.Add("@OrderField", OrderField);
        return param;
      }
    }


  }

}

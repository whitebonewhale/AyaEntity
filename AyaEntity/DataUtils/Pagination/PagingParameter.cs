using Dapper;
using System.Linq;

namespace AyaEntity.DataUtils
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
      this.PageIndex = 1;
      this.RowSize = 10;
      this.OrderField = field;
      this.OrderType = type;
    }

    public PagingParameter(int pi, int ps, string field, SortMode type)
    {
      this.OrderType = type;
      this.OrderField = field;
      this.RowSize = ps;
      this.PageIndex = pi < 1 ? 1 : pi;
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
    public int StartRow { get { return (this.PageIndex - 1) * this.RowSize; } }
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
      get { return string.IsNullOrEmpty(this._sql_columns) ? "*" : this._sql_columns; }
      set { this._sql_columns = value; }
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
      this.TableName = tableName;
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
        param.Add("@StartRow", this.StartRow);
        param.Add("@RowSize", this.RowSize);
        param.Add("@OrderField", this.OrderField);
        return param;
      }
    }


  }

}

using System.Collections.Generic;

namespace AyaEntity.DataUtils
{
  public class PagingResult<T>
  {
    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }
    /// <summary>
    /// 数据总数
    /// </summary>
    public int Total { get; set; }
    /// <summary>
    /// 每页数据条数
    /// </summary>
    public int RowSize { get; set; }
    /// <summary>
    /// 当前页码内容
    /// </summary>
    public IEnumerable<T> Rows { get; set; }
  }
}

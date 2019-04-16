using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity.SqlStatement
{
  /// <summary>
  /// sql语句（组装）生成类
  /// </summary>
  public interface ISqlBuilder
  {
    DynamicParameters SqlParameters();

    SqlAttribute SqlAttribute();
    string Build();
  }
}

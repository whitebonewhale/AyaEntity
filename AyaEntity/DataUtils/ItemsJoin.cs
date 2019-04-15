using System;
using System.Collections.Generic;
using System.Text;

namespace AyaEntity
{
  /// <summary>
  /// 数组join字符串扩展工具类
  /// 主要用于生成及拼接sql
  /// </summary>
  public static class ItemsJoin
  {
    public static string Join<Key, Value>(this Dictionary<Key, Value> item, string separator, Func<KeyValuePair<Key, Value>, string> func)
    {
      if (separator == null)
        separator = String.Empty;

      StringBuilder strmem = new StringBuilder();
      using (var en = item.GetEnumerator())
      {
        if (en.MoveNext())
        {
          strmem.Append(func(en.Current));
        }
        while (en.MoveNext())
        {
          strmem.Append(separator);
          strmem.Append(func(en.Current));
        }
      }
      return strmem.ToString();
    }

    public static string Join<T>(this IEnumerable<T> item, string separator, Func<T, string> func)
    {
      if (separator == null)
        separator = String.Empty;

      StringBuilder strmem = new StringBuilder();
      using (var en = item.GetEnumerator())
      {
        if (en.MoveNext())
        {
          strmem.Append(func(en.Current));
        }
        while (en.MoveNext())
        {
          strmem.Append(separator);
          strmem.Append(func(en.Current));
        }
      }
      return strmem.ToString();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AyaEntity.DataUtils
{
  /// <summary>
  /// 数组join字符串扩展工具类
  /// 主要用于拼接sql
  /// </summary>
  public static class ItemsJoin
  {

    public static bool IsEmpty(this Array array)
    {
      return array == null || array.Length == 0;
    }

    public static string Join<Key, Value>(this Dictionary<Key, Value> item, string separator, Func<KeyValuePair<Key, Value>, string> func)
    {

      if (separator == null)
      {
        separator = String.Empty;
      }

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
      {
        separator = String.Empty;
      }

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

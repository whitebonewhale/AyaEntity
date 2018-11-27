using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiraEntity.Tools
{
    public static class ParametersJoin
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

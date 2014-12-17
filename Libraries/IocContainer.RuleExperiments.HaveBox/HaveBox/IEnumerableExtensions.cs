using System;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Collections.Specialized;
#endif
using System.Linq;

namespace HaveBox
{
    internal static class IEnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static bool IsIEnumerable(this Type type)
        {
            return type.Name.Contains("IEnumerable`1") && type.Namespace == "System.Collections.Generic";
        }
        
#if !SILVERLIGHT
        public static IEnumerable<KeyValuePair<string, string>> ToIEnumerable(this NameValueCollection source)
        {
            return source.AllKeys.SelectMany(source.GetValues, (k, v) => new KeyValuePair<string, string>(k, v));
        }
#endif
    }
}

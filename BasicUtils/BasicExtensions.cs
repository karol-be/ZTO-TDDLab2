using System;
using System.Collections.Generic;

namespace BasicUtils
{
    public static class BasicExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            if (values.IsNotEmpty())
            {
                foreach (T local in values)
                {
                    eachAction(local);
                }
            }
            return values;
        }


        public static System.Collections.IEnumerable Each(this System.Collections.IEnumerable values, Action<object> eachAction)
        {
            foreach (object obj2 in values)
            {
                eachAction(obj2);
            }
            return values;
        }
        public static bool IsEmpty(this System.Collections.IEnumerable collection)
        {
            if (collection != null)
            {
                return !collection.GetEnumerator().MoveNext();
            }
            return true;
        }

        public static bool IsEmpty(this string stringValue)
        {
            return string.IsNullOrEmpty(stringValue);
        }

        public static bool IsNotEmpty(this System.Collections.IEnumerable collection)
        {
            return ((collection != null) && collection.GetEnumerator().MoveNext());
        }

        public static bool IsNotEmpty(this string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tests.TDDLab.Core.Utils
{
    public static class CollectionExtensions
    {
        public static IReadOnlyDictionary<TK,TV> GetAsReadOnly<TK,TV>(this IDictionary<TK,TV> dict)
        {
            return new ReadOnlyDictionary<TK,TV>(dict);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model
{
    public static class DictionaryExtensions
    {
        public static void UnionWith<A, B>(this Dictionary<A, B> dictionaryA, Dictionary<A, B> dictionaryB)
        {
            foreach (var pair in dictionaryB)
                dictionaryA.TryAdd(pair.Key, pair.Value);
        }
    }
}

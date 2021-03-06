using System;
using System.Linq;

namespace ContinuationToken.Tests.Formatting
{
    public static class FormatUtils
    {
        public static (object[] Arr, Type[] Types) MakeArray(params object[] entries)
        {
            return (entries, entries.Select(i => i?.GetType() ?? typeof(object)).ToArray());
        }
    }
}

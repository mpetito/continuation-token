using System;
using System.Collections.Generic;

namespace ContinuationToken.Reflection
{
    public class MethodResolver : IMethodResolver
    {
        public virtual MethodBinding GetCompareMethod<TProp>()
        {
            var comparer = Comparer<TProp>.Default;
            var type = comparer.GetType();
            var method = type.GetMethod("Compare") ?? throw new NotSupportedException();

            return new MethodBinding(method, comparer);
        }
    }
}
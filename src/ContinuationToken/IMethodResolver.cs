using ContinuationToken.Reflection;
using System;

namespace ContinuationToken
{
    /// <summary>
    /// Resolves comparison methods for a given property type.
    /// </summary>
    public interface IMethodResolver
    {
        /// <summary>
        /// Resolves a method for comparing a property of type <typeparamref name="TProp"/>.
        /// </summary>
        /// <typeparam name="TProp">Property type.</typeparam>
        /// <returns>Method suitable for use with comparison expressions.</returns>
        /// <remarks>
        /// May return a static method that receives two arguments or an instance method that receives a single argument.
        /// Static method variant is preferred for cases when the property value may be null.
        /// By default it returns an instance method of <see cref="System.Collections.Generic.Comparer{T}.Default"/>.
        /// </remarks>
        MethodBinding GetCompareMethod<TProp>();

        MethodBinding GetCompareMethod(Type type)
        {
            var method = GetType().GetMethod(nameof(GetCompareMethod), 1, Type.EmptyTypes)?.MakeGenericMethod(type);

            return method?.Invoke(this, null) as MethodBinding ?? throw new NotSupportedException();
        }
    }
}
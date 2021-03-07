using ContinuationToken.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken
{
    /// <summary>
    /// Component of a continuation token used to sort and filter a query.
    /// </summary>
    /// <typeparam name="T">Query type.</typeparam>
    /// <see cref="SortedProperty{T, TProp}"/>
    public interface ISortedProperty<T> : IEnumerable<ISortedProperty<T>>
        where T : class
    {
        /// <summary>
        /// Property type expressed by this instance.
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Next property in the key specification, or null.
        /// </summary>
        ISortedProperty<T>? Next { get; }

        /// <summary>
        /// Sets the next property in the key specification.
        /// </summary>
        /// <param name="next">Next key property.</param>
        void Append(ISortedProperty<T> next);

        /// <summary>
        /// Projects the property values from an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="last">Instance.</param>
        /// <returns>Property value.</returns>
        /// <remarks>
        /// <paramref name="last"/> is expected to be the last witnessed record from the query when generating a token string.
        /// </remarks>
        object? GetValue(T last);

        /// <summary>
        /// Applies sorting for this and all following properties.
        /// </summary>
        /// <param name="query">Query to sort.</param>
        /// <returns>Sorted query.</returns>
        IOrderedQueryable<T> Sort(IQueryable<T> query);

        /// <summary>
        /// Produces a filtering expression for this and all following properties.
        /// </summary>
        /// <param name="values">Key values to use when filtering.</param>
        /// <returns>Filtering expression, or null if no key value was available to use when filtering.</returns>
        Expression? Filter(IEnumerator<object?> values);
    }
}
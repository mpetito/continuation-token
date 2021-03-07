using System;
using System.Linq.Expressions;

namespace ContinuationToken
{
    /// <summary>
    /// Builds a continuation token specification.
    /// </summary>
    /// <typeparam name="T">Query type.</typeparam>
    public interface ITokenBuilder<T> where T : class
    {
        /// <summary>
        /// Sort a property in ascending order.
        /// </summary>
        /// <typeparam name="TProp">Property type.</typeparam>
        /// <param name="property">Property expression to sort.</param>
        /// <returns>This builder for fluent configuration.</returns>
        ITokenBuilder<T> Ascending<TProp>(Expression<Func<T, TProp>> property);

        /// <summary>
        /// Sort a property in descending order.
        /// </summary>
        /// <typeparam name="TProp">Property type.</typeparam>
        /// <param name="property">Property expression to sort.</param>
        /// <returns>This builder for fluent configuration.</returns>
        ITokenBuilder<T> Descending<TProp>(Expression<Func<T, TProp>> property);

        /// <summary>
        /// Use a formatter for converting a continuation token to / from a string representation.
        /// </summary>
        /// <param name="formatter">Formatter to use.</param>
        /// <returns>This builder for fluent configuration.</returns>
        /// <remarks>Default formatter is a composition of <see cref="Formatting.JsonTokenFormatter"/> and <see cref="Formatting.Base64TokenFormatter"/> formatters.</remarks>
        ITokenBuilder<T> UseFormatter(ITokenFormatter formatter);

        /// <summary>
        /// Use a resolver for comparing properties while sorting.
        /// </summary>
        /// <param name="resolver">Resolver to use.</param>
        /// <returns>This builder for fluent configuration.</returns>
        ITokenBuilder<T> UseResolver(IMethodResolver resolver);

        /// <summary>
        /// Use a factory for building the continuation token using configured options.
        /// </summary>
        /// <param name="factory">Factory to use.</param>
        /// <returns>This builder for fluent configuration.</returns>
        ITokenBuilder<T> UseFactory(Func<ITokenOptions<T>, IQueryContinuationToken<T>> factory);

        /// <summary>
        /// Creates a continuation token using the current specification.
        /// </summary>
        /// <returns>Token.</returns>
        IQueryContinuationToken<T> Build();
    }
}
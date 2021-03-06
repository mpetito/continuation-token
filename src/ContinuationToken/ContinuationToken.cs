using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken
{
    /// <summary>
    /// Implements a continuation token specification.
    /// </summary>
    /// <typeparam name="T">Query type.</typeparam>
    /// <remarks>
    /// A continuation token is composed of a forward-linked-list of properties.
    /// The tuple projected by these properties should form a unique key for <typeparamref name="T"/>.
    /// 
    /// When generating a token string, the tuple for the last witnessed result from a query is serialized to a string.
    /// When resuming from a token string, the tuple is deserialized and the query is filtered and sorted to start after the last result.
    /// </remarks>
    internal class ContinuationToken<T> : IContinuationToken<T> where T : class
    {
        private readonly ITokenFormatter _formatter;
        private readonly ParameterExpression _input;

        private readonly ISortedProperty<T> _head;
        private readonly Type[] _propertyTypes;

        public ContinuationToken(TokenBuilder<T> builder, ISortedProperty<T> head)
            : this(builder.Formatter, builder.Input, head)
        {
        }

        public ContinuationToken(ITokenFormatter formatter, ParameterExpression input, ISortedProperty<T> head)
        {
            _formatter = formatter;
            _input = input;
            _head = head;
            _propertyTypes = _head.Select(p => p.PropertyType).ToArray();
        }

        public string? GetToken(T last)
        {
            if (last is null)
                return null;

            var arr = _head.Select(prop => prop.GetValue(last)).ToArray();

            return _formatter.Serialize(arr, _propertyTypes);
        }

        public IEnumerable<object?> ParseToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Enumerable.Empty<object>();

            try
            {
                return _formatter.Deserialize(token, _propertyTypes);
            }
            catch
            {
                return Enumerable.Empty<object>();
            }
        }

        public IOrderedQueryable<T> SortQuery(IQueryable<T> query)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            return _head.Sort(query);
        }

        public IQueryable<T> FilterQuery(IQueryable<T> query, string? token)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            var values = ParseToken(token).GetEnumerator();

            var expression = _head.Filter(values);
            if (expression is null)
                return query;

            var predicate = Expression.Lambda<Func<T, bool>>(expression, _input);

            return query.Where(predicate);
        }


        public IOrderedQueryable<T> ResumeQuery(IQueryable<T> query, string? token)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            return SortQuery(FilterQuery(query, token));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken.Providers
{
    /// <summary>
    /// Token implementation that serializes a bookmark using the last witnessed key properties.
    /// </summary>
    /// <typeparam name="T">Query type.</typeparam>
    internal class BookmarkToken<T> : IQueryContinuationToken<T> where T : class
    {
        private readonly ITokenFormatter _formatter;
        private readonly ISortedProperty<T> _properties;

        private readonly ParameterExpression _input;
        private readonly Type[] _propertyTypes;

        public BookmarkToken(ITokenOptions<T> options)
            : this(options.Formatter, options.Properties, options.Input)
        {
        }

        public BookmarkToken(ITokenFormatter formatter, ISortedProperty<T> properties, ParameterExpression input)
        {
            _formatter = formatter;
            _properties = properties;
            _input = input;
            _propertyTypes = _properties.Select(p => p.PropertyType).ToArray();
        }

        public string? GetNextToken(T? last)
        {
            if (last is null)
                return null;

            var arr = _properties.Select(prop => prop.GetValue(last)).ToArray();

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

            return _properties.Sort(query);
        }

        public IQueryable<T> FilterQuery(IQueryable<T> query, string? token)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            var values = ParseToken(token).GetEnumerator();

            var expression = _properties.Filter(values);
            if (expression is null)
                return query;

            var predicate = Expression.Lambda<Func<T, bool>>(expression, _input);

            return query.Where(predicate);
        }


        public IQueryContinuation<T> ResumeQuery(IQueryable<T> query, string? token)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            var resumed = SortQuery(FilterQuery(query, token));

            return new BookmarkContinuation<T>(this, resumed);
        }
    }
}
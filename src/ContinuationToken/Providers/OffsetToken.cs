using System;
using System.Linq;

namespace ContinuationToken.Providers
{
    /// <summary>
    /// Token implementation that serializes an offset.
    /// </summary>
    /// <typeparam name="T">Query type.</typeparam>
    internal class OffsetToken<T> : IQueryContinuationToken<T> where T : class
    {
        private static readonly Type[] _propertyTypes = new Type[] { typeof(int?) };

        private readonly ITokenFormatter _formatter;
        private readonly ISortedProperty<T> _properties;

        public OffsetToken(ITokenOptions<T> options)
            : this(options.Formatter, options.Properties)
        {
        }

        public OffsetToken(ITokenFormatter formatter, ISortedProperty<T> properties)
        {
            _formatter = formatter;
            _properties = properties;
        }

        public string? GetNextToken(int offset, int count)
        {
            if (count == 0)
                return null;

            var arr = new object[] { offset + count };

            return _formatter.Serialize(arr, _propertyTypes);
        }

        public int? ParseToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return default;

            try
            {
                var values = _formatter.Deserialize(token, _propertyTypes);

                return values.Cast<int?>().FirstOrDefault();
            }
            catch
            {
                return default;
            }
        }

        public IOrderedQueryable<T> SortQuery(IQueryable<T> query)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            return _properties.Sort(query);
        }

        public IQueryable<T> OffsetQuery(IOrderedQueryable<T> query, int? offset)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (!offset.HasValue)
                return query;

            return query.Skip(offset.Value);
        }

        public IQueryContinuation<T> ResumeQuery(IQueryable<T> query, string? continuationToken = null)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            var offset = ParseToken(continuationToken);
            var resumed = OffsetQuery(SortQuery(query), offset);

            return new OffsetContinuation<T>(this, resumed, offset);
        }
    }
}

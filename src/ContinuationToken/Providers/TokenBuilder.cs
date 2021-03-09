using ContinuationToken.Formatting;
using ContinuationToken.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken.Providers
{
    internal class TokenBuilder<T> : ITokenBuilder<T>, ITokenOptions<T>
        where T : class
    {
        private readonly List<ISortedProperty<T>> _properties = new();
        private Func<ITokenOptions<T>, IQueryContinuationToken<T>> _factory = options => new BookmarkToken<T>(options);

        public ISortedProperty<T> Properties => _properties.First();

        public ParameterExpression Input { get; } = Expression.Parameter(typeof(T), "x");

        public ITokenFormatter Formatter { get; private set; } = new Base64TokenFormatter(new JsonTokenFormatter());

        public ComparisonProviderFactory ComparisonProviders { get; private set; } = new ComparisonProviderFactory();

        private ITokenBuilder<T> Sort<TProp>(Expression<Func<T, TProp>> property, bool descending)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            var prop = new SortedProperty<T, TProp>(
                Unify(property),
                ComparisonProviders.GetProvider(typeof(TProp)),
                descending: descending,
                first: _properties.Count == 0);

            _properties.LastOrDefault()?.Append(prop);
            _properties.Add(prop);

            return this;
        }

        public ITokenBuilder<T> Ascending<TProp>(Expression<Func<T, TProp>> property)
            => Sort(property, descending: false);

        public ITokenBuilder<T> Descending<TProp>(Expression<Func<T, TProp>> property)
            => Sort(property, descending: true);

        public ITokenBuilder<T> UseFactory(Func<ITokenOptions<T>, IQueryContinuationToken<T>> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            return this;
        }

        public ITokenBuilder<T> UseFormatter(ITokenFormatter formatter)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            return this;
        }

        public ITokenBuilder<T> UseComparer(Type type, IComparisonProvider provider)
        {
            ComparisonProviders.Register(type, provider);
            return this;
        }

        public IQueryContinuationToken<T> Build()
        {
            if (_properties.Count == 0)
                throw new InvalidOperationException("One or more sorted properties must be configured for a continuation token.");

            return _factory(this);
        }

        public override string ToString()
        {
            return string.Join(", ", _properties);
        }

        private Expression<Func<T, TProp>> Unify<TProp>(Expression<Func<T, TProp>> property)
        {
            return Expression.Lambda<Func<T, TProp>>(
                ParameterUnification.Replace(property.Body, property.Parameters.Single(), Input),
                Input);
        }
    }
}
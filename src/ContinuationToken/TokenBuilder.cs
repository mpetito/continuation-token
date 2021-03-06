﻿using ContinuationToken.Formatting;
using ContinuationToken.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken
{
    internal class TokenBuilder<T> : ITokenBuilder<T> where T : class
    {
        private readonly List<ISortedProperty<T>> _properties = new List<ISortedProperty<T>>();

        public ParameterExpression Input { get; } = Expression.Parameter(typeof(T), "x");

        public ITokenFormatter Formatter { get; private set; } = new Base64TokenFormatter(new JsonTokenFormatter());

        public IMethodResolver Resolver { get; private set; } = new MethodResolver();

        private ITokenBuilder<T> Sort<TProp>(Expression<Func<T, TProp>> property, bool descending)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            var prop = new SortedProperty<T, TProp>(
                Unify(property), 
                Resolver.GetCompareMethod<TProp>(),
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

        public ITokenBuilder<T> UseFormatter(ITokenFormatter formatter)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            return this;
        }

        public ITokenBuilder<T> UseResolver(IMethodResolver resolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            return this;
        }

        public IContinuationToken<T> Build()
        {
            if (_properties.Count == 0)
                throw new InvalidOperationException("One or more sorted properties must be configured for a continuation token.");

            return new ContinuationToken<T>(this, _properties.First());
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
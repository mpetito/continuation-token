using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ContinuationToken.Reflection
{
    /// <summary>
    /// Gets or creates an <see cref="IComparisonProvider"/> for a given type.
    /// </summary>
    public class ComparisonProviderFactory
    {
        private static readonly DefaultComparison _default = new();
        protected static IComparisonProvider Default => _default;

        /// <summary>
        /// Types that we know do not have a LessThan operator defined.
        /// </summary>
        /// <remarks>
        /// This optimizes away an exception for common cases we know of in advance.
        /// </remarks>
        private static ISet<Type> KnownGenerics { get; } = new HashSet<Type>(new[]
        {
            // value types (automatically includes nullable versions)
            typeof(Guid),
            typeof(bool)
        }.SelectMany(t => new[] { t, typeof(Nullable<>).MakeGenericType(t) }))
        {
            // reference types
            typeof(string)
        };

        private readonly Dictionary<Type, IComparisonProvider> _providerCache = new();

        public void Register(Type type, IComparisonProvider provider)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (provider is null) throw new ArgumentNullException(nameof(provider));

            _providerCache[type] = provider;
        }

        public IComparisonProvider GetProvider(Type type)
        {
            if (_providerCache.TryGetValue(type, out var provider))
                return provider;

            if (KnownGenerics.Contains(type))
                return CreateCachedGenericProvider(type);

            if (_default.IsSupported(type))
                return Default;

            return CreateCachedGenericProvider(type);
        }

        private IComparisonProvider CreateCachedGenericProvider(Type type)
        {
            var factory = Activator.CreateInstance(typeof(GenericComparison<>).MakeGenericType(type)) as IComparisonProvider
                ?? throw new NotSupportedException();

            _providerCache[type] = factory;

            return factory;
        }

        /// <summary>
        /// Provides comparison using binary operator expressions.
        /// </summary>
        internal class DefaultComparison : IComparisonProvider
        {
            public Expression Equal(Type type, Expression left, Expression right)
                // left == right
                => Expression.Equal(left, right);

            public Expression LessThan(Type type, Expression left, Expression right)
                // left < right
                => Expression.LessThan(left, right);

            public bool IsSupported(Type type)
            {
                var p = Expression.Parameter(type);

                try
                {
                    // test if we can!
                    Equal(type, p, p);
                    LessThan(type, p, p);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Provides comparison using generic comparer.
        /// </summary>
        /// <typeparam name="T">Type for comparison.</typeparam>
        internal class GenericComparison<T> : IComparisonProvider
        {
            private readonly Expression _comparer;
            private readonly MethodInfo _compareMethod;
            private readonly Expression _zero = Expression.Constant(0);

            public GenericComparison()
            {
                var comparer = Comparer<T>.Default;

                _comparer = Expression.Constant(comparer);
                _compareMethod = comparer.GetType().GetMethod("Compare")
                    ?? throw new NotSupportedException();
            }

            public Expression Equal(Type type, Expression left, Expression right)
                // left == right
                => Expression.Equal(left, right);

            public Expression LessThan(Type type, Expression left, Expression right)
                // Comparer<T>.Default.Compare(left, right) < 0
                => Expression.LessThan(Expression.Call(_comparer, _compareMethod, left, right), _zero);
        }
    }
}
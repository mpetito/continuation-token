using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken.Providers
{
    internal class SortedProperty<T, TProp> : ISortedProperty<T> where T : class
    {
        private readonly Expression<Func<T, TProp>> _property;
        private readonly IComparisonProvider _comparer;
        private readonly Func<T, TProp> _accessor;

        public SortedProperty(Expression<Func<T, TProp>> property, IComparisonProvider comparer, bool descending = false, bool first = false)
        {
            _property = property;
            _comparer = comparer;
            _accessor = property.Compile();

            Descending = descending;
            First = first;
        }

        public Type PropertyType => typeof(TProp);

        public bool Descending { get; }

        public bool First { get; }

        public ISortedProperty<T>? Next { get; private set; }

        public void Append(ISortedProperty<T> next) => Next = next;

        public object? GetValue(T instance) => _accessor(instance);

        /// <summary>
        /// Recursively produces a boolean filtering expression for this and all following properties.
        /// </summary>
        /// <param name="values">Key value enumerator.</param>
        /// <returns>Filtering expression, or null if no key value to filter by.</returns>
        /// <remarks>
        /// Given expressions for property P in {A, B, C, ...} of the form:
        ///   Compare(P) => Compare([Property P], [P Key Value]) <> 0, and
        ///   Equal(P) => Compare([Property P], [P Key Value]) == 0
        /// 
        /// The filtering expression to resume a query from the key values looks like:
        ///   Compare(A) OR (Equal(A) AND (
        ///     Compare(B) OR (Equal(B) AND (
        ///       Compare(C) OR (Equal(C) AND ...
        ///         Compare(N)) ...))))
        /// </remarks>
        public Expression? Filter(IEnumerator<object?> values)
        {
            if (!values.MoveNext())
                return default;

            var prop = _property.Body;
            var value = Expression.Constant(
                values.Current,
                typeof(TProp));

            var predicate = Descending
                ? _comparer.LessThan(PropertyType, prop, value)
                : _comparer.LessThan(PropertyType, value, prop);

            var tail = Next?.Filter(values);

            if (tail is null)
                return predicate;

            return Expression.Or(predicate, Expression.And(_comparer.Equal(PropertyType, prop, value), tail));
        }

        /// <summary>
        /// Recursively sorts a query for by this and all following properties.
        /// </summary>
        /// <inheritdoc/>
        public IOrderedQueryable<T> Sort(IQueryable<T> query)
        {
            var sorted = (Descending, First, query) switch
            {
                (false, false, IOrderedQueryable<T> ordered) => ordered.ThenBy(_property),
                (true, false, IOrderedQueryable<T> ordered) => ordered.ThenByDescending(_property),
                (false, _, _) => query.OrderBy(_property),
                (true, _, _) => query.OrderByDescending(_property)
            };

            return Next?.Sort(sorted) ?? sorted;
        }

        public override string ToString()
        {
            return $"{_property.Body}{(Descending ? " desc" : null)}";
        }

        public IEnumerator<ISortedProperty<T>> GetEnumerator()
        {
            IEnumerable<ISortedProperty<T>> GetEnumerable()
            {
                ISortedProperty<T>? node = this;
                while (node is not null)
                {
                    yield return node;
                    node = node.Next;
                }
            }

            return GetEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
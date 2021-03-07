using System;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken.Tests
{
    public class TestScenario
    {
        private Func<IOrderedQueryable<TestRecord>, IOrderedQueryable<TestRecord>> _sorter = query => query;

        public ITokenBuilder<TestRecord> Builder { get; } = QueryContinuationToken.Configure<TestRecord>();

        public IQueryContinuationToken<TestRecord> Token => Builder.Build();

        public IOrderedQueryable<TestRecord> Sort(IQueryable<TestRecord> query)
        {
            return _sorter(query.OrderBy(r => 0));
        }

        public TestScenario Ascending<TProp>(Expression<Func<TestRecord, TProp>> property)
        {
            Builder.Ascending(property);

            var inner = _sorter;
            _sorter = query => inner(query).ThenBy(property);

            return this;
        }

        public TestScenario Descending<TProp>(Expression<Func<TestRecord, TProp>> property)
        {
            Builder.Descending(property);

            var inner = _sorter;
            _sorter = query => inner(query).ThenByDescending(property);

            return this;
        }

        public override string ToString()
        {
            return Builder.ToString();
        }

        public void Deconstruct(
            out IQueryContinuationToken<TestRecord> continuationToken,
            out Func<IQueryable<TestRecord>, IOrderedQueryable<TestRecord>> sorter)
        {
            continuationToken = Token;
            sorter = Sort;
        }
    }

}

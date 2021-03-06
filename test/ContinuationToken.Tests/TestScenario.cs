using System;
using System.Linq;
using System.Linq.Expressions;

namespace ContinuationToken.Tests
{
    public class TestScenario
    {
        private readonly ITokenBuilder<TestRecord> _builder = ContinuationTokens.ConfigureToken<TestRecord>();
        private Func<IOrderedQueryable<TestRecord>, IOrderedQueryable<TestRecord>> _sorter = query => query;

        public IContinuationToken<TestRecord> ContinuationToken => _builder.Build();

        public IOrderedQueryable<TestRecord> Sort(IQueryable<TestRecord> query)
        {
            return _sorter(query.OrderBy(r => 0));
        }

        public TestScenario Ascending<TProp>(Expression<Func<TestRecord, TProp>> property)
        {
            _builder.Ascending(property);

            var inner = _sorter;
            _sorter = query => inner(query).ThenBy(property);

            return this;
        }

        public TestScenario Descending<TProp>(Expression<Func<TestRecord, TProp>> property)
        {
            _builder.Descending(property);

            var inner = _sorter;
            _sorter = query => inner(query).ThenByDescending(property);

            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        public void Deconstruct(
            out IContinuationToken<TestRecord> continuationToken,
            out Func<IQueryable<TestRecord>, IOrderedQueryable<TestRecord>> sorter)
        {
            continuationToken = ContinuationToken;
            sorter = Sort;
        }
    }

}

using ContinuationToken.Reflection;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ContinuationToken.Tests.Reflection
{
    public class ComparisonExpressionTests
    {
        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(int))]
        [InlineData(typeof(long))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(Guid))]
        public void HasComparerFor(Type type)
        {
            var factory = new ComparisonProviderFactory();
            var provider = factory.GetProvider(type);

            var left = Expression.Parameter(type, "left");
            var right = Expression.Parameter(type, "right");

            var equal = provider.Equal(type, left, right);
            Assert.NotNull(equal);
            var lessThan = provider.LessThan(type, left, right);
            Assert.NotNull(lessThan);

            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                // check for nullable type compat
                var nullableType = typeof(Nullable<>).MakeGenericType(type);
                HasComparerFor(nullableType);
            }
        }
    }
}

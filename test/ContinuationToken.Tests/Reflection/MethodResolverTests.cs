using ContinuationToken.Reflection;
using System;
using Xunit;

namespace ContinuationToken.Tests.Reflection
{
    public class MethodResolverTests
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
            IMethodResolver resolver = new MethodResolver();

            var binding = resolver.GetCompareMethod(type);

            Assert.NotNull(binding);

            if (type.IsValueType)
            {
                // check for nullable type compat
                var nullableType = typeof(Nullable<>).MakeGenericType(type);
                var nullableBinding = resolver.GetCompareMethod(nullableType);
                Assert.NotNull(nullableBinding);
            }
        }
    }
}

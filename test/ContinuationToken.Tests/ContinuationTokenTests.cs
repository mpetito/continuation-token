using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static ContinuationToken.Tests.TestRecords;

namespace ContinuationToken.Tests
{
    public class ContinuationTokenTests
    {
        internal Mock<ITokenFormatter> Formatter { get; } = new Mock<ITokenFormatter>();

        internal ParameterExpression Input { get; } = Expression.Parameter(typeof(TestRecord));

        internal Mock<ISortedProperty<TestRecord>> Head { get; } = new Mock<ISortedProperty<TestRecord>>();

        internal Type PropertyType { get; } = typeof(string);

        internal ContinuationToken<TestRecord> Token { get; }

        public ContinuationTokenTests()
        {
            Head.Setup(o => o.GetEnumerator())
                .Returns(() => Enumerable.Repeat(Head.Object, 1).GetEnumerator());

            Head.Setup(o => o.PropertyType)
                .Returns(PropertyType);

            Token = new ContinuationToken<TestRecord>(Formatter.Object, Input, Head.Object);
        }

        [Fact]
        public void IgnoresBlankTokenStrings()
        {
            Token.ParseToken(null);
            Token.ParseToken("");
            Token.ParseToken(" ");

            Formatter.VerifyNoOtherCalls();
        }

        [Fact]
        public void IgnoresInvalidTokenString()
        {
            Formatter.Setup(o => o.Deserialize(It.IsAny<string>(), It.IsAny<Type[]>()))
                .Throws(new NotSupportedException());

            var output = Token.ParseToken("invalid");

            Assert.NotNull(output);
            Assert.Empty(output);
        }

        [Fact]
        public void SkipsFilteringWithoutToken()
        {
            var data = GenerateRecords(1).AsQueryable();

            var output = Token.FilterQuery(data, null);

            Assert.Same(data, output);
        }

        [Fact]
        public void NullTokenOnceExhausted()
        {
            var output = Token.GetToken(null);

            Assert.Null(output);
        }

        [Fact]
        public void PassesTypesDuringSerialiation()
        {
            var data = GenerateRecords(1).First();

            Token.GetToken(data);

            Formatter.Verify(o => o.Serialize(
                It.IsAny<object[]>(),
                It.Is((Type[] types) => types.Single() == PropertyType)));
        }

        [Fact]
        public void PassesTypesDuringDeserialiation()
        {
            var input = "*";

            Token.ParseToken(input);

            Formatter.Verify(o => o.Deserialize(
                input,
                It.Is((Type[] types) => types.Single() == PropertyType)));
        }
    }
}

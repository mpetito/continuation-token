using ContinuationToken.Providers;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace ContinuationToken.Tests
{
    public class OffsetTokenTests
    {
        internal Mock<ITokenFormatter> Formatter { get; } = new Mock<ITokenFormatter>();

        internal Mock<ISortedProperty<TestRecord>> Head { get; } = new Mock<ISortedProperty<TestRecord>>();

        internal OffsetToken<TestRecord> Token { get; }

        public OffsetTokenTests()
        {
            Head.Setup(o => o.GetEnumerator())
                .Returns(() => Enumerable.Repeat(Head.Object, 1).GetEnumerator());

            Token = new OffsetToken<TestRecord>(Formatter.Object, Head.Object);
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

            Assert.Null(output);
        }

        [Fact]
        public void NullTokenOnceExhausted()
        {
            var output = Token.GetNextToken(1, 0);

            Assert.Null(output);
        }
    }
}

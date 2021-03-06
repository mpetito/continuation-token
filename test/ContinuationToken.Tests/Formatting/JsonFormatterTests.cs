using ContinuationToken.Formatting;
using System;
using Xunit;
using static ContinuationToken.Tests.Formatting.FormatUtils;

namespace ContinuationToken.Tests.Formatting
{

    public class JsonFormatterTests
    {
        internal JsonTokenFormatter Formatter { get; } = new JsonTokenFormatter();

        [Fact]
        public void EmptyArray()
        {
            var (input, types) = MakeArray();
            var serialized = Formatter.Serialize(input, types);
            var deserialized = Formatter.Deserialize(serialized, types);

            Assert.Equal("[]", serialized);
            Assert.Equal(input, deserialized);
        }

        [Fact]
        public void PrimitiveArray()
        {
            var (input, types) = MakeArray(1, 1L, 1M);
            var serialized = Formatter.Serialize(input, types);
            var deserialized = Formatter.Deserialize(serialized, types);

            Assert.Equal(input, deserialized);
        }

        [Fact]
        public void NullableArray()
        {
            var (input, types) = MakeArray(null, (int?)1, (long?)1L, (decimal?)1M);
            types[0] = typeof(int?);

            var serialized = Formatter.Serialize(input, types);
            var deserialized = Formatter.Deserialize(serialized, types);

            Assert.Equal(input, deserialized);
        }

        [Fact]
        public void StringArray()
        {
            var (input, types) = MakeArray(null, "a", "b", "c");
            types[0] = typeof(string);

            var serialized = Formatter.Serialize(input, types);
            var deserialized = Formatter.Deserialize(serialized, types);

            Assert.Equal(input, deserialized);
        }

        [Fact]
        public void MixedArray()
        {
            var (input, types) = MakeArray(null, "a", 1, (long?)1);
            var serialized = Formatter.Serialize(input, types);
            var deserialized = Formatter.Deserialize(serialized, types);

            Assert.Equal(input, deserialized);
        }

        [Fact]
        public void DateTimePrecision()
        {
            var (input, types) = MakeArray(DateTime.MinValue, DateTime.MaxValue, DateTime.UnixEpoch, DateTime.Today.Subtract(TimeSpan.FromTicks(1)));

            var serialized = Formatter.Serialize(input, types);
            var deserialized = Formatter.Deserialize(serialized, types);

            Assert.Equal(input, deserialized);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("1")]
        [InlineData("\"\"")]
        public void IgnoresNonArrayJson(string json)
        {
            var formatter = new JsonTokenFormatter();

            var (_, types) = MakeArray(1);
            var deserialized = formatter.Deserialize(json, types);

            Assert.Empty(deserialized);
        }
    }
}

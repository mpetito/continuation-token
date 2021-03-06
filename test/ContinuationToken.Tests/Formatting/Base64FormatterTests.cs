using ContinuationToken.Formatting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static ContinuationToken.Tests.Formatting.FormatUtils;

namespace ContinuationToken.Tests.Formatting
{
    public class Base64FormatterTests
    {
        internal Mock<ITokenFormatter> Inner { get; } = new Mock<ITokenFormatter>();

        internal Base64TokenFormatter Formatter { get; }

        internal string Serialized { get; set; } = "*";

        internal object[] Deserialized { get; } = Array.Empty<object>();

        internal Type[] Types { get; } = new Type[] { typeof(string) };

        public Base64FormatterTests()
        {
            Inner.Setup(o => o.Serialize(Deserialized, Types))
                .Returns(() => Serialized);

            Inner.Setup(o => o.Deserialize(Serialized, Types))
                .Returns(() => Deserialized);

            Formatter = new Base64TokenFormatter(Inner.Object);
        }

        [Fact]
        public void Serializes()
        {
            var input = Formatter.Serialize(Deserialized, Types);
            var output = Formatter.Deserialize(input, Types);

            Assert.Equal(Deserialized, output);
        }
    }
}

using System;
using System.Text;

namespace ContinuationToken.Formatting
{
    /// <summary>
    /// Wrapper for an inner formatter that produces an "opaque" base64 encoded token.
    /// </summary>
    /// <inheritdoc/>
    public class Base64TokenFormatter : ITokenFormatter
    {
        private readonly ITokenFormatter _inner;
        
        public Encoding Encoding { get; }

        public Base64TokenFormatter(ITokenFormatter inner, Encoding? encoding = default)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            Encoding = encoding ?? Encoding.UTF8;
        }

        public object?[] Deserialize(string token, Type[] types)
        {
            return _inner.Deserialize(Encoding.GetString(Convert.FromBase64String(token)), types);
        }

        public string Serialize(object?[] array, Type[] types)
        {
            return Convert.ToBase64String(Encoding.GetBytes(_inner.Serialize(array, types)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ContinuationToken.Formatting
{
    /// <summary>
    /// Formatter that uses <see cref="System.Text.Json"/> serialization for key values.
    /// </summary>
    /// <inheritdoc/>
    public class JsonTokenFormatter : ITokenFormatter
    {
        private readonly JsonSerializerOptions _options;

        public JsonTokenFormatter()
            : this(new JsonSerializerOptions())
        {
        }

        public JsonTokenFormatter(JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public object?[] Deserialize(string token, Type[] types)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(token));
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
                return Array.Empty<object?>();

            var entries = new List<object?>(types.Length);
            while (entries.Count < types.Length && reader.Read())
            {
                var entry = JsonSerializer.Deserialize(ref reader, types[entries.Count], _options);
                entries.Add(entry);
            }

            return entries.ToArray();
        }

        public string Serialize(object?[] array, Type[] types)
        {
            return JsonSerializer.Serialize(array, _options);
        }
    }
}

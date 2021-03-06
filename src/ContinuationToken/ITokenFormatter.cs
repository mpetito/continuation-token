using System;

namespace ContinuationToken
{
    /// <summary>
    /// Serializes the key properties for a continuation token to / from a string.
    /// </summary>
    /// <seealso cref="Formatting.JsonTokenFormatter"/>
    /// <seealso cref="Formatting.Base64TokenFormatter"/>
    public interface ITokenFormatter
    {
        /// <summary>
        /// Serializes key values to a string.
        /// </summary>
        /// <param name="array">Key values.</param>
        /// <param name="types">Types for each key property.</param>
        /// <returns>Serialized string.</returns>
        string Serialize(object?[] array, Type[] types);

        /// <summary>
        /// Deserializes key values from a string.
        /// </summary>
        /// <param name="token">Token string.</param>
        /// <param name="types">Types for each key property.</param>
        /// <returns>Deserialized key values.</returns>
        object?[] Deserialize(string token, Type[] types);
    }
}
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ContinuationToken.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]  // for moq
namespace ContinuationToken
{
    public static class ContinuationTokens
    {
        /// <summary>
        /// Creates a new token configuration.
        /// </summary>
        /// <typeparam name="T">Query type.</typeparam>
        /// <returns>Builder used to configure the token specification.</returns>
        public static ITokenBuilder<T> ConfigureToken<T>() where T : class => new TokenBuilder<T>();
    }
}
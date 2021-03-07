using ContinuationToken.Providers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ContinuationToken.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]  // for moq
namespace ContinuationToken
{
    public static class QueryContinuationToken
    {
        /// <summary>
        /// Creates a new token configuration.
        /// </summary>
        /// <typeparam name="T">Query type.</typeparam>
        /// <returns>Builder used to configure the token specification.</returns>
        public static ITokenBuilder<T> Configure<T>() where T : class => new TokenBuilder<T>();

        public static ITokenBuilder<T> BookmarkToken<T>(this ITokenBuilder<T> builder) where T : class
            => builder.UseFactory(options => new BookmarkToken<T>(options));

        public static ITokenBuilder<T> OffsetToken<T>(this ITokenBuilder<T> builder) where T : class
            => builder.UseFactory(options => new OffsetToken<T>(options));
    }
}
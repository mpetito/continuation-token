using System.Collections.Generic;
using System.Linq;

namespace ContinuationToken.Providers
{
    internal class BookmarkContinuation<T> : IQueryContinuation<T> where T : class
    {
        public BookmarkContinuation(BookmarkToken<T> token, IQueryable<T> query)
        {
            Token = token;
            Query = query;
        }

        public BookmarkToken<T> Token { get; }

        public IQueryable<T> Query { get; }

        public string? GetNextToken(IEnumerable<T> results) 
            => Token.GetNextToken(results?.LastOrDefault());
    }
}
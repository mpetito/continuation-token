using System.Collections.Generic;
using System.Linq;

namespace ContinuationToken.Providers
{
    internal class OffsetContinuation<T> : IQueryContinuation<T> where T : class
    {
        public OffsetContinuation(OffsetToken<T> token, IQueryable<T> query, int? offset)
        {
            Token = token;
            Query = query;
            Offset = offset;
        }

        public OffsetToken<T> Token { get; }

        public IQueryable<T> Query { get; }

        public int? Offset { get; }

        public string? GetNextToken(IEnumerable<T> results) 
            => Token.GetNextToken(Offset.GetValueOrDefault(), results.Count());
    }
}

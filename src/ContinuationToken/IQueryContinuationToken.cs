using System.Collections.Generic;
using System.Linq;

namespace ContinuationToken
{
    /// <summary>
    /// Resumes a sorted query from a continuation token string.
    /// </summary>
    /// <typeparam name="T">Query type.</typeparam>
    /// <remarks>
    /// A continuation token is composed of a forward-linked-list of properties.
    /// The tuple projected by these properties should form a unique key for <typeparamref name="T"/>.
    /// 
    /// When generating a token string, the tuple for the last witnessed result from a query is serialized to a string.
    /// When resuming from a token string, the tuple is deserialized and the query is filtered and sorted to start after the last result.
    /// </remarks>
    public interface IQueryContinuationToken<T> where T : class
    {
        IQueryContinuation<T> ResumeQuery(IQueryable<T> query, string? lastToken = default);
    }

    public interface IQueryContinuation<T> where T : class
    {
        IQueryable<T> Query { get; }

        string? GetNextToken(IEnumerable<T> results);
    }
}
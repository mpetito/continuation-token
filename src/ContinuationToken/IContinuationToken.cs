using System.Linq;

namespace ContinuationToken
{
    /// <summary>
    /// Resumes a sorted query from a continuation token string.
    /// </summary>
    /// <typeparam name="T">Query type.</typeparam>
    public interface IContinuationToken<T> where T : class
    {
        string? GetToken(T last = default);

        IOrderedQueryable<T> ResumeQuery(IQueryable<T> query, string? continuationToken = default);
    }
}
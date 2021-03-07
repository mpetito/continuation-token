using System.Linq.Expressions;

namespace ContinuationToken
{
    public interface ITokenOptions<T> where T : class
    {
        ISortedProperty<T> Properties { get; }
        ParameterExpression Input { get; }
        ITokenFormatter Formatter { get; }
        IMethodResolver Resolver { get; }
    }
}
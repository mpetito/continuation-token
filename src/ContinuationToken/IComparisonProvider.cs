using System;
using System.Linq.Expressions;

namespace ContinuationToken
{
    /// <summary>
    /// Creates comparison expressions for a given property type.
    /// </summary>
    public interface IComparisonProvider
    {
        Expression Equal(Type type, Expression left, Expression right);

        Expression LessThan(Type type, Expression left, Expression right);
    }
}
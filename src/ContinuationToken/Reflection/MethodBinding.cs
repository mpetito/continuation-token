using System.Linq.Expressions;
using System.Reflection;

namespace ContinuationToken.Reflection
{
    public class MethodBinding
    {
        public MethodInfo Method { get; }

        public Expression? Instance { get; }

        public MethodBinding(MethodInfo method)
        {
            Method = method;
        }

        public MethodBinding(MethodInfo method, object instance)
            : this(method)
        {
            Instance = Expression.Constant(instance);
        }

        public MethodCallExpression Apply(params Expression[] arguments)
        {
            if (Method.IsStatic)
            {
                return Expression.Call(Method, arguments);
            }
            else
            {
                return Expression.Call(Instance, Method, arguments);
            }
        }
    }
}
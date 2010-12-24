using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Helpers
{
    public class CompiledLambdaParse
    {
        private static Dictionary<Type, Delegate> CompiledLambdaCache = new Dictionary<Type, Delegate>();

        static public Func<String, TTarget> GetParseLambda<TTarget>()
        {
            var targetType = typeof(TTarget);

            if (CompiledLambdaCache.ContainsKey(targetType))
            {
                return (Func<String, TTarget>)CompiledLambdaCache[targetType];
            }

            var parseMethod = targetType.GetMethod(
                "Parse",
                new[] { typeof(String) }
            );

            if (parseMethod == null)
                throw new NotSupportedException();

            var parameter = Expression.Parameter(typeof(String), "value");

            var expression = Expression.Convert(
                Expression.Call(
                    Expression.Constant(null),
                    parseMethod,
                    parameter
                ),
                targetType
            );

            var lambda = Expression.Lambda(expression, parameter).Compile();

            CompiledLambdaCache.Add(targetType, lambda);

            return (Func<String, TTarget>)lambda;
        }
    }
}

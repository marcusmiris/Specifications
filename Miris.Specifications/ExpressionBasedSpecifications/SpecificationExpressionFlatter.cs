using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Miris.Specifications.Internals.Extensions;
using Miris.Specifications.Internals.Extensions.Reflection;
using static Miris.Specifications.Internals.Extensions.Reflection.ReflectionSpecifications;

namespace Miris.Specifications.ExpressionBasedSpecifications
{
    /// <summary>
    ///     Se a expressão de uma <see cref="ExpressionBasedSpecification{T1}" /> faz referência a uma
    ///     outra <see cref="ExpressionBasedSpecification{T2}" />, esta classe tenta combinar ambas em uma
    ///     única expression.
    /// </summary>
    public class SpecificationExpressionFlatter<TCandidateItem>
        : ExpressionVisitor
    {
        private SpecificationExpressionFlatter()
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!DahPraReduzir(node))
                return base.VisitMethodCall(node);

            // identifica o primeiro parâmetro.
            var firstParameter = node.Method.GetParameters().FirstOrDefault()?.ParameterType;

            #region ' tryReplaceWith(...) '

            Func<Type, Type, Type, Expression> tryReplaceWith = (firstArgumentType, classType, predicateType) =>
            {
                if (!(firstParameter?.Implements(firstArgumentType) ?? false)) return null;

                // tenta encontrar novo método.
                var methodName = node.Method.Name;
                MethodInfo method;
                try
                {
                    var secondParameterTypeSpec = new ExpressionBasedSpecification<MethodInfo>(m =>
                        m.MakeGenericMethod(typeof(TCandidateItem)).GetParameters()[1].ParameterType == predicateType);

                    method = classType
                        .GetMethod(
                            Named(methodName),
                            Static,
                            WithNParameters(2),
                            FirstParameterOfType(firstArgumentType),
                            secondParameterTypeSpec)
                        .MakeGenericMethod(firstParameter.GetGenericArguments());
                }
                catch (Exception ex)
                {
                    throw new AggregateException(
                        new MissingMethodException(classType.FullName, methodName),
                        ex);
                }

                // recupera a expressão que representa a Specification
                var specificationExpression =
                    (Expression)
                    ((dynamic) Expression.Lambda<Func<object>>(node.Arguments[1]).Compile()()).Expression;

                // Flat!
                var flatedExpression = Expression.Call(
                    method,
                    new[]
                    {
                        node.Arguments.First(),
                        specificationExpression
                    });

                return flatedExpression;
            };

            #endregion

            return tryReplaceWith(typeof(IQueryable<>), typeof(Queryable),
                       typeof(Expression<Func<TCandidateItem, bool>>
                       )) // <-- HACK: necessário que `IQueryable` tenha precedência sobre `IEnumerable` aqui, uma vez que o primeiro implementa o segundo.
                   ?? tryReplaceWith(typeof(IEnumerable<>), typeof(Enumerable), typeof(Func<TCandidateItem, bool>))
                   ?? base.VisitMethodCall(node)
                ;
        }

        private static bool DahPraReduzir(Expression expression)
        {
            var asMethodCall = expression as MethodCallExpression;
            return asMethodCall != null
                   && asMethodCall.Method.DeclaringType == typeof(ExpressionBasedSpecificationExtensions);
        }

        public static Expression<Func<TCandidateItem, TRetorno>> Flat<TRetorno>(
            Expression<Func<TCandidateItem, TRetorno>> expression)
        {
            return DahPraReduzir(expression.Body)
                ? new SpecificationExpressionFlatter<TCandidateItem>().Visit(expression) as
                    Expression<Func<TCandidateItem, TRetorno>>
                : expression;
        }
    }
}
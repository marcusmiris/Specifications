using System;
using System.Linq.Expressions;

namespace Miris.Specifications.ExpressionBasedSpecifications
{
    public interface IExpressionBasedSpecification<in T>
    {
        bool SatisfiedBy(T candidate);

        Expression<Func<TOutput, bool>> GetExpression<TOutput>() where TOutput : T;
    }
}
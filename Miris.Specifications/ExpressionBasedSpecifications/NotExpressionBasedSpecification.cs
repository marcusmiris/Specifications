using System;
using System.Linq.Expressions;

namespace Miris.Specifications.ExpressionBasedSpecifications
{
    public sealed class NotExpressionBasedSpecification<T>
        : ExpressionBasedSpecification<T>
    {
        #region ' Constructor '

        public NotExpressionBasedSpecification(ExpressionBasedSpecification<T> originalSpecification)
        {
            if (originalSpecification == null) throw new ArgumentNullException(nameof(originalSpecification));
            var originalExpression = originalSpecification.Expression;

            var negatedExpression = System.Linq.Expressions.Expression.Not(originalExpression.Body);

            // define a expressão que será retornada 
            Expression = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(
                negatedExpression,
                originalExpression.Parameters);
        }

        #endregion

        public override Expression<Func<T, bool>> Expression { get; }
    }
}
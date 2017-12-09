using System;
using System.Linq.Expressions;
using Miris.Specifications.Internals.LinqExpressions;

namespace Miris.Specifications.ExpressionBasedSpecifications
{
    /// <summary>
    ///     Classe proposta para ser a base das classes
    ///     <c>Miris.Specifications.ExpressionBasedSpecifications.AndExpressionBasedSpecification</c> e
    ///     <c>Miris.Specifications.ExpressionBasedSpecifications.OrExpressionBasedSpecification</c>.
    ///     Sua necessidade apareceu para abstrair a lógica em comum entre elas: ambas as classes
    ///     precisavam realizar a combinação das expressões, ajustando todas as referências
    ///     de <c>System.Linq.Expressions.Expression.ParameterExpression</c> em ambas as expressões.
    /// </summary>
    public abstract class CompositeExpressionBasedSpecification<T>
        : ExpressionBasedSpecification<T>
    {
        public delegate Expression ExpressionFactoryMethod(Expression left, Expression right);

        #region ' Constructor '

        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="factoryMethod">
        ///     Indica o Factory Method que é usado para combinar as expressões.
        ///     Como sugestão podem ser informados os métodos
        ///     <c>System.Linq.Expressions.Expression.Or</c> e
        ///     <c>System.Linq.Expressions.Expression.And</c>.
        /// </param>
        protected CompositeExpressionBasedSpecification(
            IExpressionBasedSpecification<T> left,
            IExpressionBasedSpecification<T> right,
            ExpressionFactoryMethod factoryMethod)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            // Marcus Miris @ 27/07/2016
            // Alguém pode me perguntar "porque o novo nome do parâmetro é X e não Y?". 
            // Bem... A resposta está na Wikipedia:
            //      "In mathematics, a predicate is commonly understood to be a Boolean-valued 
            //      function P: X→ {true, false}, called the predicate on X"
            //  See more in https://en.wikipedia.org/wiki/Predicate_(mathematical_logic)` (acessado @ 27/07/2016).
            var newParameter = System.Linq.Expressions.Expression.Parameter(typeof(T), @"x");

            var leftExp = left.GetExpression<T>().WithParameter(newParameter);
            var rightExp = right.GetExpression<T>().WithParameter(newParameter);

            var resultExp = factoryMethod(leftExp.Body, rightExp.Body);

            Expression = System.Linq.Expressions.Expression
                .Lambda<Func<T, bool>>(resultExp, newParameter);
        }

        #endregion

        public override Expression<Func<T, bool>> Expression { get; }
    }
}
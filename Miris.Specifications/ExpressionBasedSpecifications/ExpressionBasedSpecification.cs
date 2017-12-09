using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Miris.Specifications.ExpressionBasedSpecifications
{
    [DebuggerDisplay(@"{Expression}")]
    public class ExpressionBasedSpecification<T>
        : Specification<T>
            , IExpressionBasedSpecification<T>
    {
        #region ' Private Members '

        /// <summary>
        ///     Lazy Object para acesso à Expression Tree compilada.
        ///     O seu uso é para evitar que a Expression venha a ser compilada
        ///     mais de uma vez.
        /// </summary>
        private readonly Lazy<Func<T, bool>> _compiledExpression;

        #endregion

        #region ' IExpressionBasedSpecification '

        Expression<Func<TOutput, bool>> IExpressionBasedSpecification<T>.GetExpression<TOutput>()
        {
            return System.Linq.Expressions.Expression.Lambda<Func<TOutput, bool>>(
                Expression.Body, Expression.Parameters);
        }

        #endregion

        #region ' Constructor '

        protected ExpressionBasedSpecification()
        {
            _compiledExpression = new Lazy<Func<T, bool>>(() => Expression.Compile());
        }

        /// <summary>
        ///     Permite a crição direta de uma especificação a partir de uma Expression Tree.
        /// </summary>
        public ExpressionBasedSpecification(Expression<Func<T, bool>> expression)
            : this()
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            Expression = SpecificationExpressionFlatter<T>.Flat(expression);
        }

        #endregion

        #region ' Specification<T> '

        /// <summary>
        ///     Recupera a Expression Tree definida pela especificação atual.
        /// </summary>
        public virtual Expression<Func<T, bool>> Expression { get; }

        /// <summary>
        ///     Determina se a especificação atual é satisfeita pelo objeto informado.
        /// </summary>
        public override bool SatisfiedBy(T candidate)
        {
            return _compiledExpression.Value(candidate);
        }

        #endregion

        #region ' Operators Overloads '

        public static ExpressionBasedSpecification<T> operator !(ExpressionBasedSpecification<T> originalSpecification)
        {
            return new NotExpressionBasedSpecification<T>(originalSpecification);
        }

        #region ' And (&) '

        public static ExpressionBasedSpecification<T> operator &(
            ExpressionBasedSpecification<T> leftSideSpecification,
            ExpressionBasedSpecification<T> rightSideSpecification)
        {
            return new AndExpressionBasedSpecification<T>(leftSideSpecification, rightSideSpecification);
        }

        public static ExpressionBasedSpecification<T> operator &(
            IExpressionBasedSpecification<T> leftSideSpecification,
            ExpressionBasedSpecification<T> rightSideSpecification)
        {
            return new AndExpressionBasedSpecification<T>(leftSideSpecification, rightSideSpecification);
        }

        public static ExpressionBasedSpecification<T> operator &(
            ExpressionBasedSpecification<T> leftSideSpecification,
            IExpressionBasedSpecification<T> rightSideSpecification)
        {
            return new AndExpressionBasedSpecification<T>(leftSideSpecification, rightSideSpecification);
        }

        #endregion

        #region ' Or (|) '

        public static ExpressionBasedSpecification<T> operator |(
            ExpressionBasedSpecification<T> leftSideSpecification,
            ExpressionBasedSpecification<T> rightSideSpecification)
        {
            return new OrExpressionBasedSpecification<T>(leftSideSpecification, rightSideSpecification);
        }

        public static ExpressionBasedSpecification<T> operator |(
            IExpressionBasedSpecification<T> leftSideSpecification,
            ExpressionBasedSpecification<T> rightSideSpecification)
        {
            return new AndExpressionBasedSpecification<T>(leftSideSpecification, rightSideSpecification);
        }

        public static ExpressionBasedSpecification<T> operator |(
            ExpressionBasedSpecification<T> leftSideSpecification,
            IExpressionBasedSpecification<T> rightSideSpecification)
        {
            return new AndExpressionBasedSpecification<T>(leftSideSpecification, rightSideSpecification);
        }

        #endregion

        #endregion
    }
}
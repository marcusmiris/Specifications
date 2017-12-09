using System;
using System.Linq.Expressions;

namespace Miris.Specifications.Internals.LinqExpressions
{
    /// <summary>
    ///     Classe que pode ser usada para substituir um parãmetro
    ///     de uma Expression Tree.
    /// </summary>
    public class ExpressionParameterReplacer
        : ExpressionVisitor
    {
        private readonly ParameterExpression _newParameter;
        private readonly ParameterExpression _oldParameter;

        #region ' Constructor '

        /// <param name="oldParameter">
        ///     A instância do parâmetro que deve ser substituído.
        /// </param>
        /// <param name="newParameter">
        ///     A instância do novo parãmetro.
        /// </param>
        public ExpressionParameterReplacer(
            ParameterExpression oldParameter,
            ParameterExpression newParameter)
        {
            if (oldParameter == null) throw new ArgumentNullException(nameof(oldParameter));
            if (newParameter == null) throw new ArgumentNullException(nameof(newParameter));

            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        #endregion

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter
                ? _newParameter
                : base.VisitParameter(node);
        }
    }
}
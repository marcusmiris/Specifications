namespace Miris.Specifications.ExpressionBasedSpecifications
{
    public class AndExpressionBasedSpecification<T>
        : CompositeExpressionBasedSpecification<T>
    {
        #region ' Constructor '

        public AndExpressionBasedSpecification(
            IExpressionBasedSpecification<T> left,
            IExpressionBasedSpecification<T> right)
            : base(left, right, System.Linq.Expressions.Expression.And)
        {
        }

        #endregion
    }
}
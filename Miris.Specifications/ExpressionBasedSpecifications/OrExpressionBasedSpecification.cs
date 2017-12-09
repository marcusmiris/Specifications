namespace Miris.Specifications.ExpressionBasedSpecifications
{
    public class OrExpressionBasedSpecification<T>
        : CompositeExpressionBasedSpecification<T>
    {
        public OrExpressionBasedSpecification(
            IExpressionBasedSpecification<T> left,
            IExpressionBasedSpecification<T> right)
            : base(left, right, System.Linq.Expressions.Expression.Or)
        {
        }
    }
}
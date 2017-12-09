using System.Collections.Generic;
using System.Linq.Expressions;

namespace Miris.Specifications.Internals.LinqExpressions.Comparision
{
    public class ExpressionEqualityComparer : IEqualityComparer<Expression>
    {
        public static ExpressionEqualityComparer Instance = new ExpressionEqualityComparer();

        public bool Equals(Expression a, Expression b)
        {
            return new ExpressionComparison(a, b).AreEqual;
        }

        public int GetHashCode(Expression expression)
        {
            return new HashCodeCalculation(expression).HashCode;
        }

        public bool Equals<TPredicate>(Expression<TPredicate> a, Expression<TPredicate> b)
        {
            return Equals(a, b as Expression);
        }
    }
}
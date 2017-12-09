// https://github.com/lytico/db4o/blob/11fa71fc00feff0ef3a4fd1e39ed01e00c9311bc/db4o.net/Db4objects.Db4o.Linq/Db4objects.Db4o.Linq/Expressions/ExpressionEnumeration.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Miris.Specifications.Internals.LinqExpressions.Comparision
{
    internal class ExpressionEnumeration : ExpressionVisitor, IEnumerable<Expression>
    {
        private readonly List<Expression> _expressions = new List<Expression>();

        public ExpressionEnumeration(Expression expression)
        {
            Visit(expression);
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return _expressions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public sealed override Expression Visit(Expression expression)
        {
            if (expression == null) return null;
            _expressions.Add(expression);
            return base.Visit(expression);
        }
    }
}
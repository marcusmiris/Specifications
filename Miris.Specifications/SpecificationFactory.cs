using System;
using System.Linq.Expressions;
using Miris.Specifications.ExpressionBasedSpecifications;

namespace Miris.Specifications
{
    public static class SpecificationFactory
    {
        public static ExpressionBasedSpecification<TCantidade> Specification<TCantidade>(
            Expression<Func<TCantidade, bool>> expression)
        {
            return new ExpressionBasedSpecification<TCantidade>(expression);
        }
    }
}
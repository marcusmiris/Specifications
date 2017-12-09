using System;
using System.Collections.Generic;
using System.Linq;

namespace Miris.Specifications.ExpressionBasedSpecifications
{
    public static class ExpressionBasedSpecificationExtensions
    {
        #region ' Where(...) '

        public static IQueryable<T> Where<T>(
            this IQueryable<T> source,
            IExpressionBasedSpecification<T> specification)
        {
            if (source == null) return null;
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return source.Where(specification.GetExpression<T>());
        }

        public static IEnumerable<T> Where<T>(
            this IEnumerable<T> source,
            IExpressionBasedSpecification<T> specification)
        {
            if (source == null) return null;
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return source.Where(specification.SatisfiedBy);
        }

        public static IQueryable<T> Where<T>(
            this IQueryable<T> source,
            IExpressionBasedSpecification<T> specification,
            params IExpressionBasedSpecification<T>[] otherSpecifications)
        {
            if (source == null) return null;
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return otherSpecifications.Aggregate(source.Where(specification),
                (query, spec) => query.Where(spec));
        }

        #endregion

        #region ' Any(...) '

        public static bool Any<T>(
            this IQueryable<T> source,
            IExpressionBasedSpecification<T> specification)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return source.Any(specification.GetExpression<T>());
        }

        public static bool Any<T>(
            this IEnumerable<T> source,
            IExpressionBasedSpecification<T> specification)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return source.Any(specification.SatisfiedBy);
        }

        #endregion
    }
}
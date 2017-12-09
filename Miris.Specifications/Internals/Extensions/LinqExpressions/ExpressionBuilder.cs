﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace Miris.Specifications.Internals.LinqExpressions
{
    /// <summary>
    ///     Extension methods for adding AND and OR with parameters rebinder
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        ///     Cria e retorna uma Expression baseada na expressão atual, porém
        ///     utilizando o Parameter informado.
        /// </summary>
        public static Expression<Func<T, bool>> WithParameter<T>(
            this Expression<Func<T, bool>> expression,
            ParameterExpression newParameter)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (newParameter == null) throw new ArgumentNullException(nameof(newParameter));

            var oldParameter = expression.Parameters.Single();

            return (Expression<Func<T, bool>>) new ExpressionParameterReplacer(oldParameter, newParameter)
                .Visit(expression);
        }

        /// <summary>
        ///     Compose two expressions and merge all in a new expression
        /// </summary>
        /// <typeparam name="T">Type of params in expression</typeparam>
        /// <param name="first">Expression instance</param>
        /// <param name="second">Expression to merge</param>
        /// <param name="merge">Function to merge</param>
        /// <returns>New merged expression</returns>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (merge == null) throw new ArgumentNullException(nameof(merge));


            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new {f, s = second.Parameters[i]})
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        ///     And operator
        /// </summary>
        /// <typeparam name="T">Type of params in expression</typeparam>
        /// <param name="first">Right Expression in AND operation</param>
        /// <param name="second">Left Expression in And operation</param>
        /// <returns>New AND expression</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return first.Compose(second, Expression.And);
        }

        /// <summary>
        ///     Or operator
        /// </summary>
        /// <typeparam name="T">Type of param in expression</typeparam>
        /// <param name="first">Right expression in OR operation</param>
        /// <param name="second">Left expression in OR operation</param>
        /// <returns>New Or expressions</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return first.Compose(second, Expression.Or);
        }
    }
}
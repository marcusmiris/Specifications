using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Miris.Specifications.ComMensagens;

namespace Miris.Specifications
{
    public static class SpecificationExtensions
    {
        public static bool IsSatisfiedBy<T>(this ISpecification<T> specification, T candidate) where T : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            return specification.SatisfiedBy(candidate);
        }

        public static IEnumerable<T> GetAllNotSatisfiedBy<T>(
            this ISpecification<T> specification,
            IEnumerable<T> lista)
        {
            //Todas passaram no teste quando a quantidade total for igual a quantidade que satisfazem a Specification
            return lista.Where(s => !specification.SatisfiedBy(s));
        }


        public static bool SatisfiesAll<T>(
            this T candidate,
            IEnumerable<ISpecification<T>> specifications)
        {
            if (candidate == null) throw new ArgumentNullException(nameof(candidate));
            if (specifications == null) throw new ArgumentNullException(nameof(specifications));

            return specifications.All(spec => spec.SatisfiedBy(candidate));
        }

        #region ' GarantirQue(...) '

        /// <summary>
        ///     Garante que a especificação informada é satisfeita pelo objeto.
        /// </summary>
        [DebuggerStepThrough]
        public static void GarantirQue<T, TSpecification>(this T candidate, TSpecification specification)
            where T : class
            where TSpecification : ISpecification<T>, ISpecificationComMensagens<T>
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            if (!specification.SatisfiedBy(candidate))
            {
                var mensagemDeErro = string.Join("; ", specification.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate));
                throw new SpecificationException<T, TSpecification>(candidate, specification, mensagemDeErro);
            }
        }

        /// <summary>
        ///     Garante que uma especificação é satisfeita pelo objeto informado.
        /// </summary>
        /// <exception cref="SpecificationException&lt;T,TSpecification&gt;">
        ///     Se o objeto candidato não atender a especificação.
        /// </exception>
        public static void GarantirQue<T, TSpecification>(this T candidate, TSpecification specification,
            string exceptionMessageFormat, params object[] exceptionMessageArgs)
            where T : class
            where TSpecification : ISpecification<T>
        {
            GarantirQue(candidate, specification, string.Format(exceptionMessageFormat, exceptionMessageArgs));
        }

        /// <summary>
        ///     Garante que uma especificação é satisfeita pelo objeto informado.
        /// </summary>
        /// <exception cref="SpecificationException&lt;T,TSpecification&gt;">
        ///     Se o objeto candidato não atender a especificação.
        /// </exception>
        public static void GarantirQue<T, TSpecification>(this T candidate, TSpecification specification,
            string mensagemDeErro)
            where T : class
            where TSpecification : ISpecification<T>
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            if (!specification.SatisfiedBy(candidate))
                throw new SpecificationException<T, TSpecification>(candidate, specification, mensagemDeErro);
        }

        /// <summary>
        ///     Garante que um critério booleano é satisfeito pelo objeto informado.
        /// </summary>
        public static void GarantirQue<T>(this T candidate, Expression<Func<T, bool>> criterio, string mensagemDeErro)
            where T : class
        {
            if (criterio == null)
                throw new ArgumentNullException(nameof(criterio));

            var atendeuAoCriterio = criterio.Compile().Invoke(candidate);

            if (!atendeuAoCriterio)
                throw new Exception(mensagemDeErro);
        }

        /// <summary>
        ///     Garante que um critério booleano é satisfeito pelo objeto informado.
        /// </summary>
        public static void GarantirQue<T>(
            this T candidate,
            Expression<Func<T, bool>> criterio,
            string exceptionMessageFormat,
            params object[] exceptionMessageArgs)
            where T : class
        {
            GarantirQue(candidate, criterio, string.Format(exceptionMessageFormat, exceptionMessageArgs));
        }

        /// <summary>
        ///     Garante que um critério booleano é satisfeito pelo objeto informado.
        /// </summary>
        public static void GarantirQue<T>(this T candidate, Expression<Func<T, bool>> criterio,
            Func<string> mensagemDeErroFactory)
            where T : class
        {
            var mensagemDeErro = mensagemDeErroFactory.Invoke();
            GarantirQue(candidate, criterio, mensagemDeErro);
        }

        #endregion
    }
}
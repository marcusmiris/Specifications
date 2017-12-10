using System;

namespace Miris.Specifications
{
    /// <summary>
    ///     Exception utilizada para evidenciar que uma determinada entidade não satisfaz uma determinada Specification
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TSpecification"></typeparam>
    public class SpecificationException<TEntity, TSpecification> : Exception
        where TSpecification : ISpecification<TEntity>
    {
        public SpecificationException(
            TEntity entidade,
            TSpecification specification,
            string message)
            : base(message)
        {
            Entidade = entidade;
            Specification = specification;
        }

        public TEntity Entidade { get; set; }
        public TSpecification Specification { get; set; }
    }
}
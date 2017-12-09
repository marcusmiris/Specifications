using System;
using System.Collections.Generic;
using System.Linq;
using Miris.Specifications.Internals.Extensions;

namespace Miris.Specifications.ComMensagens
{
    /// <summary>
    ///     A Logic OR Specification
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    public sealed class OrSpecificationComMensagens<T>
        : SpecificationComMensagens<T>
    {
        #region Public Constructor

        /// <summary>
        ///     Default constructor for AndSpecification
        /// </summary>
        /// <param name="left">Left side specification</param>
        /// <param name="right">Right side specification</param>
        public OrSpecificationComMensagens(
            ISpecificationComMensagens<T> left,
            ISpecificationComMensagens<T> right)
        {
            _leftSideSpecification = left ?? throw new ArgumentNullException(nameof(left));
            _rightSideSpecification = right ?? throw new ArgumentNullException(nameof(right));
        }

        #endregion

        #region Members

        private readonly ISpecificationComMensagens<T> _rightSideSpecification;
        private readonly ISpecificationComMensagens<T> _leftSideSpecification;

        #endregion

        #region ' ISpecificationComMensagens '

        public override ISpecificationComMensagens<T> Body => this;

        public override bool SatisfiedBy(T candidate)
        {
            return _leftSideSpecification.SatisfiedBy(candidate)
                   || _rightSideSpecification.SatisfiedBy(candidate);
        }

        public override IEnumerable<string> GetRazoesPelasQuaisNaoFoiSatisfeita(T candidate)
        {
            return !_leftSideSpecification.SatisfiedBy(candidate) && !_rightSideSpecification.SatisfiedBy(candidate)
                ? _leftSideSpecification.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate)
                    .Concat(_rightSideSpecification.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate))
                : Enumerable.Empty<string>();
        }

        public override IEnumerable<string> GetRazoesPelasQuaisFoiSatisfeita(T candidate)
        {
            return _leftSideSpecification.GetRazoesPelasQuaisFoiSatisfeita(candidate)
                .Se(_leftSideSpecification.SatisfiedBy(candidate), Enumerable.Empty<string>()).Concat(
                    _rightSideSpecification.GetRazoesPelasQuaisFoiSatisfeita(candidate)
                        .Se(_rightSideSpecification.SatisfiedBy(candidate), Enumerable.Empty<string>()));
        }

        #endregion
    }
}
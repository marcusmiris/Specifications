using System;
using System.Collections.Generic;
using System.Linq;

namespace Miris.Specifications.ComMensagens
{
    public class NotSpecificationComMensagens<TCandidate>
        : SpecificationComMensagens<TCandidate>
    {
        private readonly ISpecificationComMensagens<TCandidate> _originalSpecification;

        #region ' ctor '

        public NotSpecificationComMensagens(
            ISpecificationComMensagens<TCandidate> originalSpecification)
        {
            _originalSpecification =
                originalSpecification ?? throw new ArgumentNullException(nameof(originalSpecification));
        }

        #endregion

        #region' SpecificationComMensagens '

        public override ISpecificationComMensagens<TCandidate> Body => this;

        public override bool SatisfiedBy(TCandidate candidate)
        {
            return !_originalSpecification.SatisfiedBy(candidate);
        }

        public override IEnumerable<string> GetRazoesPelasQuaisNaoFoiSatisfeita(TCandidate candidate)
        {
            return !SatisfiedBy(candidate)
                ? _originalSpecification.GetRazoesPelasQuaisFoiSatisfeita(candidate)
                : Enumerable.Empty<string>();
        }

        public override IEnumerable<string> GetRazoesPelasQuaisFoiSatisfeita(TCandidate candidate)
        {
            return SatisfiedBy(candidate)
                ? _originalSpecification.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate)
                : Enumerable.Empty<string>();
        }

        #endregion
    }
}
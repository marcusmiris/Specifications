using System.Collections.Generic;

namespace Miris.Specifications.ComMensagens
{
    public abstract class SpecificationComMensagens<TCandidate>
        : Specification<TCandidate>
            , ISpecificationComMensagens<TCandidate>
    {
        public abstract ISpecificationComMensagens<TCandidate> Body { get; }

        #region ' ISpecificationComMensagens<TCandidate> '

        public override bool SatisfiedBy(TCandidate candidate)
        {
            return Body.SatisfiedBy(candidate);
        }

        public virtual ISpecificationComMensagens<TCandidate> PreCondition => null;

        public virtual IEnumerable<string> GetRazoesPelasQuaisNaoFoiSatisfeita(TCandidate candidate)
        {
            return Body.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate);
        }

        public virtual IEnumerable<string> GetRazoesPelasQuaisFoiSatisfeita(TCandidate candidate)
        {
            return Body.GetRazoesPelasQuaisFoiSatisfeita(candidate);
        }

        #endregion

        #region ' Operator Overrides '

        #region ' Or '

        public static SpecificationComMensagens<TCandidate> operator |(
            SpecificationComMensagens<TCandidate> leftSpec,
            SpecificationComMensagens<TCandidate> rightSpec)
        {
            return new OrSpecificationComMensagens<TCandidate>(leftSpec, rightSpec);
        }

        public static OrSpecificationComMensagens<TCandidate> operator |(
            ISpecificationComMensagens<TCandidate> leftSpec,
            SpecificationComMensagens<TCandidate> rightSpec)
        {
            return new OrSpecificationComMensagens<TCandidate>(leftSpec, rightSpec);
        }

        public static OrSpecificationComMensagens<TCandidate> operator |(
            SpecificationComMensagens<TCandidate> leftSpec,
            ISpecificationComMensagens<TCandidate> rightSpec)
        {
            return new OrSpecificationComMensagens<TCandidate>(leftSpec, rightSpec);
        }

        #endregion

        #region ' And '

        public static SpecificationComMensagens<TCandidate> operator &(
            SpecificationComMensagens<TCandidate> leftSpec,
            SpecificationComMensagens<TCandidate> rightSpec)
        {
            return new AndSpecificationComMensagens<TCandidate>(leftSpec, rightSpec);
        }

        public static AndSpecificationComMensagens<TCandidate> operator &(
            SpecificationComMensagens<TCandidate> leftSpec,
            ISpecificationComMensagens<TCandidate> rightSpec)
        {
            return new AndSpecificationComMensagens<TCandidate>(leftSpec, rightSpec);
        }

        public static AndSpecificationComMensagens<TCandidate> operator &(
            ISpecificationComMensagens<TCandidate> leftSpec,
            SpecificationComMensagens<TCandidate> rightSpec)
        {
            return new AndSpecificationComMensagens<TCandidate>(leftSpec, rightSpec);
        }

        #endregion

        public static NotSpecificationComMensagens<TCandidate> operator !(
            SpecificationComMensagens<TCandidate> originalSpec)
        {
            return new NotSpecificationComMensagens<TCandidate>(originalSpec);
        }

        #endregion
    }
}
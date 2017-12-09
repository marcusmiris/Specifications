using System.Collections.Generic;

namespace Miris.Specifications.ComMensagens
{
    public interface ISpecificationComMensagens<in TCandidate>
        : ISpecification<TCandidate>
    {
        ISpecificationComMensagens<TCandidate> PreCondition { get; }
        IEnumerable<string> GetRazoesPelasQuaisNaoFoiSatisfeita(TCandidate candidate);
        IEnumerable<string> GetRazoesPelasQuaisFoiSatisfeita(TCandidate candidate);
    }
}
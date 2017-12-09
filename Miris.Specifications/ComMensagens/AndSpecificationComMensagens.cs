using System;
using System.Collections.Generic;
using System.Linq;

namespace Miris.Specifications.ComMensagens
{
    /// <summary>
    ///     A Logic AND Specification
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    public sealed class AndSpecificationComMensagens<T>
        : SpecificationComMensagens<T>
    {
        #region ' Public Constructor

        /// <summary>
        ///     Default constructor for AndSpecification
        /// </summary>
        /// <param name="left">Left side specification</param>
        /// <param name="right">Right side specification</param>
        public AndSpecificationComMensagens(
            ISpecificationComMensagens<T> left,
            ISpecificationComMensagens<T> right)
        {
            _leftSpec = left ?? throw new ArgumentNullException(nameof(left));
            _rightSpec = right ?? throw new ArgumentNullException(nameof(right));
        }

        #endregion

        #region ' Members '

        private readonly ISpecificationComMensagens<T> _rightSpec;
        private readonly ISpecificationComMensagens<T> _leftSpec;

        #endregion

        #region ' ISpecificationComMensagens '

        public override ISpecificationComMensagens<T> Body => this;

        public override bool SatisfiedBy(T candidate)
        {
            return _leftSpec.SatisfiedBy(candidate)
                   && _rightSpec.SatisfiedBy(candidate);
        }

        public override IEnumerable<string> GetRazoesPelasQuaisNaoFoiSatisfeita(T candidate)
        {
            if (SatisfiedBy(candidate)) yield break;

            var leftSpecSatisfied = _leftSpec.SatisfiedBy(candidate);

            if (!leftSpecSatisfied)
                foreach (var razao in _leftSpec.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate))
                    yield return razao;

            // as mensagens de right são exibidas em ao menos uma das situações: 
            //  - left é satisfeita (logo precisa buscar a msg a partir de right);
            //  - right satisfaz pre condição.
            var satisfazPreCondicao = _rightSpec.PreCondition == null || _rightSpec.PreCondition.SatisfiedBy(candidate);
            if (leftSpecSatisfied || satisfazPreCondicao)
                foreach (var razao in _rightSpec.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate))
                    yield return razao;
        }

        public override IEnumerable<string> GetRazoesPelasQuaisFoiSatisfeita(T candidate)
        {
            return SatisfiedBy(candidate)
                ? _leftSpec.GetRazoesPelasQuaisFoiSatisfeita(candidate)
                    .Concat(_rightSpec.GetRazoesPelasQuaisFoiSatisfeita(candidate))
                : Enumerable.Empty<string>();
        }

        #endregion
    }
}
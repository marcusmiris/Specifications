using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Miris.Specifications.ComMensagens
{
    public class DirectSpecificationComMensagens<TCandidate>
        : SpecificationComMensagens<TCandidate>
    {
        private readonly string _mensagemDeErro;
        private readonly string _mensagemDeSucesso;
        private readonly Predicate<TCandidate> _predicado;

        #region ' Constructor '

        public DirectSpecificationComMensagens(
            Predicate<TCandidate> predicado,
            string mensagemDeErro,
            string mensagemDeSucesso)
            : this(predicado, mensagemDeErro, mensagemDeSucesso, null)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="predicado"></param>
        /// <param name="mensagemDeErro"></param>
        /// <param name="mensagemDeSucesso"></param>
        /// <param name="precondition">
        ///     Determina o contexto sob a qual a specification atual só faz sentido dentro dele.
        /// </param>
        public DirectSpecificationComMensagens(
            Predicate<TCandidate> predicado,
            string mensagemDeErro,
            string mensagemDeSucesso,
            ISpecificationComMensagens<TCandidate> precondition)
        {
            _predicado = predicado;
            _mensagemDeErro = mensagemDeErro;
            _mensagemDeSucesso = mensagemDeSucesso;
            PreCondition = precondition;
        }

        #endregion

        #region ' ISpecificationComMensagens<TCandidate> '

        public override ISpecificationComMensagens<TCandidate> Body => this;

        /// <summary>
        ///     Determina a condição que precisa estar estabelecida para que, sob pena de não
        ///     conseguir determinar se a specification é satisfeita ou não.
        /// </summary>
        /// <summary>
        ///     Por exemplo, só é possível determinar se uma pessoa tem mais de 18 anos (body da specification)
        ///     se a idade tiver sido devidamente informada (pré condição). Ou ainda, somente é possível determinar
        ///     o sexo da pessoa se esta for pessoa física.
        /// </summary>
        public override ISpecificationComMensagens<TCandidate> PreCondition { get; }

        private bool SatisfazPreCondicao(TCandidate candidate)
        {
            return PreCondition == null || PreCondition.SatisfiedBy(candidate);
        }

        [DebuggerStepThrough]
        public override bool SatisfiedBy(TCandidate candidate)
        {
            return SatisfazPreCondicao(candidate) && _predicado.Invoke(candidate);
        }

        [DebuggerStepThrough]
        public override IEnumerable<string> GetRazoesPelasQuaisNaoFoiSatisfeita(TCandidate candidate)
        {
            if (!SatisfazPreCondicao(candidate))
                foreach (var razao in PreCondition.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate))
                    yield return razao;
            else if (!SatisfiedBy(candidate)) yield return _mensagemDeErro;
        }

        public override IEnumerable<string> GetRazoesPelasQuaisFoiSatisfeita(TCandidate candidate)
        {
            if (SatisfazPreCondicao(candidate) && SatisfiedBy(candidate))
                yield return _mensagemDeSucesso;
        }

        #endregion
    }
}
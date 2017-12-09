using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miris.Specifications.ComMensagens;

namespace Miris.Specifications.Tests.ComMensagens
{
    [TestClass]
    public class SpecificationComMensagensPreconditionTests
    {

        #region ' Members '

        private readonly object _candidate = new object();
        private const string ErrorMessage = @"Error Message";
        private const string SuccessMessage = @"Success Message";
        private const string LeftErrorMessage = "Left Error Message";
        private const string RightErrorMessage = "Right spec unsatisfaction";
        private const string LeftSuccessMessage = "Left Success Message";
        private const string RightSuccessMessage = "Right Success Message";
        private const string NoMatterMessage = "no matter";

        private static SpecificationComMensagens<object> AlwaysSatisfiedSpec(string successMessage) =>
            new DirectSpecificationComMensagens<object>(c => true, NoMatterMessage, successMessage);

        private static SpecificationComMensagens<object> AlwaysFailSpec(string errorMessage) =>
            new DirectSpecificationComMensagens<object>(c => false, errorMessage, NoMatterMessage);

        private static SpecificationComMensagens<object> PreconditionedSatisfiedSpec(
            string specSuccessMessage = NoMatterMessage,
            string preconditionSuccessMessage = NoMatterMessage) =>
            new DirectSpecificationComMensagens<object>(
                c => true,
                NoMatterMessage,
                specSuccessMessage,
                precondition: AlwaysSatisfiedSpec(preconditionSuccessMessage));

        private static SpecificationComMensagens<object> WithPreconditionFailureSpec(
            string specUnsatisfactionErrorMessage = NoMatterMessage,
            string preconditionErrorMessage = NoMatterMessage) =>
            new DirectSpecificationComMensagens<object>(
                c => true,
                specUnsatisfactionErrorMessage, 
                NoMatterMessage, 
                precondition: AlwaysFailSpec(preconditionErrorMessage));

        #endregion

        #region ' Direct '

        [TestMethod]
        public void TestaDirectSpecificationPreCondition_Sucesso()
        {
            var spec = PreconditionedSatisfiedSpec(specSuccessMessage: SuccessMessage);

            Assert.IsTrue(spec.SatisfiedBy(_candidate));

            var razoes = spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate).ToList();
            Assert.AreEqual(1, razoes.Count);
            Assert.AreEqual(SuccessMessage, razoes.Single());

            Assert.AreEqual(0, spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).Count());
        }

        [TestMethod]
        public void TestaDirectSpecificationPreCondition_Fail()
        {
            var spec = WithPreconditionFailureSpec(preconditionErrorMessage: ErrorMessage);

            Assert.IsFalse(spec.SatisfiedBy(_candidate));

            var razoes = spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).ToList();
            Assert.AreEqual(1, razoes.Count);
            Assert.AreEqual(ErrorMessage, razoes.Single());

            Assert.AreEqual(0, spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate).Count());
        }

        [TestMethod]
        public void TestaDirectSpecificationPreCondition_EvitarInvokeDesnecessarioDoBody()
        {
            var qtdVezesBodyExecutada = 0;
            var spec = new DirectSpecificationComMensagens<object>(
                o => {
                    qtdVezesBodyExecutada += 1;
                    return true;
                },
                NoMatterMessage, NoMatterMessage,
                precondition: AlwaysFailSpec(NoMatterMessage));

            Assert.IsFalse(spec.SatisfiedBy(_candidate));
            var _1 = spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate);
            var _2 = spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate);

            Assert.AreEqual(0, qtdVezesBodyExecutada);
        }

        #endregion

        #region ' And Also '

        [TestMethod]
        public void TestaAndSpecificationPreCondition_Sucess()
        {
            var spec = PreconditionedSatisfiedSpec(preconditionSuccessMessage: LeftSuccessMessage) 
                && PreconditionedSatisfiedSpec(preconditionSuccessMessage: RightSuccessMessage);

            Assert.IsTrue(spec.SatisfiedBy(_candidate));

            var razoes = spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate).ToList();
            Assert.AreEqual(2, razoes.Count);
            Assert.IsFalse(razoes.Contains(LeftSuccessMessage));
            Assert.IsFalse(razoes.Contains(RightSuccessMessage));

            Assert.AreEqual(0, spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).Count());
        }

        [TestMethod]
        public void TestaAndSpecificationPreCondition_Fail()
        {
            #region ' left precondition false '
            {
                var spec = WithPreconditionFailureSpec(preconditionErrorMessage: LeftErrorMessage) 
                    && PreconditionedSatisfiedSpec();

                Assert.IsFalse(spec.SatisfiedBy(_candidate));

                var razoes = spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).ToList();
                Assert.AreEqual(1, razoes.Count);
                Assert.AreEqual(LeftErrorMessage, razoes.Single());
            
                Assert.AreEqual(0, spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate).Count());
            }
            #endregion

            #region ' right precondition false '
            {
                var spec = PreconditionedSatisfiedSpec() 
                    && WithPreconditionFailureSpec(preconditionErrorMessage: RightErrorMessage);

                Assert.IsFalse(spec.SatisfiedBy(_candidate));

                var razoes = spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).ToList();
                Assert.AreEqual(1, razoes.Count);
                Assert.AreEqual(RightErrorMessage, razoes.Single());

                Assert.AreEqual(0, spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate).Count());
            }
            #endregion

            #region ' both preconditions unsatisfaction '
            {
                var spec = WithPreconditionFailureSpec(preconditionErrorMessage: LeftErrorMessage) 
                    && WithPreconditionFailureSpec(preconditionErrorMessage: RightErrorMessage);

                Assert.IsFalse(spec.SatisfiedBy(_candidate));

                var razoes = spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).ToList();
                Assert.AreEqual(1, razoes.Count);   
                Assert.AreEqual(LeftErrorMessage, razoes.Single()); // deve exibir apenas a mensagem de left.
            }
            #endregion
        }

        #endregion

        #region ' Not Direct 

        [TestMethod]
        public void TestaNotDirectSpecificationPreCondition_Sucesso()
        {
            const string naoAtendeuAPreCondicaoErrorMessage = @"Precondition message";
            var spec = !WithPreconditionFailureSpec(preconditionErrorMessage: naoAtendeuAPreCondicaoErrorMessage);

            Assert.IsTrue(spec.SatisfiedBy(_candidate));

            var razoes = spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate).ToList();
            Assert.AreEqual(1, razoes.Count);
            Assert.AreEqual(naoAtendeuAPreCondicaoErrorMessage, razoes.Single()); // ... pq não atendeu a pré-condição.

            Assert.AreEqual(0, spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).Count());
        }

        [TestMethod]
        public void TestaNotDirectSpecificationPreCondition_Fail()
        {
            const string preconditionSuccessMessage = @"Spec Success message";
            var spec = !PreconditionedSatisfiedSpec(preconditionSuccessMessage: preconditionSuccessMessage);

            Assert.IsFalse(spec.SatisfiedBy(_candidate));

            var razoes = spec.GetRazoesPelasQuaisNaoFoiSatisfeita(_candidate).ToList();
            Assert.AreEqual(1, razoes.Count);
            Assert.AreNotEqual(preconditionSuccessMessage, razoes.Single()); // não deve ser a mensagem de sucesso da precondição.

            Assert.AreEqual(0, spec.GetRazoesPelasQuaisFoiSatisfeita(_candidate).Count());
        }

        #endregion

        #region ' Não deve avaliar body se precondição não for satisfeita '

        
        #endregion

    }
}

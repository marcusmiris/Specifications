using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miris.Specifications.ComMensagens;
using Miris.Specifications.Tests.Stub;

namespace Miris.Specifications.Tests.ComMensagens
{
    [TestClass]
    public class SpecificationComMensagensCompositionTests
    {
        private SpecificationComMensagens<ClienteStub> _nomeContemMarcusSpec;
        private SpecificationComMensagens<ClienteStub> _nomeContemViniciusSpec;
        private readonly ClienteStub _marcus = new ClienteStub() { Nome = "Marcus" };
        private readonly ClienteStub _vinicius = new ClienteStub() { Nome = "Vinicius" };
        private const string NomeContemMarcusMessage = @"Nome contem 'Marcus'";
        private const string NomeContemViniciusMessage = @"Nome contém 'Vinícius'";
        private const string NomeNaoContemMarcusMessage = @"Nome não contém 'Marcus'";
        private const string NomeNaoContemViniciusMessage = @"Nome não contem 'Vinícius'";

        [TestInitialize]
        public void TestInitialize()
        {
            bool ÉoMarcus(ClienteStub a) => a.Nome.Contains("Marcus");
            bool ÉoVinícius(ClienteStub a) => a.Nome.Contains("Vinicius");

            _nomeContemMarcusSpec = new DirectSpecificationComMensagens<ClienteStub>(ÉoMarcus, NomeNaoContemMarcusMessage, NomeContemMarcusMessage);
            _nomeContemViniciusSpec = new DirectSpecificationComMensagens<ClienteStub>(ÉoVinícius, NomeNaoContemViniciusMessage, NomeContemViniciusMessage);
        }

        [TestMethod]
        public void TestaDirectSpecification_Sucesso()
        {
            Assert.IsTrue(_nomeContemMarcusSpec.SatisfiedBy(_marcus));
            Assert.IsFalse(_nomeContemViniciusSpec.SatisfiedBy(_marcus));
        }

        #region ' Or '

        [TestMethod]
        public void TestaOrSpecification_Sucesso()
        {
            ISpecificationComMensagens<ClienteStub> orSpecification = _nomeContemMarcusSpec || _nomeContemViniciusSpec;

            #region ' atende LEFT '
            {
                Assert.IsTrue(orSpecification.SatisfiedBy(_marcus));

                // razões não atendeu
                Assert.AreEqual(0, orSpecification.GetRazoesPelasQuaisNaoFoiSatisfeita(_marcus).Count());

                // razões atendeu
                Assert.AreEqual(1, orSpecification.GetRazoesPelasQuaisFoiSatisfeita(_marcus).Count());
                Assert.AreEqual(NomeContemMarcusMessage, orSpecification.GetRazoesPelasQuaisFoiSatisfeita(_marcus).Single());
            }
            #endregion

            #region ' atende RIGHT '
            {
                Assert.IsTrue(orSpecification.SatisfiedBy(_vinicius));

                // razões não atendeu
                Assert.AreEqual(0, orSpecification.GetRazoesPelasQuaisNaoFoiSatisfeita(_vinicius).Count());

                // razões atendeu
                Assert.AreEqual(1, orSpecification.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Count());
                Assert.AreEqual(NomeContemViniciusMessage, orSpecification.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Single());
            }
            #endregion

            #region ' atende AMBOS '
            {
                var candidate = new ClienteStub {Nome = "Marcus Vinicius"};

                // verify 
                Assert.IsTrue(orSpecification.SatisfiedBy(candidate));

                // razões não atendeu
                Assert.AreEqual(0, orSpecification.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate).Count());

                // razões atendeu
                Assert.AreEqual(2, orSpecification.GetRazoesPelasQuaisFoiSatisfeita(candidate).Count());
                var razoes = orSpecification.GetRazoesPelasQuaisFoiSatisfeita(candidate).ToList();
                Assert.IsTrue(razoes.Contains(NomeContemMarcusMessage) && razoes.Contains(NomeContemViniciusMessage));
            }
            #endregion

        }

        [TestMethod]
        public void TestaOrSpecification_Fail()
        {
            ISpecificationComMensagens<ClienteStub> orSpecification = _nomeContemMarcusSpec || _nomeContemViniciusSpec;

            var candidate = new ClienteStub() {Nome = "Juan"};

            Assert.IsFalse(orSpecification.SatisfiedBy(candidate));

            var razoes = orSpecification.GetRazoesPelasQuaisNaoFoiSatisfeita(candidate).ToList();
            Assert.AreEqual(2, razoes.Count);
            Assert.IsTrue(razoes.Contains(NomeNaoContemMarcusMessage));
            Assert.IsTrue(razoes.Contains(NomeNaoContemViniciusMessage));

            Assert.AreEqual(0, orSpecification.GetRazoesPelasQuaisFoiSatisfeita(candidate).Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaOrSpecification_LeftArgumentNullException()
        {
            ISpecificationComMensagens<ClienteStub> orSpecification = new OrSpecificationComMensagens<ClienteStub>(null, _nomeContemViniciusSpec);

            orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaOrSpecification_RightArgumentNullException()
        {
            ISpecificationComMensagens<ClienteStub> orSpecification = new OrSpecificationComMensagens<ClienteStub>(_nomeContemMarcusSpec, null);

            orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.Fail();
        }

        #endregion

        #region ' And '

        [TestMethod]
        public void TestaAndSpecification_Sucesso()
        {
            ISpecificationComMensagens<ClienteStub> ehMarcusVinicius
                = _nomeContemMarcusSpec && _nomeContemViniciusSpec;

            var marcusVinicius = new ClienteStub() {Nome = "Marcus Vinicius"};

            Assert.IsTrue(ehMarcusVinicius.SatisfiedBy(marcusVinicius));

            // razões atendeu
            var razoes = ehMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(marcusVinicius).ToList();
            Assert.AreEqual(2, razoes.Count);
            Assert.IsTrue(razoes.Contains(NomeContemMarcusMessage));
            Assert.IsTrue(razoes.Contains(NomeContemViniciusMessage));

            // razoes não atendeu
            Assert.AreEqual(0, ehMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(marcusVinicius).Count());
        }

        [TestMethod]
        public void TestaAndSpecification_Fail()
        {
            ISpecificationComMensagens<ClienteStub> ehMarcusVinicius
                = _nomeContemMarcusSpec && _nomeContemViniciusSpec;

            #region ' Não atende LEFT '
            {
                Assert.IsFalse(ehMarcusVinicius.SatisfiedBy(_vinicius));

                // razoes não atendeu
                var razoes = ehMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_vinicius).ToList();
                Assert.AreEqual(1, razoes.Count);
                Assert.IsTrue(razoes.Contains(NomeNaoContemMarcusMessage));

                Assert.AreEqual(0, ehMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Count());
            }
            #endregion

            #region ' Não atende RIGHT '
            {
                Assert.IsFalse(ehMarcusVinicius.SatisfiedBy(_marcus));
                var razoes = ehMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_marcus).ToList();
                Assert.AreEqual(1, razoes.Count);
                Assert.IsTrue(razoes.Contains(NomeNaoContemViniciusMessage));

                Assert.AreEqual(0, ehMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(_marcus).Count());
            }
            #endregion

            #region ' Não atende a ambas '
            {
                var juan = new ClienteStub { Nome = "Juan" };

                Assert.IsFalse(ehMarcusVinicius.SatisfiedBy(juan));

                var razoes = ehMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(juan).ToList();
                Assert.AreEqual(2, razoes.Count);
                Assert.IsTrue(razoes.Contains(NomeNaoContemMarcusMessage));
                Assert.IsTrue(razoes.Contains(NomeNaoContemViniciusMessage));

                Assert.AreEqual(0, ehMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(juan).Count());
            }
            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaAndSpecification_LeftArgumentNullException()
        {
            ISpecificationComMensagens<ClienteStub> andSpecification = new AndSpecificationComMensagens<ClienteStub>(null, _nomeContemViniciusSpec);

            andSpecification.SatisfiedBy(_marcus);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaAndSpecification_RightArgumentNullException()
        {
            ISpecificationComMensagens<ClienteStub> orSpecification = new AndSpecificationComMensagens<ClienteStub>(_nomeContemMarcusSpec, null);

            orSpecification.SatisfiedBy(_marcus);

            Assert.Fail();
        }

        #endregion

    }
}

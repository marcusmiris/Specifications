using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miris.Specifications.ComMensagens;
using Miris.Specifications.Tests.Stub;

namespace Miris.Specifications.Tests.ComMensagens
{
    [TestClass]
    public class SpecificationComMensagesNegationTests
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
            _nomeContemMarcusSpec = new DirectSpecificationComMensagens<ClienteStub>(
                a => a.Nome.Contains(@"Marcus"), 
                NomeNaoContemMarcusMessage,
                NomeContemMarcusMessage);

            _nomeContemViniciusSpec = new DirectSpecificationComMensagens<ClienteStub>(
                a => a.Nome.Contains(@"Vinicius"),
                NomeNaoContemViniciusMessage,
                NomeContemViniciusMessage);
        }

        #region ' !Direct '
        [TestMethod]
        public void TestaNotDirectSpecification_Sucesso()
        {
            ISpecificationComMensagens<ClienteStub> naoContemMarcus = !_nomeContemMarcusSpec;

            Assert.IsTrue(naoContemMarcus.SatisfiedBy(_vinicius));
            Assert.AreEqual(1, naoContemMarcus.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Count());
            Assert.AreEqual(NomeNaoContemMarcusMessage, naoContemMarcus.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Single());

            Assert.AreEqual(0, naoContemMarcus.GetRazoesPelasQuaisNaoFoiSatisfeita(_vinicius).Count());
        }

        [TestMethod]
        public void TestaNotDirectSpecification_Fail()
        {
            ISpecificationComMensagens<ClienteStub> naoContemMarcus = !_nomeContemMarcusSpec;

            Assert.IsFalse(naoContemMarcus.SatisfiedBy(_marcus));

            Assert.AreEqual(0, naoContemMarcus.GetRazoesPelasQuaisFoiSatisfeita(_marcus).Count());
            Assert.AreEqual(1, naoContemMarcus.GetRazoesPelasQuaisNaoFoiSatisfeita(_marcus).Count());
            Assert.AreEqual(NomeContemMarcusMessage, naoContemMarcus.GetRazoesPelasQuaisNaoFoiSatisfeita(_marcus).Single());

        }

        #endregion

        #region ' !And '

        [TestMethod]
        public void TestaNAndSpecification_Sucesso()
        {
            ISpecificationComMensagens<ClienteStub> naoMarcusVinicius = !(_nomeContemMarcusSpec && _nomeContemViniciusSpec);

            #region ' Juan '
            {
                
                var juan = new ClienteStub() { Nome = "Juan" };

                Assert.IsTrue(naoMarcusVinicius.SatisfiedBy(juan));

                {
                    var razoes = naoMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(juan).ToList();
                    Assert.AreEqual(2, razoes.Count);
                    Assert.IsTrue(razoes.Contains(NomeNaoContemMarcusMessage));
                    Assert.IsTrue(razoes.Contains(NomeNaoContemViniciusMessage));
                }
            

                Assert.AreEqual(0, naoMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_vinicius).Count());
            }
            #endregion

            #region ' Marcus '
            {
                Assert.IsTrue(naoMarcusVinicius.SatisfiedBy(_marcus));

                // razões não atendeu
                Assert.AreEqual(1, naoMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(_marcus).Count());
                Assert.AreEqual(NomeNaoContemViniciusMessage, naoMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(_marcus).Single());

                // razões atendeu
                Assert.AreEqual(0, naoMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_marcus).Count());
            }
            #endregion

            #region ' Vinicius '
            {
                Assert.IsTrue(naoMarcusVinicius.SatisfiedBy(_vinicius));

                // razões não atendeu
                Assert.AreEqual(1, naoMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Count());
                Assert.AreEqual(NomeNaoContemMarcusMessage, naoMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Single());

                // razões atendeu
                Assert.AreEqual(0, naoMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_vinicius).Count());
            }
            #endregion

        }

        [TestMethod]
        public void TestaNAndSpecification_Fail()
        {
            SpecificationComMensagens<ClienteStub> marcusViniciusSpec = _nomeContemMarcusSpec && _nomeContemViniciusSpec;
            ISpecificationComMensagens<ClienteStub> naoMarcusVinicius = !marcusViniciusSpec;

            #region ' Vinicius '
            {
                var marcusVinicius = new ClienteStub { Nome = "Marcus Vinicius"};

                Assert.IsFalse(naoMarcusVinicius.SatisfiedBy(marcusVinicius));

                // razões pelas quais falhou
                {
                    var razoes = naoMarcusVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(marcusVinicius).ToList();
                    Assert.AreEqual(2, razoes.Count);
                    Assert.IsTrue(razoes.Contains(NomeContemMarcusMessage));
                    Assert.IsTrue(razoes.Contains(NomeContemViniciusMessage));
                }


                // razões pela qual atendeu
                Assert.AreEqual(0, naoMarcusVinicius.GetRazoesPelasQuaisFoiSatisfeita(marcusVinicius).Count());
                
            }
            #endregion
        }

        #endregion

        #region ' !Or '

        [TestMethod]
        public void TestaNOrSpecification_Sucess()
        {
            SpecificationComMensagens<ClienteStub> marcusOuVinicius = _nomeContemMarcusSpec || _nomeContemViniciusSpec;
            ISpecificationComMensagens<ClienteStub> nemMarcusNemVinicius = !marcusOuVinicius;

            #region ' Juan '
            {

                var juan = new ClienteStub() { Nome = "Juan" };

                Assert.IsTrue(nemMarcusNemVinicius.SatisfiedBy(juan));

                {
                    var razoes = nemMarcusNemVinicius.GetRazoesPelasQuaisFoiSatisfeita(juan).ToList();
                    Assert.AreEqual(2, razoes.Count);
                    Assert.IsTrue(razoes.Contains(NomeNaoContemMarcusMessage));
                    Assert.IsTrue(razoes.Contains(NomeNaoContemViniciusMessage));
                }


                Assert.AreEqual(0, nemMarcusNemVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(juan).Count());
            }
            #endregion

        }


        [TestMethod]
        public void TestaNOrSpecification_Fail()
        {
            SpecificationComMensagens<ClienteStub> marcusOuVinicius = _nomeContemMarcusSpec || _nomeContemViniciusSpec;
            ISpecificationComMensagens<ClienteStub> nemMarcusNemVinicius = !marcusOuVinicius;

            
            #region ' Marcus '
            {
                Assert.IsFalse(nemMarcusNemVinicius.SatisfiedBy(_marcus));

                // razões não atendeu
                Assert.AreEqual(1, nemMarcusNemVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_marcus).Count());
                Assert.AreEqual(NomeContemMarcusMessage, nemMarcusNemVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_marcus).Single());

                // razões atendeu
                Assert.AreEqual(0, nemMarcusNemVinicius.GetRazoesPelasQuaisFoiSatisfeita(_marcus).Count());
            }
            #endregion

            #region ' Vinicius '
            {
                Assert.IsFalse(nemMarcusNemVinicius.SatisfiedBy(_vinicius));

                // razões não atendeu
                Assert.AreEqual(1, nemMarcusNemVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_vinicius).Count());
                Assert.AreEqual(NomeContemViniciusMessage, nemMarcusNemVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(_vinicius).Single());

                // razões atendeu
                Assert.AreEqual(0, nemMarcusNemVinicius.GetRazoesPelasQuaisFoiSatisfeita(_vinicius).Count());
            }
            #endregion

            #region ' Marcus Vinícius '
            {
                var marcusVinicius = new ClienteStub { Nome = "Marcus Vinicius" };

                Assert.IsFalse(nemMarcusNemVinicius.SatisfiedBy(marcusVinicius));

                // razões pelas quais falhou
                {
                    var razoes = nemMarcusNemVinicius.GetRazoesPelasQuaisNaoFoiSatisfeita(marcusVinicius).ToList();
                    Assert.AreEqual(2, razoes.Count);
                    Assert.IsTrue(razoes.Contains(NomeContemMarcusMessage));
                    Assert.IsTrue(razoes.Contains(NomeContemViniciusMessage));
                }


                // razões pela qual atendeu
                Assert.AreEqual(0, nemMarcusNemVinicius.GetRazoesPelasQuaisFoiSatisfeita(marcusVinicius).Count());

            }
            #endregion

        }

        #endregion
    }
}

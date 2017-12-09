using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miris.Specifications.ComMensagens;

namespace Miris.Specifications.Tests.ComMensagens
{
    [TestClass]
    public class SpecificationComMensagensContravarianceTests
    {
        [TestMethod]
        public void InterfaceContravariance()
        {
            var naoNulo = new DirectSpecificationComMensagens<object>(obj => obj != null, string.Empty, string.Empty);
            ISpecificationComMensagens<string> stringSpec = naoNulo;
            
            Assert.IsTrue(stringSpec.SatisfiedBy("Nao nulo"));
            Assert.IsFalse(stringSpec.SatisfiedBy(null));
        }

        [TestMethod]
        public void AndContravariance()
        {
            var naoNulo = new DirectSpecificationComMensagens<object>(obj => obj != null, string.Empty, string.Empty);
            var contemMarcus = new DirectSpecificationComMensagens<string>(s => s.Contains(@"Marcus"), string.Empty, string.Empty);

            var strSpec = naoNulo & contemMarcus;

            Assert.IsTrue(strSpec.SatisfiedBy("Olá Marcus!"));
            Assert.IsFalse(strSpec.SatisfiedBy("Olá Fulano!"));
        }

        [TestMethod]
        public void OrContravariance()
        {
            var nulo = new DirectSpecificationComMensagens<object>(obj => obj == null, string.Empty, string.Empty);
            var contemMarcus = new DirectSpecificationComMensagens<string>(s => s.Contains(@"Marcus"), string.Empty, string.Empty);

            var strSpec = nulo | contemMarcus;

            Assert.IsTrue(strSpec.SatisfiedBy("Olá Marcus!"));
            Assert.IsTrue(strSpec.SatisfiedBy(null));
            Assert.IsFalse(strSpec.SatisfiedBy("Olá Fulano!"));
        }

        [TestMethod]
        public void NotContravariance()
        {
            var nulo = new DirectSpecificationComMensagens<object>(obj => obj == null, string.Empty, string.Empty);
            ISpecificationComMensagens<string> strSpec = !nulo;

            Assert.IsTrue(strSpec.SatisfiedBy("Olá Marcus!"));
            Assert.IsFalse(strSpec.SatisfiedBy(null));
        }

    }
}

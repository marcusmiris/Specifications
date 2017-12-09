using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miris.Specifications.Tests
{
    


    [TestClass]
    public class EnumSpecification
    {

        enum Enumerador
        {
            Valor1 = 1,
            Valor2 = 2,
            Valor3 = 3,
        }

        [TestMethod]
        public void EnumSpecificationUnitTest()
        {
            
            var maiorQueDois = new DirectSpecification<Enumerador>(x => (int)x > 2);

            Assert.IsFalse(maiorQueDois.SatisfiedBy(Enumerador.Valor1));
            Assert.IsFalse(maiorQueDois.SatisfiedBy(Enumerador.Valor2));
            Assert.IsTrue(maiorQueDois.SatisfiedBy(Enumerador.Valor3));
        }
    }
}

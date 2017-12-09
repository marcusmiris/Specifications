using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miris.Specifications.Tests.Stub;

namespace Miris.Specifications.Tests
{
    [TestClass]
    public class DefaultSpecificationTest
    {
        private ISpecification<ClienteStub> specificationLeft;
        private ISpecification<ClienteStub> specificationRight;
        private ClienteStub clienteMarcus = new ClienteStub() { Nome = "Marcus" };
        private ClienteStub clienteVinicius = new ClienteStub() { Nome = "Vinicius" };

        [TestInitialize]
        public void TestInitialize()
        {
            Func<ClienteStub, bool> contemMarcus = a => a.Nome.Contains("Marcus");
            Func<ClienteStub, bool> contemVinicius = a => a.Nome.Contains("Vinicius");

            specificationLeft = new DirectSpecification<ClienteStub>(contemMarcus);
            specificationRight = new DirectSpecification<ClienteStub>(contemVinicius);
        }

        [TestMethod]
        public void TestaDirectSpecification_Sucesso()
        {
            bool result = specificationLeft.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.IsTrue(result);
        }



        [TestMethod]
        public void TestaOrSpecification_Sucesso()
        {
            ISpecification<ClienteStub> orSpecification = new OrSpecification<ClienteStub>(specificationLeft, specificationRight);

            bool result = orSpecification.SatisfiedBy(clienteMarcus);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestaOrSpecification_Fail()
        {
            ISpecification<ClienteStub> orSpecification = new OrSpecification<ClienteStub>(specificationLeft, specificationRight);

            bool result = orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Juan" });

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaOrSpecification_LeftArgumentNullException()
        {
            ISpecification<ClienteStub> orSpecification = new OrSpecification<ClienteStub>(null, specificationRight);

            orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaOrSpecification_RightArgumentNullException()
        {
            ISpecification<ClienteStub> orSpecification = new OrSpecification<ClienteStub>(specificationLeft, null);

            orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.Fail();
        }

        [TestMethod]
        public void TestaAndSpecification_Sucesso()
        {
            ISpecification<ClienteStub> orSpecification = new AndSpecification<ClienteStub>(specificationLeft, specificationRight);

            bool result = orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus Vinicius" });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestaAndSpecification_Fail()
        {
            ISpecification<ClienteStub> andSpecification = new AndSpecification<ClienteStub>(specificationLeft, specificationRight);

            bool result = andSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaAndSpecification_LeftArgumentNullException()
        {
            ISpecification<ClienteStub> orSpecification = new AndSpecification<ClienteStub>(null, specificationRight);

            orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestaAndSpecification_RightArgumentNullException()
        {
            ISpecification<ClienteStub> orSpecification = new AndSpecification<ClienteStub>(specificationLeft, null);

            orSpecification.SatisfiedBy(new ClienteStub() { Nome = "Marcus" });

            Assert.Fail();
        }

    }
}

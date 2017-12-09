using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miris.Specifications.Tests
{
    [TestClass]
    public class VarianceTests
    {
        [TestMethod]
        public void IsContravariante()
        {
            var isContravariante = typeof (ISpecification<IList>)
                .IsAssignableFrom(typeof (ISpecification<IEnumerable>));

            Assert.IsTrue(isContravariante);
        }

    } 
}

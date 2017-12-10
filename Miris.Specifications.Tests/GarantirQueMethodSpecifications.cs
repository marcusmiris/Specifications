using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miris.Specifications.ComMensagens;

namespace Miris.Specifications.Tests
{
    [TestClass]
    public class GarantirQueMethodSpecifications
    {
        [TestMethod]
        public void GarantirTipoPrimitivoSpecificationTest()
        {
            const decimal candidate = 3m;

            try
            {
                // abaixo são chamados os overloads conhecidos do método.

                candidate.GarantirQue(
                    new DirectSpecification<decimal>(d => d > 0),
                    @"Valor não é maior que zero");

                candidate.GarantirQue(d => d > 0, @"Valor não é maior que zero");
                candidate.GarantirQue(d => d > 0, () => @"Valor não é maior que zero");
                candidate.GarantirQue(new DirectSpecificationComMensagens<decimal>(
                    d => d > 0,
                    @"Valor não é maior que zero",
                    @"Valor é maior que zero"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

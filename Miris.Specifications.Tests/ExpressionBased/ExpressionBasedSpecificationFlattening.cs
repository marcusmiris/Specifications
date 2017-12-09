using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miris.Specifications.ExpressionBasedSpecifications;
using Miris.Specifications.Internals.LinqExpressions.Comparision;

namespace Miris.Specifications.Tests.ExpressionBased
{
    public class Veiculo
    {
        public int QtdRodas { get; set; }
    }

    public class Proprietario
    {
        public IEnumerable<Veiculo> Veiculos { get; set; }
    }

    [TestClass]
    public class ExpressionBasedSpecificationFlattening
    {
        /// <summary>
        ///     Garante que a specification está reduzindo a expressão de entrada 
        ///     usando <see cref="SpecificationExpressionFlatter{TCandidateItem}"/>.
        /// </summary>
        [TestMethod]
        public void SpecificationFlatsExpression()
        {
            var ehCarro = new ExpressionBasedSpecification<Veiculo>(p => p.QtdRodas == 4);

            #region ' spec #1 '
            {
                var specification = new ExpressionBasedSpecification<Proprietario>(p => p.Veiculos.Any(ehCarro));

                var areEquals = ExpressionEqualityComparer.Instance.Equals(
                    l => l.Veiculos.Any(p => p.QtdRodas == 4),
                    specification.Expression);
                Assert.IsTrue(areEquals, "A Specification não reduziu a expressão de entrada.");

            }
            #endregion

            #region ' spec of IEnumerable '
            {
                var specification = new ExpressionBasedSpecification<IEnumerable<Veiculo>>(l => l.Any(ehCarro));

                var areEquals = ExpressionEqualityComparer.Instance.Equals(
                    l => l.Any(p => p.QtdRodas == 4),
                    specification.Expression);
                Assert.IsTrue(areEquals, "A Specification não reduziu a expressão de entrada.");
            }
            #endregion
        }

        /// <summary>
        ///     Testes de possíveis reduções de specifications.
        /// </summary>
        [TestMethod]
        public void SpecificationExpressionFlatterTests()
        {
            var naoNulo = new ExpressionBasedSpecification<object>(o => o != null);

            // Any
            AreEquals(l => l.Any(o => o != null), SpecificationExpressionFlatter<IEnumerable<object>>.Flat(l => l.Any(naoNulo)));
            AreEquals(l => l.Any(o => o != null), SpecificationExpressionFlatter<IQueryable<object>>.Flat(l => l.Any(naoNulo)));

            // Where
            AreEquals(l => l.Where(o => o != null), SpecificationExpressionFlatter<IEnumerable<object>>.Flat(l => l.Where(naoNulo)));
            AreEquals(l => l.Where(o => o != null), SpecificationExpressionFlatter<IQueryable<object>>.Flat(l => l.Where(naoNulo)));
        }

        /// <summary>
        ///     Helper method que valida a expressão da <see cref="ExpressionBasedSpecification{T}"/>
        /// </summary>
        private static void AreEquals<TDelegate>(
            Expression<TDelegate> expected,
            Expression<TDelegate> actual)
        {
            Assert.IsTrue(
                ExpressionEqualityComparer.Instance.Equals(
                    expected,
                    actual),
                $"As expressões não são iguais para {(expected.Body as MethodCallExpression)?.Method.Name}/{expected.Parameters.First().Type.Name}");
        }
    }
}

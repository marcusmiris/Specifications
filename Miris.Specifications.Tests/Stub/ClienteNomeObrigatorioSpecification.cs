namespace Miris.Specifications.Tests.Stub
{
    public class ClienteNomeObrigatorioSpecification : Specification<ClienteStub>
    {
        public override bool SatisfiedBy(ClienteStub candidate)
        {
            return !string.IsNullOrWhiteSpace(candidate.Nome);
        }
    }
}

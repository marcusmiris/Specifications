namespace Miris.Specifications
{
    public interface ISpecification<in T>
    {
        bool SatisfiedBy(T candidate);
    }
}
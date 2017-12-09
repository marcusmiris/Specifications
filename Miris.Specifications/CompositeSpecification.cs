namespace Miris.Specifications
{
    public abstract class CompositeSpecification<T> : Specification<T>
    {
        public abstract ISpecification<T> LeftSpecification { get; }

        public abstract ISpecification<T> RightSpecification { get; }
    }
}
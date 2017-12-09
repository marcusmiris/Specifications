namespace Miris.Specifications
{
    public class TrueSpecification<T> : Specification<T>
    {
        #region Specification overrides

        public override bool SatisfiedBy(T candidate)
        {
            return true;
        }

        #endregion
    }
}
namespace Miris.Specifications.Internals.Extensions
{
    internal static class ObjectExtensions
    {
        public static T Se<T>(
            this T resultado,
            bool condicao,
            T elseValue = default(T))
        {
            return condicao ? resultado : elseValue;
        }
    }
}
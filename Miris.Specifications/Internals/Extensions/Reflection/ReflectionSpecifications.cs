using System;
using System.Reflection;

namespace Miris.Specifications.Internals.Extensions.Reflection
{
    public static class ReflectionSpecifications
    {
        /// <summary>
        ///     Satisfeita para membros estáticos.
        /// </summary>
        public static ISpecification<MemberInfo> Static =>
            new DirectSpecification<MemberInfo>(member =>
            {
                var p = member as PropertyInfo;
                if (p != null) return ((p.GetGetMethod() ?? p.GetSetMethod())?.IsStatic).GetValueOrDefault();

                var m = member as MethodBase;
                if (m != null) return m.IsStatic;

                throw new NotImplementedException($"Método não implementado para membros do tipo {member.GetType()}.");
            });

        /// <summary>
        ///     Satisfeita para propriedade não-estáticas.
        /// </summary>
        public static ISpecification<MemberInfo> NonStatic => new NotSpecification<MemberInfo>(Static);

        /// <summary>
        ///     Satisfeita para propriedade não-estáticas.
        /// </summary>
        public static ISpecification<PropertyInfo> OfType<T>()
        {
            return new DirectSpecification<PropertyInfo>(p => p.PropertyType == typeof(T));
        }

        public static ISpecification<MemberInfo> Named(string name)
        {
            return new DirectSpecification<MemberInfo>(p => p.Name.Equals(name));
        }

        /// <summary>
        ///     Valida a quantidade de parãmetros do método.
        /// </summary>
        /// <param name="n">
        ///     Quantidade de parâmetros esperada.
        /// </param>
        public static ISpecification<MethodBase> WithNParameters(int n)
        {
            return new DirectSpecification<MethodBase>(m => m.GetParameters().Length == n);
        }

        public static ISpecification<MethodBase> ParameterOfType(uint parameterIndex, Type type)
        {
            return new DirectSpecification<MethodBase>(m =>
            {
                var parameters = m.GetParameters();
                if (parameters.Length < parameterIndex + 1) return false;

                var paramType = parameters[parameterIndex].ParameterType;
                return paramType == type
                       || type.IsGenericTypeDefinition && paramType.IsGenericType &&
                       !paramType.IsGenericTypeDefinition && paramType.GetGenericTypeDefinition() == type
                    ;
            });
        }

        public static ISpecification<MethodBase> FirstParameterOfType(Type type)
        {
            return ParameterOfType(0, type);
        }

        public static ISpecification<MethodBase> SecondParameterOfType(Type type)
        {
            return ParameterOfType(1, type);
        }
    }
}
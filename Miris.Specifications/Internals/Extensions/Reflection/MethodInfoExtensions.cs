using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Miris.Specifications.Internals.Extensions.Reflection
{
    public static class MethodInfoExtensions
    {
        public static MethodInfo GetMethod(
            this Type type,
            params ISpecification<MethodInfo>[] specifications)
        {
            return GetMethods(type, specifications).Single();
        }


        public static IEnumerable<MethodInfo> GetMethods(
            this Type type,
            params ISpecification<MethodInfo>[] specifications)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetMethods()
                .Where(prop => prop.SatisfiesAll(specifications));
        }
    }
}
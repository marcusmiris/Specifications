using System;

namespace Miris.Specifications.Internals.Extensions.Reflection
{
    public class InterfaceType

    {
        #region ' constructor '

        public InterfaceType(Type wrappedType)
        {
            if (wrappedType == null) throw new ArgumentNullException(nameof(wrappedType));

            if (!wrappedType.IsInterface)
                throw new ArgumentException(@"Type isn't a interface.", nameof(wrappedType));

            Type = wrappedType;
        }

        #endregion

        public Type Type { get; }

        #region ' operators '

        public static implicit operator InterfaceType(Type type)
        {
            return new InterfaceType(type);
        }

        public static implicit operator Type(InterfaceType @interface)
        {
            return @interface.Type;
        }

        #endregion
    }
}
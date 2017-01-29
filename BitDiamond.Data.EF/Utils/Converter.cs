using System;
using System.Linq;
using System.Collections.Generic;
using static Axis.Luna.Extensions.ObjectExtensions;
using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Data.EF.Utils
{
    public class DomainConverter
    {
        private Registry _registry = new Registry();

        public DomainConverter(Action<Registry> conversionRegistration)
        {
            conversionRegistration.Invoke(_registry);
        }

        public Y Convert<X, Y>(X obj, ConversionContext context = null)
            => (Y)(context ?? (context = new ConversionContext(this))).GetOrAdd(obj, _obj =>
            {
                var _dynamic = _registry.TypeRegistry[new ConversionVector { From = typeof(X), To = typeof(Y) }];
                var converter = (Func<X, ConversionContext, Y>)_dynamic;
                return converter.Invoke(obj, context);
            });

        public object Convert<X>(X obj, ConversionContext context = null)
            => (context ?? (context = new ConversionContext(this))).GetOrAdd(obj, _obj =>
            {
                var tx = typeof(X);
                var _dynamic = _registry.TypeRegistry.First(_kvp => _kvp.Key.From == tx).Value;
                return _dynamic.Invoke(obj, context);
            });




        public class Registry
        {
            internal readonly Dictionary<ConversionVector, dynamic> TypeRegistry = new Dictionary<ConversionVector, dynamic>();

            public Registry Register<Domain, Entity>(Func<Domain, ConversionContext, Entity> toEntity,
                                                     Func<Entity, ConversionContext, Domain> toDomain)
            {
                var etype = typeof(Entity);
                var dtype = typeof(Domain);

                if (toEntity == null ||
                    toDomain == null ||
                    etype == dtype)
                    throw new Exception("invalid conversion");

                TypeRegistry.Add(new ConversionVector { From = etype, To = dtype }, toDomain);
                TypeRegistry.Add(new ConversionVector { From = dtype, To = etype}, toEntity);

                return this;
            }

            internal Registry()
            { }
        }

        internal class ConversionVector
        {
            internal Type From { get; set; }
            internal Type To { get; set; }

            public override int GetHashCode() => ValueHash(new object[] { From.ThrowIfNull(), To.ThrowIfNull() });
            public override bool Equals(object obj)
            {
                var other = obj.As<ConversionVector>();
                return other != null &&
                       other.From == From &&
                       other.To == To &&
                       other.GetHashCode() == GetHashCode();
            }
        }
    }
}

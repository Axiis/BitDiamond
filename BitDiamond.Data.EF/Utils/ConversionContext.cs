using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;

namespace BitDiamond.Data.EF.Utils
{
    public class ConversionContext
    {
        private Dictionary<object, object> _cache = new Dictionary<object, object>();

        public DomainConverter Converter { get; private set; }


        public bool IsCached(object from) => _cache.ContainsKey(from);

        internal ConversionContext CacheValue(object from, object to) => this.UsingValue(@this => _cache[from] = to);

        internal object GetCachedValue(object from) => _cache[from];

        internal object GetOrAdd(object from, Func<object, object> valueGenerator) => _cache.GetOrAdd(from, _from => valueGenerator(_from));

        internal ConversionContext(DomainConverter converter)
        {
            Converter = converter;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TarjimonOfficeUZ.Core.Translation
{
    internal sealed class ReverseTranslationCache
    {
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        public bool TryGet(string key, out string value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Set(string key, string value)
        {
            _cache[key] = value;
        }
    }
}

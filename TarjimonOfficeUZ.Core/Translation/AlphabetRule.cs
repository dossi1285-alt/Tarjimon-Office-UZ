using System;
using System.Collections.Generic;
using System.Text;

namespace TarjimonOfficeUZ.Core.Translation
{
    /// <summary>
    /// Bitta transliteratsiya qoidasini ifodalaydi.
    /// </summary>
    public sealed class AlphabetRule
    {
        /// <summary>
        /// Manba yozuv.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Natija yozuv.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Yangi qoida yaratadi.
        /// </summary>
        /// <param name="source">Manba matn.</param>
        /// <param name="target">Natija matn.</param>
        public AlphabetRule(string source, string target)
        {
            Source = source;
            Target = target;
        }
    }
}

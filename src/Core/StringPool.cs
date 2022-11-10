using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    /// <summary>
    /// <para>The string pool starts with the STR magic and contains 2-byte aligned strings. The magic is not checked at all since strings are directly referred to using pointers.</para>
    /// <para>All strings in the pool are Pascal strings, i.e. length-prefixed strings. The length is an unsigned 16 bit integer. Strings are stored in reverse order of their binary representation.</para>
    /// </summary>
    public class StringPool
    {
        internal string Magic = "SRC ";
    }
}

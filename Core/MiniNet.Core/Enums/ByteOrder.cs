using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    /// <summary>
    /// This enum-type specifies either big-endian (Big) or little-endian (Little),
    /// which indicate whether the most-significant bits are placed first or last in memory.
    /// </summary>
    public enum ByteOrder
    {
        /// <summary>
        /// Most-significant bits are placed first in memory.
        /// </summary>
        BigEndian,

        /// <summary>
        /// Most-significant bits are placed last in memory.
        /// </summary>
        LittleEndian
    }
}

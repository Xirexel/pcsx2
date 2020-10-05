using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSX2Emul.Util
{
    /// <summary>
    /// Specifies the number of bits in the bit field structure
    /// Maximum number of bits are 64
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class BitFieldNumberOfBitsAttribute : Attribute
    {
        /// <summary>
        /// Initializes new instance of BitFieldNumberOfBitsAttribute with the specified number of bits
        /// </summary>
        /// <param name="bitCount">The number of bits the bit field will contain (Max 64)</param>
        public BitFieldNumberOfBitsAttribute(byte bitCount)
        {
            if ((bitCount < 1) || (bitCount > 32))
                throw new ArgumentOutOfRangeException("bitCount", bitCount,
                "The number of bits must be between 1 and 32.");

            BitCount = bitCount;
        }

        /// <summary>
        /// The number of bits the bit field will contain
        /// </summary>
        public byte BitCount { get; private set; }
    }

    /// <summary>
    /// Specifies the length of each bit field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class BitFieldInfoAttribute : Attribute
    {
        /// <summary>
        /// Initializes new instance of BitFieldInfoAttribute with the specified field offset and length
        /// </summary>
        /// <param name="offset">The offset of the bit field</param>
        /// <param name="length">The number of bits the bit field occupies</param>
        public BitFieldInfoAttribute(byte offset, byte length)
        {
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// The offset of the bit field
        /// </summary>
        public byte Offset { get; private set; }

        /// <summary>
        /// The number of bits the bit field occupies
        /// </summary>
        public byte Length { get; private set; }
    }

    public interface IBitField { }

}

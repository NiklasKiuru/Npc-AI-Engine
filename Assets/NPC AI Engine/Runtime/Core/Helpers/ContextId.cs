using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Aikom.AIEngine
{   
    /// <summary>
    /// General purpose identifier that generates a hash from the objects type within a single context
    /// </summary>
    /// <remarks>
    /// The generated type hash uses CRC32 but unlike object.GetHashCode it is consistent through different application executions.
    /// Due to the inherent nature of CRC32 the amount of different types derived from the generic context type cannot be large
    /// </remarks>
    [Serializable]
    public struct ContextId : IEquatable<ContextId>
    {
        [SerializeField] private int _contextIterator;
        [SerializeField] private uint _typeHash;

        /// <summary>
        /// Generates a new context id. The id is only unique within the given <paramref name="context"/> and might collide outside of it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="indexable"></param>
        /// <returns></returns>
        public static ContextId Generate<T>(IReadOnlyCollection<T> context, T indexable) where T : IContextIndexable
        {   
            var newId = new ContextId();
            var type = indexable.GetType();
            var byteArray = Encoding.ASCII.GetBytes(type.ToString());
            newId._typeHash = Crc32.Get(byteArray);
            var largestIndex = GenerateSimple(context, indexable);
            newId._contextIterator = largestIndex;
            return newId;
        }

        /// <summary>
        /// Generates a unique id for an indexable within this context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="indexable"></param>
        /// <returns></returns>
        public static int GenerateSimple<T>(IReadOnlyCollection<T> context, T indexable) where T : IContextIndexable
        {
            var type = indexable.GetType();
            var largestIndex = 0;
            foreach (var item in context)
            {
                if (item.GetType() == type && item.Id > largestIndex)
                {
                    largestIndex = item.Id;
                }
            }
            largestIndex++;
            return largestIndex;
        }

        public bool Equals(ContextId other)
        {
            return other._contextIterator == _contextIterator && other._typeHash == _typeHash;
        }

        public override string ToString()
        {
            return $"{_typeHash} : {_contextIterator}";
        }
    }


    public interface IContextIndexable
    {
        public int Id { get; }
    }
    /// <summary>
    /// Performs 32-bit reversed cyclic redundancy checks.
    /// </summary>
    /// <remarks>
    /// Source: https://rosettacode.org/wiki/CRC-32#C.23
    /// </remarks>
    public class Crc32
    {
        /// <summary>
        /// Generator polynomial (modulo 2) for the reversed CRC32 algorithm. 
        /// </summary>
        private const uint s_generator = 0xEDB88320;

        /// <summary>
        /// Contains a cache of calculated checksum chunks.
        /// </summary>
        private static readonly uint[] m_checksumTable;

        static Crc32()
        {
            // Constructs the checksum lookup table. Used to optimize the checksum.
            m_checksumTable = Enumerable.Range(0, 256).Select(i =>
            {
                var tableEntry = (uint)i;
                for (var j = 0; j < 8; ++j)
                {
                    tableEntry = ((tableEntry & 1) != 0)
                        ? (s_generator ^ (tableEntry >> 1))
                        : (tableEntry >> 1);
                }
                return tableEntry;
            }).ToArray();
        }

        #region Methods
        /// <summary>
        /// Calculates the checksum of the byte stream.
        /// </summary>
        /// <param name="byteStream">The byte stream to calculate the checksum for.</param>
        /// <returns>A 32-bit reversed checksum.</returns>
        public static uint Get<T>(IEnumerable<T> byteStream)
        {
            try
            {
                // Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
                return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
                          (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
            }
            catch (FormatException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
            catch (InvalidCastException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
            catch (OverflowException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
        }
        #endregion

        
    }
}

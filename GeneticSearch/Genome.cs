using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSearch
{
    public class Genome : IEquatable<Genome>
    {
        public bool Equals(Genome other)
        {
            return memory.SequenceEqual(other.memory);
        }

        public override string ToString()
        {
            return string.Join(",", memory);
        }

        public override bool Equals(object obj)
        {
            return Equals((Genome) obj);
        }

        public override int GetHashCode()
        {
            return ((IStructuralEquatable) memory).GetHashCode(EqualityComparer<object>.Default);
        }

        public static bool operator ==(Genome left, Genome right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Genome left, Genome right)
        {
            return !Equals(left, right);
        }

        private readonly byte[] memory;

        public int Size { get { return memory.Length * 8; } }

        public Genome(byte[] bytes)
        {
            memory = new byte[bytes.Length];
            Array.Copy(bytes, memory, bytes.Length);
        }

        public byte[] GetMemory()
        {
            var t = new byte[memory.Length];
            Array.Copy(memory, t, memory.Length);
            return t;
        }

        public static Genome Mutate(Genome source, IEnumerable<int> mutatingGenes)
        {
            var length = source.memory.Length * 8;
            var bytes = new byte[source.memory.Length];
            Array.Copy(source.memory, bytes, bytes.Length);

            foreach (var mutatingGene in mutatingGenes)
            {
                var bitIndex = mutatingGene % length;
                var byteIndex = bitIndex / 8;
                var mask = (byte)(1 << (bitIndex % 8));
                bytes[byteIndex] ^= mask;
            }

            return new Genome(bytes);
        }

        public static Genome OnePointCrossover(Genome a, Genome b, int cutPoint)
        {
            var aBytes = a.memory;
            var bBytes = b.memory;

            var byteLength = aBytes.Length;

            var resultBytes = new byte[byteLength];

            var boundaryByte = cutPoint / 8;

            for (var i = 0; i < boundaryByte; i++)
            {
                resultBytes[i] = aBytes[i];
            }

            if (boundaryByte < byteLength)
            {
                var boundaryBit = cutPoint % 8;
                var mask = boundaryBit > 0 ? (byte)((1 << boundaryBit) - 1) : 0;
                resultBytes[boundaryByte] = (byte)((aBytes[boundaryByte] & mask | bBytes[boundaryByte] & ~mask));

                for (var i = cutPoint + 1; i < aBytes.Length; i++)
                {
                    resultBytes[i] = bBytes[i];
                }
            }

            return new Genome(resultBytes);
        }
    }
}
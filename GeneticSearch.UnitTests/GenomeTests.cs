using System;
using NUnit.Framework;

namespace GeneticSearch.UnitTests
{
    public class GenomeTests
    {
        [Test]
        [TestCase(
            new byte[] { 255, 255, 255 },
            new byte[] { 0, 0, 0 },
            new byte[] { 255, 15, 0 }, 12)]
        [TestCase(
            new byte[] { 255, 255, 255 },
            new byte[] { 0, 0, 0 },
            new byte[] { 15, 0, 0 }, 4)]
        [TestCase(
            new byte[] { 255, 255, 255 },
            new byte[] { 0, 0, 0 },
            new byte[] { 255, 255, 15 }, 20)]
        [TestCase(
            new byte[] { 255 },
            new byte[] { 0 },
            new byte[] { 0 }, 0)]
        [TestCase(
            new byte[] { 255 },
            new byte[] { 0 },
            new byte[] { 255 }, 8)]
        public void GenomeCrossovers(byte[] x, byte[] y, byte[] c, int splice)
        {
            var actual = Genome.OnePointCrossover(new Genome(x), new Genome(y), splice);
            CollectionAssert.AreEqual(c, actual.GetMemory());
        }

        [Test]
        public void GenomeMutates()
        {
            var source = new Genome(new byte[] { 0, 0, 0 });
            var actual = Genome.Mutate(source, new[] { 0, 23 });
            var expected = new byte[] { 1, 0, 128 };
            CollectionAssert.AreEqual(expected, actual.GetMemory());
        }

        [Test]
        public void GenomeMutates2()
        {
            var source = new Genome(new byte[] { 0, 0 });
            for (var i = 0; i < 8; i++)
            {
                var actual = Genome.Mutate(source, new[] { i });
                var expected = new byte[] { (byte)(1 << (i)), 0 };

                Console.WriteLine("Set bit {0} yields: {1}", i, BitConverter.ToInt16(actual.GetMemory(), 0));
                CollectionAssert.AreEqual(expected, actual.GetMemory());    
            }

            for (var i = 8; i < 16; i++)
            {
                var actual = Genome.Mutate(source, new[] { i });
                var expected = new byte[] { 0, (byte)(1 << (i-8)) };

                Console.WriteLine("Set bit {0} yields: {1}", i, BitConverter.ToUInt16(actual.GetMemory(), 0));
                CollectionAssert.AreEqual(expected, actual.GetMemory());
            }
        }
    }
}
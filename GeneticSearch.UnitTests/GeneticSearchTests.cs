using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace GeneticSearch.UnitTests
{
    class SineFit
    {
        static Random r = new Random(1234567);

        public static NeuralNetwork CreateDefault()
        {
            return new NeuralNetwork(2, 1, 5);
        }

        public static double Fitness(Genome genome, int numberOfSamples)
        {
            var nn = Deserialize(genome);

            var cost =
                Enumerable.Range(0, numberOfSamples)
                    .Select(i => (double)i/(numberOfSamples-1) * Math.PI * 2)
                    //.Select(i=>r.NextDouble() * Math.PI * 2)
                    .Select(x =>
                    {
                        return Compute(nn, x) - Math.Sin(x);
                    })
                    .Select(d => d * d).Sum() / numberOfSamples / 2;

            return cost;
        }

        public static double Compute(NeuralNetwork nn, double x)
        {
            var args = new List<double>(3){x, x*x, x*x*x};
            
            return nn.Compute(args).First();
        }

        public static NeuralNetwork Deserialize(Genome genome)
        {
            var nn = CreateDefault();

            nn.ReadState(genome.GetMemory());
            return nn;
        }
    }

    class GeneticSearchTests
    {
        [Test]
        public void ValueTest()
        {
            var g = Fit(new Genome(new byte[] { 245, 255, 255, 255 }));
            Console.WriteLine(g);
        }

        Tuple<Genome, double> FitExpensive(Genome g)
        {
            return Tuple.Create(g, ExpensiveFitness(g));
        }

        Tuple<Genome, double> Fit(Genome g)
        {
            return Tuple.Create(g, Fitness(g));
        }

        double GetFitness(Tuple<Genome, double> fitted)
        {
            return fitted.Item2;
        }

        Genome GetGenome(Tuple<Genome, double> fitted)
        {
            return fitted.Item1;
        }

        [Test]
        public void FindNumberGenome()
        {
            var random = new Random(123123012);

            var breed = new[] { FitExpensive(EncodeInitialGenome()) }.ToList();

            var fitness = double.MaxValue;

            //var epsilon = 1e-9;
            var epsilon = 0.00001;

            Console.WriteLine("Epsilon: {0}", epsilon);

            var generation = 0;

            while (fitness > epsilon && generation < 60000000)
            {
                generation++;
                fitness = breed.Select(GetGenome).Min(i => GetFitness(FitExpensive(i)));

                if (generation % 100000 == 0)
                {
                    DumpProgress(generation, fitness);

                    if (generation % 1000000 == 0)
                    {
                        DumpSolution(breed, generation);
                    }
                }

                if (fitness <= epsilon)
                    break;

                breed = NextBreed(breed, GetGenome, GetFitness, Fit, random, generation);
            }

            DumpProgress(generation, fitness);
            DumpSolution(breed, generation);

            Console.WriteLine("Solution at generation {0}:", generation);
        }

        private void DumpSolution(List<Tuple<Genome, double>> breed, int generation)
        {
            var best = breed.OrderBy(GetFitness).Take(3).ToList();

            var nn = SineFit.Deserialize(best.First().Item1);

            var numberOfSamplesToDump = 100;
            for (var i = 0; i < numberOfSamplesToDump; i++)
            {
                var x = (double)i / (numberOfSamplesToDump-1) * Math.PI * 2;
                var y = SineFit.Compute(nn, x);
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", generation, x.ToString(CultureInfo.InvariantCulture),
                    y.ToString(CultureInfo.InvariantCulture), Math.Abs(y - Math.Sin(x)).ToString(CultureInfo.InvariantCulture));
            }
        }

        static void DumpProgress(int generation, double fitness)
        {
            Console.WriteLine("Generation {0}, Fitness {1}, Swap rate: {2}", generation, fitness, (double)swaps / generation);
        }


        //private List<T> NextBreed<T>(List<T> prev, Func<T, Genome> getGenome, Func<T, double> getFitness, Func<Genome, T> fit, Random random)
        //{
        //    if (prev.Count == 0)
        //        throw new InvalidOperationException("population was empty");
        //    var populationSize = 30;
        //    var parentsSize = 3;

        //    var fitInidividuals = prev.Distinct().ToList();

        //    var parentsA = RouletteWheelSelection(fitInidividuals, i => 1.0 / getFitness(i), random, parentsSize).ToList();
        //    var parentsB = RouletteWheelSelection(fitInidividuals, i => 1.0 / getFitness(i), random, parentsSize).ToList();

        //    var genomeLength = getGenome(prev.First()).Size;
        //    var genomeLengthReal = (double)genomeLength;
        //    var breed =
        //        from a in parentsA
        //        from b in parentsB
        //        //where !a.Equals(b)
        //        let splicePoint = random.Next(genomeLength)
        //        let childA = Genome.OnePointCrossover(getGenome(a), getGenome(b), splicePoint)
        //        let childB = Genome.OnePointCrossover(getGenome(b), getGenome(a), splicePoint)
        //        from kind in new[]
        //        {
        //            a,
        //            b,
        //            fit(childA),
        //            fit(childB),
        //            fit(Mutate(childA, 1.0/genomeLengthReal * 1, random)),
        //            fit(Mutate(childB, 1.0/genomeLengthReal * 1, random)),
        //            //fit(Mutate(childA, 1.0/genomeLengthReal * 3, random)),
        //            //fit(Mutate(childB, 1.0/genomeLengthReal * 3, random)),
        //            fit(Mutate(childA, 1.0/genomeLengthReal * 10, random)),
        //            fit(Mutate(childB, 1.0/genomeLengthReal * 10, random)),
        //            //fit(Mutate(childA, 1.0/genomeLengthReal * 100, random)),
        //            //fit(Mutate(childB, 1.0/genomeLengthReal * 100, random)),
        //        }
        //        select kind;

        //    var nextBreed = breed.Distinct().ToList();

        //    //if (nextBreed.Count < 1)
        //    //    return prev;

        //    var nextPopulation = RouletteWheelSelection(nextBreed, i => 1.0 / getFitness(i), random, populationSize).Distinct().ToList();

        //    var bestFit = nextPopulation.Min(getFitness);

        //    var prevBestFit = fitInidividuals.Min(getFitness);

        //    if (bestFit > prevBestFit)
        //    {
        //        return prev;
        //            //.Concat(nextPopulation)
        //            //.ToList();
        //    }

        //    return nextPopulation;
        //}

        private static long swaps = 0;

        private List<T> NextBreed<T>(List<T> prev, Func<T, Genome> getGenome, Func<T, double> getFitness,
            Func<Genome, T> fit, Random random, int generation)
        {
            var prevOne = prev.Single();
            var genome = getGenome(prevOne);

            var t = 100.0 / generation;

            var mutant = fit(Mutate(genome, (1.0 /genome.Size) * (1.0 + 0.02 * genome.Size * random.NextDouble()), random));

            var newFitness = (float)getFitness(mutant);
            var prevFitness = (float)getFitness(prevOne);

            if (newFitness < prevFitness || Math.Exp(-1.0 * (newFitness - prevFitness) / t) > random.NextDouble())
            {
                swaps ++;
                return new List<T> { mutant };
            }

            return prev;
        }

        //private List<T> NextBreed<T>(List<T> prev, Func<T, Genome> getGenome, Func<T, double> getFitness,
        //    Func<Genome, T> fit, Random random)
        //{
        //    var prevOne = prev.Single();
        //    var genome = getGenome(prevOne);
        //    //var newOne = fit(Mutate(genome, 1.0/genome.Size, random));

        //    var mutants = Enumerable.Range(0, 3)
        //        .Select(i => fit(Mutate(genome, random.NextDouble() * 2, random))).ToList();

        //    var minFitness = mutants.Min(i=>(float)getFitness(i));

        //    var newOne = mutants.First(i => (float)getFitness(i)  == minFitness);

        //    //var newOne =
        //    //    mutants.OrderBy(getFitness)
        //    //        .Take(1).First();

        //    var newFitness = getFitness(newOne);
        //    var prevFitness = getFitness(prevOne);
        //    if (newFitness < prevFitness)
        //    {
        //        return new List<T> { newOne };
        //    }

        //    return prev;
        //}

        IEnumerable<T> RouletteWheelSelection<T>(IList<T> source, Func<T, double> weight, Random r, int populationSize)
        {
            Func<T, double> limitedWeight = i => Math.Min(1e10, Math.Max(1e-10, weight(i)));
            var sumWeight = source.Sum(limitedWeight);

            var skipCount = 0;
            for (var count = 0; count < populationSize; )
            {
                var index = r.Next(0, source.Count);
                var item = source[index];
                var select = r.NextDouble() < limitedWeight(item) / sumWeight;
                if (select)
                {
                    yield return item;
                    count++;
                }
                else
                {
                    skipCount++;
                }
            }

            if (skipCount > source.Count)
            {
                //Console.WriteLine("Skipped: {0} to select {1}", skipCount, populationSize);
            }
        }

        int DiscreetSize(double x, double chance)
        {
            var discreetSize = (int)(x + chance);

            return discreetSize;
        }

        private IEnumerable<int> MutationSequence(Genome g, double probability, Random r)
        {
            var count = DiscreetSize(probability * g.Size, r.NextDouble());
            while (count > 0)
            {
                count--;
                yield return r.Next(0, g.Size);
            }
        }

        private Genome Mutate(Genome genome, double probablity, Random random)
        {
            var mutationSequence = MutationSequence(genome, probablity, random);
            var mutate = Genome.Mutate(genome, mutationSequence);
            return mutate;
        }

        private double shift = int.MaxValue / 2;

        double Fitness(Genome genome)
        {
            return SineFit.Fitness(genome, 10);
        }

        double ExpensiveFitness(Genome genome)
        {
            return SineFit.Fitness(genome, 10);
        }

        Genome EncodeInitialGenome()
        {
            return new Genome(SineFit.CreateDefault().Serialize());
        }

        //double Fitness(Genome genome)
        //{
        //    var value = Decode(genome);

        //    var ideal = -Math.PI * 0.1;
        //    return Math.Abs(value - ideal) > 0.0001 ? 1.0 : 0.0;
        //}

        //double Decode(Genome genome)
        //{
        //    var mem = genome.GetMemory();

        //    var i = BitConverter.ToUInt64(mem, 0);

        //    return i * (2.0 / UInt64.MaxValue) - 1.0;
        //}

        double Decode(Genome genome)
        {
            var mem = genome.GetMemory();

            var i = BitConverter.ToUInt64(mem, 0);

            return i * (2.0 / UInt64.MaxValue) - 1.0;
        }

        //Genome EncodeInitialGenome()
        //{
        //    return new Genome(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 });
        //}

        [Test]
        public void HighestLowest()
        {
            var low = Decode(new Genome(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }));

            Console.WriteLine(low);

            var high = Decode(new Genome(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }));

            Console.WriteLine(high);

            var zero = Decode(new Genome(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 }));

            Console.WriteLine(zero);
        }

        [Test]
        public void TestByteFloat()
        {
            var sourcesToEncode = new[] { -100, -10, -1, -0.5, -0.1, -0.01, 0, 0.01, 0.1, 0.5, 1, 10, 100 };
            var neuralNetwork = new NeuralNetwork(1, 1, 1);
            foreach (var x in sourcesToEncode)
            {
                var byteEncoded = neuralNetwork.Encode(x);

                var decoded = neuralNetwork.Decode(byteEncoded);

                Console.WriteLine("x: {0} \ty: {1} \tz:{2}", x, byteEncoded, decoded);
            }
        }
    }
}

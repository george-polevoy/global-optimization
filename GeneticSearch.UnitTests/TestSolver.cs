using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GeneticSearch.UnitTests
{
    static class EnumerableMinimizationExtensions
    {
        public static T OfMinCost<T>(this IEnumerable<T> source, Func<T,double> cost)
        {
            using (var i = source.GetEnumerator())
            {
                if (!i.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");
                var current = i.Current;
                var currentCost = cost(current);
                while (i.MoveNext())
                {
                    var next = i.Current;
                    var nextCost = cost(next);
                    if (!(nextCost < currentCost)) continue;
                    current = next;
                    currentCost = nextCost;
                }
                return current;
            }
        }
    }
    class TestSolver
    {
        [Test]
        public void OfMinCostTest()
        {
            var items = new[] {"a", "aa", "aaa"};
            Assert.AreEqual("a", items.OfMinCost(i=>i.Length));
            Assert.AreEqual("aaa", items.OfMinCost(i=>-i.Length));
        }

        [Test]
        public void TestSolvers(
            //[Values(10,5,1,0.5,01,0.005)] double initialTemperature,
            //[Values(0.5, 0.25, 0.1, 0.05, 0.025, 0.01)] double initialDiameter,
            //[Values(0.1, 0.01, 0.001)] double finalTemperature
            )
        {
            var seed = 123456;
            var sequence = new Random(seed);
            var initial = new SineSolution(sequence, 1.0);
            var nIterations = 10000000;
            var scalarOfScalarTabularCost = new ScalarOfScalarTabularCost(FunctionRange.CreateFunctionPoints(0, Math.PI*2, 10,
                Math.Sin));

            var initialTemperature = 1.0;
            var initialDiameter = 0.01;
            var finalTemperature = 0.1;
            
            var solution = new SimulatedAnnealingSolver<double, double, SineSolution>().FindSolutions(
                scalarOfScalarTabularCost, initial, sequence, initialDiameter, initialTemperature,
                Math.Pow(finalTemperature/initialTemperature, 1.0/nIterations)
                ).Take(nIterations);
                //.OfMinCost(i=>i.Cost);

            var count = 0;
            foreach (var sol in solution)
            {
                count++;
                if (count % 100000 == 0)
                    Console.WriteLine("Count: {0}, Selectivity: {1}, AvgCost {2} ", count, sol.Selectivity, sol.Cost);
            }
        }

        [Test]
        public void BruteForce()
        {
            var seed = 123;
            var sequence = new Random(seed);
            
            var diameter = 3;
            
            var nIterations = 1000000;
                
            var fit = new ScalarOfScalarTabularCost(FunctionRange.CreateFunctionPoints(0, Math.PI*2, 10,
                Math.Sin));

            var q =
                from p in Enumerable.Range(0, 100).AsParallel()
                let initial = new SineSolution(sequence, 5.0)
                let subsequence = new Random(sequence.Next(100, 100000))
                select
                    Enumerable.Range(0, nIterations).Select(j =>
                    {
                        var candidate = initial.Variate(subsequence, diameter);
                        return new
                        {
                            candidate,
                            cost = fit.Cost(candidate)
                        };
                    }).OfMinCost(i=>i.cost);

            var bestOfBest = q.OfMinCost(i=>i.cost);

            Console.WriteLine(bestOfBest.cost);
            Console.WriteLine(bestOfBest.candidate);
        }
    }
}

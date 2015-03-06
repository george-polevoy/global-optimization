using System;
using System.Linq;
using NUnit.Framework;

namespace GeneticSearch.UnitTests
{
    class TestSolver
    {
        [Test]
        public void TestSolvers()
        {
            var initial = new SineSolution();

            var solution =
                new SimulatedAnnealingSolver<double, double, SineSolution>().FindSolutions(
                    new ScalarOfScalarTabularCost(FunctionRange.CreateFunctionPoints(0, Math.PI*2, 10,
                        (x) => Math.Sin(x))), initial, new Random(123), 0.01, 10)
                    .TakeWhile(s => s.Cost > 0.1)
                    .Take(100000)
                    .Min(i=>i.Cost);

            Console.WriteLine(solution);
        }
    }
}

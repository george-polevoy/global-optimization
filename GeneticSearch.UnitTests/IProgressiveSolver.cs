using System;
using System.Collections.Generic;

namespace GeneticSearch.UnitTests
{
    public interface IProgressiveSolver<TArg, TResult, TSolution> where TSolution : IFunction<TArg, TResult>
    {
        IEnumerable<AnnealingResult<TSolution, TArg, TResult>> FindSolutions(ICostEvaluator<TArg, TResult> cost, TSolution initial, Random sequence, double diameter, double temperature, double decay);
    }

    public class AnnealingResult<TSolution, TArg, TResult> where TSolution : IFunction<TArg, TResult>
    {
        public TSolution Solution { get; set; }
        public double Cost { get; set; }
        public double Selectivity { get; set; }

        public AnnealingResult(TSolution solution, double cost, double selectivity)
        {
            Solution = solution;
            Cost = cost;
            Selectivity = selectivity;
        }
    }

    public class SimulatedAnnealingSolver<TArg, TResult, TSolution> : IProgressiveSolver<TArg, TResult, TSolution> where TSolution : IFunction<TArg, TResult>, ISolution<TSolution, TArg>
    {
        AnnealingResult<TSolution, TArg, TResult> Compute(TSolution solution, ICostEvaluator<TArg, TResult> cost, double selectivity)
        {
            return new AnnealingResult<TSolution, TArg, TResult>(solution, cost.Cost(solution), selectivity);
        }

        public IEnumerable<AnnealingResult<TSolution, TArg, TResult>> FindSolutions(ICostEvaluator<TArg, TResult> cost,  TSolution initial, Random sequence, double diameter, double temperature, double decay)
        {
            var prev = Compute(initial, cost, 0);

            var count = 0;
            var selectionCount = 0;

            while (true)
            {
                var next = Compute(prev.Solution.Variate(sequence, diameter * temperature), cost, (double)selectionCount / count);

                if (next.Cost <= prev.Cost || Math.Exp(-(next.Cost - prev.Cost)/temperature) > sequence.NextDouble())
                {
                    selectionCount ++;
                    count++;
                    yield return next;
                    prev = next;
                    temperature *= decay;
                }
                else
                {
                    count++;
                    yield return prev;
                }
            }
        }
    }
}
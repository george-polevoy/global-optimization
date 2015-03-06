using System;
using System.Collections.Generic;

namespace GeneticSearch.UnitTests
{
    public interface IProgressiveSolver<TArg, TResult, TSolution> where TSolution : IFunction<TArg, TResult>
    {
        IEnumerable<AnnealingResult<TSolution, TArg, TResult>> FindSolutions(ICostEvaluator<TArg, TResult> cost, TSolution initial, Random sequence, double diameter, double temperature);
    }

    public class AnnealingResult<TSolution, TArg, TResult> where TSolution : IFunction<TArg, TResult>
    {
        public TSolution Solution { get; set; }
        public double Cost { get; set; }

        public AnnealingResult(TSolution solution, double cost)
        {
            Solution = solution;
            Cost = cost;
        }
    }

    public class SimulatedAnnealingSolver<TArg, TResult, TSolution> : IProgressiveSolver<TArg, TResult, TSolution> where TSolution : IFunction<TArg, TResult>, ISolution<TSolution, TArg>
    {
        AnnealingResult<TSolution, TArg, TResult> Compute(TSolution solution, ICostEvaluator<TArg, TResult> cost)
        {
            return new AnnealingResult<TSolution, TArg, TResult>(solution, cost.Cost(solution));
        }

        public IEnumerable<AnnealingResult<TSolution, TArg, TResult>> FindSolutions(ICostEvaluator<TArg, TResult> cost,  TSolution initial, Random sequence, double diameter, double temperature)
        {
            var prev = Compute(initial, cost);
            
            while (true)
            {
                var next = Compute(prev.Solution.Variate(sequence, diameter * temperature), cost);

                if (next.Cost < prev.Cost || Math.Exp(-(next.Cost - prev.Cost)*temperature) > sequence.NextDouble())
                {
                    yield return next;
                }
                else
                {
                    yield return prev;
                }
            }
        }

        public IEnumerable<AnnealingResult<TSolution, TArg, TResult>> FindSolutions(TSolution initial, Random sequence, double diameter, double temperature,
            double templeratureDecrease)
        {
            throw new NotImplementedException();
        }
    }
}
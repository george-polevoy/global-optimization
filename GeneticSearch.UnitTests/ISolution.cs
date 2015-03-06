using System;

namespace GeneticSearch.UnitTests
{
    public interface ISolution<out TSolution, TArg> where TSolution : ISolution<TSolution, TArg>
    {
        TSolution Variate(Random r, double diameter);
    }

    public interface IFunction<in TArg, out TResult>
    {
        TResult Evaluate(TArg arg);
    }
}
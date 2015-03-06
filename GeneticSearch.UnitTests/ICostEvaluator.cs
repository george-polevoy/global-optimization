namespace GeneticSearch.UnitTests
{
    public interface ICostEvaluator<out TArg, in TResult>
    {
        double Cost(IFunction<TArg, TResult> solution);
    }
}
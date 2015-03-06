using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSearch.UnitTests
{
    public static class FunctionRange
    {
        public static IEnumerable<Tuple<double, double>> CreateFunctionPoints(double xmin, double xmax, int count, Func<double, double> f)
        {
            return Enumerable.Range(0, count).Select(i => xmin + (xmax - xmin)*(double) i/(count - 1)).Select(x => Tuple.Create(x, f(x)));
        }
    }
}
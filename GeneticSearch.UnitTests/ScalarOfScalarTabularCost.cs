using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSearch.UnitTests
{
    public class ScalarOfScalarTabularCost : ICostEvaluator<double, double>
    {
        private IEnumerable<Tuple<double, double>> Points { get; set; }
        
        public ScalarOfScalarTabularCost(IEnumerable<Tuple<double,double>> points)
        {
            Points = points.ToList();
        }

        public double Cost(IFunction<double, double> solution)
        {
            const int numberOfSamples = 10;
            var enumerable = Points.Select(p => solution.Evaluate(p.Item1) - p.Item2);
            var s = 0.0;
            var thresh = 2.0 * 0.5/numberOfSamples;
            
            foreach (var i in enumerable)
            {
                s += i*i;
                //if (s > thresh)
                //    return s;
            }
            return s/numberOfSamples;
        }
    }
}
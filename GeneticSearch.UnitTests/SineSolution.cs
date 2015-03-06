using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSearch.UnitTests
{
    public class SineSolution : ISolution<SineSolution, double>, IFunction<double, double>
    {
        private const int bands = 6;

        NeuralNetwork aprox = new NeuralNetwork(bands, 1, bands);

        public SineSolution()
        {
        }

        public SineSolution(NeuralNetwork nn)
        {
            aprox = nn;
        }

        public double Evaluate(double arg)
        {
            var powers = new List<double>(bands);
            powers.Add(arg);
            for (var i = 1; i < bands; i++)
            {
                powers[i] = powers[i - 1]*arg;
            }
            return aprox.Compute(powers).First();
        }

        public SineSolution Variate(Random r, double diameter)
        {
            return new SineSolution(aprox.Variate(r, diameter));
        }
    }
}
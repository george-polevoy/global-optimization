using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSearch.UnitTests
{
    public class SineSolution : ISolution<SineSolution, double>, IFunction<double, double>
    {
        private const int bands = 6;

        private readonly NeuralNetwork network;

        public SineSolution()
        {
            network = new NeuralNetwork(bands, 1, bands);
        }

        private SineSolution(NeuralNetwork network)
        {
            this.network = network;
        }

        public double Evaluate(double arg)
        {
            var powers = new List<double>(bands);
            powers.Add(arg);
            for (var i = 1; i < bands; i++)
            {
                powers.Add(powers[i - 1]*arg);
            }
            return network.Compute(powers).First();
        }

        public SineSolution Variate(Random r, double diameter)
        {
            return new SineSolution(network.Variate(r, diameter));
        }
    }
}
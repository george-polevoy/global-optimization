using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSearch.UnitTests
{
    public class ConstantSolution : ISolution<ConstantSolution, double>, IFunction<double, double>
    {
        private readonly NeuralNetwork network;

        public ConstantSolution()
        {
            network = new NeuralNetwork(1, 1, 4);
        }

        private ConstantSolution(NeuralNetwork network)
        {
            this.network = network;
        }

        public double Evaluate(double arg)
        {
            return network.Compute(new []{arg}).First();
        }

        public ConstantSolution Variate(Random r, double diameter)
        {
            return new ConstantSolution(network.Variate(r, diameter));
        }
    }

    public class SineSolution : ISolution<SineSolution, double>, IFunction<double, double>
    {
        public override string ToString()
        {
            return string.Format("Network: {0}", network);
        }

        private const int bands = 1;

        private readonly NeuralNetwork network;

        public SineSolution(Random sequence, double d)
        {
            network = new NeuralNetwork(bands, 1, 3);
            network.ReadState(Enumerable.Range(0,10000).Select(i=>(sequence.NextDouble() - 0.5) * d));
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
                powers.Add(powers[i - 1]*arg*arg);
            }
            return network.Compute(powers).First();
        }

        public SineSolution Variate(Random r, double diameter)
        {
            return new SineSolution(network.Variate(r, diameter));
        }
    }
}